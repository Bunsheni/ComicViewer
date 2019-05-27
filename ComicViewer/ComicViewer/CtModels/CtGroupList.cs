using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ComicViewer.CtModels
{
    public class CtGroupList : CtModelList
    {
        public CtGroupList(List<CtGroup> models)
        {
            foreach (CtGroup model in models)
            {
                Add(model);
            }
        }
        public CtGroup GetGroup(string id)
        {
            return (CtGroup)GetModel(id);
        }
    }
    public class CtGroupObservableCollection : CtModelObservableCollection
    {
        public CtGroupObservableCollection() : base()
        {
        }
        public CtGroupObservableCollection(List<CtGroup> list) : base(list.Cast<CtModel>().ToList())
        {
        }
        public CtGroup GetGroup(string id)
        {
            return (CtGroup)base.GetModel(id);
        }

        public void UpdateModel(CtGroup model)
        {
            this.SetItem(GetIndex(model.Id), model);
        }
    }
}