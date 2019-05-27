using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;
using ComicViewer.CtModels;
using ComicViewer.Views;
using Xamarin.Essentials;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace ComicViewer
{
    public partial class App : Application
    {
        private const string LanguageKey = "ProgramLanguage";
        private const string NotificationsEnableKey = "NotificationsEnabled";
        private const string ImageHiddingEnableKey = "ImageHiddingEnabled";
        private const string MainDirectoryKey = "MainDirectory";
        private const string HighImageResolutionKey = "HighImageResolution";

        public bool HighImageResolution
        {
            get
            {
                if (Properties.ContainsKey(HighImageResolutionKey)
                    && Properties[HighImageResolutionKey] != null)
                    return (bool)Properties[HighImageResolutionKey];
                else
                    return false;
            }
            set
            {
                Properties[HighImageResolutionKey] = value;
                SavePropertiesAsync();
            }
        }

        public string MainDirectory
        {
            get
            {
                if (Properties.ContainsKey(MainDirectoryKey)
                    && Properties[MainDirectoryKey] != null)
                    return (string)Properties[MainDirectoryKey];
                else
                    return FileSystem.AppDataDirectory;
            }
            set
            {
                Properties[MainDirectoryKey] = value;
                SavePropertiesAsync();
            }
        }

        public string ProgramLanguage
        {
            get
            {
                if (Properties.ContainsKey(LanguageKey) && Properties[LanguageKey] != null)
                {
                    return Properties[LanguageKey].ToString();
                }
                else
                {
                    Properties[LanguageKey] = CtModel.LanguageStringArray[0];
                    SavePropertiesAsync();
                    return Properties[LanguageKey].ToString();
                }
            }
            set
            {
                Properties[LanguageKey] = value;
                SavePropertiesAsync();
            }
        }

        public CtLanguage CtProgramLanguage
        {
            get
            {
                int temp = CtModel.LanguageStringArray.ToList().IndexOf(ProgramLanguage);
                if (temp < 0)
                    return CtLanguage.KOREAN;
                else
                    return (CtLanguage)temp;
            }
            set
            {
                if ((int)value < -1)
                    ProgramLanguage = CtModel.LanguageStringArray[(int)CtLanguage.KOREAN];
                else
                    ProgramLanguage = CtModel.LanguageStringArray[(int)value];
            }
        }

        public bool NotificationsEnabled
        {
            get
            {
                if (Properties.ContainsKey(NotificationsEnableKey)
                    && Properties[NotificationsEnableKey] != null)
                    return (bool)Properties[NotificationsEnableKey];
                else
                    return false;
            }
            set
            {
                Properties[NotificationsEnableKey] = value;
                SavePropertiesAsync();
            }
        }

        public bool ImageHiddingEnabled
        {
            get
            {
                if (Properties.ContainsKey(ImageHiddingEnableKey)
                    && Properties[ImageHiddingEnableKey] != null)
                    return (bool)Properties[ImageHiddingEnableKey];
                else
                    return false;
            }
            set
            {
                Properties[ImageHiddingEnableKey] = value;
                SavePropertiesAsync();
            }
        }


        public App()
        {
            CtComic.HighImageResolution = HighImageResolution;
            CtModel.ProgramLanguage = CtProgramLanguage;
            InitializeComponent();
            MainPage = new ComicViewer.Views.MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            ((MainPage)MainPage).CtDb.Close();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
