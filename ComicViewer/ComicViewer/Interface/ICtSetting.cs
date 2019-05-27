
using System;
using System.Collections.Generic;
using System.Text;

using ComicViewer.CtModels;

namespace ComicViewer
{
    public interface ICtSetting
    {
        CtLanguage ProgramLanguage { get; set; }
    }
}
