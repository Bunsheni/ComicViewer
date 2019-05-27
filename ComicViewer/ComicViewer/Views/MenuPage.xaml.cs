using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ComicViewer.CtModels;
using ComicViewer.Models;
using ComicViewer.ViewModels;

namespace ComicViewer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public Command NavigateCommand { get; set; }
        public ObservableCollection<HomeMenuItem> MenuItems { get; set; }

        public MenuPage()
        {
            InitializeComponent();
            MenuItems = new ObservableCollection<HomeMenuItem>(new[]
            {
                new HomeMenuItem(MenuItemType.All),
                new HomeMenuItem(MenuItemType.Favorite),
                new HomeMenuItem(MenuItemType.WebViewer),
                new HomeMenuItem(MenuItemType.Search),
                new HomeMenuItem(MenuItemType.Browse),
                new HomeMenuItem(MenuItemType.NetWork),
                new HomeMenuItem(MenuItemType.Setting),
                new HomeMenuItem(MenuItemType.About)
            });
            NavigateCommand = new Command(async (object id) => await NavigateAsync(id));
            BindingContext = this;
        }
               
        public void UpdateLanguages()
        {
            foreach (HomeMenuItem item in MenuItems)
            {
                item.UpdateLanguage();
            }
        }

        public async Task NavigateAsync(object id)
        {
            await (Application.Current.MainPage as MainPage).NavigateFromMenu((MenuItemType)id);
        }

        private void ListViewMenu_ItemSelectedAsync(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                NavigateCommand.Execute(((HomeMenuItem)e.SelectedItem).Id);
                ListViewMenu.SelectedItem = null;
            }
        }
    }
}