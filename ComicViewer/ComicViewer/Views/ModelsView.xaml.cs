using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ComicViewer.ViewModels;
using ComicViewer.CtModels;

namespace ComicViewer.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ModelsView : ContentPage
    {
        public ModelsViewModel viewModel;
		public ModelsView (CtModelType type)
		{
			InitializeComponent ();
            BindingContext = viewModel = new ModelsViewModel(type);
        }

        protected override void OnAppearing()
        {
            TabbedPage tabbedPage = (TabbedPage)Parent;

            if (tabbedPage != null && tabbedPage.CurrentPage == this)
                viewModel.LoadModelsCommand.Execute(null);
            base.OnAppearing();
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            viewModel.LoadModelsCommand.Execute(null);
        }

        private async void comicListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            string[] SortsEn = { "검색", "편집", "삭제" };
            string select = await DisplayActionSheet(null, null, null, SortsEn);
            CtModel model = e.Item as CtModel;
            if(select == "검색")
            {
                await viewModel.RootPage.NavigateFromMenu(MenuItemType.All);
                viewModel.RootPage.MainComicPage.ModelSearch(model, true);
            }
            else if(select == "편집")
            {
                await Navigation.PushAsync(new ModelDetailPage(model));
            }
            else if (select == "삭제")
            {
                await viewModel.CtDb.DeleteModel(model);

            }

        }
    }
}