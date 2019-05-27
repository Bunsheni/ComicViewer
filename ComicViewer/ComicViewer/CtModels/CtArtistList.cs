using System.Collections.Generic;
using System.Linq;

namespace ComicViewer.CtModels
{
    public class CtArtistList : CtModelList
    {
        public CtArtistList(List<CtArtist> models)
        {
            foreach (CtArtist model in models)
            {
                Add(model);
            }
        }
        public CtArtist GetArtist(string id)
        {
            return (CtArtist)GetModel(id);
        }
    }

    public class CtArtistObservableCollection : CtModelObservableCollection
    {
        public CtArtistObservableCollection() : base()
        {
        }
        public CtArtistObservableCollection(List<CtArtist> list) : base(list.Cast<CtModel>().ToList())
        {
        }
        public CtArtist GetArtist(string id)
        {
            return (CtArtist)GetModel(id);
        }

        public void UpdateArtist(CtArtist model)
        {
            this.SetItem(GetIndex(model.Id), model);
        }
    }
}
