using ComicViewer.CtModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComicViewer.ViewModels
{
    public class WebViewerModel : BaseViewModel
    {
        public CtComic Comic;
        public WebViewerModel() : base(MenuItemType.WebViewer) { }
    }
}
