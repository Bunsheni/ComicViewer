using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComicViewer.CtModels
{
    public class CtWord : CtModel
    {
        public enum CtWordLangType { JAPANESE = 0, ENGLISH = 1, ABENGLISH = 2, TITLE = 3 }

        public new enum Column { NAMEEN, NAMEKR, SPACE, TYPEFWD, TYPEBCK, DIVIDER, LANGUAGE }
        public new static readonly string[] PropertiesName = { "NameEn", "NameKr", "Space", "Foward Type", "Backward Type", "Divider", "Language" };
        public new static readonly string[] SortsKr = { "이름(영)", "이름(한)", "공백", "전방유형", "후방유형", "분류기", "언어" };
        public new static readonly string[] SortsEn = { "NameEn", "NameKr", "Space", "Foward Type", "Backward Type", "Divider", "Language" };
        public new static string[] _colunmText { get { return ProgramLanguage == 0 ? SortsKr : SortsEn; } }
        public new static readonly Type[] types = { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) };
        public static int SortingColumn;

        [Ignore]
        public override string[] ColunmText { get { return _colunmText; } }
        [Ignore]
        public override Type[] Types { get { return types; } }
        [Ignore]
        public override object SortingPropertie
        {
            get
            {
                switch (SortingColumn)
                {
                    case 0:
                        return NameEn;
                    case 1:
                        return NameKr;
                    case 2:
                        return TypeFwd;
                    case 3:
                        return TypeBck;
                    case 4:
                        return Divider;
                    case 5:
                        return Language;
                    case 7:
                        return Space;
                    default:
                        return NameEn;
                }
            }
        }

        [Ignore]
        public override CtModelType CtType { get { return CtModelType.WORD; } }
        [Ignore]
        public override string Name
        {
            get
            {
                return NameEn;
            }
            set
            {
                NameEn = value;
                OnPropertyChanged();
            }
        }
        [Ignore]
        public override string Id { get { return NameEn; } }
        [Ignore]
        public override string Detail
        {
            get
            {
                return NameKr;
            }
        }
        [Ignore]
        public override string Synonym
        {
            get
            {
                return string.Empty;
            }
            set
            {
            }
        }


        [PrimaryKey]
        public override string NameEn
        {
            get
            {
                return values[(int)Column.NAMEEN].ToString();
            }
            set
            {
                values[(int)Column.NAMEEN] = value;
                OnPropertyChanged();
            }
        }
        public override string NameKr
        {
            get
            {
                return values[(int)Column.NAMEKR].ToString();
            }
            set
            {
                values[(int)Column.NAMEKR] = value;
                OnPropertyChanged();
            }
        }
        public string TypeFwd
        {
            get
            {
                return values[(int)Column.TYPEFWD].ToString();
            }
            set
            {
                values[(int)Column.TYPEFWD] = value;
                OnPropertyChanged();
            }
        }
        public string TypeBck
        {
            get
            {
                return values[(int)Column.TYPEBCK].ToString();
            }
            set
            {
                values[(int)Column.TYPEBCK] = value;
                OnPropertyChanged();
            }
        }
        public string Divider
        {
            get
            {
                return values[(int)Column.DIVIDER].ToString();
            }
            set
            {
                values[(int)Column.DIVIDER] = value;
                OnPropertyChanged();
            }
        }
        public string Language
        {
            get
            {
                if (values[(int)Column.LANGUAGE] == null)
                    return string.Empty;
                else
                    return values[(int)Column.LANGUAGE].ToString();
            }
            set
            {
                values[(int)Column.LANGUAGE] = value;
                OnPropertyChanged();
            }
        }
        public string Space
        {
            get
            {
                return values[(int)Column.SPACE].ToString();
            }
            set
            {
                values[(int)Column.SPACE] = value;
                OnPropertyChanged();
            }
        }

        public CtWord() : base()
        {
            this.NameEn = string.Empty;
            this.NameKr = string.Empty;
            this.TypeFwd = string.Empty;
            this.TypeBck = string.Empty;
            this.TypeFwd = string.Empty;
            this.Divider = string.Empty;
            this.Language = string.Empty;
            this.Space = string.Empty;
        }

        public CtWord(object[] values) : base(values)
        {

        }

        public CtWord(string en, string kr, string space, string fwd, string bck, string cat, string language) : base()
        {
            this.NameEn = en;
            this.NameKr = kr;
            this.TypeFwd = fwd;
            this.TypeBck = bck;
            this.Divider = cat;
            this.Language = language;
            this.Space = space;
        }
        public CtWord(string en, string kr) : base()
        {
            NameEn = en;
            NameKr = kr;
            this.TypeFwd = string.Empty;
            this.TypeBck = string.Empty;
            this.TypeFwd = string.Empty;
            this.Divider = string.Empty;
            this.Language = string.Empty;
            this.Space = string.Empty;
        }
        public override List<string> GetTags() { return Library.StringDivider(values[(int)Column.NAMEEN] + "/" + values[(int)Column.NAMEKR], "/"); }
        public override CtModel Clone()
        {
            CtModel model = new CtWord();
            for (int i = 0; i < model.values.Length; i++)
            {
                model.values[i] = this.values[i];
            }
            return model;
        }
    }
}
