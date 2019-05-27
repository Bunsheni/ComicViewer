using System;
using Xamarin.Forms;
using System.Threading;
using System.Threading.Tasks;
using ComicViewer.iOS;
using Foundation;

[assembly: Dependency(typeof(ComicViewer.iOS.WebKit))]

namespace ComicViewer.iOS
{
    public class WebKit : IWebKit
    {
        public async Task<string> GetWebClintContentsAsync(string url)
        {
            return url;
        }
        Task<string> IWebKit.GetWebClintContents(string url)
        {
            return GetWebClintContentsAsync(url);
        }
    }
}

