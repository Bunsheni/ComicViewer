using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using ComicViewer.CtModels;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace ComicViewer.ViewModels
{
    public class ModelsViewModel : BaseViewModel
    {
        public Command LoadModelsCommand { get; set; }
        private IEnumerable<CtModel> _models;      
        public IEnumerable<CtModel> Models
        {
            get { return _models; }
            set { SetProperty(ref _models, value); }
        }

        public string Key { get; set; }

        public CtModelType Type { get; set; }

        public string ModelTitle { get; set; }


        public ModelsViewModel(CtModelType type)
        {
            Key = string.Empty;
            Type = type;
            ModelTitle = CtModel.GetModelName(type);
            LoadModelsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }


        private async Task ExecuteLoadItemsCommand()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                try
                {
                    if(Key.Length != 0)
                    {
                        using (CtModelList models = await RootPage.CtDb.GetModels(Type))
                        {
                            Models = (models.FindAll(i => i.GetTags().Exists(j => j.ToLower().StartsWith(Key.ToLower())))).OrderBy(item => item.SortingPropertie);
                        }
                    }
                    else
                    {
                        Models = (await RootPage.CtDb.GetModels(Type)).OrderBy(item => item.SortingPropertie);
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
            }
        }
    }
}
