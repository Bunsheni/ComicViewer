using ComicViewer.CtModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace ComicViewer.ViewModels
{
    public enum MenuItemType
    {
        All,
        Favorite,
        WebViewer,
        Search,
        Browse,
        NetWork,
        Setting,
        About
    }
    public class HomeMenuItem : INotifyPropertyChanged
    {
        private static readonly string[] _title_en = new string[]{"All", "Favorite", "WebViewer", "Search", "Browser", "Network", "Setting", "About" };
        private static readonly string[] _title_kr = new string[] { "전체목록", "즐겨찾기", "웹 뷰어", "검색옵션", "태그목록", "네트워크","설정", "어플리케이션 정보" };
        public MenuItemType Id { get; set; }
        public string Title
        {
            get
            {
                return GetTitle(Id);
            }
        }
        public HomeMenuItem(MenuItemType id)
        {
            Id = id;
        }

        public static string GetTitle(MenuItemType id)
        {
            if (((App)Application.Current).CtProgramLanguage == CtLanguage.KOREAN)
                return _title_kr[(int)id];
            else
                return _title_en[(int)id];
        }

        public void UpdateLanguage()
        {
            OnPropertyChanged("Title");
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
