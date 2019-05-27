using SQLite;
using System.IO;
using System.Threading.Tasks;

namespace ComicViewer
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection(string name);
        Task<string> SetBaseDirectory(string fileName);
        string GetBaseDirectory();
        Task<string> CreateDirectory(string folderName);
        Task<Stream> GetNewFileStream(string fileName);
        Task<bool> ExistFile(string fileName);
        Task Move(string srcFile, string destFile);
    }
}

