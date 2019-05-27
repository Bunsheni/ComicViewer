using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using ComicViewer.CtModels;
using Xamarin.Forms;

namespace ComicViewer.ViewModels
{
    public class ModelDetailViewModel : BaseViewModel
    {
        private ObservableCollection<CtModel> _comics = new ObservableCollection<CtModel>();
        private double _progress;
        public double ProgressValue
        {
            get
            {
                return _progress;
            }
            set
            {
                SetProperty(ref _progress, value);
            }
        }

        public ObservableCollection<CtModel> Comics
        {
            get { return _comics; }
            set { SetProperty(ref _comics, value); }
        }

        bool _progresson;
        public bool ProgressOn { get { return _progresson; } set { SetProperty(ref _progresson, value); } }

        public CtModel Model;
        public CtModel OldModel;
        public ModelDetailViewModel(CtModel model)
        {
            _progresson = false;
            Model = model.Clone();
            OldModel = model;
        }

        public async Task<List<CtModel>> GetNameCharacter(string nameen)
        {
            return (await CtDb.GetCharacters()).FindAll(i => (string.Compare(i.NameEn, nameen, true) == 0) || Library.StringDivider(i.NameEn, " ").Exists(j => string.Compare(j, nameen, true) == 0));
        }

        public async Task<CtComicList> SaveWord(CtWordList words)
        {
            int count = 0;
            int progress = 0;
            if (words != null && words.Count != 0)
            {
                CtComicList fixedModels = new CtComicList();
                List<CtModel> comics = (await CtDb.GetComics()).FindAll(i => ((CtComic)i).TitleContains(words));
                CtWordList wordset = new CtWordList();
                wordset.AddRange(await CtDb.GetWords());
                wordset.UpdateAddModel(words);
                ProgressOn = true;
                foreach (CtComic comic in comics)
                {
                    CtComic clone = comic.Clone() as CtComic;
                    try
                    {
                        if (await clone.TranslateTitle(wordset, await CtDb.GetNames(), null))
                        {
                            count++;
                            fixedModels.Add(clone);
                        }
                    }
                    catch
                    {            

                    }
                    finally
                    {
                        progress++;
                        ProgressValue = (double)progress / comics.Count;
                    }
                }
                return fixedModels;
            }
            return null;
        }

        public async Task<CtModelList> FixModel(CtModelList models)
        {
            int count = 0;
            int progress = 0;
            CtComicList fixedModels = new CtComicList();
            if (models.Count > 0)
            {
                List<CtModel> comics = new List<CtModel>();

                CtNameList nameset = new CtNameList();
                nameset.AddRange(await CtDb.GetNames());

                foreach (CtModel model in models)
                {
                    comics.AddRange((await CtDb.GetComics()).FindAll(i => ((CtComic)i).Contains(model) || ((CtComic)i).TitleContains(model.NameEn)));
                    if(model.CtType == CtModelType.NAME)
                        nameset.UpdateAddModel(model);
                }
                ProgressOn = true;


                foreach (CtComic comic in comics)
                {
                    CtComic clone = comic.Clone() as CtComic;
                    try
                    {
                        bool res = false;
                        foreach(CtModel model in models)
                        {
                            res = clone.FixModel(model) || res;
                        }
                        res = clone.FixTitle() || res;
                        if (!clone.IsTranslated)
                        {
                            res = await clone.TranslateTitle(await CtDb.GetWords(), nameset, null) || res;
                        }
                        if (res)
                        {
                            fixedModels.Add(clone);
                        }
                        count++;
                    }
                    catch
                    {
                        CtComic newComic = await CtWebService.searchWorkInfoFromWeb(clone.Workid);                        
                        await CtDb.FixComicAsync(newComic, null);
                        fixedModels.Add(newComic);
                        count++;
                    }
                    finally
                    {
                        progress++;
                        ProgressValue = (double)progress / comics.Count;
                    }                    
                }
                ProgressOn = false;
            }
            return fixedModels;
        }

        public async Task SaveModels(CtModelList models)
        {
            await CtDb.UpdateAddModels(models);
            RootPage.ListViewUpdateRequestAsync();
        }
    }
}
