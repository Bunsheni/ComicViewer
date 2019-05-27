using SQLite;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ComicViewer.CtModels;

namespace ComicViewer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class CtSQLiteDb : ICtDataStore
    {
        private CtComicList _comics;
        private CtArtistList _artists;
        private CtGroupList _groups;
        private CtSeriesList _serieses;
        private CtCharacterList _characters;
        private CtTagList _tags;
        private CtNameList _names;
        private CtWordList _words;

        private SQLiteAsyncConnection _connection;
        public AutoResetEvent _waitforListComplete;
        AsyncLock myLock = new AsyncLock();
        public bool IsLoaded;

        public CtSQLiteDb()
        {
            _waitforListComplete = new AutoResetEvent(false);
        }

        public SQLiteAsyncConnection GetConnection()
        {
            if (_connection == null)
            {
                _connection = DependencyService.Get<ISQLiteDb>().GetConnection("ComicViewerH.db3");
            }
            return _connection;
        }

        public void Close()
        {
            if (_connection != null)
            {
                _connection.GetConnection().Close();
                _connection.GetConnection().Dispose();
                SQLiteAsyncConnection.ResetPool();
                _connection = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }


        public async Task CreatCtTable()
        {
            await GetConnection().CreateTableAsync<CtComic>();
            await GetConnection().CreateTableAsync<CtArtist>();
            await GetConnection().CreateTableAsync<CtGroup>();
            await GetConnection().CreateTableAsync<CtSeries>();
            await GetConnection().CreateTableAsync<CtCharacter>();
            await GetConnection().CreateTableAsync<CtTag>();
            await GetConnection().CreateTableAsync<CtName>();
            await GetConnection().CreateTableAsync<CtWord>();
        }

        public async Task DropDerpTagTable()
        {
            await GetConnection().DropTableAsync<CtComic>();
        }

        public async Task Load()
        {
            IsLoaded = true;
            _waitforListComplete.Reset();

            await CreatCtTable();            

            if (_artists != null)
                _artists.Dispose();
            _artists = new CtArtistList(await GetConnection().Table<CtArtist>().ToListAsync());

            if (_groups != null)
                _groups.Dispose();
            _groups = new CtGroupList(await GetConnection().Table<CtGroup>().ToListAsync());

            if (_serieses != null)
                _serieses.Dispose();
            _serieses = new CtSeriesList(await GetConnection().Table<CtSeries>().ToListAsync());

            if (_characters != null)
                _characters.Dispose();
            _characters = new CtCharacterList(await GetConnection().Table<CtCharacter>().ToListAsync());

            if (_tags != null)
                _tags.Dispose();
            _tags = new CtTagList(await GetConnection().Table<CtTag>().ToListAsync());

            if (_names != null)
                _names.Dispose();
            _names = new CtNameList(await GetConnection().Table<CtName>().ToListAsync());

            if (_words != null)
                _words.Dispose();
            var ctWords = await GetConnection().Table<CtWord>().ToListAsync();
            _words = new CtWordList(await GetConnection().Table<CtWord>().ToListAsync());

            if (_comics != null)
                _comics.Dispose();
            _comics = new CtComicList(await GetConnection().Table<CtComic>().ToListAsync());
            _waitforListComplete.Set();

            if (_comics == null || _artists == null || _groups == null || _serieses == null || _characters == null || _tags == null || _words == null)
            {
                IsLoaded = false;
            }
        }

        public async Task<CtComicList> GetComics()
        {
            using (var releaser = await myLock.LockAsync())
            {
                if (_comics == null)
                {
                    await Task.Run(() => { _waitforListComplete.WaitOne(); });
                }
                return _comics;
            }
        }

        public async Task<CtArtistList> GetArtists()
        {
            if (_artists == null)
            {
                await Task.Run(() => { _waitforListComplete.WaitOne(); });
            }
            return _artists;
        }

        public async Task<CtGroupList> GetGroups()
        {
            if (_groups == null)
            {
                await Task.Run(() => { _waitforListComplete.WaitOne(); });
            }
            return _groups;
        }

        public async Task<CtSeriesList> GetSerieses()
        {
            if (_serieses == null)
            {
                await Task.Run(() => { _waitforListComplete.WaitOne(); });
            }
            return _serieses;
        }

        public async Task<CtCharacterList> GetCharacters()
        {
            if (_characters == null)
            {
                await Task.Run(() => { _waitforListComplete.WaitOne(); });
            }
            return _characters;
        }

        public async Task<CtTagList> GetTags()
        {
            if (_tags == null)
            {
                await Task.Run(() => { _waitforListComplete.WaitOne(); });
            }
            return _tags;
        }

        public async Task<CtNameList>  GetNames()
        {
            if (_names == null)
            {
                await Task.Run(() => { _waitforListComplete.WaitOne(); });
            }
            return _names;
        }

        public async Task<CtWordList>  GetWords()
        {
            if (_words == null)
            {
                await Task.Run(() => { _waitforListComplete.WaitOne(); });
            }
            return _words;
        }
        
        public async Task<CtComic> GetComic(string workid)
        {
            return (await GetComics()).GetComic(workid);
        }

        public async Task InsertComic(CtComic comic)
        {
            await GetConnection().InsertAsync(comic);
            (await GetComics()).Insert(0, comic);
        }

        public async Task DeleteComic(string workid)
        {
            CtComic comic = (await GetComics()).GetComic(workid);
            await GetConnection().DeleteAsync(comic);
            (await GetComics()).Remove(comic);
        }

        public async Task UpdateComic(CtComic comic)
        {
            if (comic != (await GetComics()).GetComic(comic.Workid))
                (await GetComics()).UpdateModel(comic);
            await GetConnection().UpdateAsync(comic);
        }

        public async Task<bool> FixComicAsync(CtComic comic, IWebConnection web)
        {
            bool res;
            using (CtModelList newmodels = new CtModelList())
            {
                res = comic.FixTitle();
                res = comic.FixAllTag(await GetArtists(), await GetGroups(), await GetSerieses(), await GetCharacters(), await GetModels(CtModelType.ALL), await GetNames(), newmodels) || res;
                if (!comic.IsTranslated)
                {
                    res = await comic.TranslateTitle(await GetWords(), await GetNames(), web) || res;
                }
                await AddModels(newmodels);
            }
            return res;
        }

        public async Task<CtModelList> GetModels(CtModelType type)
        {
            if (type == CtModelType.COMIC)
                return (await GetComics());
            else if (type == CtModelType.ARTIST)
                return (await GetArtists());
            else if (type == CtModelType.GROUP)
                return (await GetGroups());
            else if (type == CtModelType.SERIES)
                return (await GetSerieses());
            else if (type == CtModelType.CHARACTER)
                return (await GetCharacters());
            else if (type == CtModelType.TAG)
                return (await GetTags());
            else if (type == CtModelType.NAME)
                return (await GetNames());
            else if (type == CtModelType.WORD)
                return (await GetWords());
            else if (type == CtModelType.ALL)
            {
                CtModelList models = new CtModelList();
                models.AddRange(await GetArtists());
                models.AddRange(await GetGroups());
                models.AddRange(await GetSerieses());
                models.AddRange(await GetCharacters());
                models.AddRange(await GetTags());
                return models;
            }
            else
                return null;
        }

        public async Task<CtModel> GetModel(CtModelType type, string id)
        {
            if (type == CtModelType.COMIC)
                return (await GetComics()).GetComic(id);
            else if (type == CtModelType.ARTIST)
                return (await GetArtists()).GetArtist(id);
            else if (type == CtModelType.GROUP)
                return (await GetGroups()).GetGroup(id);
            else if (type == CtModelType.SERIES)
                return (await GetSerieses()).GetSeries(id);
            else if (type == CtModelType.CHARACTER)
                return (await GetCharacters()).GetCharacter(id);
            else if (type == CtModelType.TAG)
                return (await GetTags()).GetTag(id);
            else if (type == CtModelType.NAME)
                return (await GetNames()).GetName(id);
            else if (type == CtModelType.WORD)
                return (await GetWords()).GetWord(id);
            else
                return null;
        }

        public async Task UpdateAddModel(CtModel model)
        {
            if((await GetModels(model.CtType)).UpdateAddModel(model))
                await GetConnection().UpdateAsync(model);
            else
                await GetConnection().InsertAsync(model);
        }

        public async Task UpdateAddModels(CtModelList models)
        {
            foreach (CtModel model in models)
            {
                if ((await GetModels(model.CtType)).UpdateAddModel(model))
                    await GetConnection().UpdateAsync(model);
                else
                    await GetConnection().InsertAsync(model);
            }
        }

        public async Task UpdateModels(CtModelList models)
        {
            foreach (CtModel model in models)
            {
                (await GetModels(model.CtType)).UpdateModel(model);
                await GetConnection().UpdateAsync(model);
            }
        }

        public async Task AddModel(CtModel model)
        {
            (await GetModels(model.CtType)).Add(model);
            await GetConnection().InsertAsync(model);
        }

        public async Task DeleteModel(CtModel model)
        {
            (await GetModels(model.CtType)).Remove(model);
            await GetConnection().DeleteAsync(model);
        }

        public async Task AddModels(CtModelList models)
        {
            foreach(CtModel model in models)
            {
                try
                {
                    (await GetModels(model.CtType)).Add(model);
                    await GetConnection().InsertAsync(model);
                }
                catch
                {

                }
            }
        }

        public async Task<List<CtSuggestionItem>> GetSuggestionItemsAsync(string key, CtModelType type)
        {
            List<CtSuggestionItem> items = new List<CtSuggestionItem>();
            if(type == CtModelType.ALL)
            {
                for (CtModelType i = CtModelType.ARTIST; i <= CtModelType.TAG; i++)
                {
                    List<CtModel> models = (await GetModels(i)).FindAll(j => j.Contains(key));
                    foreach (var model in models)
                    {
                        foreach (string str in model.GetTags())
                        {
                            foreach (string str2 in Library.StringDivider(str, " "))
                            {
                                if (str2.ToLower().StartsWith(key.ToLower()))
                                {
                                    items.Add(new CtSuggestionItem(str, model));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                CtModelList models = await GetModels(type);
                foreach (var model in models)
                {
                    foreach (string str in model.GetTags())
                    {
                        if ((key.Length < str.Length && string.Compare(key, str.Substring(0, key.Length), true) == 0) ||
                                (str.ToLower().Contains(key.ToLower())) && key.Length > 2 || key.Length == 0)
                        {
                            items.Add(new CtSuggestionItem(str, model));
                        }
                    }
                }
            }
            return items;            
        }
    }

    public class AsyncSemaphore
    {
        private readonly static Task s_completed = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> m_waiters = new Queue<TaskCompletionSource<bool>>();
        private int m_currentCount;

        public AsyncSemaphore(int initialCount)
        {
            if (initialCount < 0) throw new ArgumentOutOfRangeException("initialCount");
            m_currentCount = initialCount;
        }

        public Task WaitAsync()
        {
            lock (m_waiters)
            {
                if (m_currentCount > 0)
                {
                    --m_currentCount;
                    return s_completed;
                }
                else
                {
                    var waiter = new TaskCompletionSource<bool>();
                    m_waiters.Enqueue(waiter);
                    return waiter.Task;
                }
            }
        }

        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (m_waiters)
            {
                if (m_waiters.Count > 0)
                    toRelease = m_waiters.Dequeue();
                else
                    ++m_currentCount;
            }
            if (toRelease != null)
                toRelease.SetResult(true);
        }
    }

    public class AsyncLock
    {
        private readonly AsyncSemaphore m_semaphore;
        private readonly Task<Releaser> m_releaser;

        public AsyncLock()
        {
            m_semaphore = new AsyncSemaphore(1);
            m_releaser = Task.FromResult(new Releaser(this));
        }
        public Task<Releaser> LockAsync()
        {
            var wait = m_semaphore.WaitAsync();
            return wait.IsCompleted ?
                m_releaser :
                wait.ContinueWith((_, state) => new Releaser((AsyncLock)state),
                    this, CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        public struct Releaser : IDisposable
        {
            private readonly AsyncLock m_toRelease;

            internal Releaser(AsyncLock toRelease) { m_toRelease = toRelease; }

            public void Dispose()
            {
                if (m_toRelease != null)
                    m_toRelease.m_semaphore.Release();
            }
        }
    }
}
