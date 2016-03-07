using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Library
{
    public class SerializationClass
    {
        public class Lists
        {
            public List<Form1.Books> BooksList = new List<Form1.Books>();
            public List<Form1.Series> SeriesList = new List<Form1.Series>();
        }

        //public void Save(List<Form1.Books> l1, List<Form1.Series> l2, string filename)
        //{
        //    var p = new List<Lists>()
        //    {
        //        new Lists()
        //        {BooksList = l1, SeriesList = l2}
        //    };



        //    try
        //    {
        //        using (var fs = new StreamWriter(filename))
        //        {
        //            var s = new XmlSerializer(p.GetType());
        //            s.Serialize(fs, p);
        //        }
        //    }
        //    catch (Exception e)
        //    { }
        //}
        public void Read(ref List<Form1.Books> l1, ref List<Form1.Series> l2, string filename)
        {
            try
            {
                using (var rd = new StreamReader(filename))
                {
                    var s = new XmlSerializer(typeof(List<Lists>));
                    var p = (List<Lists>)s.Deserialize(rd);
                    foreach (var c in p)
                    {
                        l1 = c.BooksList;
                        l2 = c.SeriesList;
                    }
                }
            }
            catch (Exception e)
            { }
        }
    }
}
