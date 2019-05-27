using ComicViewer.CtModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ComicViewer.ViewModels;

namespace ComicViewer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModelsTabPage : TabbedPage
    {
        public ModelsTabPage ()
        {
            InitializeComponent();
            for(CtModelType i = CtModelType.ARTIST; i< CtModelType.COUNT; i++)
            {
                Children.Add(new ModelsView(i));
            }
            BindingContext = new BaseViewModel(MenuItemType.Browse);
        }

        private void AddItem_Clicked(object sender, EventArgs e)
        {
            CtModelType type = ((ModelsView)CurrentPage).viewModel.Type;
            if(type == CtModelType.ARTIST)
            {
                Navigation.PushAsync(new ModelDetailPage(new CtArtist()));
            }
            else if (type == CtModelType.GROUP)
            {
                Navigation.PushAsync(new ModelDetailPage(new CtGroup()));
            }
            else if (type == CtModelType.SERIES)
            {
                Navigation.PushAsync(new ModelDetailPage(new CtSeries()));
            }
            else if (type == CtModelType.CHARACTER)
            {
                Navigation.PushAsync(new ModelDetailPage(new CtCharacter()));
            }
            else if (type == CtModelType.TAG)
            {
                Navigation.PushAsync(new ModelDetailPage(new CtTag()));
            }
            else if (type == CtModelType.NAME)
            {
                Navigation.PushAsync(new ModelDetailPage(new CtName()));
            }
            else if (type == CtModelType.WORD)
            {
                Navigation.PushAsync(new ModelDetailPage(new CtWord()));
            }
        }

        private void DeleteClicked(object sender, EventArgs e)
        {
            //((ModelsView)SelectedItem).viewModel.
        }
    }
}