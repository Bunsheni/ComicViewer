using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ComicViewer.CtModels
{
    public class CtSeriesList : CtModelList
    {
        public CtSeriesList(List<CtSeries> models)
        {
            foreach (CtSeries model in models)
            {
                Add(model);
            }
        }
        public CtSeries GetSeries(string id)
        {
            return (CtSeries)GetModel(id);
        }
    }
    public class CtSeriesObservableCollection : CtModelObservableCollection
    {
        public CtSeriesObservableCollection() : base()
        {
        }
        public CtSeriesObservableCollection(List<CtSeries> list) : base(list.Cast<CtModel>().ToList())
        {
        }
        public CtSeries GetSeries(string id)
        {
            return (CtSeries)base.GetModel(id);
        }

        public void UpdateModel(CtSeries model)
        {
            this.SetItem(GetIndex(model.Id), model);
        }
    }
}
