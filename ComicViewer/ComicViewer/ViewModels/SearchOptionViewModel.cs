using ComicViewer.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using ComicViewer.CtModels;
using System.Threading.Tasks;
using ComicViewer.Models;

namespace ComicViewer.ViewModels
{
    public class SearchOptionViewModel : BaseViewModel
    {
        public SearchOptionViewModel() : base(MenuItemType.Search)
        {
        }

        public bool Korean
        {
            get
            {
                return CtFilter.Languages[(int)CtLanguage.KOREAN];
            }
            set
            {
                CtFilter.Languages[(int)CtLanguage.KOREAN] = value;
            }
        }
        public bool English
        {
            get
            {
                return CtFilter.Languages[(int)CtLanguage.ENGLISH];
            }
            set
            {
                CtFilter.Languages[(int)CtLanguage.ENGLISH] = value;
            }
        }
        public bool Japanese
        {
            get
            {
                return CtFilter.Languages[(int)CtLanguage.JAPANESE];
            }
            set
            {
                CtFilter.Languages[(int)CtLanguage.JAPANESE] = value;
            }
        }
        public bool Hidden
        {
            get
            {
                return CtFilter.Hidden;
            }
            set
            {
                CtFilter.Hidden = value;
            }
        }

        public bool Local
        {
            get
            {
                return CtFilter.Local;
            }
            set
            {
                CtFilter.Local = value;
            }
        }

        public bool Normal
        {
            get
            {
                return (CtFilter.Type2 == ComicType2.UNKNOWN || CtFilter.Type2 == ComicType2.NORMAL);
            }
            set
            {
                SetType2(value, Adult);
            }
        }
        public bool Adult
        {
            get
            {
                return (CtFilter.Type2 == ComicType2.UNKNOWN || CtFilter.Type2 == ComicType2.ADULT);
            }
            set
            {
                SetType2(Normal, value);
            }
        }

        internal void SetType2(bool normal, bool adult)
        {
            if (normal && !adult)
            {
                CtFilter.Type2 = ComicType2.NORMAL;
            }
            else if (!normal && adult)
            {
                CtFilter.Type2 = ComicType2.ADULT;
            }
            else
                CtFilter.Type2 = ComicType2.UNKNOWN;
        }

        public void UpdateRequest()
        {
            RootPage.ListViewUpdateRequestAsync();
        }
    }
}
