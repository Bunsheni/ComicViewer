using ComicViewer.CtModels;
using ComicViewer.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace ComicViewer
{
    class CtDownloader
    {
        enum DownLoadState { WAIT, DOWNLOAD, COMPLETE, STOP, ERROR }
        List<string> _pagelist;
        public CtComic Work;
        private int _page;
        private int _downloadIndex;
        private int _byteSize = 0;
        private bool _downloadStop = false;
        private float progress;
        public int DownloadSuccessCount = 0;
        public int DownloadProgress = 0;
        public List<Thread> threadlist = new List<Thread>();
        private CtFileService fileService = new CtFileService();
        public string DownloadState { get; private set; }
        System.Timers.Timer timer;

        /// <summary>
        /// Event
        /// </summary>
        public event EventHandler<DownloadEventArgs> OnProgressDownload;

        public CtDownloader(CtComic work)
        {
            this.Work = work;
            _pagelist = Library.StringDivider(Work.PageUrl, "/");
            _page = _pagelist.Count;
        }

        public void DownloadStop()
        {
            _downloadStop = true;
        }

        public async void DownloadWork(bool multi)
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += new ElapsedEventHandler(timer_Tick);
            timer.Start();


            DownloadProgress = 1;


            for (_downloadIndex = 0; _downloadIndex < _page; _downloadIndex++)
            {
                if (_downloadStop)
                {
                    goto STOP_DOWNLOAD;
                }
                await DownloadImageAsync(_downloadIndex);
            }

            System.GC.Collect(3, GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();

            bool flag = false;
            string message = string.Empty;
            for (int index = 0; index < _page; index++)
            {
                if (! await ImageExist(index))
                {
                    flag = true;
                    if (message.Length == 0)
                        message = index.ToString();
                    else
                        message += ", " + index.ToString();
                }
            }

            System.GC.Collect();
            if (flag == true)
            {
                Console.WriteLine(message + "페이지 파일 다운로드에 실패했습니다.", "알림");
                DownloadProgress = 4;
                goto STOP_DOWNLOAD;
            }
            DownloadProgress = 3;
            _downloadStop = true;

        STOP_DOWNLOAD:
            foreach (Thread th in threadlist)
            {
                th.Abort();
            }
            threadlist.Clear();
            System.GC.Collect();
        }

        public string GetWebUrl(string workid)
        {
            string worklink;
            if (workid[0] == 'm')
                worklink = string.Format("http://wasabisyrup.com/archives/{0}", workid.Substring(1));
            else if (workid[0] == 'l')
                return string.Empty;
            else if (workid[0] == 'g')
                worklink = string.Format("https://marumaru.in/b/manga/{0}", workid.Substring(1));
            else
                worklink = string.Format("https://hitomi.la/reader/{0}.html", workid);
            return worklink;
        }
        
        private async Task<bool> ImageExist(int index)
        {
            return await fileService.ExistFile(string.Format("{0,0:000}.jpg", index)) || await fileService.ExistFile(string.Format("{0,0:000}.gif", index));
        }

        //private void DownloadImage(int index)
        //{
        //    HttpWebRequest request;
        //    HttpWebResponse response;
        //    int retriedCount = 0;
        //    string strIndex = string.Format("{0,0:000}", index);
        //    string temp = strIndex + "_temp";
        //    string name = _pagelist[index].EndsWith(".gif") ? strIndex + ".gif" : strIndex + ".jpg";
        //RESTART:
        //    if (!CtSmb.GetSmbFileItem(Work, name).file.Exists())
        //    {
        //        try
        //        {
        //            request = (HttpWebRequest)WebRequest.Create(Work.ImageUrl + _pagelist[index]);
        //            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
        //            request.Referer = GetWebUrl(Work.Workid);
        //            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
        //            response = (HttpWebResponse)request.GetResponse();
        //            bool bImage = response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase);
        //            if ((response.StatusCode == HttpStatusCode.OK ||
        //                response.StatusCode == HttpStatusCode.Moved ||
        //                response.StatusCode == HttpStatusCode.Redirect)
        //                && bImage)
        //            {
        //                using (Stream inputStream = response.GetResponseStream())
        //                using (SharpCifs.Util.Sharpen.OutputStream outputStream = CtSmb.OpenWrite(Work, temp))
        //                {
        //                    try
        //                    {
        //                        int tempsize = 0;
        //                        byte[] buffer = new byte[4096];
        //                        int bytesRead;
        //                        while (true)
        //                        {
        //                            bytesRead = inputStream.Read(buffer, 0, buffer.Length);
        //                            if (bytesRead != 0)
        //                            {
        //                                try
        //                                {
        //                                    OnProgressDownload?.Invoke(this, new DownloadEventArgs(DownloadSuccessCount, _page, (float)tempsize / inputStream.Length));
        //                                }
        //                                catch { }
        //                                outputStream.Write(buffer, 0, bytesRead);
        //                                tempsize += bytesRead;
        //                            }
        //                            else
        //                            {
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    catch (IOException ex)
        //                    {
        //                        Console.WriteLine(index + "페이지 다운로드에 실패했습니다.\n" + ex.Message);
        //                        inputStream.Close();
        //                        outputStream.Close();
        //                        return;
        //                    }
        //                    inputStream.Close();
        //                    outputStream.Close();
        //                }
        //                CtSmb.GetSmbFileItem(Work, temp).file.RenameTo(CtSmb.GetSmbFileItem(Work, name).file);
        //                DownloadSuccessCount++;
        //                try
        //                {
        //                    //if (index == 0)
        //                    //    mif.GetDataBase().InsertImage(Image.FromFile(fd3), Work.workid);
        //                }
        //                catch (Exception ex)
        //                {
        //                    Console.WriteLine(index + "첫 페이지를 표지로 삽입에 실패했습니다.\n" + ex.Message);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            if (retriedCount < 5)
        //            {
        //                retriedCount++;
        //                goto RESTART;
        //            }
        //            else
        //            {
        //                Console.WriteLine(index + "페이지 다운로드에 실패했습니다.\n" + ex.Message);
        //                return;
        //            }
        //        }
        //    }
        //}

        public string GetImageFilePath(string path, CtComic comic, int index)
        {
            string temp = string.Format("{0,0:000}", index);
            string fd = Path.Combine(path, comic.Workid, temp);
            string res;
            if (Library.StringDivider(comic.PageUrl, "/")[index].EndsWith(".gif"))
                res = fd + ".gif";
            else
                res = fd + ".jpg";
            return res;
        }
        
        private async Task DownloadImageAsync(int index)
        {
            string url = Work.ImageUrl + _pagelist[index];
            string filePath = fileService.GetImageFilePath(Work, index);
            string tempFilePath = filePath.Replace("jpg","tmp").Replace("gif","tmp");
            HttpWebRequest request;
            HttpWebResponse response;
            fileService.CreateDirectory(Work.GetDirectory());
            if (! await fileService.ExistComicImageFile(Work, index))
            {
                try
                {
                    request = (HttpWebRequest)WebRequest.Create(url);
                    request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                    response = (HttpWebResponse)request.GetResponse();
                    bool bImage = response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase);
                    if ((response.StatusCode == HttpStatusCode.OK ||
                        response.StatusCode == HttpStatusCode.Moved ||
                        response.StatusCode == HttpStatusCode.Redirect)
                        && bImage)
                    {
                        using (Stream inputStream = response.GetResponseStream())
                        using (Stream outputStream = await fileService.GetNewFileStream(tempFilePath))
                        {
                            try
                            {
                                int tempsize = 0;
                                byte[] buffer = new byte[4096];
                                int bytesRead;
                                while (true)
                                {
                                    bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                                    if (bytesRead != 0)
                                    {
                                        try
                                        {
                                            try
                                            {
                                                OnProgressDownload?.Invoke(this, new DownloadEventArgs(DownloadSuccessCount, _page, (float)tempsize / inputStream.Length));
                                            }
                                            catch { }
                                            progress = (float)tempsize / inputStream.Length;
                                        }
                                        catch { }
                                        outputStream.Write(buffer, 0, bytesRead);
                                        tempsize += bytesRead;
                                    }
                                    else
                                    {
                                        progress = 0;
                                        break;
                                    }
                                }
                            }
                            catch
                            {
                                inputStream.Close();
                                outputStream.Close();
                                return;
                            }
                            inputStream.Close();
                            outputStream.Close();
                            await fileService.Move(tempFilePath, filePath);
                        }
                        DownloadSuccessCount++;
                        if (Device.RuntimePlatform == Device.Android)
                        {
                            DependencyService.Get<IMedia>().UpdateGallery(Path.Combine(fileService.GetBaseDirectory(), filePath));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public class DownloadEventArgs : EventArgs
        {
            public DownloadEventArgs(int i, int m, float b)
            {
                Page = i;
                Max = m;
                Byte = b;
            }
            public int Page { get; set; }
            public int Max { get; set; }
            public float Byte { get; set; }
        }

        private void countByte(int byteLength)
        {
            Object byteCountLockObject = new object();
            lock (byteCountLockObject)
            {
                _byteSize += byteLength;
            }
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (DownloadProgress == 0)
            {
                DownloadState = "연결중";
            }
            else if (DownloadProgress == 1)
            {
                DownloadState = Library.ByteUnitTransform(_byteSize) + "/sec";
            }
            else if (DownloadProgress == 2)
            {
                DownloadState = "압축중";
            }
            else if (DownloadProgress == 3)
            {
                DownloadState = "완료";
            }
            else if (DownloadProgress == 4)
            {
                DownloadState = "중단";
            }
            else
            {
                DownloadState = "오류";
            }
            try
            {
                _byteSize = 0;
            }
            catch { return; }
        }
    }
}
