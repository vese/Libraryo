using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryManager
{
    class ListStructures
    {
        public struct Books
        {
            public string Name;
            public string Genre;
            public string Descr;
            public int SeriesID;
        }
        public struct Series
        {
            public string Name;
            public string Genre;
            public List<int> BooksIDs;
        }
    }
}
