using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ComicViewer.ViewModels;
using ComicViewer.CtModels;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Headers;
using System.IO;
using System.Collections.Generic;
using ComicViewer.Services;

namespace ComicViewer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ComicDetailPage : ContentPage, IWebConnection
    {
        private ComicDetailViewModel viewModel;
        public ComicDetailPage(CtComic comic)
        {
            InitializeComponent();
            BindingContext = viewModel = new ComicDetailViewModel(comic, this as IWebConnection);
            titleView.Children.Remove(titleEditor);
        }

        private void titleView_SizeChanged(object sender, EventArgs e)
        {
                titleEditor.WidthRequest = tableView.Width - 102;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            titleView.Children.Remove(titleLabel);
            titleView.Children.Add(titleEditor);
            titleView.Padding = new Thickness(0);
            titleEditor.Text = viewModel.ComicTitle;
            titleEditor.Focus();
        }

        private void TitleEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            string temp = e.NewTextValue;
            if (temp != null && (temp.Contains("\r") || temp.Contains("\n")))
            {
                viewModel.ComicTitle = titleEditor.Text = temp.Replace("\r", string.Empty).Replace("\n", string.Empty);
                Device.StartTimer(new TimeSpan(100), TestHandleFun);
            }
        }

        private bool TestHandleFun()
        {
            titleView.Children.Add(titleLabel);
            titleView.Children.Remove(titleEditor);
            titleView.Padding = new Thickness(0, 6);
            titleEditor.Unfocus();
            return false;
        }
        //private void TitleEditor_Completed(object sender, EventArgs e)
        //{
        //    titleView.Children.Remove(titleEditor);
        //    titleView.Children.Add(titleLabel);
        //}

        private async void UpdateItem_Clicked(object sender, EventArgs e)
        {
            TestHandleFun();
            await viewModel.UpdateComic();
        }

        private async void SaveItem_Clicked(object sender, EventArgs e)
        {
            TestHandleFun();
            await viewModel.SaveComic();
            viewModel.RootPage.ListViewUpdateRequestAsync();
            await DisplayAlert("알림", "저장되었습니다.", "확인");
        }

        private async void FixItem_Clicked(object sender, EventArgs e)
        {
            TestHandleFun();
            await viewModel.FixComic();
        }

        private void ClearItem_Clicked(object sender, EventArgs e)
        {
            TestHandleFun();
            viewModel.ClearComic();
        }

        private async void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length > 0)
            {
                List<CtSuggestionItem> suggestionItems = await viewModel.GetSuggestionItemsAsync(e.NewTextValue);
                if (suggestionItems.Count > 0)
                {
                    suggestionListView.ItemsSource = suggestionItems;
                    tableView.IsVisible = false;
                    suggestionView.IsVisible = true;
                }
                else
                {
                    tableView.IsVisible = true;
                    suggestionView.IsVisible = false;
                }
            }
            else
            {
                tableView.IsVisible = true;
                suggestionView.IsVisible = false;
            }
        }

        private void SuggestionListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            CtSuggestionItem item = e.SelectedItem as CtSuggestionItem;
            if (item != null)
            {
                viewModel.AddTag(item.Model);
            }
            searchBar.Text = string.Empty;
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            viewModel.DownloadComic();
        }
        
        public async Task<string> TransWebBrowserInitAsync(string text, string from, string to)
        {
            return await webView.TransWebBrowserInitAsync(text, from, to);
        }

        public async Task<string> GetWebClintContentsAsync(string url)
        {
            return await webView.GetWebClintContentsAsync(url);
        }
    }

    public class EditorGrows : Editor
    {
        public EditorGrows()
        {
            this.TextChanged += (sender, e) =>
            {
                this.InvalidateMeasure();
            };
        }
    }
}