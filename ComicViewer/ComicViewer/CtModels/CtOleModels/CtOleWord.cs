using System;
using System.Collections.Generic;
using System.Text;


namespace ComicViewer.CtModels
{
    class CtOleWord : CtWord, IOleModel
    {

        public static readonly CtOleModelType _objectType = new CtOleModelType()
        {
            Name = "단어 목록",
            TableNames = new string[] { "Word Table" },
            Fields = new OleField[]{
                new OleField(PropertiesName[(int)Column.NAMEEN], OLEDB_DATATYPE.text)
                {not_null = true},
                new OleField(PropertiesName[(int)Column.NAMEKR], OLEDB_DATATYPE.text)
                {not_null = true},
                new OleField(PropertiesName[(int)Column.SPACE], OLEDB_DATATYPE.text)
                {not_null = true},
                new OleField(PropertiesName[(int)Column.TYPEFWD], OLEDB_DATATYPE.text)
                {not_null = true},
                new OleField(PropertiesName[(int)Column.TYPEBCK], OLEDB_DATATYPE.text)
                {not_null = true},
                new OleField(PropertiesName[(int)Column.DIVIDER], OLEDB_DATATYPE.text)
                {not_null = true},
                new OleField(PropertiesName[(int)Column.LANGUAGE], OLEDB_DATATYPE.text)
                {not_null = true},
            },
            NewModel = new NewModel(CtOleWord.NewModel),
            ListViewindex = new int[] { (int)Column.NAMEEN, (int)Column.NAMEKR, (int)Column.TYPEFWD, (int)Column.TYPEBCK, (int)Column.DIVIDER, (int)Column.LANGUAGE, (int)Column.SPACE }
        };
        
        public CtOleModelType ObjectType
        {
            get
            {
                return _objectType;
            }
        }

        public CtOleWord(object[] values) : base(values) { }

        public static CtOleWord NewModel(object[] values)
        {
            return new CtOleWord(values);
        }
    }
}
