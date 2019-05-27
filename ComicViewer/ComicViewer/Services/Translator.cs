using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;

namespace ComicViewer.Services
{
    class Translator
    {
        private object lockObject = new object();
        private string from = "ja";
        private string to = "ko";
        WebView webView;
        AutoResetEvent waitForNavComplete;

        public Translator(WebView webView)
        {
            this.webView = webView;
            webView.Navigated += WebView_Navigated;
            waitForNavComplete = new AutoResetEvent(false);
        }

        private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            waitForNavComplete.Set();
        }

        public void set_language(string from, string to)
        {
            this.from = from;
            this.to = to;

        }

        public async Task transWebBrowserInitAsync(string text, string from, string to)
        {
            string res = "";
            try
            {
                webView.Source  = "http://text-to-speech-translator.paralink.com/default.asp";
                await Task.Run(() => { waitForNavComplete.WaitOne(); });
                waitForNavComplete.Reset();

                await webView.EvaluateJavaScriptAsync(string.Format("document.getElementById(\"{0}\").value = \"{1}\"", "langs1", from));
                await webView.EvaluateJavaScriptAsync(string.Format("DDMENU(0)"));
                await webView.EvaluateJavaScriptAsync(string.Format("document.getElementById(\"{0}\").value = \"{1}\"", "langs2", to));
                await webView.EvaluateJavaScriptAsync(string.Format("DDMENU(1)"));
                await webView.EvaluateJavaScriptAsync("LABLE('google');");
                await webView.EvaluateJavaScriptAsync(string.Format("document.getElementById(\"{0}\").value = \"{1}\"", "source", text));
                await webView.EvaluateJavaScriptAsync("translateTEXTonload(GEBI('source').value);");
                await Task.Run(() => { waitForNavComplete.WaitOne(); });
                waitForNavComplete.Reset();

                res = await webView.EvaluateJavaScriptAsync("document.getElementById(\"target\").value;");

            }
            catch
            {
                Console.WriteLine("번역기 서버가 불안정합니다.", "알림");
            }
        }
    }
}
