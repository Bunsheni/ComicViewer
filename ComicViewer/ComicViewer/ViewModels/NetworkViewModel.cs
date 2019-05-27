using ComicViewer.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ComicViewer.ViewModels
{
    public class NetworkViewModel : BaseViewModel
    {
        public NetworkViewModel() : base(MenuItemType.NetWork) { }

        ObservableCollection<SmbFileItem> _smblist = new ObservableCollection<SmbFileItem>();

        public ObservableCollection<SmbFileItem> FileList
        {
            get { return _smblist; }
            set { SetProperty(ref _smblist, value); }
        }
    }
}
