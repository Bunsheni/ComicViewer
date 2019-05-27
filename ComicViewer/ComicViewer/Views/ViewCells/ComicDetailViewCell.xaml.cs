
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Xamarin.Essentials;

using ComicViewer.CtModels;
using ComicViewer.Services;

namespace ComicViewer.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ComicDetailViewCell : ViewCell
    {
        private bool pageLock;
        private App RootApp { get => Application.Current as App; }
        private MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        private CtSQLiteDb CtDb { get => RootPage.CtDb; }
        public Page ParentPageView { get => RootPage.CurrentPage; }
        public CtComic Comic
        {
            get => BindingContext as CtComic;
        }
        public List<CtModel> Comics
        {
            get
            {
                if(ParentPageView.GetType() == typeof(ComicPage))
                    return ((ComicPage)ParentPageView).viewModel.Comics.ToList();
                else if (ParentPageView.GetType() == typeof(ModelDetailPage))
                    return ((ModelDetailPage)ParentPageView).viewModel.Comics.ToList();
                return null;
            }
        }

        public ComicDetailViewCell ()
		{
            InitializeComponent ();
        }

        public int Index
        {
            get
            {
                if (Comics != null)
                    return Comics.IndexOf(Comic);
                else
                    return 0;
            }
        }


        protected override void OnAppearing()
        {
            if(BindingContext != null)
            {
                if (((CtComic)this.BindingContext).IsFavorite)
                    Favorite.Text = "★";
                else
                    Favorite.Text = "☆";
            }
            base.OnAppearing();
        }

        private async void OnTapGestureRecognizerTapped(object sender, EventArgs e)
        {
            if (pageLock) return;
            pageLock = true;
            //RootPage.ImagePage.ImageLoad(((CtComic)BindingContext).GetPageImageUrl(0), ((Image)sender).Source);
            //await Application.Current.MainPage.Navigation.PushModalAsync(RootPage.ImagePage);
            if(Comics != null)
            {
                RootPage.ImageListPage.ImageLoad(Comics, Index);
                await ParentPageView.Navigation.PushAsync(RootPage.ImageListPage);
            }
            pageLock = false;
        }

        private async void ArtistAndGroupTapped(object sender, EventArgs e)
        {
            if (pageLock) return;
            pageLock = true;
            CtModelList models = new CtModelList();
            foreach (string str in ((CtComic)this.BindingContext).GetTags(CtModelType.ARTIST, false))
            {
                models.Add(await CtDb.GetModel(CtModelType.ARTIST, str));
            }
            foreach (string str in ((CtComic)this.BindingContext).GetTags(CtModelType.GROUP, false))
            {
                models.Add(await CtDb.GetModel(CtModelType.GROUP, str));
            }
            if (models.Count > 0)
            {
                DisplayAndSearch(models);
            }
            pageLock = false;
        }
        private async void SeriesTapped(object sender, EventArgs e)
        {
            if (pageLock) return;
            pageLock = true;
            CtModelList models = new CtModelList();
            foreach (string str in ((CtComic)this.BindingContext).GetTags(CtModelType.SERIES, false))
            {
                models.Add(await CtDb.GetModel(CtModelType.SERIES, str));
            }
            if (models.Count > 0)
            {
                DisplayAndSearch(models);
            }
            pageLock = false;
        }
        private async void CharacterTapped(object sender, EventArgs e)
        {
            if (pageLock) return;
            pageLock = true;
            CtModelList models = new CtModelList();
            foreach (string str in ((CtComic)this.BindingContext).GetTags(CtModelType.CHARACTER, false))
            {
                models.Add(await CtDb.GetModel(CtModelType.CHARACTER, str));
            }
            if (models.Count > 0)
            {
                DisplayAndSearch(models);
            }
            pageLock = false;
        }
        private async void TagTapped(object sender, EventArgs e)
        {
            if (pageLock) return;
            pageLock = true;
            CtModelList models = new CtModelList();
            foreach (string str in ((CtComic)this.BindingContext).GetTags(CtModelType.TAG, false))
            {
                models.Add(await CtDb.GetModel(CtModelType.TAG, str));
            }
            if (models.Count > 0)
            {
                DisplayAndSearch(models);
            }
            pageLock = false;
        }

        private async void FavoriteTapped(object sender, EventArgs e)
        {
            CtComic comic = BindingContext as CtComic;
            comic.IsFavorite = !comic.IsFavorite;
            if (comic.IsFavorite)
                Favorite.Text = "★";
            else
                Favorite.Text = "☆";
            await CtDb.UpdateComic(comic);
        }

        public async void DisplayAndSearch(CtModelList models)
        {
            List<string> dis = new List<string>();
            foreach (CtModel model in models)
            {
                dis.Add(model.Name);
            }
            string select = await ParentPageView.DisplayActionSheet(null, null, null, dis.ToArray());
            int selectedindex = dis.FindIndex(i => i == select);
            if (selectedindex < 0) return;
            CtModel selectedModel = models[selectedindex];
            RootPage.MainComicPage.ModelSearch(selectedModel, false);
            await RootPage.NavigateFromMenu(ViewModels.MenuItemType.All);
        }
    }
}