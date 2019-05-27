using System;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using Windows.Storage;
using ComicViewer.Windows;
using Windows.Storage.Streams;

[assembly: Dependency(typeof(SQLiteDb))]
namespace ComicViewer.Windows
{
    public class SQLiteDb : ISQLiteDb
    {
        StorageFolder BaseFolder = KnownFolders.PicturesLibrary;

        public SQLiteAsyncConnection GetConnection(string name)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            var path = Path.Combine(localFolder.Path, name);
            return new SQLiteAsyncConnection(path);
        }

        public async Task<string> SetBaseDirectory(string folderName)
        {
            BaseFolder = await BaseFolder.GetFolderAsync(folderName);
            return BaseFolder.Path;
        }

        public string GetBaseDirectory()
        {
            return BaseFolder.Path;
        }

        public async Task<string> CreateDirectory(string folderName)
        {
            var temp = await BaseFolder.TryGetItemAsync(folderName);
            if (temp == null)
                temp = await BaseFolder.CreateFolderAsync(folderName);
            return temp.Path;
        }

        public async Task<bool> ExistFile(string fileName)
        {
            var temp = await BaseFolder.TryGetItemAsync(fileName);
            return temp != null;
        }

        public async Task<Stream> GetNewFileStream(string fileName)
        {
            StorageFile newFile;
            var temp1 = await BaseFolder.TryGetItemAsync(fileName);
            if (temp1 == null)
            {
                newFile = await BaseFolder.CreateFileAsync(fileName);
            }
            else
            {
                newFile = await BaseFolder.GetFileAsync(fileName);
            }
            return await newFile.OpenStreamForWriteAsync();
        }

        public async Task Move(string srcFile, string destFile)
        {
            string[] strs = destFile.Split('\\');
            string destFileName = strs[strs.Length - 1];
            strs = srcFile.Split('\\');
            string srcFileName = strs[strs.Length - 1];
            string destFolderName = destFile.Substring(0, destFile.Length - destFileName.Length - 1);
            string srcFolderName = destFile.Substring(0, srcFile.Length - srcFileName.Length - 1);

            var temp = await BaseFolder.TryGetItemAsync(srcFile);
            if (temp == null)
                return;
            if (srcFolderName != destFolderName)
            {
                temp = await BaseFolder.TryGetItemAsync(destFolderName);
                if (temp == null)
                {
                    await BaseFolder.CreateFolderAsync(destFolderName);
                }
            }
            await (await BaseFolder.GetFileAsync(srcFile)).RenameAsync(destFileName);
        }
    }
}

