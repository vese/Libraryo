using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace LibraryManager
{
    public class SerializationClass
    {
        public class Lists
        {
            public List<Form1.Books> BooksList = new List<Form1.Books>();
            public List<Form1.Series> SeriesList = new List<Form1.Series>();
        }

        public void Save(List<Form1.Books> l1, List<Form1.Series> l2, string filename)
        {
            var p = new List<Lists>()
            {
                new Lists()
                {BooksList = l1, SeriesList = l2}
            };
            
                

            try
            {
                using (var fs = new StreamWriter(filename))
                {
                    var s = new XmlSerializer(p.GetType());
                    s.Serialize(fs, p);
                }
            }
            catch (Exception e)
            { }
        }
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


        public void Savebnf(List<Form1.FTPnf> l1, string filename)
        {
            try
            {
                using (var fs = new StreamWriter(filename))
                {
                    var s = new XmlSerializer(l1.GetType());
                    s.Serialize(fs, l1);
                }
            }
            catch (Exception e)
            { }
        }
        public void Readbnf(ref List<Form1.FTPnf> l1, string filename)
        {
            try
            {
                using (var rd = new StreamReader(filename))
                {
                    var s = new XmlSerializer(typeof(List<Form1.FTPnf>));
                    var p = (List<Form1.FTPnf>)s.Deserialize(rd);
                    foreach (var c in p)
                    {
                        l1.Add(c);
                    }
                }
            }
            catch (Exception e)
            { }
        }
    }
}
