using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ComicViewer.CtModels
{
    public interface ICtDataStore
    {
        Task Load();
        Task<CtComicList> GetComics();
        Task<CtArtistList> GetArtists();
        Task<CtGroupList> GetGroups();
        Task<CtSeriesList> GetSerieses();
        Task<CtCharacterList> GetCharacters();
        Task<CtTagList> GetTags();
        Task<CtNameList> GetNames();
        Task<CtWordList> GetWords();
        Task<CtComic> GetComic(string workid);
        Task InsertComic(CtComic comic);
        Task DeleteComic(string workid);
        Task UpdateComic(CtComic comic);
        Task<CtModelList> GetModels(CtModelType type);
        Task AddModels(CtModelList models);
        Task<List<CtSuggestionItem>> GetSuggestionItemsAsync(string key, CtModelType type);
    }
}
