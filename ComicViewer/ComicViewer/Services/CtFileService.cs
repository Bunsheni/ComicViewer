using ComicViewer.CtModels;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using Xamarin.Forms;
using Xamarin.Essentials;

namespace ComicViewer.Services
{
    public class CtFileService
    {
        public async void OpenFilePicker()
        {
            try
            {
                FileData fileData = await CrossFilePicker.Current.PickFile();
                if (fileData == null)
                    return; // user canceled file picking

                string fileName = fileData.FileName;
                string filePath = fileData.FilePath;
                string contents = System.Text.Encoding.UTF8.GetString(fileData.DataArray);

                System.Console.WriteLine("File name chosen: " + fileName);
                System.Console.WriteLine("File data: " + contents);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Exception choosing file: " + ex.ToString());
            }
        }

        public CtFileService()
        {
            DependencyService.Get<ISQLiteDb>().SetBaseDirectory("ComicViewer");
        }

        public string GetBaseDirectory()
        {
            return DependencyService.Get<ISQLiteDb>().GetBaseDirectory();
        }

        public async void CreateDirectory(string name)
        {
            await DependencyService.Get<ISQLiteDb>().CreateDirectory(name);
        }

        public async Task<Stream> GetNewFileStream(string name)
        {
            return await DependencyService.Get<ISQLiteDb>().GetNewFileStream(name);
        }

        public async Task<bool> ExistFile(string name)
        {
            return await DependencyService.Get<ISQLiteDb>().ExistFile(name);
        }

        public string GetImageFilePath(CtComic comic, int index)
        {
            return Path.Combine(comic.GetDirectory(), string.Format("{0,0:000}", index) + comic.GetExtention(index));
        }

        public async Task<bool> ExistComicImageFile(CtComic comic, int index)
        {
            return await ExistFile(GetImageFilePath(comic, index));
        }

        public async Task Move(string src, string dest)
        {
            await DependencyService.Get<ISQLiteDb>().Move(src, dest);
        }
    }
}
