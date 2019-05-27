using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ComicViewer.CtModels
{
    public class CtWordList : CtModelList
    {
        public CtWordList() : base() { }
        public CtWordList(List<CtWord> models)
        {
            foreach (CtWord model in models)
            {
                Add(model);
            }
        }
        public CtWord GetWord(string id)
        {
            return (CtWord)base.GetModel(id);
        }
    }


    public class CtWordObservableCollection : CtModelObservableCollection, IDisposable
    {
        public CtWordObservableCollection() : base()
        {
        }
        public CtWordObservableCollection(List<CtWord> list) : base(list.Cast<CtModel>().ToList())
        {
        }
        public CtWord GetWord(string id)
        {
            return (CtWord)base.GetModel(id);
        }

        public void UpdateModel(CtWord model)
        {
            this.SetItem(GetIndex(model.Id), model);
        }
    }
}
