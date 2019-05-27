using System;
using System.Collections.Generic;
using System.Text;

namespace ComicViewer.CtModels
{
    public interface ICtModel
    {
        List<CtComic> GetComicList();
        List<CtArtist> GetArtistList();
        List<CtGroup> GetGroupList();
        List<CtSeries> GetSeriesList();
        List<CtCharacter> GetCharacterList();
        List<CtTag> GetTagList();
        CtNameList GetNames();
        CtWordList GetWords();
    }
}
