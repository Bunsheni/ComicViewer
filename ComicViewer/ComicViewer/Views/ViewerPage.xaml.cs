using ComicViewer.CtModels;
using ComicViewer.Services;
using SharpCifs.Smb;
using SharpCifs.Util.Sharpen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicViewer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ViewerPage : ContentPage
    {
        private int _pageindex;
        private ImageSource _imageSource;
        private ImageSource _nextSource;
        public ImageSource ImageSource { get { return _imageSource; } set { SetProperty(ref _imageSource, value); } }
        public ImageSource NextSource { get { return _nextSource; } set { SetProperty(ref _nextSource, value); } }
        public int PageIndex { get { return _pageindex; } set { SetProperty(ref _pageindex, value); } }

        public List<ImageSource> imageSources = new List<ImageSource>();

        public CtComic Comic;
        public ViewerPage(CtComic comic)
        {
            InitializeComponent();
            BindingContext = this;
            Comic = comic;
            Task.Run(() => ImageSourceRun());
        }

        protected override void OnAppearing()
        {
            PageIndex = 0;
            PageRun(PageIndex);
            base.OnAppearing();
        }

        private void OnTapGestureRecognizerTapped(object sender, EventArgs e)
        {
            PageIndex++;
            PageRun(PageIndex);
        }

        public void PageRun(int index)
        {
            if(Comic.Page > index)
            {
                while (imageSources.Count <= index)
                {
                    if (index != PageIndex) return;
                }
                ImageSource = imageSources[index];
            }
        }

        public void ImageSourceRun()
        {
            int index = 0;
            SmbFile[] files = CtSmb.GetSmbFileItem(Comic).file.ListFiles();
            while (files.Length > index)
            {
                InputStream inputStream = files[index].GetInputStream();
                MemoryStream ms = new MemoryStream();
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, bytesRead);
                }
                imageSources.Add(ImageSource.FromStream(() => { return ms; }));
                index++;
                files = CtSmb.GetSmbFileItem(Comic).file.ListFiles();
            }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

    }
}