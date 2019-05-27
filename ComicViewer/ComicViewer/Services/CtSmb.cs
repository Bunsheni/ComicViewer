using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using ComicViewer.CtModels;
using SharpCifs.Netbios;
using SharpCifs.Smb;
using SharpCifs.Util.Sharpen;
using Xamarin.Forms;

namespace ComicViewer.Services
{
    public class CtSmb
    {
        public static SmbFileItem GetSmbFileItem()
        {
            SharpCifs.Config.SetProperty("jcifs.smb.client.lport", "8137");
            SmbFile lan = new SmbFile("smb://GUEST:@192.168.219.100/Manga/works/");

            try
            {
                if (lan.Exists())
                    return new SmbFileItem(lan);
                else
                    return null;
            }
            catch
            {                
                return null;
            }
        }

        public static string GetComicDirectory(CtComic comic)
        {
            string directory = "smb://GUEST:@192.168.219.100/Manga/works/" + string.Format($"{comic.ArtistEn.Trim(' ', '.')}\\{comic.TitleEn.Trim(' ', '.')}\\{comic.Language}\\{comic.Workid}\\")
                    .Replace("?", "？").Replace("*", "＊").Replace(" / ", "／").Replace("/", "／").Replace(":", "：")
                    .Replace("|", "｜").Replace("<", "＜").Replace(">", "＞").Replace(@"""", "'").Trim(' ', '.').Replace("\\", "/");
            return directory;
        }

        public static SmbFileItem GetSmbFileItem(CtComic comic)
        {
            SharpCifs.Config.SetProperty("jcifs.smb.client.lport", "8137");
            return new SmbFileItem(new SmbFile(GetComicDirectory(comic)));
        }

        public static SmbFileItem GetSmbFileItem(CtComic comic, string name)
        {
            SharpCifs.Config.SetProperty("jcifs.smb.client.lport", "8137");
            return new SmbFileItem(new SmbFile(GetComicDirectory(comic) + name));
        }

        public static OutputStream OpenWrite(CtComic comic, string name)
        {
            SharpCifs.Config.SetProperty("jcifs.smb.client.lport", "8137");
            SmbFileItem item = GetSmbFileItem(comic);
            if (!item.file.Exists())
                item.file.Mkdirs();
            item = GetSmbFileItem(comic, name);
            if(!item.file.Exists())
                item.file.CreateNewFile();
            return item.file.GetOutputStream();
        }

        public static List<InputStream> OpenRead(CtComic comic)
        {
            List<InputStream> res = new List<InputStream>();
            SharpCifs.Config.SetProperty("jcifs.smb.client.lport", "8137");
            SmbFileItem item = GetSmbFileItem(comic);
            foreach(SmbFile file in item.file.ListFiles())
            {
                res.Add(file.GetInputStream());
            }
            return res;
        }

        public static List<ImageSource> GetImageStreams(CtComic comic)
        {
            List<ImageSource> res = new List<ImageSource>();
            SharpCifs.Config.SetProperty("jcifs.smb.client.lport", "8137");
            SmbFileItem item = GetSmbFileItem(comic);
            foreach (SmbFile file in item.file.ListFiles())
            {
                res.Add(ImageSource.FromStream(() => { return file.GetInputStream(); }));
            }
            return res;
        }
    }

    public class SmbFileItem
    {
        public SmbFile file;
        public SmbFileItem(SmbFile file)
        {
            this.file = file;
        }


        public string Name
        {
            get
            {
                return file.GetName().TrimEnd('/');
            }
        }

        public string LastModDate
        {
            get
            {
                var epocDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return epocDate.AddMilliseconds(file.LastModified()).ToLocalTime().ToString(("yyyy-MM-dd HH:mm:ss"));
            }
        }
        public string Type
        {
            get
            {
                return file.IsDirectory() ? "dir" : "file";
            }
        }

        public List<SmbFileItem> ListFiles()
        {
            List<SmbFileItem> items = new List<SmbFileItem>();
            foreach (SmbFile file in file.ListFiles())
            {
                items.Add(new SmbFileItem(file));
            }
            return items;
        }
    }
}
