using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ComicViewer.CtModels
{
    public class CtCharacterList : CtModelList
    {
        public CtCharacterList(List<CtCharacter> models)
        {
            foreach (CtCharacter model in models)
            {
                Add(model);
            }
        }
        public CtCharacter GetCharacter(string id)
        {
            return (CtCharacter)GetModel(id);
        }
    }
    public class CtCharacterObservableCollection : CtModelObservableCollection
    {
        public CtCharacterObservableCollection() : base()
        {
        }
        public CtCharacterObservableCollection(List<CtCharacter> list) : base(list.Cast<CtModel>().ToList())
        {
        }
        public CtCharacter GetCharacter(string id)
        {
            return (CtCharacter)base.GetModel(id);
        }

        public void UpdateModel(CtCharacter model)
        {
            this.SetItem(GetIndex(model.Id), model);
        }
    }
}
