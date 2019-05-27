using System;
using System.Collections.Generic;
using System.Text;

namespace ComicViewer.CtModels
{
    interface IOleModel
    {
        CtOleModelType ObjectType { get; }
    }
}
