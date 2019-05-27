using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ComicViewer
{
    public interface IWebKit
    {
        Task<string> GetWebClintContents(string url);
    }
}
