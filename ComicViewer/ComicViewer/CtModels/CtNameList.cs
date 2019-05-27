using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ComicViewer.CtModels
{
    public class CtNameList : CtModelList
    {
        public CtNameList() : base() { }
        public CtNameList(List<CtName> models)
        {
            foreach (CtName model in models)
            {
                Add(model);
            }
        }
        public CtName GetName(string id)
        {
            return (CtName)base.GetModel(id, true);
        }
    }


    public class CtNameObservableCollection : CtModelObservableCollection, IDisposable
    {
        public CtNameObservableCollection() : base()
        {
        }
        public CtNameObservableCollection(List<CtName> list) : base(list.Cast<CtModel>().ToList())
        {
        }
        public CtName GetName(string id)
        {
            return (CtName)base.GetModel(id);
        }

        public void UpdateModel(CtName model)
        {
            this.SetItem(GetIndex(model.Id), model);
        }
    }
}
