using System;
using System.Collections.Generic;
using System.Linq;

namespace ComicViewer.CtModels
{
    public class CtFilter
    {
        public static string[] categoryList = { "index", "artist", "group", "series", "character", "tag" };
        public static string[] languageList = { "korean", "english", "japanese", "english" };
        public static string[] LanguageArray = { "한국어", "English", "日本語" };

        public static ComicType Type;
        public static ComicType2 Type2;
        public static List<bool> Languages;
        public static bool Hidden, Local;

        public string Key;
        public CtModelList models;
        public DateTime startDate, endDate;
        public bool favorite;

        public CtFilter(string key)
        {
            this.Key = key.Trim();
            startDate = Convert.ToDateTime("2000-1-1");
            Type = ComicType.UNKNOWN;
            Type2 = ComicType2.UNKNOWN;
            Languages = new List<bool>(){ true, true, true };
            models = new CtModelList();
            favorite = false;
        }

        public CtFilter Copy()
        {
            CtFilter ct = new CtFilter(this.Key);
            ct.startDate = this.startDate;
            ct.endDate = this.endDate;
            ct.models.AddRange(models);
            return ct;
        }

        public void Clear()
        {
            this.Key = string.Empty;
            models.Clear();
        }

        //public string HitomiUrl
        //{
        //    get
        //    {
        //        string path, temp;
        //        if(models.Count > 0)
        //        {
        //            path = "https://hitomi.la/search.html?" + Key;
        //            temp = Key.Trim();
        //            if (temp.Length != 0)
        //            {
        //                path = string.Concat(path, temp.ToLower().Replace(" ", "_"), "%20");
        //            }
        //            if (ArtistList.Count != 0)
        //            {
        //                foreach (CtArtist term in ArtistList)
        //                {
        //                    path = string.Concat(path, "artist:", term.NameEn.ToLower().Replace(" ", "_"), "%20");
        //                }
        //            }
        //            if (GroupList.Count != 0)
        //            {
        //                foreach (CtGroup term in GroupList)
        //                {
        //                    path = string.Concat(path, "group:", term.NameEn.ToLower().Replace(" ", "_"), "%20");
        //                }
        //            }
        //            if (SeriesList.Count != 0)
        //            {
        //                foreach(CtSeries term in SeriesList)
        //                {
        //                    path = string.Concat(path, "series:", term.NameEn.ToLower().Replace(" ", "_"), "%20");
        //                }
        //            }
        //            if (CharacterList.Count != 0)
        //            {
        //                foreach (CtCharacter term in CharacterList)
        //                {
        //                    path = string.Concat(path, "character:", term.NameEn.ToLower().Replace(" ", "_"), "%20");
        //                }
        //            }
        //            if (TagList.Count != 0)
        //            {
        //                foreach (CtTag term in TagList)
        //                {
        //                    path = string.Concat(path, (term.Gender == 1 ? "female:" : (term.Gender == 2) ? "male:" : "tag:"), term.NameEn.ToLower().Replace(" ", "_"), "%20").Replace("♀", "").Replace("♂", "").Trim();
        //                }

        //            }
        //            path += "#";
        //            path = path.Replace("%20#", "#");
        //        }
        //        else
        //        {
        //            if (this.Key.Length == 0)
        //                path = "https://hitomi.la/index-" + languageList[(int)LanguageList[0]] + "-";
        //            else if (category == 0)
        //            {
        //                path = "https://hitomi.la/search.html?" + Key + "#";
        //            }
        //            else
        //            {
        //                if (this.Key.Contains("♀")) this.Key = "female%3A" + this.Key.Replace("♀", "").Trim();
        //                path = "https://hitomi.la/" + categoryList[this.category] + "/" + this.Key.Replace(" ", "%20").ToLower() + "-" + languageList[(int)LanguageList[0]] + "-";
        //            }
        //        }
        //        return path;
        //    }
        //}

        public bool checkInfo(string infoen, string worken)
        {
            bool flag;
            if (worken.Length == 0 && infoen.Length != 0)
            {
                return false;
            }
            else
            {
                flag = true;
                foreach (string str in Library.StringDivider(worken, "/"))
                {
                    if (string.Compare(str.Trim(), infoen, true) == 0) flag = false;
                }
                if (flag) return false;
            }
            return true;
        }

        public bool checkInfo(string infoen, string infokr, string worken, string workkr)
        {
            bool flag;
            if ((workkr.Length == 0 && infokr.Length != 0) ||
                (worken.Length == 0 && infoen.Length != 0))
            {
                return false;
            }
            else
            {
                flag = true;
                foreach (string str in Library.StringDivider(worken, "/"))
                {
                    if (string.Compare(str.Trim(), infoen, true) == 0) flag = false;
                }
                foreach (string str in Library.StringDivider(workkr, "/"))
                {
                    if (string.Compare(str.Trim(), infokr, true) == 0) flag = false;
                }
                if (flag) return false;
            }
            return true;
        }

        public bool Filtering(CtComic model)
        {
            return Filtering(model, false);
        }
        public bool Filtering(CtComic model, bool hitomi)
        {
            if (Key == model.Id) return true;
            if (!Hidden && model.IsHidden) return false;
            if (favorite && !model.IsFavorite) return false;

            int languageIndex = LanguageArray.ToList().FindIndex(i => string.Compare(i, model.Language, true) == 0);
            if (0 <= languageIndex && !Languages[languageIndex])
                return false;
            
            if (Type2 != ComicType2.UNKNOWN && Type2 != model.Type2 && model.Type2 != ComicType2.UNKNOWN)
            {
                return false;
            }


            if (Type != ComicType.UNKNOWN && Type != model.Type1 && model.Type1 != ComicType.UNKNOWN)
            {
                return false;
            }

            if (Key.Length != 0 && !model.TitleContains(Key))
            {
                return false;
            }

            if (models.Count > 0)
            {
                foreach (CtModel ct in models)
                {
                    if (!model.GetTags(ct.CtType, false).Contains(ct.Id))
                        return false;
                }
            }
            return true;
        }

        public CtComicList Filtering(CtComicList comics)
        {
            CtComicList res = new CtComicList();

            foreach (CtComic comic in comics)
            {
                if (this.Filtering(comic))
                {
                    res.Add(comic);
                }
            }
            return res;
        }


        public bool Contain(string all, string one)
        {
            foreach (string str in Library.StringDivider(all, "/"))
            {
                if (string.Compare(str.Trim(), one, true) == 0)
                    return true;
            }
            return false;
        }

        public string Text
        {
            get
            {
                string[] texts = new string[(int)CtModelType.COUNT];
                if (Key.Length != 0)
                    texts[0] = "제목:" + Key;

                foreach (CtModel model in models)
                {
                    if (texts[(int)CtModelType.ARTIST].Length != 0)
                        texts[(int)CtModelType.ARTIST] += ", " + model.Name;
                    else
                        texts[(int)CtModelType.ARTIST] += model.Text;
                    break;
                }
                string res = string.Empty;
                foreach(string str in texts)
                {
                    res += ", " + str;
                }
                return res.Trim(',',' ');
            }
        }
    }
}
