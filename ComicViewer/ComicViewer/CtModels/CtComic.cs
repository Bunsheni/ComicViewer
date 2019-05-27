using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace ComicViewer.CtModels
{
    public enum ComicType { UNKNOWN = 0, COMPLETE = 1, WEEKLY = 2, BIWEEKLY = 3, MONTHLY = 4, SINGLESTORY = 5, SINGLELINE = 6, OCCASIONAL = 7, MANGA = 8, DOUJINSHI = 9, ARTISTCG = 10 }
    public enum ComicType2 { UNKNOWN = 0, NORMAL = 1, ADULT = 2 }
    public partial class CtComic : CtModel
    {
        public static readonly string[] Languages = { "한국어", "English", "日本語" };
        public static readonly string[] TypesKr = { "미분류", "완결", "주간", "격주", "월간", "격월", "단편", "단행본", "비정기", "망가", "동인지", "작화 CG" };
        public static readonly string[] TypesEn = { "Unknown", "Complete", "Weekly", "Biweekly", "Monthly", "Bimonthly", "Single-story", "Single-line", "Occasional", "Manga", "Doujinshi", "Artist CG" };
        public static readonly string[] Types2Kr = { "미분류", "일반", "성인" };
        public static readonly string[] Types2En = { "Unknown", "Normal", "Adult" };

        public new enum Column {
            ID,
            LANGUAGE, PAGE, TYPE1, TYPE2,
            UPLOADEDDATE, MODIFIEDDATE,
            TITLEEN, TITLEKR,
            ARTISTEN, ARTISTKR,
            GROUPEN, GROUPKR,
            SERIESEN, SERIESKR,
            CHARACTEREN, CHARACTERKR,
            TAGEN, TAGKR,
            COVERURL, IMAGEURL, PAGEURL,
            SUBWORKSID, PARENTID,
            ISSUBWORK, ISTRANSLATED, ISFAVORITE, ISHIDDEN
        }

        public new static readonly string[] PropertiesName = {
            "ID",
            "Language", "Page", "Type1", "Type2",
            "UploadedDate", "ModifiedDate",
            "TitleEn", "TitleKr",
            "ArtistEn", "ArtistKr",
            "GroupEn", "GroupKr",
            "SeriesEn", "SeriesKr",
            "CharacterEn", "CharacterKr",
            "TagEn", "TagKr",
            "CoverUrl", "ImageUrl", "PageUrl",
            "SubworksID", "ParentID",
            "IsSubwork", "IsTranslated", "IsFavorite", "IsHidden"
        };

        public static readonly string[] PropertiesNameKr = 
            {
            "ID",
            "언어", "페이지", "유형1", "유형2",
            "업로드날짜", "수정한날짜",
            "제목(영)","제목(한)",
            "작가(영)", "작가(한)",
            "그룹(영)", "그룹(한)",
            "시리즈(영)", "시리즈(한",
            "인물(영)", "인물(한)",
            "태그(영)", "태그(한)",
            "표지링크", "이미지링크", "페이지링크",
            "하위작품ID", "상위작품ID",
            "하위작품", "번역고정", "즐겨찾기", "숨김작품"
        };

        public new static readonly string[] SortsKr = { "업로드 날짜", "수정한 날짜", "제목", "작가", "그룹", "시리즈", "페이지" };
        public new static readonly string[] SortsEn = { "Uploaded Date", "Modified Date", "Title", "Artist", "Group", "Series", "Page" };
        public new static string[] _colunmText { get { return ProgramLanguage == 0 ? SortsKr : SortsEn; } }

        public new static readonly Type[] types = 
            {
            typeof(string),
            typeof(string), typeof(int), typeof(ComicType), typeof(ComicType2),
            typeof(DateTime), typeof(DateTime),
            typeof(string), typeof(string),
            typeof(string), typeof(string),
            typeof(string), typeof(string),
            typeof(string), typeof(string),
            typeof(string), typeof(string),
            typeof(string), typeof(string),
            typeof(string), typeof(string), typeof(string),
            typeof(string), typeof(string),
            typeof(bool), typeof(bool), typeof(bool), typeof(bool)
        };
        public static int SortingColumn;

        public static bool HighImageResolution;

        [Ignore]
        public override Type[] Types { get { return types; } }

        public static void SetSortingColumn(string text)
        {
            SortingColumn = _colunmText.ToList().FindIndex(item => string.Compare(item, text) == 0);
        }
        
        [Ignore]
        public override object SortingPropertie
        {
            get
            {
                switch (SortingColumn)
                {
                    case 0:
                        return UploadedDate;
                    case 1:
                        return ModifiedDate;
                    case 2:
                        return Title;
                    case 3:
                        return Artist;
                    case 4:
                        return Group;
                    case 5:
                        return Series;
                    case 6:
                        return Page;
                    default:
                        return UploadedDate;
                }
            }
        }
        [Ignore]
        public override CtModelType CtType { get { return CtModelType.COMIC; } }
        [Ignore]
        public override string Name { get { return Title; } set { } }
        [Ignore]
        public override string Id { get { return Workid; } }
        [Ignore]
        public override string Detail
        {
            get
            {
                return Tag;
            }
        }
        [Ignore]
        public override string NameEn { get { return TitleEn; } set { } }
        [Ignore]
        public override string NameKr { get { return TitleKr; } set { } }
        [Ignore]
        public override string Synonym { get { return string.Empty; } set { } }
        [Ignore]
        public string Title
        {
            get
            {
                return ProgramLanguage == 0 ? TitleKr : TitleEn;
            }
            set
            {
                TitleEn = TitleKr = value;
                OnPropertyChanged();
            }
        }
        [Ignore]
        public string Artist
        {
            get
            {
                return ProgramLanguage == 0 ? ArtistKr : ArtistEn;
            }
            set
            {
                ArtistEn = ArtistKr = value;
                OnPropertyChanged();
            }
        }
        [Ignore]
        public string Group
        {
            get
            {
                return ProgramLanguage == 0 ? GroupKr : GroupEn;
            }
            set
            {
                GroupEn = GroupKr = value;
                OnPropertyChanged();
            }
        }
        [Ignore]
        public string Series
        {
            get
            {
                return ProgramLanguage == 0 ? SeriesKr : SeriesEn;
            }
            set
            {
                SeriesEn = SeriesKr = value;
                OnPropertyChanged();
            }
        }
        [Ignore]
        public string Character
        {
            get
            {
                return ProgramLanguage == 0 ? CharacterKr : CharacterEn;
            }
            set
            {
                CharacterEn = CharacterKr = value;
                OnPropertyChanged();
            }
        }
        [Ignore]
        public string Tag
        {
            get
            {
                return ProgramLanguage == 0 ? TagKr : TagEn;
            }
            set
            {
                TagEn = TagKr = value;
                OnPropertyChanged();
            }
        }
        [Ignore]
        public string TypeEn
        {
            get { return TypesEn[(int)Type1]; }
        }
        [Ignore]
        public string TypeKr
        {
            get { return TypesKr[(int)Type1]; }
        }
        [Ignore]
        public string TypeStr
        {
            get
            {
                return ProgramLanguage == 0 ? TypeKr : TypeEn;
            }
            set
            {
                for (int i = 0; i < TypesEn.Count(); i++)
                {
                    if (string.Compare(TypesEn[i], value, true) == 0)
                        Type1 = (ComicType)i;
                }
            }
        }
        [Ignore]
        public string Type2En
        {
            get { return Types2En[(int)Type2]; }
        }
        [Ignore]
        public string Type2Kr
        {
            get { return Types2Kr[(int)Type2]; }
        }
        [Ignore]
        public string Type2Str
        {
            get
            {
                return ProgramLanguage == 0 ? Type2Kr : Type2En;
            }
            set
            {
                for (int i = 0; i < Types2En.Count(); i++)
                {
                    if (string.Compare(Types2En[i], value, true) == 0)
                        Type2 = (ComicType2)i;
                }
            }
        }
        [Ignore]
        public string PageStr
        {
            get
            {
                return Page.ToString();
            }
            set
            {
                int page;
                if (int.TryParse(value, out page))
                {
                    Page = page;
                    OnPropertyChanged();
                }
            }
        }
        [Ignore]
        public string PageStrLabel
        {
            get
            {
                return PageStr + (ProgramLanguage == 0 ? "페이지" : "page");
            }
        }
        [Ignore]
        public string UploadedDateStr
        {
            get
            {
                return UploadedDate == null ? "" : UploadedDate.ToString("yyyy-MM-dd tt h:mm:ss");
            }
            set
            {
                DateTime date;
                if (DateTime.TryParse(value, out date))
                    UploadedDate = date;
                OnPropertyChanged();
            }
        }
        [Ignore]
        public string ModifiedDateStr
        {
            get
            {
                return ModifiedDate == null ? "" : ModifiedDate.ToString("yyyy-MM-dd tt h:mm:ss");
            }
            set
            {
                DateTime date;
                if (DateTime.TryParse(value, out date))
                    ModifiedDate = date;
                OnPropertyChanged();
            }
        }
        [Ignore]
        public string ArtistAndGroup
        {
            get
            {
                if (Artist.Length == 0 && Group.Length == 0)
                {
                    return ProgramLanguage == 0 ? "작가 미상" : "Unkown Artist";
                }
                else
                {

                    return Artist.Length == 0 ? Group.Replace(" / ", ", ") : Artist.Replace(" / ", ", ") + (Group.Length == 0 ? string.Empty : " | " + Group.Replace(" / ", ", "));
                }
            }
        }

        public bool CoverVisible
        {
            get
            {
                return !((App)Xamarin.Forms.Application.Current).ImageHiddingEnabled;
            }
        }

        //RealProperties
        [PrimaryKey]
        public string Workid
        {
            get
            {
                return values[(int)Column.ID].ToString();
            }
            set
            {
                values[(int)Column.ID] = value;
                OnPropertyChanged();
            }
        }
        public string TitleEn
        {
            get
            {
                return values[(int)Column.TITLEEN].ToString();
            }
            set
            {
                values[(int)Column.TITLEEN] = value;
                OnPropertyChanged();
            }
        }
        public string TitleKr
        {
            get
            {
                return values[(int)Column.TITLEKR].ToString();
            }
            set
            {
                values[(int)Column.TITLEKR] = value;
                OnPropertyChanged();
            }
        }
        public string ArtistEn
        {
            get
            {
                return values[(int)Column.ARTISTEN].ToString();
            }
            set
            {
                values[(int)Column.ARTISTEN] = value;
                OnPropertyChanged();
            }
        }
        public string ArtistKr
        {
            get
            {
                return values[(int)Column.ARTISTKR].ToString();
            }
            set
            {
                values[(int)Column.ARTISTKR] = value;
                OnPropertyChanged();
            }
        }
        public string GroupEn
        {
            get
            {
                return values[(int)Column.GROUPEN].ToString();
            }
            set
            {
                values[(int)Column.GROUPEN] = value;
                OnPropertyChanged();
            }
        }
        public string GroupKr
        {
            get
            {
                return values[(int)Column.GROUPKR].ToString();
            }
            set
            {
                values[(int)Column.GROUPKR] = value;
                OnPropertyChanged();
            }
        }
        public string SeriesEn
        {
            get
            {
                return values[(int)Column.SERIESEN].ToString();
            }
            set
            {
                values[(int)Column.SERIESEN] = value;
                OnPropertyChanged();
            }
        }
        public string SeriesKr
        {
            get
            {
                return values[(int)Column.SERIESKR].ToString();
            }
            set
            {
                values[(int)Column.SERIESKR] = value;
                OnPropertyChanged();
            }
        }
        public string CharacterEn
        {
            get
            {
                return values[(int)Column.CHARACTEREN].ToString();
            }
            set
            {
                values[(int)Column.CHARACTEREN] = value;
                OnPropertyChanged();
            }
        }
        public string CharacterKr
        {
            get
            {
                return values[(int)Column.CHARACTERKR].ToString();
            }
            set
            {
                values[(int)Column.CHARACTERKR] = value;
                OnPropertyChanged();
            }
        }
        public string TagEn
        {
            get
            {
                return values[(int)Column.TAGEN].ToString();
            }
            set
            {
                values[(int)Column.TAGEN] = value;
                OnPropertyChanged();
            }
        }
        public string TagKr
        {
            get
            {
                return values[(int)Column.TAGKR].ToString();
            }
            set
            {
                values[(int)Column.TAGKR] = value;
                OnPropertyChanged();
            }
        }
        public ComicType Type1
        {
            get
            {
                return (ComicType)values[(int)Column.TYPE1];
            }
            set
            {
                values[(int)Column.TYPE1] = value;
                OnPropertyChanged();
            }
        }
        public ComicType2 Type2
        {
            get
            {
                return (ComicType2)values[(int)Column.TYPE2];
            }
            set
            {
                values[(int)Column.TYPE2] = value;
                OnPropertyChanged();
            }
        }
        public string Language
        {
            get
            {
                return values[(int)Column.LANGUAGE].ToString();
            }
            set
            {
                values[(int)Column.LANGUAGE] = value;
                OnPropertyChanged();
            }
        }
        public string CoverUrl
        {
            get
            {
                if(HighImageResolution)
                    return values[(int)Column.COVERURL].ToString();
                else
                    return values[(int)Column.COVERURL].ToString().Replace("bigtn", "smalltn");
            }
            set
            {
                values[(int)Column.COVERURL] = value;
                OnPropertyChanged();
            }
        }

        public string ImageUrl
        {
            get
            {
                return values[(int)Column.IMAGEURL].ToString();
            }
            set
            {
                values[(int)Column.IMAGEURL] = value;
                OnPropertyChanged();
            }
        }
        public string PageUrl
        {
            get
            {
                return values[(int)Column.PAGEURL].ToString();
            }
            set
            {
                values[(int)Column.PAGEURL] = value;
                OnPropertyChanged();
            }
        }
        public int Page
        {
            get
            {
                return (int)values[(int)Column.PAGE];
            }
            set
            {
                values[(int)Column.PAGE] = value;
                OnPropertyChanged();
            }
        }
        public DateTime UploadedDate
        {
            get
            {
                return (DateTime)values[(int)Column.UPLOADEDDATE];
            }
            set
            {
                values[(int)Column.UPLOADEDDATE] = value;
                OnPropertyChanged();
            }
        }
        public DateTime ModifiedDate
        {
            get
            {
                return (DateTime)values[(int)Column.MODIFIEDDATE];
            }
            set
            {
                values[(int)Column.MODIFIEDDATE] = value;
                OnPropertyChanged();
            }
        }
        public string SubworksId
        {
            get
            {
                return values[(int)Column.SUBWORKSID].ToString();
            }
            set
            {
                values[(int)Column.SUBWORKSID] = value;
                OnPropertyChanged();
            }
        }
        public string ParentId
        {
            get
            {
                return values[(int)Column.PARENTID].ToString();
            }
            set
            {
                values[(int)Column.PARENTID] = value;
                OnPropertyChanged();
            }
        }
        public bool IsFavorite
        {
            get
            {
                return (bool)values[(int)Column.ISFAVORITE];
            }
            set
            {
                values[(int)Column.ISFAVORITE] = value;
                OnPropertyChanged();
            }
        }
        public bool IsSubWork
        {
            get
            {
                return (bool)values[(int)Column.ISSUBWORK];
            }
            set
            {
                values[(int)Column.ISSUBWORK] = value;
                OnPropertyChanged();
            }
        }
        public bool IsHidden
        {
            get
            {
                return (bool)values[(int)Column.ISHIDDEN];
            }
            set
            {
                values[(int)Column.ISHIDDEN] = value;
                OnPropertyChanged();
            }
        }
        public bool IsTranslated
        {
            get
            {
                return (bool)values[(int)Column.ISTRANSLATED];
            }
            set
            {
                values[(int)Column.ISTRANSLATED] = value;
                OnPropertyChanged();
            }
        }

        public CtComic() : this(string.Empty) { }
        public CtComic(string workid) : base()
        {
            Workid = workid;
            CoverUrl = string.Empty;
            ImageUrl = string.Empty;
            PageUrl = string.Empty;
            TitleEn = string.Empty;
            TitleKr = string.Empty;
            ArtistEn = string.Empty;
            ArtistKr = string.Empty;
            GroupEn = string.Empty;
            GroupKr = string.Empty;
            SeriesEn = string.Empty;
            SeriesKr = string.Empty;
            CharacterEn = string.Empty;
            CharacterKr = string.Empty;
            TagEn = string.Empty;
            TagKr = string.Empty;
            Language = string.Empty;
            Type1 = ComicType.UNKNOWN;
            Type2 = ComicType2.UNKNOWN;
            SubworksId = string.Empty;
            ParentId = string.Empty;
            IsFavorite = false;
            IsHidden = false;
            IsSubWork = false;
            IsTranslated = false;
        }

        public override CtModel Clone()
        {
            CtModel model = new CtComic();
            for (int i = 0; i < model.values.Length; i++)
            {
                model.values[i] = this.values[i];
            }
            return model;
        }

        public override List<string> GetTags()
        {
            List<string> res = new List<string>();
            res.Add(TitleEn);
            res.Add(TitleKr);
            res.AddRange(Library.StringDivider(ArtistEn, " / "));
            res.AddRange(Library.StringDivider(ArtistKr, " / "));
            res.AddRange(Library.StringDivider(GroupEn, " / "));
            res.AddRange(Library.StringDivider(GroupKr, " / "));
            res.AddRange(Library.StringDivider(SeriesEn, " / "));
            res.AddRange(Library.StringDivider(SeriesKr, " / "));
            res.AddRange(Library.StringDivider(CharacterEn, " / "));
            res.AddRange(Library.StringDivider(CharacterKr, " / "));
            res.AddRange(Library.StringDivider(TagEn, " / "));
            res.AddRange(Library.StringDivider(TagKr, " / "));
            return res;
        }
    }
}
