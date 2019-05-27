using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicViewer.CtModels
{
    public class StringInfo
    {
        public List<CtWord> wordInfolist;
        public List<CtModel> combinedWords;
        public List<string> wordStringlist;
        public List<string> originStringlist;
        public List<int> wordIndexList;
        string originalString;
        string originalStringLower;
        public int Ecounter;
        public int Jcounter;
        public int TransCounter;
        public int LastCount;
        public int AdditionalCount;

        public CtWord Name2Word(CtName name)
        {
            return new CtWord(name.NameEn, name.NameKr, "false", "명사(사람이름)", "명사(사람이름)", string.Empty, name.Jp ? "일본" : "외국");
        }

        public StringInfo(string sentance, CtComic work, CtWordList wordlist, CtNameList namelist)
        {
            int i, j, tempInt, tempInt2;
            bool flag, seriesflag, nameflag, combinedflag;
            CtWord emptyWord, newWord, tempWord, frontword;
            string tempword, tempstr, tempstr2, combinedstr;
            List<string> tempList = new List<string>();
            List<string> newstrList = new List<string>();
            CtWord seriesWord = null;
            wordInfolist = new List<CtWord>();
            wordStringlist = new List<string>();
            originStringlist = new List<string>();
            wordIndexList = new List<int>();
            Ecounter = 0;
            Jcounter = 0;
            TransCounter = 0;
            LastCount = 0;
            AdditionalCount = 0;
            originalString = sentance;
            originalStringLower = sentance.ToLower();
            seriesflag = false;
            tempstr = string.Empty;
            combinedstr = string.Empty;


            wordlist.Sort(new CtModelSortComparer(-1, SortOrder.Descending));


            //시리즈에서 찾기
            if (sentance.Length != 0 && work.SeriesEn.Length != 0 && sentance.ToLower().Contains(work.SeriesEn.ToLower()) && Library.IsWord(sentance.RemoveMark().ToLower(), work.SeriesEn.ToLower()))
            {
                tempstr = work.SeriesEn.ToLower().Replace(" ", "");
                sentance = sentance.Replace(work.SeriesEn, tempstr);
                seriesWord = new CtWord(tempstr, work.SeriesKr);
                seriesWord.Space = "false";
                seriesWord.TypeFwd = "명사(추상)";
                seriesWord.TypeBck = "명사(추상)";
                seriesWord.Language = "일본";
                seriesflag = true;
            }

            //합성단어 찾아 바꾸기
            combinedWords = wordlist.FindAll(item => (item.NameEn.Contains('-') || item.NameEn.Contains(' ')) && sentance.ToLower().Contains(item.NameEn.ToLower()));
            foreach (CtWord word in combinedWords)
            {
                tempstr2 = word.NameEn.ToLower();
                if (word.NameEn.Contains('-') || word.NameEn.Contains(' '))
                {
                    while (sentance.ToLower().Contains(tempstr2))
                    {
                        tempInt = sentance.ToLower().IndexOf(tempstr2);
                        tempInt2 = sentance.ToLower().IndexOf(tempstr2) + tempstr2.Length;
                        if (!((tempInt > 0 && Library.isEnglish(sentance[tempInt - 1]) && tempstr2[0] != '-') || (tempInt2 < sentance.Length && Library.isEnglish(sentance[tempInt2]) && tempstr2[tempstr2.Length - 1] != '-')))
                        {
                            if (
                                   (tempInt == 0 || sentance[tempInt - 1] < 'A' || 'z' < sentance[tempInt - 1]) && (tempInt2 == sentance.Length || sentance[tempInt2] < 'A' || 'z' < sentance[tempInt2])
                                || (word.NameEn.First() == '-')
                                || (word.NameEn.Last() == '-')
                                )
                            {
                                sentance = sentance.Substring(0, tempInt) + word.NameKr + sentance.Substring(tempInt2, sentance.Length - tempInt2);
                            }
                            else
                                break;
                        }
                        else
                            break;
                    }
                }
            }

            List<string> templist = Library.StringDivider(sentance, " ");
            emptyWord = new CtWord(string.Empty, string.Empty);

            for (j = 0; j < templist.Count; j++)
            {
                combinedflag = false;
                flag = false;
                newWord = null;
                tempword = templist[j];
                if (seriesflag)
                {
                    if (string.Compare(tempword, tempstr, true) == 0)
                    {
                        newWord = seriesWord;
                        flag = true;
                    }
                }

                //문장부호 제거
                tempword = templist[j].Replace('。', '.').Replace("？", "?").Replace("！", "!").Replace("：", ":")
                    .Trim(' ', '「', '」', '『', '』', '!', ',', '(', ')', '[', ']', '<', '>', '.', '"', '\'', '?', '~', '-', '+', '*', '&', '%', ':', ';', '《', '》', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9');

                //빈문자열이면 그냥 추가
                if(tempword.Length != 0)
                {
                    //시리즈 목록에서 찾기
                    if (seriesflag)
                    {
                        if (string.Compare(tempword, tempstr, true) == 0)
                        {
                            newWord = seriesWord;
                            flag = true;
                        }
                    }

                    //대문자가 아니면 이름에서 찾음
                    if (newWord == null && string.Compare(tempword, tempword.ToUpper()) != 0)
                    {
                        for (i = 0; i < namelist.Count; i++)
                        {
                            if (string.Compare(tempword, namelist[i].Id, true) == 0)
                            {
                                newWord = Name2Word((CtName)namelist[i]);
                                flag = true;
                                break;
                            }
                        }
                    }

                    nameflag = false;
                    //이름이 캐릭터명에 있는지 조사
                    if (newWord != null)
                    {
                        foreach (string charstr2 in Library.StringDivider(work.CharacterEn.Replace(" / ", " "), " "))
                        {
                            if (string.Compare(newWord.NameEn, charstr2) == 0)
                            {
                                nameflag = true;
                                break;
                            }
                        }
                    }

                    //캐릭터 명에 없으면 단어목록에서 찾기
                    if (newWord == null || !nameflag)
                    {
                        tempWord = (CtWord)wordlist.GetModel(tempword);
                        if (tempWord != null)
                        {
                            newWord = tempWord;
                            flag = true;
                            if (combinedWords.Count > 0)
                            {
                                CtWord word = combinedWords.Find(item => item.NameKr == templist[j]) as CtWord;
                                if (word != null)
                                {
                                    combinedstr = word.NameEn;
                                    combinedflag = true;
                                }
                            }

                        }
                        else
                        {
                            tempWord = (CtWord)wordlist.GetModel(tempword, true);
                            if (tempWord != null)
                            {
                                newWord = tempWord;
                                //strList[j] = strList[j].Replace(tempword, tempword2);
                                //work.TitleEn = work.TitleEn.Replace(tempword, tempword2);
                                flag = true;
                            }
                        }
                        if (j == 0 && tempWord != null && templist[j].IndexOf(tempword) == 0 && tempWord.TypeFwd == "접미사")
                        {
                            tempWord = null;
                            flag = false;
                        }
                    }

                    wordlist.Sort(new CtModelSortComparer(-1, SortOrder.Ascending));
                    //못 찾은 경우와 찾은 경우
                    if (flag && newWord != null)
                    {
                        wordInfolist.Add(newWord);
                        wordIndexList.Add(j + AdditionalCount);
                        wordStringlist.Add(templist[j]);
                        if (combinedflag && combinedstr.Length > 0)
                        {
                            originStringlist.Add(combinedstr);
                        }
                        else
                        {
                            originStringlist.Add(templist[j]);
                        }
                        if (newWord.NameEn != newWord.NameKr)
                            IncTransWord();
                        LastCount = j + AdditionalCount;
                    }
                    else//단어를 나눠서 검색
                    {
                        flag = false;
                        if (!Library.isUpperCase(tempword))
                        {
                            foreach (CtWord word in wordlist)
                            {
                                if (tempword.ToLower().EndsWith(word.NameEn.ToLower()) && 
                                    ((word.TypeFwd.Contains("접미사(명)") && word.NameEn.Length > 1 && tempword.Length - word.NameEn.Length > 4) || 
                                    ((word.TypeFwd.Contains("접미사") || word.TypeFwd.Contains("접속사")) && word.NameEn.Length > 2 && tempword.Length - word.NameEn.Length > 3) || 
                                    ((word.TypeFwd.Contains("명사") || word.TypeFwd.Contains("동사") || word.TypeFwd.Contains("형용사")) && word.NameEn.Length > 3 && tempword.Length - word.NameEn.Length > 3)))
                                {
                                    if (tempword.Length > word.NameEn.Length && string.Compare(tempword.Substring(tempword.Length - word.NameEn.Length), word.NameEn, true) == 0)
                                    {
                                        tempstr = tempword.Substring(0, tempword.Length - word.NameEn.Length);
                                        frontword = (CtWord)wordlist.GetModel(tempstr);
                                        if (frontword == null)
                                        {
                                            frontword = (CtWord)wordlist.GetModel(tempstr, true);
                                            if (frontword != null)
                                                work.TitleEn = work.TitleEn.Replace(tempstr, frontword.NameEn);
                                            else
                                            {
                                                CtName name = (CtName)namelist.GetModel(tempstr);
                                                if (name != null)
                                                {
                                                    frontword = Name2Word(name);
                                                }
                                            }
                                        }
                                        if (frontword != null && 
                                            ((frontword.NameEn.Length == 2 && frontword.TypeFwd.Contains("명사") && frontword.TypeBck.Contains("명사")) || 
                                              (frontword.NameEn.Length > 2 && (!frontword.TypeFwd.Contains("접미사") || frontword.TypeFwd.Contains("명사") || frontword.TypeFwd.Contains("동사") || frontword.TypeFwd.Contains("형용사")))))
                                        {
                                            wordInfolist.Add(frontword);
                                            wordIndexList.Add(j + AdditionalCount);
                                            IncTransWord();
                                            LastCount = j + AdditionalCount;
                                            AdditionalCount++;
                                            wordInfolist.Add(word);
                                            wordIndexList.Add(j + AdditionalCount);
                                            IncTransWord();
                                            LastCount = j + AdditionalCount;
                                            int d = templist[j].ToLower().IndexOf(frontword.NameEn.ToLower()) + frontword.NameEn.Length;
                                            wordStringlist.Add(templist[j].Substring(0, d));
                                            originStringlist.Add(templist[j].Substring(0, d));
                                            wordStringlist.Add(templist[j].Substring(d));
                                            originStringlist.Add(templist[j].Substring(d));
                                            flag = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (flag)
                    {
                        continue;
                    }
                }

                //시작과 끝이 숫자인 경우
                if (Library.isNumber(templist[j]))
                {
                    wordInfolist.Add(new CtWord()
                    {
                        Name = "",
                        TypeFwd = "명사(숫자)",
                        TypeBck = "명사(숫자)",
                        Language = ""
                    });
                    wordIndexList.Add(j);
                }
                else if (Library.isMark(tempword))
                {
                }
                //모두 알파벳인 경우 영단어로 추정
                else if (Library.isEnglish(tempword))
                {
                    LastCount = j + AdditionalCount;
                    wordInfolist.Add(new CtWord()
                    {
                        Name = "",
                        Language = "외국"
                    });
                    wordIndexList.Add(j + AdditionalCount);
                }
                else
                {
                    wordInfolist.Add(emptyWord);
                    wordIndexList.Add(j + AdditionalCount);
                }
                wordStringlist.Add(templist[j]);
                originStringlist.Add(templist[j]);
            }
        }

        public void IncJapanese()
        {
            Jcounter++;
        }
        public void IncEnglish()
        {
            Ecounter++;
        }
        public void IncTransWord()
        {
            TransCounter++;
        }
        public void DesJapanese()
        {
            Jcounter--;
        }
        public void DecEnglish()
        {
            Ecounter--;
        }


        public async static Task<string> TranslateTitle(string original_title, CtComic comic, CtWordList wordlist, CtNameList namelist, IWebConnection web)
        {
            string tempstr_title = string.Empty;
            string result_title = string.Empty;
            string tempstr_front = string.Empty;
            string tempstr_rare = string.Empty;
            string tempstr_ch = string.Empty;
            string tempstr_add = string.Empty;

            DivideAddOnString(original_title, out tempstr_title, out tempstr_add, out tempstr_ch);

            if (tempstr_title.Contains(" - "))
            {
                List<string> strlist = Library.StringDivider(tempstr_title, " - ");
                foreach (string str in strlist)
                {
                    if (result_title.Length == 0)
                    {
                        result_title = await TranslateWord(str, comic, wordlist, namelist, web);
                    }
                    else
                    {
                        result_title += " - " + await TranslateWord(str, comic, wordlist, namelist, web);
                    }
                }
            }
            else if (tempstr_title.Contains(" | "))
            {
                List<string> strlist = Library.StringDivider(tempstr_title, " | ");
                foreach (string str in strlist)
                {
                    if (result_title.Length == 0)
                    {
                        result_title = await TranslateWord(str, comic, wordlist, namelist, web);
                    }
                    else
                    {
                        result_title += " | " + await TranslateWord(str, comic, wordlist, namelist, web);
                    }
                }
            }
            else if (tempstr_title.Contains(" -"))
            {
                int index = tempstr_title.IndexOf(" -");
                tempstr_rare = tempstr_title.Substring(index + 2, tempstr_title.Length - index - 2).Trim(' ');
                if (tempstr_rare.Length != 0 && tempstr_rare[tempstr_rare.Length - 1] == '-')
                {
                    tempstr_front = tempstr_title.Substring(0, index).Trim(' ');
                    result_title = await TranslateWord(tempstr_front, comic, wordlist, namelist, web) + " -" + await TranslateWord(tempstr_rare, comic, wordlist, namelist, web);

                }
                else
                {
                    result_title = await TranslateWord(tempstr_title, comic, wordlist, namelist, web);
                }
            }
            else if (tempstr_title.Contains(" ~"))
            {
                int index = tempstr_title.IndexOf(" ~");
                tempstr_rare = tempstr_title.Substring(index + 2, tempstr_title.Length - index - 2).Trim(' ');
                if (tempstr_rare.Length != 0 && tempstr_rare[tempstr_rare.Length - 1] == '~')
                {
                    tempstr_front = tempstr_title.Substring(0, index).Trim(' ');
                    result_title = await TranslateWord(tempstr_front, comic, wordlist, namelist, web) + " ~" + await TranslateWord(tempstr_rare, comic, wordlist, namelist, web);

                }
                else
                {
                    result_title = await TranslateWord(tempstr_title, comic, wordlist, namelist, web);
                }
            }
            else
            {
                result_title = await TranslateWord(tempstr_title, comic, wordlist, namelist, web);
            }

            if (tempstr_ch.Length != 0)
            {
                result_title += " " + TranslateWord(tempstr_ch, comic, wordlist, namelist);
            }

            if (tempstr_add.Length != 0)
            {
                result_title += " + " + TranslateWord(tempstr_add, comic, wordlist, namelist);
            }
            return result_title;
        }

        public static void DivideAddOnString(string original, out string res, out string addon, out string volumn)
        {
            res = original;
            addon = string.Empty;
            volumn = string.Empty;
            string tempstr_front = string.Empty;
            string tempstr_rare = string.Empty;
            if (original.Contains(" + "))
            {
                string plus = Library.StringDivider(original, " + ").Last();
                if (plus.StartsWith("Zoku", StringComparison.OrdinalIgnoreCase) ||
                    plus.EndsWith("Omake", StringComparison.OrdinalIgnoreCase) ||
                    plus.EndsWith("Bonus", StringComparison.OrdinalIgnoreCase) ||
                    plus.EndsWith("Paper", StringComparison.OrdinalIgnoreCase) ||
                    plus.EndsWith("Bon", StringComparison.OrdinalIgnoreCase) ||
                    plus.EndsWith("Hon", StringComparison.OrdinalIgnoreCase) ||
                    plus.EndsWith("Poster", StringComparison.OrdinalIgnoreCase) ||
                    plus.EndsWith("Tokuten", StringComparison.OrdinalIgnoreCase) ||
                    plus.EndsWith("Matome", StringComparison.OrdinalIgnoreCase) ||
                    plus.EndsWith("Hen", StringComparison.OrdinalIgnoreCase) ||
                    plus.EndsWith("Ban", StringComparison.OrdinalIgnoreCase) ||
                    plus.Contains("Rakugaki", true) ||
                    plus.Contains("Pixiv", true) ||
                    plus.Contains("After", true) ||
                    plus.Contains("Calendar", true)
                    )
                {
                    int index = original.LastIndexOf(plus);
                    addon = original.Substring(index);
                    res = original.Substring(0, index - 3);
                }
            }

            if (res.Contains("Ch."))
            {
                int index = res.LastIndexOf("Ch.");
                if ('0' <= res[index + 3] && res[index + 3] <= '9')
                {
                    volumn = res.Substring(index);
                    res = res.Substring(0, index);
                }
            }
            else if (res.Contains("Vol."))
            {
                int index = res.LastIndexOf("Vol.");
                if ('0' <= res[index + 4] && res[index + 4] <= '9')
                {
                    volumn = res.Substring(index);
                    res = res.Substring(0, index);
                }
            }
            else if (res.Contains("Act."))
            {
                int index = res.LastIndexOf("Act.");
                if ('0' <= res[index + 4] && res[index + 4] <= '9')
                {
                    volumn = res.Substring(index);
                    res = res.Substring(0, index);
                }
            }
            res = res.Trim();
        }

        public static async Task<string> TranslateWord(string sentance, CtComic comic, CtWordList wordlist, CtNameList namelist, IWebConnection web)
        {

            if (web != null && Library.isJapanese(sentance))
            {
                return await web.TransWebBrowserInitAsync(sentance, "jp", "ko");
            }
            else
            {
                return TranslateWord(sentance, comic, wordlist, namelist);
            }
        }

        public static string TranslateWord(string sentance, CtComic comic, CtWordList wordlist, CtNameList namelist)
        {
            int realIndex, virtualindex = 0;
            int i, j;
            bool flag;
            bool spaceflag;
            CtWord ppreWord, preWord, emptyWord, newWord;
            CtWordList newWords = new CtWordList();
            string originalSentance, tempstr, tempstr2, tempword, temp, temp2, wordString, preWordString, nextWordString;
            List<string> tempList = new List<string>();
            List<string> newstrList = new List<string>();
            List<string> NameKr_str;
            List<string> space_str;
            List<string> typebck_str;
            List<string> typefwd_str;
            List<string> cat_str;
            List<string> cat_str1;
            List<string> cat_str2;
            List<string> lan_str;
            int tempInt, tempInt2;
            char tempChar;
            originalSentance = sentance;

            StringInfo stringInfo = new StringInfo(sentance, comic, wordlist, namelist);
            sentance = string.Empty;
            emptyWord = new CtWord(string.Empty, string.Empty);


            ppreWord = emptyWord;
            preWord = emptyWord;
            newWord = null;

            for (i = 0; i < stringInfo.wordInfolist.Count; i++)
            {
                realIndex = stringInfo.wordIndexList[i];
                newWord = stringInfo.wordInfolist[i];
                wordString = stringInfo.wordStringlist[realIndex];
                if (realIndex > 0)
                    preWordString = stringInfo.wordStringlist[realIndex - 1];
                else
                    preWordString = string.Empty;
                if (realIndex + 1 < stringInfo.wordStringlist.Count)
                {
                    nextWordString = stringInfo.wordStringlist[realIndex + 1];
                }
                else
                {
                    nextWordString = string.Empty;
                }

                if (newWord != emptyWord)
                {
                RESTARTLABEL:
                    CtWord nextWord;
                    if (i + 1 < stringInfo.wordInfolist.Count)
                    {
                        nextWord = stringInfo.wordInfolist[i + 1];
                    }
                    else
                    {
                        nextWord = emptyWord;
                    }
                    string NameEn = newWord.NameEn;
                    NameKr_str = Library.StringDivider(newWord.NameKr, "/");
                    space_str = Library.StringDivider(newWord.Space, "/");
                    typebck_str = Library.StringDivider(newWord.TypeBck, "/");
                    typefwd_str = Library.StringDivider(newWord.TypeFwd, "/");
                    cat_str = Library.StringDivider(newWord.Divider, "/");
                    lan_str = Library.StringDivider(newWord.Language, "/");

                    if (NameEn != string.Empty && cat_str.Count != 0 && (NameKr_str.Count != cat_str.Count + 1 || space_str.Count != cat_str.Count + 1 || typebck_str.Count != cat_str.Count + 1 || typefwd_str.Count != cat_str.Count + 1))
                    {
                        return $"{NameEn}에 오류가 있습니다.";
                    }
                    bool inverse;
                    int count = 0;
                    if (newWord.Divider.Length != 0)
                    {
                        for (j = 0; j < cat_str.Count; j++)
                        {
                            cat_str1 = Library.StringDivider(cat_str[j], "|");
                            foreach (string str2 in cat_str1)
                            {
                                count = 0;
                                cat_str2 = Library.StringDivider(str2, ",");
                                foreach (string str in cat_str2)
                                {
                                    if (str.Length == 0)
                                        continue;
                                    if (str[0] == '!')
                                    {
                                        tempstr = str.Substring(1);
                                        inverse = true;
                                    }
                                    else
                                    {
                                        tempstr = str;
                                        inverse = false;
                                    }
                                    if (preWord.TypeBck.Contains(tempstr))
                                    {
                                        if (inverse)
                                        {
                                            goto INVERSELABEL;
                                        }
                                        else
                                        {
                                            count++;
                                            continue;
                                        }
                                    }
                                    else if (tempstr == "문장시작" && (preWord.TypeBck.Contains("종결") || realIndex == 0))
                                    {
                                        if (inverse)
                                        {
                                            goto INVERSELABEL;
                                        }
                                        else
                                        {
                                            count++;
                                            continue;
                                        }
                                    }
                                    else if (tempstr == "절대외국" && (preWord.Language.Contains("외국") || nextWord.Language.Contains("외국")))
                                    {
                                        if (inverse)
                                        {
                                            goto INVERSELABEL;
                                        }
                                        else
                                        {
                                            count++;
                                            continue;
                                        }
                                    }
                                    else if (tempstr == "외국" && ( preWord.Language.Contains("외국") || nextWord.Language.Contains("외국") ) )
                                    {
                                        if (inverse)
                                        {
                                            goto INVERSELABEL;
                                        }
                                        else
                                        {
                                            count++;
                                            continue;
                                        }
                                    }
                                    else if (tempstr == "문장끝" && (!nextWordString.Contains("...") || (!newWord.TypeFwd.Contains("동사") && !newWord.TypeFwd.Contains("형용사"))) &&
                                            (
                                            (wordString.Contains(',') && !newWord.TypeFwd.Contains("동사") && (!newWord.TypeBck.Contains("조사") || newWord.TypeBck.Contains("종결"))) ||
                                            wordString.Contains('.') ||
                                            wordString.Contains('?') ||
                                            wordString.Contains('!') ||
                                            realIndex == stringInfo.LastCount ||
                                            ((nextWord.TypeFwd.Contains("종결어") || nextWord.TypeFwd.Contains("숫자")) && realIndex == stringInfo.LastCount - 1)
                                            )
                                            )
                                    {
                                        if (inverse)
                                        {
                                            goto INVERSELABEL;
                                        }
                                        else
                                        {
                                            count++;
                                            continue;
                                        }
                                    }
                                    else if (tempstr == "보조동사" && (preWord.TypeBck.Contains("행위") || preWord.TypeBck.Contains("상태")))
                                    {
                                        if (inverse)
                                        {
                                            goto INVERSELABEL;
                                        }
                                        else
                                        {
                                            count++;
                                            continue;
                                        }
                                    }
                                    else if (tempstr == "숫자" && (Library.isNumber(wordString[0]) || preWord.TypeBck.Contains("숫자") || (preWordString.Length != 0 && (Library.isNumber(preWordString.Last()) || Library.IsMark(preWordString.Last())))))
                                    {
                                        if (inverse)
                                        {
                                            goto INVERSELABEL;
                                        }
                                        else
                                        {
                                            count++;
                                            continue;
                                        }
                                    }
                                    else if (tempstr == "의문" && wordString.Contains('?'))
                                    {
                                        if (inverse)
                                        {
                                            goto INVERSELABEL;
                                        }
                                        else
                                        {
                                            count++;
                                            continue;
                                        }
                                    }
                                    else if (preWord.TypeBck.Contains(tempstr) && tempstr == "조사" && i != 0)
                                    {
                                        if (inverse)
                                        {
                                            goto INVERSELABEL;
                                        }
                                        else
                                        {
                                            count++;
                                            continue;
                                        }
                                    }

                                    if (tempstr[0] == '&' && tempstr[1] == '@' && string.Compare(tempstr.Substring(2), nextWord.NameEn, true) == 0)
                                    {
                                        if (inverse)
                                        {
                                            goto INVERSELABEL;
                                        }
                                        else
                                        {
                                            count++;
                                            continue;
                                        }
                                    }

                                    if (tempstr[0] == '&' && string.Compare(tempstr.Substring(1), preWord.NameEn, true) == 0)
                                    {
                                        if (inverse)
                                        {
                                            goto INVERSELABEL;
                                        }
                                        else
                                        {
                                            count++;
                                            continue;
                                        }
                                    }

                                    if (tempstr[0] == '@' && nextWord.TypeFwd.Contains(tempstr.Substring(1)))
                                    {
                                        if (inverse)
                                        {
                                            goto INVERSELABEL;
                                        }
                                        else
                                        {
                                            count++;
                                            continue;
                                        }
                                    }

                                    if (tempstr[0] == '#')
                                    {
                                        flag = false;
                                        for (int k = 0; k < stringInfo.wordInfolist.Count; k++)
                                        {
                                            if (k != i && string.Compare(stringInfo.wordInfolist[k].NameEn, tempstr.Substring(1), true) == 0)
                                            {
                                                flag = true;
                                                break;
                                            }
                                        }
                                        if (flag)
                                        {
                                            count++;
                                            continue;
                                        }
                                    }
                                    //ex) @-으로,&no,%-의
                                    if (tempstr[0] == '%' && tempstr[1] == '-' && count == cat_str2.Count - 1)
                                    {
                                        temp = tempstr.Substring(2);
                                        if (preWord.NameKr.Length - temp.Length >= 0 && preWord.NameKr.Substring(preWord.NameKr.Length - temp.Length) == temp)
                                        {
                                            if (inverse)
                                            {
                                                goto INVERSELABEL;
                                            }
                                            else
                                            {
                                                temp2 = newstrList.Last().Substring(0, newstrList.Last().Length - temp.Length);
                                                newstrList.RemoveAt(newstrList.Count - 1);
                                                newstrList.Add(temp2);
                                                count++;
                                                continue;
                                            }
                                        }
                                    }
                                    else if (tempstr[0] == '%')
                                    {
                                        temp = tempstr.Substring(1);
                                        if (preWord.NameKr.Length - temp.Length >= 0 && preWord.NameKr.Substring(preWord.NameKr.Length - temp.Length) == temp)
                                        {
                                            if (inverse)
                                            {
                                                goto INVERSELABEL;
                                            }
                                            else
                                            {
                                                count++;
                                                continue;
                                            }
                                        }
                                    }
                                    if (Library.isNumber(tempstr[0]) && i == Int32.Parse(tempstr))
                                    {
                                        if (inverse)
                                        {
                                            goto INVERSELABEL;
                                        }
                                        else
                                        {
                                            count++;
                                            continue;
                                        }
                                    }

                                    if (tempstr[0] == '$' && (comic.ExistsTag(tempstr.Substring(1), CtModelType.SERIES, true) || comic.ExistsTag(tempstr.Substring(1), CtModelType.CHARACTER, true)))
                                    {
                                        if (inverse)
                                        {
                                            goto INVERSELABEL;
                                        }
                                        else
                                        {
                                            count++;
                                            continue;
                                        }
                                    }


                                    if (count == cat_str2.Count - 1 && tempstr.Contains("명사후면수식") && preWord.TypeBck.Contains("명사"))
                                    {
                                        if (inverse)
                                        {
                                            goto INVERSELABEL;
                                        }
                                        else
                                        {
                                            temp = Library.extractionString(tempstr, "[", "]");
                                            if (newstrList.Last()[0] == ' ')
                                            {
                                                temp = " " + temp + newstrList.Last();
                                            }
                                            else
                                            {
                                                temp = " " + temp + " " + newstrList.Last();
                                            }
                                            newstrList.RemoveAt(newstrList.Count - 1);
                                            newstrList.Add(temp);
                                            count++;
                                            continue;
                                        }
                                    }

                                    if (tempstr == "동사후면수식" && (preWord.TypeFwd.Contains("동사") || preWord.TypeFwd.Contains("형용사")))
                                    {
                                        CtWord tw;
                                        if (preWord.TypeBck.Contains("동명사") && (temp = Library.extractionString(preWord.TypeBck, "{", "}")).Length != 0)
                                            tw = wordlist.GetWord(temp);
                                        else
                                        {
                                            tw = preWord;
                                            tw.Space = "false";
                                        }

                                        if (tw != null)
                                        {
                                            if (inverse)
                                            {
                                                goto INVERSELABEL;
                                            }
                                            else
                                            {
                                                newWord = tw;
                                                wordString = newWord.NameEn;
                                                newstrList.RemoveAt(newstrList.Count - 1);
                                                if (space_str[j] == "true")
                                                    temp = NameKr_str[j];
                                                else
                                                    temp = " " + NameKr_str[j];
                                                newstrList.Add(temp);
                                                preWord = ppreWord;
                                                goto RESTARTLABEL;
                                            }
                                        }
                                    }
                                INVERSELABEL:;
                                    if (inverse)
                                    {
                                    }
                                }
                                if (count == cat_str2.Count)
                                {
                                    goto FINISHI;
                                }
                            }
                        }
                    FINISHI:
                        newWord = new CtWord(NameEn, NameKr_str[j], space_str.Count == 0 ? "false" : space_str[j], typefwd_str[j], typebck_str[j], string.Empty, (lan_str.Count > 1) ? lan_str[j] : newWord.Language);
                    }
                    if (newWord.Language.Contains("일본"))
                        stringInfo.IncJapanese();
                    else if (newWord.Language.Contains("외국") || Library.isEnglish(newWord.NameEn))
                        stringInfo.IncEnglish();

                }

                //띄어쓰기와 앞종성 구별 처리
                spaceflag = false;
                if (newWord == emptyWord || newWord.NameEn.Length == 0 || (i == 0 && newWord.TypeFwd.Contains("접미사") && wordString.IndexOf(newWord.NameEn) == 0))
                {
                    tempword = wordString;
                }
                else
                {
                    if (newWord.NameKr.Contains('|'))
                    {
                        if(newstrList.Count > 0)
                        {
                            tempInt = 0;
                            tempstr2 = string.Empty;
                            while (tempstr2.Length == 0 && tempInt <= newstrList.Count)
                            {
                                tempInt++;
                                tempstr2 = newstrList[newstrList.Count - tempInt].Trim(' ', '"', ')', ']', '>', '}', ';', '?', '!', '~', '-', '_', '.', ',');
                            }

                            if (tempstr2.Length != 0)
                                tempChar = tempstr2.Last();
                            else
                                tempChar = ' ';
                            tempList = Library.StringDivider(newWord.NameKr, "|");
                            if (i == 0 || ((int)tempChar - 44032) % 28 == 0
                                || tempChar == 'a' || tempChar == 'A'
                                || tempChar == 'e' || tempChar == 'E'
                                || tempChar == 'h' || tempChar == 'H'
                                || tempChar == 'i' || tempChar == 'I'
                                || tempChar == 'o' || tempChar == 'O'
                                || tempChar == 'q' || tempChar == 'Q'
                                || tempChar == 'r' || tempChar == 'R'
                                || tempChar == 's' || tempChar == 'S'
                                || tempChar == 'u' || tempChar == 'U'
                                || tempChar == 'v' || tempChar == 'V'
                                || tempChar == 'w' || tempChar == 'W'
                                || tempChar == 'x' || tempChar == 'X'
                                || tempChar == 'y' || tempChar == 'Y'
                                || tempChar == 'z' || tempChar == 'Z'
                                || tempChar == 'J' || tempChar == 'K'
                                || tempChar == '2' || tempChar == '4'
                                || tempChar == '5' || tempChar == '4'
                                || tempChar == '9'
                                || (tempstr2.Length == 1 && Library.isEnglish(tempChar) && tempChar != 'L' && tempChar != 'M' && tempChar != 'N')
                                || ((int)tempChar % 28 == 44040 % 28 && tempList[0].First() == 51004)
                                )
                                tempstr = tempList[1];
                            else
                                tempstr = tempList[0];
                            tempword = wordString.ToLower().Replace(newWord.NameEn.ToLower(), tempstr);
                        }
                        else
                        {
                            tempword = wordString;
                        }
                    }
                    else
                    {
                        tempstr = newWord.NameKr;
                        tempword = wordString.ToLower().Replace(newWord.NameEn.ToLower(), tempstr);
                    }
                    if (newWord.Space == "true")
                        spaceflag = true;
                    if (wordString.Length != 0 && ('0' <= wordString.First() && wordString.First() <= '9' || !wordString.StartsWith(newWord.NameEn, StringComparison.OrdinalIgnoreCase)))
                    {
                        spaceflag = false;
                    }
                }
                if (i == 0 || spaceflag)
                    newstrList.Add(tempword);
                else
                    newstrList.Add(' ' + tempword);

                if ((!newWord.TypeBck.Contains("조사") && wordString.Last() == ',') || wordString.Last() == ('.') || wordString.Last() == ('?') || wordString.Last() == ('!'))
                {
                    newWord = new CtWord(newWord.values);
                    newWord.TypeBck += "/종결";
                }

                preWord = newWord;
                ppreWord = preWord;
                newWords.Add(newWord);
            }

            //언어에 따라 최종적으로 번역할 껀지 처리
            flag = true;
            for (i = 0; i < stringInfo.originStringlist.Count; i++)
            {
                virtualindex = -1;
                wordString = stringInfo.originStringlist[i];
                for (int k = 0; k < stringInfo.wordIndexList.Count; k++)
                {
                    if (stringInfo.wordIndexList[k] == i)
                    {
                        virtualindex = k;
                        break;
                    }
                }

                if (virtualindex == -1)
                {
                    if (i == 0)
                        sentance += wordString;
                    else
                        sentance += ' ' + wordString;
                    continue;
                }

                CtWord nextWord;
                if (virtualindex > 0)
                {
                    preWord = newWord;
                }
                else
                {
                    preWord = emptyWord;
                    emptyWord.Language = "일본";
                }
                if (virtualindex + 1 < stringInfo.wordInfolist.Count)
                {
                    nextWord = stringInfo.wordInfolist[virtualindex + 1];
                }
                else
                {
                    nextWord = emptyWord;
                }
                newWord = newWords[virtualindex] as CtWord;
                if (
                    (newWord.Language != "일본")
                    && (stringInfo.Jcounter != stringInfo.wordInfolist.Count)
                    && (stringInfo.TransCounter != stringInfo.Ecounter)
                    && (stringInfo.Jcounter == 0 || flag == false)
                    && (nextWord.Language != "일본" || i + 1 == stringInfo.originStringlist.Count)
                    || (stringInfo.Jcounter == 0 && stringInfo.TransCounter == stringInfo.Ecounter && stringInfo.Ecounter < stringInfo.wordInfolist.Count / 2)
                    || (newWord.Language != "일본" && stringInfo.originStringlist.Count - 1 > i && string.Compare(stringInfo.originStringlist[i + 1], "The", true) == 0) || (i > 0 && string.Compare(stringInfo.originStringlist[i - 1], "The", true) == 0)
                    || (newWord.Language != "일본" && stringInfo.originStringlist.Count - 1 > i && string.Compare(stringInfo.originStringlist[i + 1], "of", true) == 0) || (i > 0 && string.Compare(stringInfo.originStringlist[i - 1], "of", true) == 0)
                    || (newWord.Language != "일본" && stringInfo.originStringlist.Count - 1 > i && string.Compare(stringInfo.originStringlist[i + 1], "for", true) == 0) || (i > 0 && string.Compare(stringInfo.originStringlist[i - 1], "for", true) == 0)
                    || (newWord.Language != "일본" && stringInfo.originStringlist.Count - 1 > i && string.Compare(stringInfo.originStringlist[i + 1], "and", true) == 0) || (i > 0 && string.Compare(stringInfo.originStringlist[i - 1], "and", true) == 0)
                    || (newWord.Language != "일본" && preWord.Language != "일본" && preWord.NameEn.Length == 0 && !newWord.TypeBck.Contains("사람"))
                    || (newWord.Language.Contains("외국") && Library.isUpperCase(wordString))
                )
                {
                    if (i == 0)
                        sentance += wordString;
                    else
                        sentance += ' ' + wordString;
                    flag = false;
                }
                else if (newWord.Language == "일본")
                {
                    sentance += newstrList[virtualindex];
                    flag = true;
                }
                else
                {
                    sentance += newstrList[virtualindex];
                    if (!Library.IsMark(newstrList[virtualindex].Last()))
                    {
                        if (nextWord.NameEn.Length == 0)
                            flag = false;
                        else
                            flag = true;
                    }
                }
            }

            //노타이틀에서 처리
            if (string.Compare(sentance, "No Title") == 0)
                sentance = "제목 없음";

            return sentance.Trim();
        }




    }

}
