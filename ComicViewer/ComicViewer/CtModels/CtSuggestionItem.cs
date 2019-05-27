using System;
using System.Collections.Generic;
using System.Text;

namespace ComicViewer.CtModels
{
    public class CtSuggestionItem
    {
        public string Key { get; set; }
        public CtModelType Type { get; set; }
        public CtModel Model { get; set; }
        public string Text
        {
            get
            {
                if (Model.Name == Key)
                    return Model.Text;
                else
                    return Model.Text + "(" + Key + ")";
            }
        }

        public CtSuggestionItem(string key, CtModel obj)
        {
            Key = key;
            Model = obj;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
