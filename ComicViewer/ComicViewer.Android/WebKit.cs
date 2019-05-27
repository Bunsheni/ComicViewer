using System;
using Xamarin.Forms;
using ComicViewer.Droid;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Java.Interop;

[assembly: Dependency(typeof(WebKit))]

namespace ComicViewer.Droid
{
    public class WebKit : IWebKit
    {
        AutoResetEvent waitForNavComplete;

        async public Task<string> GetWebClintContents(string url)
        {
            waitForNavComplete = new AutoResetEvent(false);
            Xamarin.Forms.WebView webView = new Xamarin.Forms.WebView();
            webView.Navigated += WebView_Navigated;
            webView.Source = url;
            await Task.Run(() => { waitForNavComplete.WaitOne(); });
            // Reset the event (unset it)
            waitForNavComplete.Reset();
            string str = await webView.EvaluateJavaScriptAsync("document.body.innerHTML");
            return str;
        }

        private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            waitForNavComplete.Set();
        }
    }
    
}

