using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ComicViewer
{
    public interface IWebConnection
    {
        Task<string> GetWebClintContentsAsync(string url);
        Task<string> TransWebBrowserInitAsync(string text, string from, string to);
    }
}
