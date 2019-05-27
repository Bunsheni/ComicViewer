using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace ComicViewer.CtModels
{
    public class CtCharacter : CtModel
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
        public override CtModelType CtType { get { return CtModelType.CHARACTER; } }
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

        public CtCharacter() : base()
        {
            NameKr = NameEn = Synonym = string.Empty;
        }
        public CtCharacter(string name)
        {
            NameKr = NameEn = name;
            Synonym = string.Empty;
        }
        public CtCharacter(string nameen, string namekr)
        {
            NameEn = nameen;
            NameKr = namekr;
            Synonym = string.Empty;
        }

        public bool TranslateBy(CtModelList myNameList, CtModelList newNameList)
        {
            bool swap;
            List<CtName> nameList = new List<CtName>();
            string temp = NameKr;

            swap = false;

            //이름자체가 해당되는 경우는 우선적으로 바꿔주고 리턴
            CtName tempName = (CtName)myNameList.GetModel(NameEn, true);
            if (tempName != null)
            {
                NameEn = tempName.NameEn;
                if (string.Compare(tempName.NameKr, NameKr) == 0)
                    return false;
                else
                {
                    NameKr = tempName.NameKr;
                    return true;
                }
            }

            foreach (string str in Library.StringDivider(NameEn, " "))
            {
                CtName newname = (CtName)myNameList.GetModel(str, true);
                if (newname == null)
                {
                    newname = new CtName(str, str, false);
                    if(str.Trim('.','1','2','3','4','5','6','7','8','9','0').Length > 1)
                        newNameList.Add(newname);
                }
                else if (newname.NameKr.Length == 0)
                {
                    newname.NameKr = str;
                    newNameList.Add(newname);
                }
                else
                {
                    if (newname.Jp)
                        swap = true;
                }
                nameList.Add(newname);
            }

            //이제 newname으로 교체
            if (nameList.Count == 2 && swap)
            {
                NameKr = nameList[1].NameKr + " " + nameList[0].NameKr;
            }
            else
            {
                NameKr = string.Empty;
                foreach (CtName newname in nameList)
                {
                    if (NameKr.Length == 0)
                        NameKr = newname.NameKr;
                    else
                        NameKr += " " + newname.NameKr;
                }
            }

            if (string.Compare(NameKr, temp) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override CtModel Clone()
        {
            CtModel model = new CtCharacter();
            for (int i = 0; i < model.values.Length; i++)
            {
                model.values[i] = this.values[i];
            }
            return model;
        }
    }
}
