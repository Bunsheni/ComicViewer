using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using System.Threading.Tasks;

using ComicViewer.CtModels;
using ComicViewer.Models;
using ComicViewer.ViewModels;
using System.Threading;
using ComicViewer.Services;

namespace ComicViewer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {

        public readonly Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public CtSQLiteDb CtDb { get; } = new CtSQLiteDb();
        public CtFileService FileService { get; } = new CtFileService();
        public WebPage WebViewerPage { get; } = new WebPage();
        public WebPage WebViewerPage2 { get; } = new WebPage();
        public WebPage GoogleTanslatePage { get; } = new WebPage("https://translate.google.co.kr/?hl=ko#view=home&op=translate&sl=ja&tl=ko&text=");
        public ImagePage ImagePage { get; } = new ImagePage();
        public ImageListPage ImageListPage { get; } = new ImageListPage();
        public ComicPage MainComicPage { get => mainComicPage; }
        public MenuPage MenuPage { get => Master as MenuPage; }

        public Page CurrentPage
        {
            get
            {
                if (Detail.GetType() == typeof(CustomNavigationPage))
                    return ((CustomNavigationPage)Detail).CurrentPage;
                else
                    return null;
            }
        }

        public void UpdateDetailsLanguage()
        {
            MenuPage.UpdateLanguages();
            for (int i = 0; i < MenuPages.Count; i++)
            {
                ((BaseViewModel)(MenuPages[i] as NavigationPage).RootPage.BindingContext).UpdateLanguage();
            }
        }

        public MainPage()
        {
            InitializeComponent();
            MasterBehavior = MasterBehavior.Popover;
            MenuPages.Add((int)MenuItemType.All, (CustomNavigationPage)Detail);
            MenuPages.Add((int)MenuItemType.Favorite, new CustomNavigationPage(new ComicPage(null, new CtFilter(string.Empty) { favorite = true })));
            MenuPages.Add((int)MenuItemType.WebViewer, new CustomNavigationPage(WebViewerPage2));
            MenuPages.Add((int)MenuItemType.Search, new CustomNavigationPage(new SearchPage()));
            MenuPages.Add((int)MenuItemType.Browse, new CustomNavigationPage(new ModelsTabPage()));
            MenuPages.Add((int)MenuItemType.NetWork, new CustomNavigationPage(new NetworkPage()));
            MenuPages.Add((int)MenuItemType.Setting, new CustomNavigationPage(new SettingPage()));
            MenuPages.Add((int)MenuItemType.About, new CustomNavigationPage(new AboutPage()));
        }
        
        protected async override void OnAppearing()
        {
            if (!CtDb.IsLoaded)
            {
                await CtDb.Load();
            }
            base.OnAppearing();
        }
        
        public void ListViewUpdateRequestAsync()
        {
            for(int i = 0; i < MenuPages.Count; i++)
            {
                if((MenuPages[i] as NavigationPage).RootPage.GetType() == typeof(ComicPage))
                {
                    ((MenuPages[i] as NavigationPage).RootPage as ComicPage).viewModel.LoadComicsCommand.Execute(null);
                }
            }
        }
        
        public async Task NavigateFromMenu(MenuItemType id)
        {
            var newPage = MenuPages[(int)id];
            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;
                if (Device.RuntimePlatform == Device.Android)
                {
                    if (id == MenuItemType.Browse)
                        await Task.Delay(500);
                    else
                        await Task.Delay(100);
                }
            }
            IsPresented = false;
        }
    }

    public class CustomNavigationPage : NavigationPage
    {
        public CustomNavigationPage() : base() { }
        public CustomNavigationPage(Page root) : base(root) { }

        protected override bool OnBackButtonPressed()
        {
            if(CurrentPage == RootPage)
            {
                Device.BeginInvokeOnMainThread(async() => 
                {
                    var answer = await DisplayAlert("Exit", "Do you wan't to exit the App?", "Yes", "No");
                    if (answer)
                    {
                        base.OnBackButtonPressed();
                    }
                });
            }
            else
            {
                return base.OnBackButtonPressed();
            }
            return true;
        }
    }
}