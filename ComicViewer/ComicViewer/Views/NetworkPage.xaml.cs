using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCifs.Netbios;
using SharpCifs.Smb;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

using ComicViewer.Services;
using ComicViewer.ViewModels;

namespace ComicViewer.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NetworkPage : ContentPage
    {
        public NetworkViewModel viewModel;
        public NetworkPage ()
		{
			InitializeComponent ();
            BindingContext = viewModel = new NetworkViewModel();
		}

        private void Button_Clicked(object sender, EventArgs e)
        {
            SmbFileItem file = CtSmb.GetSmbFileItem();
            if(file != null)
                viewModel.FileList.Add(file);
            else
            {
                DisplayAlert("알리", "연결에 실패하였습니다.", "확인");
            }
        }
        
        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            SmbFileItem item = (SmbFileItem)e.SelectedItem;
            if(item.file.IsDirectory())
                viewModel.FileList = new ObservableCollection<SmbFileItem>(item.ListFiles());
            else if(item.file.IsFile())
            {
                if (item.Name.ToLower().EndsWith(".zip"))
                {

                }
            }
        }
    }
}