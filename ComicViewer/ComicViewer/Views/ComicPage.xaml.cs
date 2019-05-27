using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Threading;
using ComicViewer.CtModels;
using ComicViewer.ViewModels;
using ComicViewer.Models;
using Xamarin.Essentials;

namespace ComicViewer.Views
{
    public partial class ComicPage : ContentPage, IWebConnection
    {
        private bool Main;
        private bool SelectMode;
        public CtFilter Filter{ get; set; }

        public ComicsViewModel viewModel;

        public ComicPage(CtComicList models, CtFilter filter)
        {
            InitializeComponent();
            comicListView.HeightRequest = abslayout.Height;
            Filter = filter;
            BindingContext = viewModel = new ComicsViewModel(models, Filter, this, MenuItemType.Favorite);
        }

        public ComicPage()
        {
            Main = true;
            InitializeComponent();
            comicListView.HeightRequest = abslayout.Height;
            Filter = new CtFilter("");
            BindingContext = viewModel = new ComicsViewModel(null, Filter, this, MenuItemType.All);
        }

        protected override void OnAppearing()
        {
            if (viewModel.Comics == null)
            {
                Task.Run(async () => {
                    comicListView.IsRefreshing = true;
                    bool res = await viewModel.ExecuteLoadItemsCommand();
                    if (res) comicListView.IsRefreshing = false;
                });
                if (Main)
                {
                    viewModel.UpdateFromHitomiCommand.Execute(null);
                }
            }
            base.OnAppearing();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private void SearchBar_Completed(object sender, EventArgs e)
        {

        }
        
        private void ClearSelect_Clicked(object sender, EventArgs e)
        {
            SelectMode = !SelectMode;
            if (SelectMode)
            {
                SelectionBox.Icon = "Icon\\selected.png";
                SelectionBox.Text = "0 Select";
            }
            else
            {
                viewModel.ClearSelect();
                SelectionBox.Icon = "Icon\\nonselected.png";
                SelectionBox.Text = "SelectMode";
            }
        }

        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchBar.Text.Trim().Length != 0)
            {
                await Task.Run(async () => { string a = searchBar.Text; await viewModel.GetSuggestionItemAsync(); if (a != searchBar.Text) SearchBar_TextChanged(null, null); });
                suggestionView.IsVisible = true;
                contentView.IsVisible = false;
            }
            else
            {
                suggestionView.IsVisible = false;
                contentView.IsVisible = true;
                if (viewModel.CurrentFilter.Key.Length != 0 || viewModel.CurrentFilter.models.Count > 0)
                {
                    SearchAction();
                }
            }
        }

        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            SearchAction();
            suggestionView.IsVisible = false;
            contentView.IsVisible = true;
        }

        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewComicPage()));
        }

        private async void RefreshItem_Clicked(object sender, EventArgs e)
        {
            comicListView.IsRefreshing = true;
            bool res = await viewModel.ExecuteLoadItemsCommand();
            if (res) comicListView.IsRefreshing = false;
        }

        public async Task<string> TransWebBrowserInitAsync(string text, string from, string to)
        {
            return await webView.TransWebBrowserInitAsync(text, from, to);
        }

        public async Task<string> GetWebClintContentsAsync(string url)
        {
            return await webView.GetWebClintContentsAsync(url);
        }
               
        private void SuggestionListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            CtSuggestionItem item = (CtSuggestionItem)e.SelectedItem;
            if (item != null)
            {
                suggestionView.IsVisible = false;
                contentView.IsVisible = true;
                ModelSearch(item.Model, false);
            }
        }

        private void Download_Clicked(object sender, EventArgs e)
        {
            Task.Run(() => viewModel.DownloadComics());
        }
        

        private void Web_Clicked(object sender, EventArgs e)
        {
            Task.Run(async () => {
                await viewModel.UpdateComicsAsync();
            });
            
        }

        private void Update_Clicked(object sender, EventArgs e)
        {
            Task.Run(async () => {
                await viewModel.UpdateComicsAsync();
            });

        }

        private void SearchBox_ChildAdded(object sender, ElementEventArgs e)
        {
            SearchAction();
        }

        private async void Sort_Clicked(object sender, EventArgs e)
        {
            string temp = await DisplayActionSheet("정렬", "취소", null, CtComic._colunmText);
            if (string.Compare("취소", temp) != 0)
            {
                CtComic.SetSortingColumn(temp);
                viewModel.LoadComicsCommand.Execute(null);
            }
        }

        private async void SearchAction()
        {
            comicListView.IsRefreshing = true;
            await Task.Run(() => viewModel.Search());
            comicListView.IsRefreshing = false;
        }

        public async void DiplaySearchAlert(List<CtModel> models)
        {
            List<string> dis = new List<string>();
            foreach (CtModel model in models)
            {
                dis.Add(model.Name);
            }
            string select = await DisplayActionSheet(null, null, null, dis.ToArray());
            int selectedindex = dis.FindIndex(i => i == select);
            if (selectedindex < 0) return;
            CtModel selectedModel = models[selectedindex];
            ModelSearch(selectedModel, false);
        }

        public void ModelSearch(CtModel model, bool clear)
        {
            if (clear)
            {
                viewModel.ClearFilterItem();
                searchBox.Children.Clear();
            }
            viewModel.AddFilterItem(model);
            searchBox.Children.Add(new ModelLabel(model, viewModel));
        }

        private void ComicListView_Refreshing(object sender, EventArgs e)
        {
            Task.Run(async () => {
                comicListView.IsRefreshing = true;
                bool res = await viewModel.ExecuteLoadItemsCommand();
                if (res) comicListView.IsRefreshing = false;
            });
        }

        bool pageLock;
        private async void ComicListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (pageLock) return;
            pageLock = true;
            CtComic selectedComic = (CtComic)e.Item;
            if(SelectMode)
            {
                selectedComic.IsSelected ^= true;
                comicListView.SelectedItem = null;
                if (selectedComic.IsSelected)
                    viewModel.selectedComics.Add(selectedComic);
                else
                    viewModel.selectedComics.Remove(selectedComic);
                SelectionBox.Text = $"{viewModel.selectedComics.Count} Select";
            }
            else
            {
                string[] strs = { "웹 뷰어", "뷰어", "편집", "제목 복사", "단어 편집", "표지 보기", selectedComic.IsFavorite ? "즐겨찾기 해제" : "즐겨찾기 추가", selectedComic.IsHidden ? "숨김 해제" : "숨기기" };
                string select = await DisplayActionSheet(selectedComic.Title, null, null, strs);

                if (select == "웹 뷰어")
                {
                    viewModel.RootPage.WebViewerPage.Navigate(selectedComic.GetWebViewerUrl());
                    //await viewModel.RootPage.NavigateFromMenu(ViewModels.MenuItemType.WebViewer);
                    await Navigation.PushModalAsync(viewModel.RootPage.WebViewerPage);
                }
                else if (select == "뷰어")
                {
                    //await Navigation.PushModalAsync(new ViewerPage(selectedComic));
                }
                else if (select == "편집")
                {
                    await Navigation.PushAsync(new ComicDetailPage(selectedComic));
                }
                else if (select == "제목 복사")
                {
                    await Clipboard.SetTextAsync(selectedComic.TitleEn);
                }
                else if (select == "단어 편집")
                {
                    await Navigation.PushAsync(new ModelDetailPage(new CtWord(selectedComic.TitleEn, selectedComic.TitleKr, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)));
                }
                else if (select == "표지 보기")
                {
                    viewModel.RootPage.ImagePage.ImageLoad(selectedComic.GetPageImageUrl(0), selectedComic.CoverUrl);
                    await Navigation.PushModalAsync(viewModel.RootPage.ImagePage);
                }
                else if (select == "즐겨찾기 해제")
                {
                    selectedComic.IsFavorite = false;
                    await viewModel.CtDb.UpdateComic(selectedComic);
                }
                else if (select == "즐겨찾기 추가")
                {
                    selectedComic.IsFavorite = true;
                    await viewModel.CtDb.UpdateComic(selectedComic);
                }
                else if (select == "숨김 해제")
                {
                    selectedComic.IsHidden = false;
                    await viewModel.CtDb.UpdateComic(selectedComic);
                }
                else if (select == "숨기기")
                {
                    selectedComic.IsHidden = true;
                    await viewModel.CtDb.UpdateComic(selectedComic);
                }
            }
            pageLock = false;
        }
    }

    class ModelLabel : Label
    {
        static Thickness margin = new Thickness(5, 0);
        CtModel model;
        ComicsViewModel viewModel;
        public ModelLabel(CtModel model, ComicsViewModel viewModel)
        {
            this.Margin = margin;
            this.model = model;
            this.viewModel = viewModel;
            this.Text = model.Text;
            this.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    viewModel.RemoveFilterItem(model);
                    ((FlexLayout)Parent).Children.Remove(this);
                }),
                NumberOfTapsRequired = 1
            });
        }
    }
}
