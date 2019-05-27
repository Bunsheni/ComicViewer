using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using ComicViewer.CtModels;

namespace ComicViewer.ViewModels
{
    class SettingViewModel : BaseViewModel
    {
        public SettingViewModel() : base(MenuItemType.Setting) { }

        public string ProgramLanguage
        {
            get
            {
                return RootApp.ProgramLanguage;
            }
            set
            {
                RootApp.ProgramLanguage = value;
                CtModel.ProgramLanguage = RootApp.CtProgramLanguage;
                RootPage.UpdateDetailsLanguage();
                OnPropertyChanged();
            }
        }

        public string MainDirectory
        {
            get
            {
                return RootApp.MainDirectory;
            }
            set
            {
                RootApp.MainDirectory = value;
                OnPropertyChanged();
            }
        }


        public bool NotificationsEnabled
        {
            get
            {
                return RootApp.NotificationsEnabled;
            }
            set
            {
                RootApp.NotificationsEnabled = value;
                OnPropertyChanged();
            }
        }
        
        public bool ImageHiddingEnabled
        {
            get
            {
                return RootApp.ImageHiddingEnabled;
            }
            set
            {
                RootApp.ImageHiddingEnabled = value;
                OnPropertyChanged();
            }
        }
        public bool HighImageResolutionEnabled
        {
            get
            {
                return RootApp.HighImageResolution;
            }
            set
            {
                RootApp.HighImageResolution = value;
                CtComic.HighImageResolution = RootApp.HighImageResolution;
                OnPropertyChanged();
            }
        }


        public string NotificationsText
        {
            get
            {
                switch(RootApp.CtProgramLanguage)
                {
                    case CtLanguage.KOREAN:
                        return "알림 설정";
                    default:
                        return "Notifications";
                }
            }
        }

        public string ImageHiddingText
        {
            get
            {
                switch (RootApp.CtProgramLanguage)
                {
                    case CtLanguage.KOREAN:
                        return "이미지 숨기기";
                    default:
                        return "Hide Image";
                }
            }
        }
        public string HighImageResolutionText
        {
            get
            {
                switch (RootApp.CtProgramLanguage)
                {
                    case CtLanguage.KOREAN:
                        return "고화질 이미지";
                    default:
                        return "High Image Resolution";
                }
            }
        }
    }
}
