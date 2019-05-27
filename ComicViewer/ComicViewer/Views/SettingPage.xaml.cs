using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ComicViewer.CtModels;
using ComicViewer.Views;
using ComicViewer.ViewModels;

namespace ComicViewer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingPage : ContentPage
    {
        private SettingViewModel viewModel;
        public SettingPage()
        {
            BindingContext = viewModel = new SettingViewModel();
            InitializeComponent ();
        }

        protected override void OnDisappearing()
        {
            viewModel.RootPage.ListViewUpdateRequestAsync();
            base.OnDisappearing();
        }

        private async void btnPopupButton_Clicked(object sender, EventArgs e)
        {
            string language = await DisplayActionSheet(null, null, null, CtModel.LanguageStringArray);
            viewModel.ProgramLanguage = language;
        }
    }
}