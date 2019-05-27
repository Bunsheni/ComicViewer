using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ComicViewer.CtModels
{
    public class CtComicList : CtModelList
    {
        public CtComicList() : base() { }
        public CtComicList(List<CtComic> models)
        {
            foreach (CtComic model in models)
            {
                Add(model);
            }
        }
        public CtComic GetComic(string id)
        {
            return (CtComic)GetModel(id);
        }
    }


    public class CtComicObservalbeCollection : CtModelObservableCollection
    {
        public CtComicObservalbeCollection() : base()
        {
        }
        public CtComicObservalbeCollection(List<CtComic> list) : base(list.Cast<CtModel>().ToList())
        {
        }
        public CtComic GetComic(string id)
        {
            return (CtComic)GetModel(id);
        }

        public void UpdateModel(CtComic model)
        {
            this.SetItem(GetIndex(model.Id), model);
        }
    }
}
