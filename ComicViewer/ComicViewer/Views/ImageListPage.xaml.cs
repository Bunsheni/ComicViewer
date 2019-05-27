using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ComicViewer.CtModels;
using ComicViewer.Services;
using System.IO;

namespace ComicViewer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ImageListPage : ContentPage
    {
        public MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        public CtFileService FileService { get => RootPage.FileService; }
        public List<CtModel> Comics;
        public int CurrentIndex;
        public int CurrentPage;

        public ImageListPage()
		{
            CurrentIndex = 0;
            CurrentPage = 0;
            InitializeComponent ();
            BindingContext = this;
        }
        
        public async void ImageRefresh()
        {
            if(CurrentPage > 0)
            {
                imageView.LoadingPlaceholder = null;
            }
            else
            {
                imageView.LoadingPlaceholder = ((CtComic)Comics[CurrentIndex]).CoverUrl;
            }
            if (await FileService.ExistComicImageFile((CtComic)Comics[CurrentIndex], CurrentPage))
                imageView.Source = Path.Combine(FileService.GetBaseDirectory(), FileService.GetImageFilePath((CtComic)Comics[CurrentIndex], CurrentPage));
            else
                imageView.Source = ((CtComic)Comics[CurrentIndex]).GetPageImageUrl(CurrentPage);
        }

        public void ImageLoad(List<CtModel> comics, int index)
        {
            Comics = comics;
            if (CurrentIndex != index)
            {
                CurrentIndex = index;
                CurrentPage = 0;
            }
            ImageRefresh();
        }

        public void ImageLoad(int index)
        {
            CurrentIndex = index;
            CurrentPage = 0;
            ImageRefresh();
        }

        private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            switch (e.Direction)
            {
                case SwipeDirection.Left:
                    if (CurrentPage < ((CtComic)Comics[CurrentIndex]).Page - 1)
                    {
                        CurrentPage++;
                        ImageRefresh();
                    }
                    break;
                case SwipeDirection.Right:
                    if (CurrentPage > 0)
                    {
                        CurrentPage--;
                        ImageRefresh();
                    }
                    break;
                case SwipeDirection.Up:
                    CurrentPage = 0;
                    if (CurrentIndex < Comics.Count - 1)
                    {
                        CurrentIndex++;
                        ImageRefresh();
                    }
                    break;
                case SwipeDirection.Down:
                    CurrentPage = 0;
                    if (CurrentIndex > 0)
                    {
                        CurrentIndex--;
                        ImageRefresh();
                    }
                    break;
            }
        }

        private void Pre_Clicked(object sender, EventArgs e)
        {
            CurrentPage = 0;
            if (CurrentIndex > 0)
            {
                CurrentIndex--;
                ImageRefresh();
            }
        }

        private void Next_Clicked(object sender, EventArgs e)
        {
            CurrentPage = 0;
            if (CurrentIndex < Comics.Count - 1)
            {
                CurrentIndex++;
                ImageRefresh();
            }
        }

        private void Refresh_Clicked(object sender, EventArgs e)
        {
            ImageRefresh();
        }

        private void Download_Clicked(object sender, EventArgs e)
        {

        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (CurrentPage < ((CtComic)Comics[CurrentIndex]).Page - 1)
            {
                CurrentPage++;
                ImageRefresh();
            }
        }
    }
}