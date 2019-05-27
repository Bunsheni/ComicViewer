using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

using ComicViewer.CtModels;

namespace ComicViewer
{
    public interface IDbConnection
    {
        CtFilter GetFilter();
        CtSQLiteDb GetSQLiteDb();
        //void DetailNavigate(CtMasterDetailPageMenuItem item);
        void ListViewUpdateRequestAsync();
    }
}
