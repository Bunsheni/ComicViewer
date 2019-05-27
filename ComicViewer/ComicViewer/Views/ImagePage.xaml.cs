using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicViewer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ImagePage : ContentPage
    {
        public ImagePage()
        {
            InitializeComponent();
            BindingContext = this;
        }
        public ImagePage (ImageSource source)
		{
			InitializeComponent ();
            imageView.Source = source;
            BindingContext = this;
        }

        public void ImageLoad(ImageSource source)
        {
            //imageView.LoadingPlaceholder = null;
            imageView.Source = source;
        }

        public void ImageLoad(ImageSource source, ImageSource loadingimage)
        {
            imageView.LoadingPlaceholder = loadingimage;
            imageView.Source = source;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            switch (e.Direction)
            {
                case SwipeDirection.Left:
                    // Handle the swipe
                    break;
                case SwipeDirection.Right:
                    // Handle the swipe
                    break;
                case SwipeDirection.Up:
                    // Handle the swipe
                    break;
                case SwipeDirection.Down:
                    // Handle the swipe
                    break;
            }
        }

        private void Refresh_Clicked(object sender, EventArgs e)
        {

        }

        private void Download_Clicked(object sender, EventArgs e)
        {

        }
    }
}