using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;

namespace Library
{
    public partial class Form1 : Form
    {
        public string downloadingname = "";
        int j = 4;
        int firstin = 0;
        bool IsFileExist = false;
        bool noint = false;
        FTP ftpv = new FTP();
        HTTP httpv = new HTTP();
        string site;// = "libraryo.esy.es";
        string msgtoreport;
        string content = "Content.xml";
        public struct Books
        {
            public string Name;
            public string Author;
            public string Genre;
            public string Descr;
            public int SeriesID;
            public int ID;
        }
        public struct Series
        {
            public string Name;
            public string Author;
            public string Genre;
            public string Descr;
            public List<string> BooksIDs;
            public int ID;
        }

        List<Books> BooksList = new List<Books>();
        List<Series> SeriesList = new List<Series>();

        SerializationClass serial = new SerializationClass();
        Rename ren = new Rename();
        int BookName;
        List<int> BooksInSer = new List<int>();
        string OpenBook;
        public Form1()
        {
            InitializeComponent();
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
        }
        private Boolean DateCompare(string date, string datesaved)
        {
            DateTime date1 = Convert.ToDateTime(date);
            DateTime datesaved1 = Convert.ToDateTime(datesaved);
            if (date1 > datesaved1)
                return true;
            else return false;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            FileInfo file0 = new FileInfo("Libraries.txtt");
            if (!file0.Exists)
            {
                FileStream fs = file0.Create();
                fs.Close();
            }
            StreamReader reads = new StreamReader(file0.Name);
            string strs = reads.ReadLine();
            reads.Close();
            if (strs == null)
            {
                StreamWriter writes = new StreamWriter("Libraries.txtt");
                writes.Write("libraryo.esy.es");
                writes.Close();
            }

                StreamReader readftpinf = new StreamReader("Libraries.txtt");
                List<string> str = new List<string>();
                str.AddRange(readftpinf.ReadLine().Split('&'));
                site = str[0];
                str.Clear();
                readftpinf.Close();
                try
                {
                    httpv.Download("Date.txtt", site, ref msgtoreport);
                    //ftpv.Download("Date.txtt", site, login, password, ref msgtoreport);
                }
                catch (Exception nn)
                { noint = true; label1.Text = "Не удалось подключиться к серверу"; }
                    //if (downloadingname == "Date.txtt")
                if (noint == false)
            if (new FileInfo("DateSaved.txtt").Exists && new FileInfo("Content.xml").Exists)
            {
                    StreamReader ReaderDate = new StreamReader("Date.txtt");
                    string date = ReaderDate.ReadLine();
                    ReaderDate.Close();
                    StreamReader ReaderDateSaved = new StreamReader("DateSaved.txtt");
                    string datesaved = ReaderDateSaved.ReadLine();
                    ReaderDateSaved.Close();
                    if (DateCompare(date, datesaved))
                    {
                        label1.Text = "Скачивание нового Content.xml";
                        try
                        {
                            httpv.Download(content, site, ref msgtoreport);
                            //ftpv.Download(content, site, login, password, ref msgtoreport);
                            ren.del("DateSaved.txtt", ref msgtoreport);
                            ren.ren("Date", "DateSaved", ".txtt", ref msgtoreport);
                        }
                        catch (Exception nn)
                        { label1.Text = "Не удалось подключиться к серверу"; }
                    }
            }
            else
            {
                label1.Text = "Скачивание нового Content.xml";
                try
                {
                    httpv.Download(content, site, ref msgtoreport);
                    //ftpv.Download(content, site, login, password, ref msgtoreport);
                }
                catch (Exception nn)
                { label1.Text = "Не удалось подключиться к серверу"; }
                    ren.ren("Date", "DateSaved", ".txtt", ref msgtoreport);
                if (msgtoreport != "") label1.Text = msgtoreport;
            }
            serial.Read(ref BooksList, ref SeriesList, content);
            foreach (var b in BooksList)
                listBox1.Items.Add(b.Name + " " + b.Author + " " + b.Genre);
            foreach (var s in SeriesList)
                listBox2.Items.Add(s.Name + " " + s.Author + " " + s.Genre);
            if (listBox1.Items.Count > 1)
                listBox1.SelectedIndex = 0;
            if (listBox2.Items.Count > 1)
                listBox2.SelectedIndex = 0;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (firstin > j)
            {
                label1.Text = "";
            }
            firstin++;
            if (listBox1.SelectedIndex != -1)
            {
                BookName = listBox1.SelectedIndex;
                string serid = "";

                foreach (var s in SeriesList)
                    if (s.ID == BooksList[listBox1.SelectedIndex].SeriesID)
                    {
                        serid = Environment.NewLine + "Книга входит в серию книг " + s.Name + Environment.NewLine;
                        button6.Visible = true;
                        break;
                    }
                if (serid == "")
                {
                    button6.Visible = false;
                    serid = Environment.NewLine;
                }

                textBox1.Text = BooksList[listBox1.SelectedIndex].Name + Environment.NewLine + BooksList[listBox1.SelectedIndex].Author + Environment.NewLine + BooksList[listBox1.SelectedIndex].Genre + "" + serid + "" + BooksList[listBox1.SelectedIndex].Descr;
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (firstin > j)
            {
                label1.Text = "";
            }
            firstin++;
            if (listBox1.SelectedIndex != -1)
            {
                string booid = "";
                comboBox1.Items.Clear();
                BooksInSer.Clear();
                foreach (var s in SeriesList[listBox2.SelectedIndex].BooksIDs)
                    foreach (var b in BooksList)
                        if (b.ID == Convert.ToInt32(s))
                        {
                            BooksInSer.Add(b.ID);
                            comboBox1.Items.Add(b.Name);
                            if (booid == "")
                                booid = b.Name;
                            else
                                booid = booid + ", " + b.Name;
                            break;
                        }
                if (booid != "")
                    booid = Environment.NewLine + "В серию входят книги:" + Environment.NewLine + booid + Environment.NewLine;
                textBox2.Text = SeriesList[listBox2.SelectedIndex].Name + Environment.NewLine + SeriesList[listBox2.SelectedIndex].Author + Environment.NewLine + SeriesList[listBox2.SelectedIndex].Genre + "" + booid + "" + SeriesList[listBox2.SelectedIndex].Descr;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            if (comboBox1.SelectedIndex != -1)
            {
                int ind = 0;
                for (int i = 0; i < BooksList.Count; i++)
                    if (BooksList[i].ID == BooksInSer[comboBox1.SelectedIndex])
                    {
                        ind = i; break;
                    }
                listBox1.SelectedIndex = ind;

                comboBox4.SelectedIndex = 0;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            int ind = 0;
            for (int r = 0; r < SeriesList.Count ; r++)
                foreach (var t in SeriesList[r].BooksIDs)
                    if (BooksList[BookName].ID == Convert.ToInt32(t))
                    { ind = r; break;  }
            listBox2.SelectedIndex = ind;
            comboBox4.SelectedIndex = 1;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (firstin > j)
            {
                label1.Text = "";
            }
            firstin++;
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    BooksList.Sort(delegate(Books a, Books b)
                    {
                        return a.ID.CompareTo(b.ID);
                    });
                    break;
                case 1:
                    BooksList.Sort(delegate(Books a, Books b)
                    {
                        return a.Name.CompareTo(b.Name);
                    });
                    break;
                case 2:
                    BooksList.Sort(delegate(Books a, Books b)
                    {
                        return a.Author.CompareTo(b.Author);
                    });
                    break;
                case 3:
                    BooksList.Sort(delegate(Books a, Books b)
                    {
                        return a.Genre.CompareTo(b.Genre);
                    });
                    break;
            }


                    listBox1.Items.Clear();
                    foreach (var b in BooksList)
                        listBox1.Items.Add(b.Name + " " + b.Author + " " + b.Genre);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (firstin > j)
            {
                label1.Text = "";
            }
            firstin++;
            switch (comboBox3.SelectedIndex)
            {
                case 0:
                    SeriesList.Sort(delegate(Series a, Series b)
                    {
                        return a.ID.CompareTo(b.ID);
                    });
                    break;
                case 1:
                    SeriesList.Sort(delegate(Series a, Series b)
                    {
                        return a.Name.CompareTo(b.Name);
                    });
                    break;
                case 2:
                    SeriesList.Sort(delegate(Series a, Series b)
                    {
                        return a.Author.CompareTo(b.Author);
                    });
                    break;
                case 3:
                    SeriesList.Sort(delegate(Series a, Series b)
                    {
                        return a.Genre.CompareTo(b.Genre);
                    });
                    break;
            }
            listBox2.Items.Clear();
            foreach (var s in SeriesList)
                listBox2.Items.Add(s.Name + " " + s.Author + " " + s.Genre);
        }

        private void button1_Click(object sender, EventArgs e)
        {
                if (!new FileInfo(BooksList[listBox1.SelectedIndex].Name.ToString() + button1.Text).Exists)
            {
            label1.Text = "";
            try
            {
                httpv.Download(BooksList[listBox1.SelectedIndex].ID.ToString() + button1.Text, site, ref msgtoreport);
                //ftpv.Download(, site, login, password, ref msgtoreport);
                //if (downloadingname == BooksList[listBox1.SelectedIndex].ID.ToString() + button1.Text)
            }
            catch (Exception nn)
            { label1.Text = "Не удалось подключиться к серверу";/*MessageBox.Show(nn.ToString());*/ }
                ren.ren(BooksList[listBox1.SelectedIndex].ID.ToString(), BooksList[listBox1.SelectedIndex].Name.ToString(), button1.Text, ref msgtoreport);
                if (msgtoreport != "") label1.Text = msgtoreport;
            }
            else
            {
                label1.Text = "Файл " + BooksList[listBox1.SelectedIndex].Name.ToString() + button1.Text + " уже существует!";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!new FileInfo(BooksList[listBox1.SelectedIndex].Name.ToString() + button2.Text).Exists)
            {
            label1.Text = "";
            try
            {
                httpv.Download(BooksList[listBox1.SelectedIndex].ID.ToString() + button2.Text, site, ref msgtoreport);
                //ftpv.Download(, site, login, password, ref msgtoreport);
                if (msgtoreport != "") label1.Text = msgtoreport;
                //if (downloadingname == BooksList[listBox1.SelectedIndex].ID.ToString() + button2.Text)
                ren.ren(BooksList[listBox1.SelectedIndex].ID.ToString(), BooksList[listBox1.SelectedIndex].Name.ToString(), button2.Text, ref msgtoreport);
                if (msgtoreport != "") label1.Text = msgtoreport;
            }
            catch (Exception nn)
            { label1.Text = "Не удалось подключиться к серверу";/*MessageBox.Show(nn.ToString());*/ }
            }
            else
            {
                label1.Text = "Файл " + BooksList[listBox1.SelectedIndex].Name.ToString() + button2.Text + " уже существует!"; 
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!new FileInfo(BooksList[listBox1.SelectedIndex].Name.ToString() + button3.Text).Exists)
            {
            label1.Text = "";
            try
            {
                httpv.Download(BooksList[listBox1.SelectedIndex].ID.ToString() + button3.Text, site, ref msgtoreport);
                //ftpv.Download(, site, login, password, ref msgtoreport);
                if (msgtoreport != "") label1.Text = msgtoreport;
                //if (downloadingname == BooksList[listBox1.SelectedIndex].ID.ToString() + button3.Text)
                ren.ren(BooksList[listBox1.SelectedIndex].ID.ToString(), BooksList[listBox1.SelectedIndex].Name.ToString(), button3.Text, ref msgtoreport);
                if (msgtoreport != "") label1.Text = msgtoreport;
            }
            catch (Exception nn)
            { label1.Text = "Не удалось подключиться к серверу";/*MessageBox.Show(nn.ToString());*/ }
            }
            else
            {
                label1.Text = "Файл " + BooksList[listBox1.SelectedIndex].Name.ToString() + button3.Text + " уже существует!"; 
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!new FileInfo(BooksList[listBox1.SelectedIndex].Name.ToString() + button4.Text).Exists)
            {
            label1.Text = "";
                try
                {
                    httpv.Download(BooksList[listBox1.SelectedIndex].ID.ToString() + button4.Text, site, ref msgtoreport);
                    //ftpv.Download(, site, login, password, ref msgtoreport);
                    if (msgtoreport != "") label1.Text = msgtoreport;
                    //if (downloadingname == BooksList[listBox1.SelectedIndex].ID.ToString() + button4.Text)
                    ren.ren(BooksList[listBox1.SelectedIndex].ID.ToString(), BooksList[listBox1.SelectedIndex].Name.ToString(), button4.Text, ref msgtoreport);
                    if (msgtoreport != "") label1.Text = msgtoreport;
                }
                catch (Exception nn)
                { label1.Text = "Не удалось подключиться к серверу";/*MessageBox.Show(nn.ToString());*/ }
            }
            else
            {
                label1.Text = "Файл " + BooksList[listBox1.SelectedIndex].Name.ToString() + button4.Text + " уже существует!"; 
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            object miss = System.Reflection.Missing.Value;
            object path = Application.StartupPath + "\\1.docx";
            object readOnly = true;
            Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref path, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);
            string totaltext = "";
            for (int i = 0; i < docs.Paragraphs.Count; i++)
            {
                totaltext += " \r\n " + docs.Paragraphs[i + 1].Range.Text.ToString();
            }
            MessageBox.Show(totaltext);
            docs.Close();
            word.Quit();

            
            label1.Text = "";
            openFileDialog1.InitialDirectory = Application.StartupPath;
                openFileDialog1.FileName = null;
                openFileDialog1.Filter = "Файлы txt(*.txt)|*.txt|Файлы doc(*.doc)|*.doc*|Файлы fb2(*.fb2)|*.fb2|Файлы epub(*.epub)|*.epub";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    OpenBook = openFileDialog1.FileName;
                    Process process = new Process();
                    process.StartInfo.FileName = @OpenBook;
                    process.StartInfo.Arguments = "";
                    process.Start();
                }
        }
        
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            button1.Left = 285 * Form1.ActiveForm.Width / 500; button1.Top = 289 * Form1.ActiveForm.Height / 500; button1.Width = 100 * Form1.ActiveForm.Width / 500; button1.Height = 23 * Form1.ActiveForm.Height / 500;
            button2.Left = 285 * Form1.ActiveForm.Width / 500; button2.Top = 318 * Form1.ActiveForm.Height / 500; button2.Width = 100 * Form1.ActiveForm.Width / 500; button2.Height = 23 * Form1.ActiveForm.Height / 500;
            button3.Left = 285 * Form1.ActiveForm.Width / 500; button3.Top = 347 * Form1.ActiveForm.Height / 500; button3.Width = 100 * Form1.ActiveForm.Width / 500; button3.Height = 23 * Form1.ActiveForm.Height / 500;
            button4.Left = 285 * Form1.ActiveForm.Width / 500; button4.Top = 376 * Form1.ActiveForm.Height / 500; button4.Width = 100 * Form1.ActiveForm.Width / 500; button4.Height = 23 * Form1.ActiveForm.Height / 500;
            button5.Left = 285 * Form1.ActiveForm.Width / 500; button5.Top = 289 * Form1.ActiveForm.Height / 500; button5.Width = 100 * Form1.ActiveForm.Width / 500; button5.Height = 23 * Form1.ActiveForm.Height / 500;
            button6.Left = 285 * Form1.ActiveForm.Width / 500; button6.Top = 260 * Form1.ActiveForm.Height / 500; button6.Width = 100 * Form1.ActiveForm.Width / 500; button6.Height = 23 * Form1.ActiveForm.Height / 500;
            button7.Left = 270 * Form1.ActiveForm.Width / 500; button7.Top = 421 * Form1.ActiveForm.Height / 500; button7.Width = 130 * Form1.ActiveForm.Width / 500; button7.Height = 23 * Form1.ActiveForm.Height / 500;
            button8.Left = 30 * Form1.ActiveForm.Width / 500; button8.Top = 403 * Form1.ActiveForm.Height / 500; button8.Width = 130 * Form1.ActiveForm.Width / 500; button8.Height = 23 * Form1.ActiveForm.Height / 500;
            comboBox1.Left = 285 * Form1.ActiveForm.Width / 500; comboBox1.Top = 262 * Form1.ActiveForm.Height / 500; comboBox1.Width = 100 * Form1.ActiveForm.Width / 500; comboBox1.Height = 21 * Form1.ActiveForm.Height / 500;
            comboBox2.Left = 30 * Form1.ActiveForm.Width / 500; comboBox2.Top = 54 * Form1.ActiveForm.Height / 500; comboBox2.Width = 150 * Form1.ActiveForm.Width / 500; comboBox2.Height = 21 * Form1.ActiveForm.Height / 500;
            comboBox3.Left = 30 * Form1.ActiveForm.Width / 500; comboBox3.Top = 54 * Form1.ActiveForm.Height / 500; comboBox3.Width = 150 * Form1.ActiveForm.Width / 500; comboBox3.Height = 21 * Form1.ActiveForm.Height / 500;
            comboBox4.Left = 50 * Form1.ActiveForm.Width / 500; comboBox4.Top = 9 * Form1.ActiveForm.Height / 500; comboBox4.Width = 400 * Form1.ActiveForm.Width / 500; comboBox4.Height = 21 * Form1.ActiveForm.Height / 500;
            label1.Left = 27 * Form1.ActiveForm.Width / 500; label1.Top = 431 * Form1.ActiveForm.Height / 500; label1.Width = 65 * Form1.ActiveForm.Width / 500; label1.Height = 13 * Form1.ActiveForm.Height / 500;
            label2.Left = 47 * Form1.ActiveForm.Width / 500; label2.Top = 38 * Form1.ActiveForm.Height / 500; label2.Width = 87 * Form1.ActiveForm.Width / 500; label2.Height = 13 * Form1.ActiveForm.Height / 500;
            listBox1.Left = 30 * Form1.ActiveForm.Width / 500; listBox1.Top = 87 * Form1.ActiveForm.Height / 500; listBox1.Width = 150 * Form1.ActiveForm.Width / 500; listBox1.Height = 303 * Form1.ActiveForm.Height / 500;
            listBox2.Left = 30 * Form1.ActiveForm.Width / 500; listBox2.Top = 87 * Form1.ActiveForm.Height / 500; listBox2.Width = 150 * Form1.ActiveForm.Width / 500; listBox2.Height = 303 * Form1.ActiveForm.Height / 500;
            textBox1.Left = 210 * Form1.ActiveForm.Width / 500; textBox1.Top = 54 * Form1.ActiveForm.Height / 500; textBox1.Width = 250 * Form1.ActiveForm.Width / 500; textBox1.Height = 200 * Form1.ActiveForm.Height / 500;
            textBox2.Left = 210 * Form1.ActiveForm.Width / 500; textBox2.Top = 54 * Form1.ActiveForm.Height / 500; textBox2.Width = 250 * Form1.ActiveForm.Width / 500; textBox2.Height = 200 * Form1.ActiveForm.Height / 500;
            progressBar1.Left = 2 * Form1.ActiveForm.Width / 500; progressBar1.Top = 450 * Form1.ActiveForm.Height / 500; progressBar1.Width = 480 * Form1.ActiveForm.Width / 500; progressBar1.Height = 10 * Form1.ActiveForm.Height / 500;
            
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (firstin > j)
            {
                label1.Text = "";
            }
            firstin++;
            switch (comboBox4.SelectedIndex)
            {
                case 0:
                    comboBox3.Visible = false;
                    listBox2.Visible = false;
                    textBox2.Visible = false;
                    comboBox1.Visible = false;
                    button5.Visible = false;
                    comboBox2.Visible = true;
                    listBox1.Visible = true;
                    textBox1.Visible = true;
                    button1.Visible = true;
                    button2.Visible = true;
                    button3.Visible = true;
                    button4.Visible = true;
                    button6.Visible = true;
                    break;
                case 1:
                    comboBox3.Visible = true;
                    listBox2.Visible = true;
                    textBox2.Visible = true;
                    comboBox1.Visible = true;
                    button5.Visible = true;
                    comboBox2.Visible = false;
                    listBox1.Visible = false;
                    textBox1.Visible = false;
                    button1.Visible = false;
                    button2.Visible = false;
                    button3.Visible = false;
                    button4.Visible = false;
                    button6.Visible = false;
                    break;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (firstin > j)
            {
                label1.Text = "";
            }
            firstin++;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            Form2 form2 = new Form2();
            form2.ShowDialog();
            if (form2.closed)
            {
                StreamReader readftpinf = new StreamReader("Libraries.txtt");
                List<string> str = new List<string>();
                str.AddRange(readftpinf.ReadLine().Split('&'));
                site = str[0];
                str.Clear();
                readftpinf.Close();
                if (form2.changed)
                {
                    label1.Text = "Сообщение";
                    try
                    {
                        httpv.Download("Date.txtt", site, ref msgtoreport);
                        //ftpv.Download("Date.txtt", site, login, password, ref msgtoreport);
                        if (msgtoreport != "") label1.Text = msgtoreport;
                    }
                    catch (Exception nn)
                    { label1.Text = "Не удалось подключиться к серверу";/*MessageBox.Show(nn.ToString());*/ }
                        label1.Text = "Скачивание нового Content.xml";
                        try
                        {
                            httpv.Download(content, site, ref msgtoreport);
                            //ftpv.Download(content, site, login, password, ref msgtoreport);
                            ren.del("DateSaved.txtt", ref msgtoreport);
                            ren.ren("Date", "DateSaved", ".txtt", ref msgtoreport);
                        }
                        catch (Exception nn)
                        { label1.Text = "Не удалось подключиться к серверу";/*MessageBox.Show(nn.ToString());*/ }
                        if (msgtoreport != "") label1.Text = msgtoreport;
                    
                BooksList.Clear(); SeriesList.Clear(); listBox1.Items.Clear(); listBox2.Items.Clear();
                    serial.Read(ref BooksList, ref SeriesList, content);
                    foreach (var b in BooksList)
                        listBox1.Items.Add(b.Name + " " + b.Author + " " + b.Genre);
                    foreach (var s in SeriesList)
                        listBox2.Items.Add(s.Name + " " + s.Author + " " + s.Genre);
                }

            }
        }

    }
}
