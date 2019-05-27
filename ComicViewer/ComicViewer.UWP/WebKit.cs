using System;
using System.IO;
using SQLite;
using Xamarin.Forms;
using ComicViewer.Windows;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using WebView = Windows.UI.Xaml.Controls.WebView;

[assembly: Dependency(typeof(WebKit))]
namespace ComicViewer.Windows
{
    public class WebKit : IWebKit
    {
        Uri source;

        AutoResetEvent waitForNavComplete;

        public async Task<string> GetWebClintContentsAsync(string url)
        {
            this.source = new Uri(url);
            WebView webView = new WebView();
            waitForNavComplete = new AutoResetEvent(false);
            webView.NavigationCompleted += WebView_NavigationCompleted;
            webView.Navigate(source);
            // Wait for this AutoResetEvent to be Set
            // If you just did the Wait the WebView would not come up and the UI would freeze
            await Task.Run(() => { waitForNavComplete.WaitOne(); });
            // Reset the event (unset it)
            waitForNavComplete.Reset();

            string res = await webView.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });
            return res;
        }

        Task<string> IWebKit.GetWebClintContents(string url)
        {
            return GetWebClintContentsAsync(url);
        }

        private void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            // I am waiting for a specific Uri.  When I see it...
            if (args.Uri == source)
            {
                // Reset the event so we close the browser and bring up the old XAML
                waitForNavComplete.Set();
            }
        }
    }
}

