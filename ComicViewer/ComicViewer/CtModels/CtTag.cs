using System;
using System.Collections.Generic;
using SQLite;

namespace ComicViewer.CtModels
{
    public class CtTag : CtModel
    {
        public new enum Column { NAMEEN, NAMEKR, SYNONYM, GENDER }
        public new static readonly string[] PropertiesName = { "NameEn", "NameKr", "Synonym", "Gender" };
        public new static readonly string[] SortsKr = { "이름(영)", "이름(한)", "이명", "성별"};
        public new static readonly string[] SortsEn = { "Name(En)", "Name(Kr)", "Synonym", "Gender" };
        public new static readonly Type[] types = { typeof(string), typeof(string), typeof(string), typeof(int) };

        [Ignore]
        public override Type[] Types { get { return types; } }
        public static int SortingColumn;
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
                        return Synonym;
                    case 3:
                        return Gender;
                    default:
                        return NameEn;
                }
            }
        }
        [Ignore]
        public override CtModelType CtType { get { return CtModelType.TAG; } }
        [Ignore]
        public override string Name
        {
            get
            {
                return ProgramLanguage == 0 ? NameKr : NameEn;
            }
            set
            {
                NameEn = NameKr = value;
                OnPropertyChanged();
            }
        }
        [Ignore]
        public override string Id { get { return NameEn; } }

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
                if (value.EndsWith("♂"))
                    Gender = 2;
                else if (value.EndsWith("♀"))
                    Gender = 1;
                else
                    Gender = 0;
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
        public override string Synonym
        {
            get
            {
                return values[(int)Column.SYNONYM].ToString();
            }
            set
            {
                values[(int)Column.SYNONYM] = value;
                OnPropertyChanged();
            }
        }
        public int Gender
        {
            get
            {
                return (int)values[(int)Column.GENDER];
            }
            set
            {
                values[(int)Column.GENDER] = value;
                OnPropertyChanged();
            }
        }

        public CtTag() : base()
        {
            NameKr = NameEn = Synonym = string.Empty;
            Gender = 0;
        }
        public CtTag(string name)
        {
            NameKr = NameEn = name;
            Synonym = string.Empty;
            if (name.Contains("♀"))
                Gender = 1;
            else if (name.Contains("♂"))
                Gender = 2;
            else
                Gender = 0;
        }
        public CtTag(string nameen, string namekr)
        {
            NameEn = nameen;
            NameKr = namekr;
            Synonym = string.Empty;
            if (nameen.Contains("♀"))
                Gender = 1;
            else if (nameen.Contains("♂"))
                Gender = 2;
            else
                Gender = 0;
        }
        public override CtModel Clone()
        {
            CtModel model = new CtTag();
            for (int i = 0; i < model.values.Length; i++)
            {
                model.values[i] = this.values[i];
            }
            return model;
        }
    }
}
