using System;
using System.IO;
using SQLite;
using Xamarin.Forms;
using ComicViewer.iOS;
using System.Threading.Tasks;

[assembly: Dependency(typeof(SQLiteDb))]
namespace ComicViewer.iOS
{
    public class SQLiteDb : ISQLiteDb
    {
        public Task<string> CreateDirectory(string folderName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public string GetBaseDirectory()
        {
            throw new NotImplementedException();
        }

        public SQLiteAsyncConnection GetConnection(string name)
        {
			var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); 
            var path = Path.Combine(documentsPath, name);
            return new SQLiteAsyncConnection(path);
        }

        public string GetDocumentsPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public Task<Stream> GetNewFileStream(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task Move(string srcFile, string destFile)
        {
            throw new NotImplementedException();
        }

        public void SetBaseDirectory(string fileName)
        {
            throw new NotImplementedException();
        }

        Task<string> ISQLiteDb.SetBaseDirectory(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}

