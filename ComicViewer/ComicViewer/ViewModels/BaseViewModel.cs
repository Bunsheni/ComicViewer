using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

using ComicViewer.Models;
using ComicViewer.Services;
using ComicViewer.Views;

namespace ComicViewer.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private MenuItemType _type;
        public App RootApp { get => Application.Current as App; }
        public MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        public CtSQLiteDb CtDb { get => RootPage.CtDb; }
        public CtFileService FileService { get => RootPage.FileService; }

        public BaseViewModel()
        {

        }

        public BaseViewModel(MenuItemType id)
        {
            _type = id;
            OnPropertyChanged("Title");
        }

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        public string Title
        {
            get { return HomeMenuItem.GetTitle(_type); }
        }

        public void UpdateLanguage()
        {
            OnPropertyChanged("Title");
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

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
