using System;
using System.Collections.Generic;
using System.Text;

namespace ComicViewer.CtModels
{
    public enum OLEDB_DATATYPE { integer, text, datetime, bit, oledb };

    public delegate CtModel NewModel(object[] values);
    public delegate int GetListViewIndex(int originindex);

    public class CtOleModelType
    {
        public string Name { get; set; }
        public string[] TableNames { get; set; }
        public int[] ListViewindex { get; set; }
        public OleField[] Fields { get; set; }
        public NewModel NewModel { get; set; }
        public string[] ListViewnames
        {
            get
            {
                string[] names = new string[Fields.Length];
                for (int i = 0; i < ListViewindex.Length; i++)
                {
                    names[i] = Fields[ListViewindex[i]].ListViewName;
                }
                return names;
            }
        }
        public int Length
        {
            get
            {
                return Fields.Length;
            }
        }

        public int GetListViewIndex(int i)
        {
            if (ListViewindex.Length > i)
                return ListViewindex[i];
            else
                return i;
        }
    }

    public struct OleField
    {
        /// <summary>
        /// 5 data types of OleDb is be using, intager, text(long text), Datetime, yes/no and Ole object.
        /// </summary>
        public static readonly Type[] OLEDB_DATATYPE = new Type[]
        {
            typeof(int), typeof(string), typeof(DateTime), typeof(bool), typeof(byte)
        };

        public OLEDB_DATATYPE OleDataType;
        static string[] strarray_datatype = { "int", "text", "datetime", "bit", "image" };
        public string FieldName;
        public string Name;
        public string defalut_value;
        public bool not_null;
        public bool auto_increment;
        public bool primary_key;
        public int max_length;
        public int ListViewWidth;
        public string ListViewName;

        public string Datatype_length
        {
            get
            {
                return max_length > 0 ? string.Format("{0}({1})", strarray_datatype[(int)OleDataType], max_length) : strarray_datatype[(int)OleDataType];
            }
        }
        public string Field_option
        {
            get
            {
                return string.Format("{0}{1}{2}{3}",
                    not_null ? "not null " : string.Empty,
                    auto_increment ? "identity " : string.Empty,
                    primary_key ? "primary key " : string.Empty,
                    defalut_value.Length == 0 ? string.Empty : string.Format("default {0}", defalut_value)
                    ).Trim();
            }
        }
        public Type DataType
        {
            get
            {
                return OLEDB_DATATYPE[(int)OleDataType];
            }
        }


        public OleField(string name, OLEDB_DATATYPE dataType)
        {
            ListViewName = FieldName = name;
            ListViewWidth = 100;
            Name = name.Replace(" ", "_").ToLower();
            OleDataType = dataType;
            not_null = false;
            auto_increment = false;
            primary_key = false;
            defalut_value = string.Empty;
            max_length = 0;
        }
    }    
}
