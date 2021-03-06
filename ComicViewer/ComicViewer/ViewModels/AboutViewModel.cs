﻿using ComicViewer.Models;
using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace ComicViewer.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel() : base(MenuItemType.About)
        {
            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
        }

        public ICommand OpenWebCommand { get; }
    }
}