using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicViewer.CtModels
{
    public partial class CtComic
    {

        public bool TitleContains(string key)
        {
            key = key.ToLower();
            if (Workid.Contains(key) || TitleEn.ToLower().Contains(key) || TitleKr.ToLower().Contains(key))
                return true;
            else return false;
        }

        public bool TitleContains(CtWord word)
        {
            string titleen = TitleEn.ToLower().Replace("-", " ");
            string worden = word.NameEn.ToLower().Replace("-", " ").Trim('-');
            if (word.NameEn.Contains(" "))
                return titleen.Contains(worden);
            else
                return Library.StringDivider(titleen, " ").Contains(worden);
        }

        public bool TitleContains(CtWordList words)
        {
            foreach(CtWord word in words)
            {
                string key = word.NameEn.ToLower();
                if(key.Length > 1 && Workid.Contains(key) || TitleEn.ToLower().Contains(key) || TitleKr.ToLower().Contains(key))
                {
                    return true;
                }
                else if (Workid == key || Library.IsWord(TitleEn.ToLower(), key) || TitleKr.ToLower().Contains(key))
                {
                    return true;
                }
            }
            return false;
        }

        public List<string> GetTags(CtModelType type, bool kr)
        {
            List<string> res = new List<string>();
            if (type == CtModelType.ALL)
            {
                res.Add(TitleEn);
                res.Add(TitleKr);
            }
            if (type == CtModelType.ALL || type == CtModelType.ARTIST)
            {
                res.AddRange(Library.StringDivider(ArtistEn, " / "));
                if(kr)
                    res.AddRange(Library.StringDivider(ArtistKr, " / "));
            }
            if (type == CtModelType.ALL || type == CtModelType.GROUP)
            {
                res.AddRange(Library.StringDivider(GroupEn, " / "));
                if (kr)
                    res.AddRange(Library.StringDivider(GroupKr, " / "));
            }
            if (type == CtModelType.ALL || type == CtModelType.SERIES)
            {
                res.AddRange(Library.StringDivider(SeriesEn, " / "));
                if (kr)
                    res.AddRange(Library.StringDivider(SeriesKr, " / "));
            }
            if (type == CtModelType.ALL || type == CtModelType.CHARACTER)
            {
                res.AddRange(Library.StringDivider(CharacterEn, " / "));
                if (kr)
                    res.AddRange(Library.StringDivider(CharacterKr, " / "));
            }
            if (type == CtModelType.ALL || type == CtModelType.TAG)
            {
                res.AddRange(Library.StringDivider(TagEn, " / "));
                if (kr)
                    res.AddRange(Library.StringDivider(TagKr, " / "));
            }
            return res;
        }

        public List<string> GetTagsToLower(CtModelType type)
        {
            List<string> res = new List<string>();
            if (type == CtModelType.ALL)
            {
                res.Add(TitleEn.ToLower());
            }
            if (type == CtModelType.ALL || type == CtModelType.ARTIST)
            {
                res.AddRange(Library.StringDivider(ArtistEn.ToLower(), " / "));
            }
            if (type == CtModelType.ALL || type == CtModelType.GROUP)
            {
                res.AddRange(Library.StringDivider(GroupEn.ToLower(), " / "));
            }
            if (type == CtModelType.ALL || type == CtModelType.SERIES)
            {
                res.AddRange(Library.StringDivider(SeriesEn.ToLower(), " / "));
            }
            if (type == CtModelType.ALL || type == CtModelType.CHARACTER)
            {
                res.AddRange(Library.StringDivider(CharacterEn.ToLower(), " / "));
            }
            if (type == CtModelType.ALL || type == CtModelType.TAG)
            {
                res.AddRange(Library.StringDivider(TagEn.ToLower(), " / "));
            }
            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetWebViewerUrl()
        {
            if (Workid[0] == 'm')
                return string.Format("http://wasabisyrup.com/archives/{0}", Workid.Substring(1));
            else if (Workid[0] == 'l')
                return string.Empty;
            else if (Workid[0] == 'g')
                return string.Format("https://marumaru.in/b/manga/{0}", Workid.Substring(1));
            else
                return string.Format("https://hitomi.la/reader/{0}.html#1", Workid);
        }


        public bool Contains(CtModel model)
        {
            return GetTags(model.CtType, false).Exists(i => model.GetTags().Exists(j => string.Compare(i.Replace(" ", "").ToLower(), j.Replace(" ", "").ToLower(), true) == 0));
        }

        public bool AddModel(CtModel model)
        {
            bool flag = false;
            if (!Contains(model))
            {
                if (model.CtType == CtModelType.ARTIST)
                    if (ArtistEn.Length == 0)
                    {
                        ArtistEn = ((CtArtist)model).NameEn;
                        ArtistKr = ((CtArtist)model).NameKr;
                    }
                    else
                    {
                        ArtistEn += " / " + ((CtArtist)model).NameEn;
                        ArtistKr += " / " + ((CtArtist)model).NameKr;
                    }
                else if (model.CtType == CtModelType.GROUP)
                    if (GroupEn.Length == 0)
                    {
                        GroupEn = ((CtGroup)model).NameEn;
                        GroupKr = ((CtGroup)model).NameKr;
                    }
                    else
                    {
                        GroupEn += " / " + ((CtGroup)model).NameEn;
                        GroupKr += " / " + ((CtGroup)model).NameKr;
                    }
                else if (model.CtType == CtModelType.SERIES)
                    if (SeriesEn.Length == 0)
                    {
                        SeriesEn = ((CtSeries)model).NameEn;
                        SeriesKr = ((CtSeries)model).NameKr;
                    }
                    else
                    {
                        SeriesEn += " / " + ((CtSeries)model).NameEn;
                        SeriesKr += " / " + ((CtSeries)model).NameKr;
                    }
                else if (model.CtType == CtModelType.CHARACTER)
                    if (CharacterEn.Length == 0)
                    {
                        CharacterEn = ((CtCharacter)model).NameEn;
                        CharacterKr = ((CtCharacter)model).NameKr;
                    }
                    else
                    {
                        CharacterEn += " / " + ((CtCharacter)model).NameEn;
                        CharacterKr += " / " + ((CtCharacter)model).NameKr;
                    }
                else if (model.CtType == CtModelType.TAG)
                    if (TagEn.Length == 0)
                    {
                        TagEn = ((CtTag)model).NameEn;
                        TagKr = ((CtTag)model).NameKr;
                    }
                    else
                    {
                        TagEn += " / " + ((CtTag)model).NameEn;
                        TagKr += " / " + ((CtTag)model).NameKr;
                    }
                else
                    return false;
            }
            return flag;
        }

        public bool ExistsTag(string str, CtModelType type, bool ignoreCase)
        {
            foreach (string strs in GetTags(type, true))
            {
                if (string.Compare(strs, str, ignoreCase) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        // Should also override == and != operators.
        public bool FixTitle()
        {
            char[] trimchar = { ' ', '!', ',', '(', ')', '[', ']', '.', '"', '\'', '?', '~', '-', '+', '*', '&', '%', ':', ';', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            bool flag = false;
            List<char> strarr = TitleEn.ToList();
            List<char> spechar = new List<char>();
            string tempstr, tempstr2;
            // 특수문자 조정
            try
            {
                TitleEn = TitleEn
                    .Replace('ā', 'a').Replace('ō', 'o').Replace('ī', 'i').Replace('â', 'a').Replace('ē', 'e')
                    .Replace('“', '"').Replace('”', '"').Replace("_", " ").Replace("…", "...").Replace("・・・", "...").Replace("・", " • ")
                    .Replace("＊", "*").Replace(" ／ ", " / ").Replace("／", " / ").Replace("｜", "|").Replace("ㅣ", "|").Replace("│", "|").Replace("│", "|")
                    .Replace("＜", " <").Replace("＞", ">").Replace("～", "~").Replace("～", "~").Replace("！", "! ").Replace(" !", "!").Replace("？", "? ").Replace(" ?", "?").Replace(" ,", ",").Replace("、", ", ")
                    .Replace("&Amp;", "&").Replace("&Quot;", "\"").Replace("&amp;", "&").Replace("&quot;", "\"").Replace("&#39;", "'").Replace("&gt;", ">").Replace("&lt;", "<")
                    .Replace("CH.", "Ch.").Replace("ch.", "Ch.").Replace("Ch. ", "Ch.").Replace("VOL.", "Vol.").Replace("vol.", "Vol.").Replace("Vol. ", "Vol.")
                    .Replace("ACT.", "Act.").Replace("act.", "Act.").Replace("Act. ", "Act.").Replace("&nbsp;", " ");
                TitleKr = TitleKr
                    .Replace('ā', 'a').Replace('ō', 'o').Replace('ī', 'i')
                    .Replace('“', '"').Replace('”', '"').Replace("_", " ").Replace("…", "...").Replace("・・・", "...").Replace("・", " • ")
                    .Replace("＊", "*").Replace(" ／ ", " / ").Replace("／", " / ").Replace("｜", "|").Replace("ㅣ", "|").Replace("│", "|").Replace("│", "|")
                    .Replace("＜", " <").Replace("＞", ">").Replace("～", "~").Replace("～", "~").Replace("！", "! ").Replace(" !", "!").Replace("？", "? ").Replace(" ?", "?").Replace(" ,", ",").Replace("、", ", ")
                    .Replace("&Amp;", "&").Replace("&Quot;", "\"").Replace("&amp;", "&").Replace("&quot;", "\"").Replace("&#39;", "'").Replace("&gt;", ">").Replace("&lt;", "<")
                    .Replace("CH.", "Ch.").Replace("ch.", "Ch.").Replace("Ch. ", "Ch.").Replace("VOL.", "Vol.").Replace("vol.", "Vol.").Replace("Vol. ", "Vol.")
                    .Replace("ACT.", "Act.").Replace("act.", "Act.").Replace("Act. ", "Act.").Replace("&nbsp;", " ");
            }
            catch { }


            // 띄어쓰기 조정
            try
            {
                bool flagd = false;
                int flagindex = 0;
                int spaceindex = -1;
                flag = false;
                strarr = TitleEn.ToList();
                for (int i = 0; i < strarr.Count; i++)
                {
                    if (strarr[i] == ' ')
                        spaceindex = i;

                    if (strarr[i] == ',')
                    {
                        if (i + 1 < strarr.Count && strarr[i + 1] != ',' && strarr[i + 1] != ' ')
                        {
                            strarr.Insert(i + 1, ' ');
                            flag = true;
                        }
                    }
                    if (Library.IsMark(strarr[i]))
                    {
                        int j;
                        for (j = i + 1; j < strarr.Count && strarr[j] != ' '; j++) ;

                        if ((i - spaceindex == 1 || i - spaceindex > 2) && (j - i > 2 || j - i == 1))
                        {
                            if (0 < i && Library.isEnglish(strarr[i - 1]) && strarr[i - 1] != ' ')
                            {
                                strarr.Insert(i, ' ');
                                flag = true;
                                i++;
                            }

                            if (i + 1 < strarr.Count && Library.isEnglish(strarr[i + 1]) && strarr[i + 1] != ' ')
                            {
                                strarr.Insert(i + 1, ' ');
                                flag = true;
                                i++;
                            }
                        }
                    }
                    if (strarr[i] == '~')
                    {
                        if (flagd)
                        {
                            if (i + 1 < strarr.Count && strarr[i + 1] != ' ')
                            {
                                strarr.Insert(i + 1, ' ');
                                flag = true;
                            }
                            if (flagindex > 0 && strarr[flagindex - 1] != ' ')
                            {
                                strarr.Insert(flagindex, ' ');
                                flag = true;
                            }
                            flagd = false;
                        }
                        else
                        {
                            flagindex = i;
                            flagd = true;
                        }
                    }
                }
                if (flag)
                {
                    TitleEn = string.Concat(strarr).Trim();
                }

                flag = false;
                int indextemp = TitleEn.IndexOf("...") + 3;
                if (indextemp > 3 && TitleEn.Length > indextemp && TitleEn[indextemp - 4] != ' ')
                {
                    foreach (char ch in trimchar)
                    {
                        if (TitleEn[indextemp] == ch)
                        {
                            flag = true;
                        }
                    }
                    if (!flag)
                        TitleEn = TitleEn.Insert(indextemp, " ");
                }
            }
            catch { }

            flag = false;
            //필요없는 텍스트 지움
            try
            {
                if (TitleEn.Contains("Uncensored"))
                {
                    TitleEn = TitleEn.Replace("Uncensored", string.Empty).Trim();
                    flag = true;
                }
                if (TitleEn.Contains("uncensored"))
                {
                    TitleEn = TitleEn.Replace("uncensored", string.Empty).Trim();
                    flag = true;
                }
                if (TitleEn.Contains("Decensored"))
                {
                    TitleEn = TitleEn.Replace("Decensored", string.Empty).Trim();
                    flag = true;
                }
                if (TitleEn.Contains("decensored"))
                {
                    TitleEn = TitleEn.Replace("decensored", string.Empty).Trim();
                    flag = true;
                }
                if (TitleKr.Contains("Uncensored"))
                {
                    TitleKr = TitleKr.Replace("Uncensored", string.Empty).Trim();
                    flag = true;
                }
                if (TitleKr.Contains("uncensored"))
                {
                    TitleKr = TitleKr.Replace("uncensored", string.Empty).Trim();
                    flag = true;
                }
                if (TitleKr.Contains("Decensored"))
                {
                    TitleKr = TitleKr.Replace("Decensored", string.Empty).Trim();
                    flag = true;
                }
                if (TitleKr.Contains("decensored"))
                {
                    TitleKr = TitleKr.Replace("decensored", string.Empty).Trim();
                    flag = true;
                }


                if (flag && !TagEn.Contains("Uncensored"))
                {
                    if (TagEn.Length == 0)
                        TagEn += "Uncensored";
                    else
                        TagEn += " / Uncensored";
                }

                if (TitleEn.Contains("Korean"))
                {
                    TitleEn = TitleEn.Replace("Korean", string.Empty).Trim();
                    TitleKr = TitleKr.Replace("Korean", string.Empty).Trim();
                }
                if (TitleEn.Contains("korean"))
                {
                    TitleEn = TitleEn.Replace("korean", string.Empty).Trim();
                    TitleKr = TitleKr.Replace("korean", string.Empty).Trim();
                }
                if (TitleEn.Contains("Korea"))
                {
                    TitleEn = TitleEn.Replace("Korea", string.Empty).Trim();
                    TitleKr = TitleKr.Replace("Korea", string.Empty).Trim();
                }
                if (TitleEn.Contains("korea"))
                {
                    TitleEn = TitleEn.Replace("korea", string.Empty).Trim();
                    TitleKr = TitleKr.Replace("korea", string.Empty).Trim();
                }
                TitleEn = TitleEn.Replace("()", string.Empty).Replace("[]", string.Empty);
                TitleKr = TitleKr.Replace("()", string.Empty).Replace("[]", string.Empty);

            }
            catch { }

            //괄호조정
            {
                flag = false;
                char[,] bracket = new char[7, 2] { { '(', ')' }, { '{', '}' }, { '[', ']' }, { '<', '>' }, { '「', '」' }, { '【', '】' }, { '『', '』' } };
                int index = 0;

                strarr = TitleEn.ToList();
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < strarr.Count; j++)
                    {
                        if (strarr[j] == bracket[i, 0])
                        {
                            if (j > 0 && strarr[j - 1] != ' ')
                            {
                                strarr.Insert(j, ' ');
                            }
                            if (flag)
                            {
                                strarr[index] = ' ';
                            }
                            flag = true;
                            index = j;
                        }
                        else if (strarr[j] == bracket[i, 1])
                        {
                            if (!flag)
                            {
                                strarr[j] = ' ';
                            }
                            else if (j < strarr.Count - 1 && strarr[j + 1] != ' ')
                            {
                                strarr.Insert(j + 1, ' ');
                            }
                            flag = false;
                        }
                        if (j == strarr.Count - 1 && flag)
                        {
                            strarr[index] = ' ';
                        }
                    }
                }
                TitleEn = string.Concat(strarr).Trim();

                index = 0;
                strarr = TitleKr.ToList();
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < strarr.Count; j++)
                    {
                        if (strarr[j] == bracket[i, 0])
                        {
                            if (j > 0 && strarr[j - 1] != ' ')
                            {
                                strarr.Insert(j, ' ');
                            }
                            if (flag)
                            {
                                strarr[index] = ' ';
                            }
                            flag = true;
                            index = j;
                        }
                        else if (strarr[j] == bracket[i, 1])
                        {
                            if (!flag)
                            {
                                strarr[j] = ' ';
                            }
                            else if (j < strarr.Count - 1 && strarr[j + 1] != ' ')
                            {
                                strarr.Insert(j + 1, ' ');
                            }
                            flag = false;
                        }
                        if (j == strarr.Count - 1 && flag)
                        {
                            strarr[index] = ' ';
                        }
                    }
                }
                TitleKr = string.Concat(strarr).Trim();
            }

            //띄어쓰기 조정
            while (TitleEn.Contains("  "))
            {
                TitleEn = TitleEn.Replace("  ", " ");
            }

            //번역 나누기
            try
            {

                if (TitleEn.Contains("|"))
                {
                    Library.extractionString(TitleEn, out tempstr, "|", out tempstr2);
                }
                else if(TitleEn.Contains("┃"))
                {
                    Library.extractionString(TitleEn, out tempstr, "┃", out tempstr2);
                }
                else if (TitleEn.Contains(" / "))
                {
                    Library.extractionString(TitleEn, out tempstr, " / ", out tempstr2);
                }
                else if (TitleEn.Contains(" l "))
                {
                    Library.extractionString(TitleEn, out tempstr, " l ", out tempstr2);
                }
                else
                {
                    tempstr = string.Empty; tempstr2 = string.Empty;
                }

                if (Library.isKorean(tempstr) && !Library.isKorean(tempstr2))
                {
                    TitleEn = tempstr2;
                    TitleKr = tempstr;
                    IsTranslated = true;
                }
                else if (!Library.isKorean(tempstr) && Library.isKorean(tempstr2))
                {
                    TitleEn = tempstr;
                    TitleKr = tempstr2;
                    IsTranslated = true;
                }
            }
            catch { }

            try
            {
                if(IsTranslated)
                {
                    string temp = string.Empty;
                    string addon = string.Empty;
                    string volumn = string.Empty;
                    StringInfo.DivideAddOnString(TitleKr, out temp, out addon, out volumn);

                    if (volumn.Length != 0)
                    {
                        TitleEn += " " + volumn;
                    }

                    if (addon.Length != 0)
                    {
                        TitleEn += " + " + addon;
                    }
                }

            }
            catch
            {

            }

            //한글 제목 채우기
            if (TitleKr.Length == 0)
            {
                TitleKr = TitleEn;
            }

            TitleKr = TitleKr.Trim(' ', '_');
            TitleEn = TitleEn.Trim(' ', '_');

            //제목없음 처리
            if (this.TitleEn.Trim().Length == 0)
            {
                this.TitleEn = "No Title";
                this.TitleKr = "제목없음";
            }
            else if (string.Compare(TitleEn, "No Title") == 0 && string.Compare(TitleKr, "제목없음") != 0)
            {
                this.TitleKr = "제목없음";
            }

            flag = false;
            if (ArtistEn.Trim().Length == 0)
            {
                this.ArtistKr = this.ArtistEn = string.Empty;
                flag = true;
            }
            if (this.ArtistKr.Trim().Length == 0)
            {
                this.ArtistKr = this.ArtistEn;
                flag = true;
            }

            if (this.GroupEn.Trim().Length == 0)
            {
                this.GroupKr = this.GroupEn = string.Empty;
                flag = true;
            }
            if (this.GroupKr.Trim().Length == 0)
            {
                this.GroupKr = this.GroupEn;
            }

            if (this.SeriesEn.Trim().Length == 0)
            {
                this.SeriesKr = this.SeriesEn = string.Empty;
                flag = true;
            }

            if (this.SeriesKr.Trim().Length == 0)
            {
                this.SeriesKr = this.SeriesEn;
                flag = true;
            }

            if (this.CharacterEn.Trim().Length == 0)
            {
                this.CharacterKr = this.CharacterEn = string.Empty;
                flag = true;
            }

            if (this.CharacterKr.Trim().Length == 0)
            {
                this.CharacterKr = this.CharacterEn;
                flag = true;
            }

            if (this.TagEn.Trim().Length == 0)
            {
                this.TagKr = this.TagEn = string.Empty;
                flag = true;
            }

            if (this.TagKr.Trim().Length == 0)
            {
                this.TagKr = this.TagEn;
                flag = true;
            }
            if (this.UploadedDate == null)
            {
                UploadedDate = DateTime.Parse("2000-1-1");
                flag = true;
            }
            if (flag || ModifiedDate == null)
                ModifiedDate = DateTime.Now;
            return flag;
        }

        public bool FixModel(CtModel model)
        {
            switch (model.CtType)
            {
                case CtModelType.ARTIST:
                    return FixArtist(model as CtArtist);
                case CtModelType.GROUP:
                    return FixGroup(model as CtGroup);
                case CtModelType.SERIES:
                    return FixSeries(model as CtSeries);
                case CtModelType.CHARACTER:
                    return FixCharacter(model as CtCharacter);
                case CtModelType.TAG:
                    return FixTag(model as CtTag);
                default:
                    return false;
            }
        }

        public bool FixArtist(CtArtist model)
        {
            bool flag = false;
            string tempkr = null;
            string tempen = null;
            ArtistEn = ArtistEn.Trim(' ', '/');
            ArtistKr = ArtistKr.Trim(' ', '/');
            List<string> tempList_en = Library.StringDivider(ArtistEn, " / ");
            List<string> tempList_kr = Library.StringDivider(ArtistKr, " / ");
            int index = tempList_en.FindIndex(i => model.GetTags().Exists(j => string.Compare(i.Replace(" ", "").ToLower(), j.Replace(" ", "").ToLower(), true) == 0));
            if (index >= 0 && string.Compare(tempList_kr[index], model.NameKr) != 0)
            {
                tempList_en[index] = model.NameEn;
                tempList_kr[index] = model.NameKr;

                for (int i = 0; i < tempList_en.Count; i++)
                {
                    if (tempList_kr.Count > i)
                    {
                        tempkr = tempList_kr[i];
                    }
                    else
                    {
                        tempkr = tempList_en[i];
                    }
                    tempen = tempList_en[i];
                    if (i == 0)
                    {
                        ArtistEn = tempen;
                        ArtistKr = tempkr;
                    }
                    else
                    {
                        ArtistEn += " / " + tempen;
                        ArtistKr += " / " + tempkr;
                    }
                }
                flag = true;
            }
            return flag;
        }

        public bool FixArtist(CtModelList models, CtModelList cts)
        {
            bool flag = false;
            // 태그리스트에서 작가, 시리즈, 캐릭터 정보를 탐지
            string tempen = string.Empty;
            string tempkr = string.Empty;
            CtArtist tempTag;
            foreach (string tagen in Library.StringDivider(ArtistEn, " / "))
            {
                flag = false;
                tempTag = null;
                foreach (CtModel model in models)
                {
                    if (model.CtType == CtModelType.ARTIST && model.GetTags().Exists(i => string.Compare(i.Replace(" ", "").ToLower(), tagen.Replace(" ", "").ToLower(), true) == 0))
                    {
                        tempTag = (CtArtist)model;
                        break;
                    }
                }
                if (tempTag != null)
                {
                    if (tempen.Length == 0)
                    {
                        tempen = tempTag.NameEn;
                        tempkr = tempTag.NameKr;
                    }
                    else
                    {
                        tempen += " / " + tempTag.NameEn;
                        tempkr += " / " + tempTag.NameKr;
                    }
                }
                else
                {
                    if (tempen.Length == 0)
                    {
                        tempen = tempkr = tagen;
                    }
                    else
                    {
                        tempen += " / " + tagen;
                        tempkr += " / " + tagen;
                    }
                    cts.Add(new CtArtist(tagen));
                }
            }
            ArtistEn = tempen;
            ArtistKr = tempkr;
            return flag;
        }

        public bool FixGroup(CtGroup model)
        {
            bool flag = false;
            string tempkr = null;
            string tempen = null;
            GroupEn = GroupEn.Trim(' ', '/');
            GroupKr = GroupKr.Trim(' ', '/');
            List<string> tempList_en = Library.StringDivider(GroupEn, " / ");
            List<string> tempList_kr = Library.StringDivider(GroupKr, " / ");
            int index = tempList_en.FindIndex(i => model.GetTags().Exists(j => string.Compare(i.Replace(" ", "").ToLower(), j.Replace(" ", "").ToLower(), true) == 0));
            if (index >= 0 && (string.Compare(tempList_kr[index], model.NameKr) != 0 || string.Compare(tempList_en[index], model.NameEn) != 0))
            {
                tempList_en[index] = model.NameEn;
                tempList_kr[index] = model.NameKr;

                for (int i = 0; i < tempList_en.Count; i++)
                {
                    if (tempList_kr.Count > i)
                    {
                        tempkr = tempList_kr[i];
                    }
                    else
                    {
                        tempkr = tempList_en[i];
                    }
                    tempen = tempList_en[i];
                    if (i == 0)
                    {
                        GroupEn = tempen;
                        GroupKr = tempkr;
                    }
                    else
                    {
                        GroupEn += " / " + tempen;
                        GroupKr += " / " + tempkr;
                    }
                }
                flag = true;
            }
            return flag;
        }

        public bool FixGroup(CtModelList models, CtModelList cts)
        {
            bool flag = false;
            // 태그리스트에서 작가, 시리즈, 캐릭터 정보를 탐지
            string tempen = string.Empty;
            string tempkr = string.Empty;
            CtGroup tempTag;

            foreach (string tagen in Library.StringDivider(GroupEn, " / "))
            {
                flag = false;
                tempTag = null;
                foreach (CtModel model in models)
                {
                    if (model.CtType == CtModelType.GROUP && model.GetTags().Exists(i => string.Compare(i.Replace(" ", "").ToLower(), tagen.Replace(" ", "").ToLower(), true) == 0))
                    {
                        tempTag = (CtGroup)model;
                        break;
                    }
                }
                if (tempTag != null)
                {
                    if (tempen.Length == 0)
                    {
                        tempen = tempTag.NameEn;
                        tempkr = tempTag.NameKr;
                    }
                    else
                    {
                        tempen += " / " + tempTag.NameEn;
                        tempkr += " / " + tempTag.NameKr;
                    }
                }
                else
                {
                    if (tempen.Length == 0)
                    {
                        tempen = tempkr = tagen;
                    }
                    else
                    {
                        tempen += " / " + tagen;
                        tempkr += " / " + tagen;
                    }
                    cts.Add(new CtGroup(tagen));
                }
            }
            GroupEn = tempen;
            GroupKr = tempkr;
            return flag;
        }

        public bool FixSeries(CtSeries model)
        {
            bool flag = false;
            string tempkr = null;
            string tempen = null;
            SeriesEn = SeriesEn.Trim(' ', '/');
            SeriesKr = SeriesKr.Trim(' ', '/');
            List<string> tempList_en = Library.StringDivider(SeriesEn, " / ");
            List<string> tempList_kr = Library.StringDivider(SeriesKr, " / ");

            if(tempList_en.Count == tempList_kr.Count)
            {
                int index = tempList_en.FindIndex(i => model.GetTags().Exists(j => string.Compare(i.Replace(" ", "").ToLower(), j.Replace(" ", "").ToLower(), true) == 0));
                if (index >= 0 && (string.Compare(tempList_kr[index], model.NameKr) != 0 || string.Compare(tempList_en[index], model.NameEn) != 0))
                {
                    tempList_en[index] = model.NameEn;
                    tempList_kr[index] = model.NameKr;

                    for (int i = 0; i < tempList_en.Count; i++)
                    {
                        if (tempList_kr.Count > i)
                        {
                            tempkr = tempList_kr[i];
                        }
                        else
                        {
                            tempkr = tempList_en[i];
                        }
                        tempen = tempList_en[i];
                        if (i == 0)
                        {
                            SeriesEn = tempen;
                            SeriesKr = tempkr;
                        }
                        else
                        {
                            SeriesEn += " / " + tempen;
                            SeriesKr += " / " + tempkr;
                        }
                    }
                    flag = true;
                }
            }
            else
            {
                throw new Exception();
            }
            return flag;
        }

        public bool FixSeries(CtModelList models, CtModelList cts)
        {
            bool flag = false;
            // 태그리스트에서 작가, 시리즈, 캐릭터 정보를 탐지
            string tempen = string.Empty;
            string tempkr = string.Empty;
            CtSeries tempTag;

            foreach (string tagen in Library.StringDivider(SeriesEn, " / "))
            {
                flag = false;
                tempTag = null;
                foreach (CtModel model in models)
                {
                    if (model.CtType == CtModelType.SERIES && model.GetTags().Exists(i => string.Compare(i.Replace(" ", "").ToLower(), tagen.Replace(" ", "").ToLower(), true) == 0))
                    {
                        tempTag = (CtSeries)model;
                        break;
                    }
                }
                if (tempTag != null)
                {
                    if (tempen.Length == 0)
                    {
                        tempen = tempTag.NameEn;
                        tempkr = tempTag.NameKr;
                    }
                    else
                    {
                        tempen += " / " + tempTag.NameEn;
                        tempkr += " / " + tempTag.NameKr;
                    }
                }
                else
                {
                    if (tempen.Length == 0)
                    {
                        tempen = tempkr = tagen;
                    }
                    else
                    {
                        tempen += " / " + tagen;
                        tempkr += " / " + tagen;
                    }
                    cts.Add(new CtSeries(tagen));
                }
            }
            SeriesEn = tempen;
            SeriesKr = tempkr;
            return flag;
        }

        public bool FixCharacter(CtCharacter model)
        {
            bool flag = false;
            string tempkr = null;
            string tempen = null;
            CharacterEn = CharacterEn.Trim(' ', '/');
            CharacterKr = CharacterKr.Trim(' ', '/');
            List<string> tempList_en = Library.StringDivider(CharacterEn, " / ");
            List<string> tempList_kr = Library.StringDivider(CharacterKr, " / ");
            int index = tempList_en.FindIndex(i => model.GetTags().Exists(j => string.Compare(i.Replace(" ", "").ToLower(), j.Replace(" ", "").ToLower(), true) == 0));
            if (index >= 0 && (string.Compare(tempList_kr[index], model.NameKr) != 0 || string.Compare(tempList_en[index], model.NameEn) != 0))
            {
                tempList_en[index] = model.NameEn;
                tempList_kr[index] = model.NameKr;

                for (int i = 0; i < tempList_en.Count; i++)
                {
                    if (tempList_kr.Count > i)
                    {
                        tempkr = tempList_kr[i];
                    }
                    else
                    {
                        tempkr = tempList_en[i];
                    }
                    tempen = tempList_en[i];
                    if (i == 0)
                    {
                        CharacterEn = tempen;
                        CharacterKr = tempkr;
                    }
                    else
                    {
                        CharacterEn += " / " + tempen;
                        CharacterKr += " / " + tempkr;
                    }
                }
                flag = true;
            }
            return flag;
        }

        public bool FixCharacter(CtModelList models, CtModelList names, CtModelList cts)
        {
            bool flag = false;
            // 태그리스트에서 작가, 시리즈, 캐릭터 정보를 탐지
            string tempen = string.Empty;
            string tempkr = string.Empty;
            CtCharacter tempTag;

            foreach (string tagen in Library.StringDivider(CharacterEn, " / "))
            {
                flag = false;
                tempTag = null;
                foreach (CtModel model in models)
                {
                    if (model.CtType == CtModelType.CHARACTER && model.GetTags().Exists(i => string.Compare(i.Replace(" ", "").ToLower(), tagen.Replace(" ", "").ToLower(), true) == 0))
                    {
                        tempTag = (CtCharacter)model;
                        break;
                    }
                }
                if (tempTag != null)
                {
                    if (tempen.Length == 0)
                    {
                        tempen = tempTag.NameEn;
                        tempkr = tempTag.NameKr;
                    }
                    else
                    {
                        tempen += " / " + tempTag.NameEn;
                        tempkr += " / " + tempTag.NameKr;
                    }
                }
                else
                {
                    if (tempen.Length == 0)
                    {
                        tempen = tempkr = tagen;
                    }
                    else
                    {
                        tempen += " / " + tagen;
                        tempkr += " / " + tagen;
                    }

                    CtCharacter newCrt = new CtCharacter(tagen);
                    newCrt.TranslateBy(names, cts);
                    cts.Add(newCrt);

                    //캐릭터를 이름으로 수정

                }
            }
            CharacterEn = tempen;
            CharacterKr = tempkr;
            return flag;
        }

        public bool FixTag(CtTag model)
        {
            bool flag = false;
            string tempkr = null;
            string tempen = null;
            TagEn = TagEn.Trim(' ', '/');
            TagKr = TagKr.Trim(' ', '/');
            List<string> tempList_en = Library.StringDivider(TagEn, " / ");
            List<string> tempList_kr = Library.StringDivider(TagKr, " / ");
            int index = tempList_en.FindIndex(i => model.GetTags().Exists(j => string.Compare(i.Replace(" ", "").ToLower(), j.Replace(" ", "").ToLower(), true) == 0));
            if (index >= 0 && (string.Compare(tempList_kr[index], model.NameKr) != 0 || string.Compare(tempList_en[index], model.NameEn) != 0))
            {
                tempList_en[index] = model.NameEn;
                tempList_kr[index] = model.NameKr;

                for (int i = 0; i < tempList_en.Count; i++)
                {
                    if (tempList_kr.Count > i)
                    {
                        tempkr = tempList_kr[i];
                    }
                    else
                    {
                        tempkr = tempList_en[i];
                    }
                    tempen = tempList_en[i];
                    if (i == 0)
                    {
                        TagEn = tempen;
                        TagKr = tempkr;
                    }
                    else
                    {
                        TagEn += " / " + tempen;
                        TagKr += " / " + tempkr;
                    }
                }
                flag = true;
            }
            return flag;
        }

        public bool FixTag(CtModelList models, CtModelList cts)
        {
            bool flag = false;
            // 태그리스트에서 작가, 시리즈, 캐릭터 정보를 탐지
            string tempen = string.Empty;
            string tempkr = string.Empty;
            CtTag tempTag;
            CtModel tempModel;

            foreach (string tagen in Library.StringDivider(TagEn, " / "))
            {
                flag = false;
                tempTag = null;
                tempModel = null;
                foreach (CtModel model in models)
                {
                    if (model.GetTags().Exists(i => string.Compare(i.Replace(" ", "").ToLower(), tagen.Replace(" ", "").ToLower(), true) == 0))
                    {
                        if (model.CtType == CtModelType.TAG)
                        {
                            tempTag = (CtTag)model;
                            break;
                        }
                        else
                        {                            
                            tempModel = model;
                        }
                    }

                }

                if (tempTag != null)
                {
                    if (tempen.Length == 0)
                    {
                        tempen = tempTag.NameEn;
                        tempkr = tempTag.NameKr;
                    }
                    else
                    {
                        tempen += " / " + tempTag.NameEn;
                        tempkr += " / " + tempTag.NameKr;
                    }
                }
                else if (tempModel != null)
                {
                    AddModel(tempModel);
                }
                else
                {
                    if (tempen.Length == 0)
                    {
                        tempen = tempkr = tagen;
                    }
                    else
                    {
                        tempen += " / " + tagen;
                        tempkr += " / " + tagen;
                    }
                    cts.Add(new CtTag(tagen));
                }
            }
            TagEn = tempen;
            TagKr = tempkr;
            return flag;
        }

        public bool FixAllTag(CtModelList artists, CtModelList groups, CtModelList serieses, CtModelList characters, CtModelList tags, CtModelList names, CtModelList new_models)
        {
            bool a = FixArtist(artists, new_models);
            bool b = FixGroup(groups, new_models);
            bool c = FixSeries(serieses, new_models);
            bool d = FixCharacter(characters, names, new_models);
            bool e = FixTag(tags, new_models);
            return (a || b || c || d || e);
        }

        public async Task<bool> TranslateTitle(CtWordList wordlist, CtNameList namelist, IWebConnection web)
        {
            return await TranslateTitle(wordlist, namelist, web, false);
        }
        public async Task<bool> TranslateTitle(CtWordList wordlist, CtNameList namelist, IWebConnection web, bool force)
        {
            string temp = TitleKr;
            if(!IsTranslated || force)
            {
                TitleKr = await StringInfo.TranslateTitle(TitleEn, this, wordlist, namelist, web);
                if (TitleKr == temp)
                    return false;
                else
                {
                    if (Library.isJapanese(temp) && !Library.isJapanese(TitleKr))
                        IsTranslated = true;
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public string GetPageImageUrl(int page)
        {
            string[] res = PageUrl.Split('/');
            if (res.Length > page && ImageUrl.Length > 0)
                return ImageUrl + res[page];
            else
                return CoverUrl;
        }

        public string GetDirectory()
        {
            string temp_artist;
            if (ArtistEn.Length - ArtistEn.Replace(" / ", "  ").Length > 2)
            {
                temp_artist = "Variable Artist";
            }
            else if (ArtistEn.Length == 0)
            {
                if (GroupEn.Length == 0)
                    temp_artist = "Unknown Artist";
                else
                    temp_artist = GroupEn;
            }
            else
            {
                temp_artist = ArtistEn;
            }
            temp_artist.Trim(' ', '.').Replace("?", "？").Replace("*", "＊").Replace(" / ", "／").Replace("/", "／").Replace(":", "：")
                    .Replace("|", "｜").Replace("<", "＜").Replace(">", "＞").Replace(@"""", "'").Trim(' ', '.');

            string tempTitle = TitleEn.Trim(' ', '.').Replace("?", "？").Replace("*", "＊").Replace(" / ", "／").Replace("/", "／").Replace(":", "：")
                    .Replace("|", "｜").Replace("<", "＜").Replace(">", "＞").Replace(@"""", "'").Trim(' ', '.');
            return Path.Combine(temp_artist, tempTitle, Language, Id);
        }

        public string GetExtention(int index)
        {
            return (PageUrl.Split('/')[index]).EndsWith(".gif") ? ".gif" : ".jpg";
        }
    }
}
