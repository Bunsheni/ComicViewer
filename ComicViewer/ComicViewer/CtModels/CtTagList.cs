using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ComicViewer.CtModels
{
    public class CtTagList : CtModelList
    {
        public CtTagList(List<CtTag> models)
        {
            foreach (CtTag model in models)
            {
                Add(model);
            }
        }
        public CtTag GetTag(string id)
        {
            return (CtTag)GetModel(id);
        }
    }
    public class CtTagObservableCollection : CtModelObservableCollection, IDisposable
    {
        public CtTagObservableCollection() : base()
        {
        }
        public CtTagObservableCollection(List<CtTag> list) : base(list.Cast<CtModel>().ToList())
        {
        }
        public CtTag GetTag(string id)
        {
            return (CtTag)base.GetModel(id);
        }

        public void UpdateModel(CtTag model)
        {
            this.SetItem(GetIndex(model.Id), model);
        }
    }
}
