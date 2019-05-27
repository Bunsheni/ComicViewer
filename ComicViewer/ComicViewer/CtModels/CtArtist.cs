using System;
using System.Collections.Generic;
using SQLite;

namespace ComicViewer.CtModels
{
    public class CtArtist : CtModel
    {
        public static int SortingColumn;

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
                        return Synonym;
                    default:
                        return NameEn;
                }
            }
        }
        [Ignore]
        public override CtModelType CtType { get { return CtModelType.ARTIST; } }
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

        public CtArtist() : base()
        {
            NameEn = NameKr = Synonym = string.Empty;
        }
        public CtArtist(string name)
        {
            NameKr = NameEn = name;
            Synonym = string.Empty;
        }
        public CtArtist(string nameen, string namekr)
        {
            NameEn = nameen;
            NameKr = namekr;
            Synonym = string.Empty;
        }

        public override CtModel Clone()
        {
            CtModel model = new CtArtist();
            for (int i = 0; i < model.values.Length; i++)
            {
                model.values[i] = this.values[i];
            }
            return model;
        }
    }
}
