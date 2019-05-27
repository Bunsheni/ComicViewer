using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Specialized;
using System.IO;
using ComicViewer.CtModels;

namespace ComicViewer
{
    class CtService
    {        
        private static string[] languageStringList = { "korean", "english", "japanese" };
        IWebConnection webConnection;
        public CtService(IWebConnection web)
        {
            webConnection = web;
        }

        public async Task<List<string>> getLastestComicsId(CtModelList comics)
        {
            int count;
            List<string> workids = new List<string>();
            string url, tempstr, infostr;
            int index = 1;
            int errorcount = 0;
            string Language = languageStringList[(int)CtLanguage.KOREAN];
            string path = "https://hitomi.la/index-" + Language + "-";
            tempstr = string.Empty;
            Language = languageStringList[(int)CtLanguage.KOREAN];
            path = "https://hitomi.la/index-" + Language + "-";
            path = path.Replace(" ", "%20").ToLower();     
            try
            {
                while (true)
                {
                    count = 0;
                    url = path + index + ".html";
                    try
                    {
                        redd:
                        infostr = await webConnection.GetWebClintContentsAsync(url);
                        if (!infostr.Contains("<div class=\"gallery-content\">"))
                        {
                            errorcount++;
                            if (errorcount > 10)
                                return null;
                            goto redd;
                        }
                    }
                    catch
                    {
                        Console.WriteLine("페이지다운로드 실패");
                        break;
                    }
                    if (infostr.Contains("<div class=\"gallery-content\">"))
                    {
                        infostr = infostr.Substring(infostr.IndexOf("<div class=\"gallery-content\">"));
                        while (infostr.Contains("<a href=\"/galleries/"))
                        {
                            try
                            {
                                Library.extractionString(infostr, out infostr, "href=\"/galleries/", ".html", out tempstr);
                                if (comics.GetModel(tempstr) == null)
                                {
                                    workids.Insert(0, tempstr);
                                    infostr = infostr.Substring(infostr.IndexOf("</div>\n</div>"));
                                    count++;
                                }
                            }
                            catch
                            {
                                infostr = infostr.Substring(infostr.IndexOf("</div>\n</div>"));
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                    if (count == 0)
                        break;
                    index++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return workids;
        }
        
        public static async Task<CtComic> searchWorkInfoFromWeb(string workid)
        {
            int i;
            DateTime date;
            CtComic tempWork = new CtComic(workid);
            tempWork.ModifiedDate = DateTime.Now;
            string tempstr, tempstr2, infostr;
            try
            {
                
                using (var wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    try
                    {
                        infostr = await wc.DownloadStringTaskAsync("https://hitomi.la/galleries/" + workid + ".html");
                    }
                    catch
                    {
                        return null;
                    }

                    tempstr = Library.extractionString(infostr, "<div class=\"cover\">", "</div>");

                    tempWork.CoverUrl = "https:" + Library.extractionString(tempstr, "<img src=\"", "\">");

                    tempstr = Library.extractionString(infostr, "<div class=\"gallery-preview", "</div>");

                    i = 0;

                    while (tempstr.Contains(workid))
                    {
                        Library.extractionString(tempstr, out tempstr, workid, "',", out tempstr2);
                        tempstr2 = tempstr2.Trim('/').Replace(".jpg.jpg", ".jpg").Replace(".png.jpg", ".png").Replace(".gif.jpg", ".gif");
                        i++;
                        if (tempWork.PageUrl == null || tempWork.PageUrl.Length == 0)
                        {
                            tempWork.PageUrl = tempstr2;
                        }
                        else
                        {
                            tempWork.PageUrl += "/" + tempstr2;
                        }
                    }
                    tempWork.Page = i;
                    infostr = Library.extractionString(infostr, "<div class=\"gallery", "</div>");
                    tempstr = Library.extractionString(infostr, "<a", "</a>");
                    tempstr = tempstr.Substring(tempstr.IndexOf(">") + 1);

                    tempWork.Title = tempstr;

                    tempstr = Library.extractionString(infostr, "<h2", "</h2>");

                    while (tempstr.Contains("<li>"))
                    {
                        Library.extractionString(tempstr, out tempstr, "<li>", "</li>", out tempstr2);
                        tempstr2 = Library.extractionString(tempstr2, ">", "</a>");
                        if (tempWork.Artist == null || tempWork.Artist.Length == 0)
                        {
                            tempWork.Artist = tempstr2;
                        }
                        else
                        {
                            tempWork.Artist += " / " + tempstr2;
                        }
                    }

                    tempstr = Library.extractionString(infostr, "<span class=\"date\">", "</span>");
                    if (DateTime.TryParse(tempstr, out date))
                    {
                        tempWork.UploadedDate = date;
                    }


                    infostr = Library.extractionString(infostr, "<table>", "</table>");

                    Library.extractionString(infostr, out infostr, "Group</td><td>", "</td>\n", out tempstr);

                    while (tempstr.Contains("<li>"))
                    {
                        Library.extractionString(tempstr, out tempstr, "<li>", "</li>", out tempstr2);
                        tempstr2 = Library.extractionString(tempstr2, ">", "</a>");
                        if (tempWork.Group == null || tempWork.Group.Length == 0)
                        {
                            tempWork.Group = tempstr2;
                        }
                        else
                        {
                            tempWork.Group += " / " + tempstr2;
                        }
                    }

                    Library.extractionString(infostr, out infostr, "Type</td><td>", "</td>\n", out tempstr);
                    tempstr = Library.extractionString(tempstr, ">", "</a>");

                    tempWork.TypeStr = tempstr.Trim('\n', ' ');
                    tempWork.Type2 = ComicType2.ADULT;

                    Library.extractionString(infostr, out infostr, "Language</td><td>", "</td>\n", out tempstr);
                    tempstr = Library.extractionString(tempstr, ">", "</a>");
                    tempWork.Language = tempstr.Trim();

                    Library.extractionString(infostr, out infostr, "Series</td><td>", "</td>\n", out tempstr);

                    while (tempstr.Contains("<li>"))
                    {
                        Library.extractionString(tempstr, out tempstr, "<li>", "</li>", out tempstr2);
                        tempstr2 = Library.extractionString(tempstr2, ">", "</a>");
                        if (tempWork.Series == null || tempWork.Series.Length == 0)
                        {
                            tempWork.Series = tempstr2;
                        }
                        else
                        {
                            tempWork.Series += " / " + tempstr2;
                        }
                    }
                    Library.extractionString(infostr, out infostr, "Characters</td><td>", "</td>\n", out tempstr);
                    while (tempstr.Contains("<li>"))
                    {
                        Library.extractionString(tempstr, out tempstr, "<li>", "</li>", out tempstr2);
                        tempstr2 = Library.extractionString(tempstr2, ">", "</a>");
                        if (tempWork.Character == null || tempWork.Character.Length == 0)
                        {
                            tempWork.Character = tempstr2;
                        }
                        else
                        {
                            tempWork.Character += " / " + tempstr2;
                        }
                    }
                    Library.extractionString(infostr, out infostr, "Tags</td><td>", "</td>\n", out tempstr);
                    while (tempstr.Contains("<li>"))
                    {
                        Library.extractionString(tempstr, out tempstr, "<li>", "</li>", out tempstr2);
                        tempstr2 = Library.extractionString(tempstr2, ">", "</a>");
                        if (tempWork.Tag == null || tempWork.Tag.Length == 0)
                        {
                            tempWork.Tag = tempstr2;
                        }
                        else
                        {
                            tempWork.Tag += " / " + tempstr2;
                        }
                    }

                    tempWork.Artist = StringExtensions.ToTitleCase(tempWork.ArtistEn).Trim();
                    tempWork.Group = StringExtensions.ToTitleCase(tempWork.GroupEn).Trim();
                    tempWork.Series = StringExtensions.ToTitleCase(tempWork.SeriesEn).Trim();
                    tempWork.Character = StringExtensions.ToTitleCase(tempWork.CharacterEn).Trim();
                    tempWork.Tag = StringExtensions.ToTitleCase(tempWork.TagEn).Trim();
                    GetImageUrlFromHitomi(tempWork);
                }
            }
            catch
            {
                return null;
            }

            return tempWork;
        }

        public static string updateMaruGroupWorkCover(string workid)
        {
            string url, str, artist_str = string.Empty, tag_str = string.Empty, coverurl = string.Empty;
            List<string> title_list = new List<string>();
            List<string> id_list = new List<string>();
            int temp_index;
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36 Edge/15.15063");

                url = string.Format("https://marumaru.in/b/manga/{0}", workid.Substring(1));
                str = wc.DownloadString(url);
                if ((temp_index = str.IndexOf("id=\"vContent")) > -1)
                {
                    str = str.Substring(temp_index);
                    if (str.Contains("http"))
                    {
                        str = str.Substring(str.IndexOf("http"));
                        Library.extractionString(str, out str, "\"", out coverurl);
                        if (!coverurl.EndsWith(".jpg"))
                            coverurl += ".jpg";
                    }
                }
            }
            return coverurl;
        }

        public static CtComic GetImageUrlFromHitomi(CtComic comic)
        {
            string tempstr, infostr;
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36 Edge/15.15063");
                try
                {
                    infostr = wc.DownloadString("https://hitomi.la/galleries/" + comic.Workid + ".html");
                }
                catch
                {
                    return comic;
                }

                tempstr = Library.extractionString(infostr, "<div class=\"cover\">", "</div>");
                comic.CoverUrl = "https:" + Library.extractionString(tempstr, "<img src=\"", "\">");

                try
                {
                    wc.DownloadData(string.Format("https://ba.hitomi.la/galleries/{0}/", comic.Workid) + Library.StringDivider(comic.PageUrl, "/")[0]);
                    comic.ImageUrl = string.Format("https://ba.hitomi.la/galleries/{0}/", comic.Workid);
                }
                catch
                {
                    try
                    {
                        wc.DownloadData(string.Format("https://aa.hitomi.la/galleries/{0}/", comic.Workid) + Library.StringDivider(comic.PageUrl, "/")[0]);
                        comic.ImageUrl = string.Format("https://aa.hitomi.la/galleries/{0}/", comic.Workid);
                    }
                    catch
                    {
                        comic.ImageUrl = string.Format("https://0a.hitomi.la/galleries/{0}/", comic.Workid);
                    }
                }
            }
            return comic;
        }

    }
}
