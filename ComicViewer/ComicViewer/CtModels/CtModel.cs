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
using System.Diagnostics;
using System.Drawing;

namespace ComicViewer.CtModels
{
    public enum SortOrder
    {
        None = 0,
        Ascending = 1,
        Descending = 2,
    }
    public enum CtLanguage { KOREAN, ENGLISH, JAPANESE, COUNT };
    public enum CtModelType { ALL, COMIC, ARTIST, GROUP, SERIES, CHARACTER, TAG, NAME, WORD, COUNT}

    public abstract class CtModel : INotifyPropertyChanged, IDisposable
    {
        public enum Column { NAMEEN, NAMEKR, SYNONYM }
        //static fields
        public static readonly string[] PropertiesName = { "NameEn", "NameKr", "Synonym" };
        public static readonly string[] SortsKr = { "이름(영)", "이름(한)", "이명" };
        public static readonly string[] SortsEn = { "Name(En)", "Name(Kr)", "Synonym" };
        public static string[] _colunmText { get { return ProgramLanguage == 0 ? SortsKr : SortsEn; } }
        public static readonly Type[] types = { typeof(string), typeof(string), typeof(string) };

        private static string[] _modelNameKrArray = { "전체", "작품", "작가", "그룹", "시리즈", "인물", "태그", "이름", "단어" };
        private static string[] _modelNameEnArray = { "All", "Comic", "Artist", "Group", "Series", "Character", "Tag", "Name", "Word" };
        public static string[] LanguageStringArray = { "한국어", "English", "日本語" };
        public static CtLanguage ProgramLanguage;

        //fields
        public object[] values;

        //properties
        public abstract string NameEn { get; set; }
        public abstract string NameKr { get; set; }
        public abstract string Synonym { get; set; }
        public abstract string Name { get; set; }
        public abstract object SortingPropertie { get; }
        public abstract CtModelType CtType { get; }
        public abstract string Id { get; }
        public abstract Type[] Types { get; }

        public virtual string[] ColunmText { get { return _colunmText; } }
        public virtual string Detail
        {
            get
            {
                string res = string.Empty;
                foreach(string temp in GetTags())
                {
                    res += temp + "/";
                }
                return res.Trim('/');
            }
        }
        public string ModelName
        {
            get
            {
                return ProgramLanguage == 0 ? _modelNameKrArray[(int)CtType] : _modelNameEnArray[(int)CtType];
            }
        }

        public string Text
        {
            get
            {
                return ModelName + ": " + Name;
            }
        }

        private bool _isselected;

        [Ignore]
        public virtual bool IsSelected
        {
            get
            {
                return _isselected;
            }
            set
            {
                _isselected = value;
                OnPropertyChanged();
                OnPropertyChanged("BackgroundColor");
            }
        }
        [Ignore]
        public virtual Color BackgroundColor
        {
            get
            {
                return IsSelected ? Color.LightGray : Color.Transparent;
            }
        }

        public CtModel()
        {
            values = new object[Types.Length];
        }

        public CtModel(object[] values)
        {
            this.values = (object[])values.Clone();
        }
        
        public static string GetModelName(CtModelType type)
        {
            return ProgramLanguage == 0 ? _modelNameKrArray[(int)type] : _modelNameEnArray[(int)type];
        }

        public virtual List<string> GetTags()
        {
            List<string> temp = Library.StringDivider(Synonym, "/");
            if (NameEn != NameKr)
            {
                temp.Insert(0, NameKr);
            }
            temp.Insert(0, NameEn);
            return temp;
        }

        public virtual bool Contains(string key)
        {
            return NameEn.ToLower().Contains(key.ToLower()) || NameKr.ToLower().Contains(key.ToLower()) || Synonym.ToLower().Contains(key.ToLower());
        }

        public abstract CtModel Clone();

        public bool Equals(CtModel other)
        {            
            return (Id.Equals(other.Id));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected bool Disposed { get; private set; }
        protected virtual void Dispose(bool disposing)
        {
            Disposed = true;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }


    public class CtModelList : List<CtModel>, IDisposable
    {
        private object lockObject = new object();
        public int[] Counts = new int[(int)CtModelType.COUNT];
        public new void Add(CtModel model)
        {
            Counts[(int)model.CtType]++;
            base.Add(model);
        }
        public void AddRange(List<CtModel> models)
        {
            foreach(CtModel model in models)
            {
                Counts[(int)model.CtType]++;
            }
            base.AddRange(models);            
        }
        public new void Clear()
        {
            for(int i =0; i<Counts.Length; i++)
            {
                Counts[i] = 0;
            }
            base.Clear();
        }
        public CtModel GetModel(int i)
        {
            return this[i];
        }
        public CtModel GetModel(string id)
        {
            return Find(i => i.Id == id);
        }
        public CtModel GetModel(string en, bool ignorecase)
        {
            return Find(i => (string.Compare(en, i.values[0].ToString(), ignorecase) == 0));
        }
        public int GetIndex(string id)
        {
            return FindIndex(i => i.Id == id);
        }
        public int UpdateModel(CtModel model)
        {
            lock (lockObject)
            {
                int index = GetIndex(model.Id);
                if (index >= 0)
                    this[index] = model;
                return index;
            }
        }
        public bool UpdateAddModel(CtModel work)
        {
            lock (lockObject)
            {
                int index = GetIndex(work.Id);
                if (index < 0)
                {
                    Insert(0, work);
                    return false;
                }
                else
                {
                    this[index] = work;
                    return true;
                }
            }
        }
        public void UpdateModel(CtModelList models)
        {
            foreach (CtModel model in models)
            {
                UpdateModel(model);
            }
        }
        public void UpdateAddModel(CtModelList models)
        {
            foreach (CtModel model in models)
            {
                UpdateAddModel(model);
            }
        }
        public void DeleteModel(string id)
        {
            lock (lockObject)
            {
                int i = GetIndex(id);
                if (i >= 0)
                    RemoveAt(i);
            }
        }

        public void Dispose()
        {
            foreach (CtModel model in this)
            {
                model.Dispose();
            }
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected bool Disposed { get; private set; }
        protected virtual void Dispose(bool disposing)
        {
            Disposed = true;
        }
    }

    public class CtModelSortComparer : IComparer<CtModel>
    {
        int col = 0;
        public SortOrder order = SortOrder.Ascending;

        public CtModelSortComparer(int col, SortOrder order)
        {
            this.col = col;
            this.order = order;
        }
        public int Compare(CtModel x, CtModel y)
        {
            int res = 0;
            if (x.Types == y.Types)
            {
                if (col < 0)
                {
                    if (((string)x.values[0]).Length == ((string)y.values[0]).Length)
                    {
                        res = 0;
                    }
                    else
                    {
                        res = ((string)x.values[0]).Length > ((string)y.values[0]).Length ? 1 : -1;
                    }
                }
                else if (x.Types[col] == typeof(string))
                    res = string.Compare(x.values[col].ToString(), y.values[col].ToString());
                else if (x.Types[col] == typeof(int))
                    res = (int)x.values[col] == (int)y.values[col] ? 0 : (int)x.values[col] > (int)y.values[col] ? 1 : -1;
                else if (x.Types[col] == typeof(DateTime))
                {
                    DateTime dtx = new DateTime();
                    DateTime dty = new DateTime();
                    DateTime.TryParse(x.values[col].ToString(), null, System.Globalization.DateTimeStyles.AssumeLocal, out dtx);
                    DateTime.TryParse(y.values[col].ToString(), null, System.Globalization.DateTimeStyles.AssumeLocal, out dty);
                    res = DateTime.Compare(dtx, dty);
                }
                else if (x.Types[col] == typeof(bool))
                {
                    res = (bool)x.values[col] == (bool)y.values[col] ? 0 : (bool)x.values[col] ? 1 : -1;
                }
                else
                    res = 0;
            }
            return (order == SortOrder.Ascending) ? res : res * -1;
        }

    }

    public class CtModelObservableCollection : ObservableCollection<CtModel>, IDisposable
    {
        public CtModelObservableCollection() : base()
        {
        }
        public CtModelObservableCollection(List<CtModel> list) : base(list)
        {
        }

        public CtModel GetModel(string id)
        {
            try
            {
                return this.Single(i => i.Id == id);
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }
        public int GetIndex(string id)
        {
            try
            {
                return this.IndexOf(this.Single(i => i.Id== id));
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }
        public void UpdateModel(CtModel model)
        {
            this.SetItem(GetIndex(model.Id), model);
        }


        public void Dispose()
        {
            foreach (CtModel model in this)
            {
                model.Dispose();
            }
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected bool Disposed { get; private set; }
        protected virtual void Dispose(bool disposing)
        {
            Disposed = true;
        }
    }
}
