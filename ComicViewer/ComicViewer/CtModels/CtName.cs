using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComicViewer.CtModels
{
    public class CtName : CtModel
    {

        public new enum Column { NAMEEN, NAMEKR, JP }
        public new static readonly string[] PropertiesName = { "NameEn", "NameKr", "West" };
        public new static readonly string[] SortsKr = { "이름(영)", "이름(한)", "유형" };
        public new static readonly string[] SortsEn = { "Name(En)", "Name(Kr)", "West" };
        public new static readonly Type[] types = { typeof(string), typeof(string), typeof(bool) };
        public static int SortingColumn;

        [Ignore]
        public override Type[] Types { get { return types; } }
        [Ignore]
        public override string[] ColunmText { get { return ProgramLanguage == 0 ? SortsKr : SortsEn; } }

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
                        return Jp;
                    default:
                        return NameEn;
                }
            }
        }
        [Ignore]
        public override CtModelType CtType { get { return CtModelType.NAME; } }
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
        public bool Jp
        {
            get
            {
                return (bool)values[(int)Column.JP];
            }
            set
            {
                values[(int)Column.JP] = value;
                OnPropertyChanged();
            }
        }

        public CtName() : base()
        {
            NameEn = string.Empty;
            NameKr = string.Empty;
            Jp = false;
        }
        public CtName(string en, string kr, bool type)
        {
            NameEn = en;
            NameKr = kr;
            Jp = type;
        }

        public CtName(object[] values) : base(values) { }

        public override CtModel Clone()
        {
            CtModel model = new CtName();
            for (int i = 0; i < model.values.Length; i++)
            {
                model.values[i] = this.values[i];
            }
            return model;
        }
    }
}
