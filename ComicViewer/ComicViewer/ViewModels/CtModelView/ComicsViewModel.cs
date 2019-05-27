using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using ComicViewer.Models;
using ComicViewer.Views;
using ComicViewer.CtModels;
using System.Linq;
using System.Collections.Generic;

namespace ComicViewer.ViewModels
{
    public class ComicsViewModel : BaseViewModel
    {
        private IEnumerable<CtModel> _comics;
        private CtWebService _service;
        private readonly object lockobject = new object();
        private readonly object downlaodLockObject = new object();
        private List<CtComic> downloadList = new List<CtComic>();
        public CtComicList selectedComics = new CtComicList();
        private CtModelList _tempModels;
        private bool downloading;

        public IEnumerable<CtModel> Comics
        {
            get { return _comics; }
            set { SetProperty(ref _comics, value); }
        }

        bool progressBarIsVisible;
        public bool ProgressBarIsVisible
        {
            get
            {
                return progressBarIsVisible;
            }
            set
            {
                progressBarIsVisible = value;
                OnPropertyChanged();
            }
        }
        int progressBarHeight;
        public int ProgressBarHeight
        {
            get
            {
                return progressBarHeight;
            }
            set
            {
                progressBarHeight = value;
                OnPropertyChanged();
            }
        }
        float progress1, progress2;
        public float Progress1
        {
            get
            {
                return progress1;
            }
            set
            {
                progress1 = value;
                OnPropertyChanged();
            }
        }
        public float Progress2
        {
            get
            {
                return progress2;
            }
            set
            {
                progress2 = value;
                OnPropertyChanged();
            }
        }
        List<CtSuggestionItem> suggestionItem;
        public List<CtSuggestionItem> SuggestionItems
        {
            get { return suggestionItem; }

            set
            {
                suggestionItem = value;
                OnPropertyChanged();
            }
        }
        string key;
        public string Key { get { return key; } set { key = value; OnPropertyChanged(); } }
        public Command LoadComicsCommand { get; set; }
        public Command UpdateFromHitomiCommand { get; set; }
        public Command AddFromHitomi { get; set; }
        public CtFilter CurrentFilter { get; set; }
        public CtComicList BaseComics { get; set; }
        public string SortButtonName
        {
            get
            {
                return ((App)Application.Current).CtProgramLanguage == CtLanguage.KOREAN ? "정렬" : "Sort";
            }
        }
        public string AddButtonName
        {
            get
            {
                return ((App)Application.Current).CtProgramLanguage == CtLanguage.KOREAN ? "추가" : "Add";
            }
        }
        public string HitomiButtonName
        {
            get
            {
                return ((App)Application.Current).CtProgramLanguage == CtLanguage.KOREAN ? "히토에서 추가" : "Add From Hitomi";
            }
        }
        public void ClearSelect()
        {
            lock (lockobject)
            {
                foreach (CtComic comic in Comics)
                {
                    if (comic.IsSelected)
                    {
                        comic.IsSelected = false;
                    }
                }
                selectedComics.Clear();
            }
        }
        public void DownloadComics()
        {
            foreach(CtComic comic in selectedComics)
            {
                lock (downlaodLockObject)
                {
                    if (!downloadList.Exists(i => i == comic))
                        downloadList.Add(comic);
                }
            }
            if (downloadList.Count > 0)
                DownloadRun();
        }

        public async Task UpdateComicsAsync()
        {
            int res = 0;
            CtComic tempComic;
            foreach (CtComic comic in selectedComics)
            {
                tempComic = await CtWebService.searchWorkInfoFromWeb(comic.Id);
                await CtDb.FixComicAsync(tempComic, _webif);
                await CtDb.UpdateComic(tempComic);
                res++;
            }
            RootPage.ListViewUpdateRequestAsync();
            await RootPage.DisplayAlert("알림", $"{res}개 작품의 정보가 업데이트 되었습니다.", "취소");
        }

        public void DownloadRun()
        {
            if (!downloading)
            {
                downloading = true;
                ProgressBarHeight = 8;
                ProgressBarIsVisible = true;
                int i = 0;
                while (true)
                {
                    CtComic comic = downloadList[i];
                    if (comic.ImageUrl.Length == 0)
                        CtWebService.GetImageUrlFromHitomi(comic);

                    //CtDownloader ctDownload = new CtDownloader(comic, RootApp.MainDirectory);
                    CtDownloader ctDownload = new CtDownloader(comic);
                    ctDownload.OnProgressDownload += CtDownload_OnProgressDownload;
                    ctDownload.DownloadWork(false);
                    lock (downlaodLockObject)
                    {
                        i++;
                        Progress2 = (float)i / downloadList.Count;
                        if (i >= downloadList.Count)
                        {
                            downloadList.Clear();
                            break;
                        }
                    }
                }
                Progress2 = 0;
                ProgressBarIsVisible = false;
                ProgressBarHeight = 0;
                downloading = false;
            }
        }

        private void CtDownload_OnProgressDownload(object sender, CtDownloader.DownloadEventArgs e)
        {
            Progress1 = (e.Page + e.Byte) / e.Max;
        }

        private IWebConnection _webif;
        public ComicsViewModel(CtComicList comics, CtFilter filter, IWebConnection web, MenuItemType id) : base (id)
        {
            BaseComics = comics;
            CurrentFilter = filter;
            _webif = web;
            _service = new CtWebService(web);
            _tempModels = new CtModelList();
            LoadComicsCommand = new Command(async (object obj) => await ExecuteLoadItemsCommand());
            UpdateFromHitomiCommand = new Command(async (object obj) => await ExecuteAddFromHitomiCommand());

            MessagingCenter.Subscribe<NewComicPage, CtComic>(this, "AddItem", (obj, item) =>
            {
                var newItem = item as CtModel;
            });
        }

        public async Task<bool> ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return false;
            IsBusy = true;
            try
            {
                using (CtComicList comics = BaseComics ?? await CtDb.GetComics())
                {
                    using (CtComicList filterdlist = CurrentFilter.Filtering(comics))
                    {
                        if (CtComic.SortingColumn < 2)
                        {
                            Comics = filterdlist.OrderByDescending(item => item.SortingPropertie);
                        }
                        else
                        {
                            Comics = filterdlist.OrderBy(item => item.SortingPropertie);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
            return true;
        }

        private object ListViewUpdateLock = new object();
        private async Task ExecuteUpdateFromHitomiCommand(object workid)
        {
            CtComic comic = await CtWebService.searchWorkInfoFromWeb((string)workid);
            if (comic != null)
            {
                await CtDb.FixComicAsync(comic, _webif);
                await CtDb.UpdateComic(comic);
                RootPage.ListViewUpdateRequestAsync();
            }
        }

        private bool HitomiIsBusy;
        //public async Task ExecuteAddFromHitomiCommand()
        //{
        //    if (HitomiIsBusy)
        //        return;
        //    HitomiIsBusy = true;
        //    int count = 0;
        //    int error = 0;
        //    int index = 0;
        //    try
        //    {
        //        ProgressBarHeight = 8;
        //        ProgressBarIsVisible = true;
        //        Progress1 = 0;
        //        while (true)
        //        {
        //            Progress1 = (float)index / 1489;
        //            List<CtComic> comics = await _service.getPageComics(BaseComics ?? await CtDb.GetComics(), index++);
        //            if (comics == null)
        //            {
        //                await RootPage.DisplayAlert("알림", $"{index}페이지 연결에 문제가 있습니다.", "확인");
        //                continue;
        //            }
        //            foreach (CtComic comic in comics)
        //            {
        //                try
        //                {
        //                    await CtDb.FixComicAsync(comic, _webif);
        //                    await CtDb.InsertComic(comic);
        //                    count++;
        //                }
        //                catch
        //                {
        //                    error++;
        //                }
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //    }
        //    finally
        //    {
        //        ProgressBarIsVisible = false;
        //        ProgressBarHeight = 0;
        //        HitomiIsBusy = false;
        //        if(count > 0)
        //        {
        //            await ExecuteLoadItemsCommand();
        //            await RootPage.DisplayAlert("알림", $"총 {Comics.Count()}중 {count}개 추가 {error}개 픽스에러" , "확인");
        //        }
        //    }
        //}

        public async Task ExecuteAddFromHitomiCommand()
        {
            if (HitomiIsBusy)
                return;
            HitomiIsBusy = true;
            int count = 0;
            try
            {
                using (CtModelList newmodels = new CtModelList())
                {
                    //List<string> workids = new List<string> { "11111", "22222" };
                    List<string> workids = await _service.getLastestComicsId(BaseComics ?? await CtDb.GetComics());
                    if (workids == null)
                    {
                        await RootPage.DisplayAlert("알림", "연결에 문제가 있습니다.", "확인");
                        return;
                    }
                    ProgressBarHeight = 8;
                    ProgressBarIsVisible = true;
                    Progress1 = 0;
                    count = 0;

                    await Task.Run(async () =>
                    {
                        foreach (string workid in workids)
                        {
                            newmodels.Clear();
                            if ((await CtDb.GetComic(workid)) == null)
                            {

                                Progress1 = (float)count / workids.Count;
                                CtComic comic = await CtWebService.searchWorkInfoFromWeb(workid);
                                if (comic == null)
                                {
                                    await RootPage.DisplayAlert("알림", "연결에 문제가 있습니다.", "확인");
                                    break;
                                }
                                await CtDb.FixComicAsync(comic, _webif);



                                await CtDb.InsertComic(comic);
                            }
                            else
                            {
                                Console.WriteLine(workid + "는 이미 존재합니다.");
                            }
                            count++;
                        }
                    });
                    ProgressBarIsVisible = false;
                    ProgressBarHeight = 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                HitomiIsBusy = false;
                if (count > 0)
                {
                    await ExecuteLoadItemsCommand();
                    await RootPage.DisplayAlert("알림", count + "개 작품이 추가되었습니다.", "확인");
                }
            }
        }

        public async Task GetSuggestionItemAsync()
        {
            SuggestionItems = await CtDb.GetSuggestionItemsAsync(Key, CtModelType.ALL);
        }

        public async Task Search()
        {
            CurrentFilter.Key = Key;
            CurrentFilter.models.Clear();
            CurrentFilter.models.AddRange(_tempModels);
            await ExecuteLoadItemsCommand();
        }

        public void AddFilterItem(CtModel model)
        {
            _tempModels.Add(model);
            Key = "";
        }
        public void RemoveFilterItem(CtModel model)
        {
            _tempModels.Remove(model);
        }
        public void ClearFilterItem()
        {
            _tempModels.Clear();
            Key = "";
        }
    }
}
