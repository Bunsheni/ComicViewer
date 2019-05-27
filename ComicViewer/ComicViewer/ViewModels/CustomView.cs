using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicViewer.ViewModels
{
    public class CustomWebView : WebView
    {
        AutoResetEvent waitForNavComplete;
        public CustomWebView()
        {
            this.Navigated += WebView_Navigated;
            waitForNavComplete = new AutoResetEvent(false);
        }
        private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            waitForNavComplete.Set();
        }
        private async Task WaitAsync()
        {
            await Task.Run(() => { waitForNavComplete.WaitOne(); });
            waitForNavComplete.Reset();
        }
    }
}
