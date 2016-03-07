using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace LibraryManager
{
    public partial class Form1 : Form
    {
        FTP ftpv = new FTP();
        string content = "Content.xml";
        string msgtoreport = "";

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
        List<string> BooksToFTP = new List<string>();
        string[] Expansions = {".txt",".fb2",".epub",".docx"};
        public struct FTPnf
        {
            public string FTPname;
            public List<string> Boo;
        }
        List<FTPnf> bnf = new List<FTPnf>();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            serial.Readbnf(ref bnf, "bnf.xml");
            if (bnf.Count != 0)
            {
                string mes = "Не хватает файлов!" + Environment.NewLine;
                foreach (var k in bnf)
                {
                    mes = mes + k.FTPname + Environment.NewLine;
                    foreach (var l in k.Boo)
                        mes = mes + "   " + l + Environment.NewLine;
                }
                MessageBox.Show(mes, "Внимание!");
            }
        }

        private void buttonUpload_Click(object sender, EventArgs e)
        {
            labelError.Text = "Загрузка началась!";
            var b = MessageBox.Show("Сохранить текущие данные в Content.xml?", "Предупреждение", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk);
            if (b == DialogResult.Yes)
            {
                serial.Save(BooksList, SeriesList, content);
                labelError.Text = "Content.xml сохранен!";
            }
            if (b != DialogResult.Cancel)
            {
                FileInfo file0 = new FileInfo("Content.xml");
                if (file0.Exists)
                {
                    textBoxSite.Enabled = false;
                    textBoxLogin.Enabled = false;
                    textBoxPassword.Enabled = false;

                    bool hj = false;
                    int indbnf = 0;
                    for (int i = 0; i < bnf.Count; i++)
                        if (bnf[i].FTPname == textBoxSite.Text)
                        {
                            indbnf = i;
                            hj = true;
                            break;
                        }
                    if (!hj)
                    {
                        FTPnf nfftp = new FTPnf();
                        nfftp.Boo = new List<string>();
                        nfftp.FTPname = textBoxSite.Text;
                        bnf.Add(nfftp);
                        indbnf = bnf.Count - 1;
                    }

                    ftpv.Upload(content, textBoxSite.Text, textBoxLogin.Text, textBoxPassword.Text, ref msgtoreport/*,textBox13.Text,textBox14.Text*/);
                    if (msgtoreport != "Файл загружен на сайт!") labelError.Text = "Не удалось подключиться к FTP"; else labelError.Text = msgtoreport;
                    foreach (var k in BooksToFTP)
                        foreach (var l in Expansions)
                        {
                            FileInfo finf = new FileInfo(k + l);
                            if (finf.Exists)
                            {
                                ftpv.Upload(k + l, textBoxSite.Text, textBoxLogin.Text, textBoxPassword.Text, ref msgtoreport/*, textBox13.Text, textBox14.Text*/);
                            }
                            else
                            {
                                bool s = false;
                                foreach (var ss in bnf[indbnf].Boo)
                                    if (ss == k + l)
                                        s = true;
                                if (!s)
                                bnf[indbnf].Boo.Add(k + l);
                            }
                        }
                    if (bnf[indbnf].Boo.Count == 0)
                        bnf.RemoveAt(indbnf);

                    if (bnf.Count != 0)
                    {
                        int z1 = bnf.Count-1;
                        for (int z = z1; z >= 0; z--)
                        {
                            if (bnf[z].FTPname == textBoxSite.Text)
                            {
                                int zz1 = bnf[z].Boo.Count-1;
                                for (int zz = zz1; zz >= 0; zz--)
                                {
                                     FileInfo finf = new FileInfo(bnf[z].Boo[zz]);
                                     if (finf.Exists)
                                     {
                                         ftpv.Upload(bnf[z].Boo[zz], textBoxSite.Text, textBoxLogin.Text, textBoxPassword.Text, ref msgtoreport);
                                         bnf[z].Boo.RemoveAt(zz);
                                     }   
                                }
                                if (bnf[z].Boo.Count == 0)
                                    bnf.RemoveAt(z);
                            }
                        }
                    }

                    serial.Savebnf(bnf, "bnf.xml");
                    if (bnf.Count != 0)
                    {
                        string mes = "Не хватает файлов!" + Environment.NewLine;
                        foreach (var k in bnf)
                        {
                            mes = mes + k.FTPname + Environment.NewLine;
                            foreach (var l in k.Boo)
                                mes = mes + "   " + l + Environment.NewLine;
                        }
                        MessageBox.Show(mes, "Внимание!");
                    }
                    BooksToFTP.Clear();
                    StreamWriter DateWriter = new StreamWriter("Date.txtt");
                    DateTime Date = DateTime.Now;
                    DateWriter.WriteLine(Date);
                    DateWriter.Close();
                    ftpv.Upload("Date.txtt", textBoxSite.Text, textBoxLogin.Text, textBoxPassword.Text, ref msgtoreport/*, textBox13.Text, textBox14.Text*/);

                    textBoxSite.Enabled = true;
                    textBoxLogin.Enabled = true;
                    textBoxPassword.Enabled = true;
                }
                else
                {
                    labelError.Text = "Файла Content.xml не существует!";
                }
            }
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            labelError.Text = "Сообщение";
            ftpv.Download(content, textBoxSite.Text, textBoxLogin.Text, textBoxPassword.Text, ref msgtoreport/*, textBox13.Text, textBox14.Text*/);
            if (msgtoreport != "") labelError.Text = "Не удалось подключиться к FTP";
        }

        private void buttonSerialize_Click(object sender, EventArgs e)
        {
            serial.Save(BooksList, SeriesList, content);
            labelError.Text = "Content.xml сохранен!";
            //MessageBox.Show(BooksList.Count.ToString());
            //MessageBox.Show(SeriesList.Count.ToString());
        }

        private void buttonDeserialize_Click(object sender, EventArgs e)
        {
            BooksToFTP.Clear();
            serial.Read(ref BooksList, ref SeriesList, content);
            dataGridView1.Rows.Clear();
            foreach (var j in BooksList)
            {
                dataGridView1.Rows.Add(j.Name, j.Author, j.Genre, j.Descr, j.ID, j.SeriesID);
            }
            dataGridView2.Rows.Clear();
            foreach (var j in SeriesList)
            {
                string BooksIDsString = "";
                foreach (var jjj in j.BooksIDs)
                    BooksIDsString += jjj + " ";
                dataGridView2.Rows.Add(j.Name, j.Author, j.Genre, j.Descr, j.ID, BooksIDsString);
            }
            labelError.Text = "Content.xml открыт!";
        }

        private void buttonAddBook_Click(object sender, EventArgs e)
        {
            Books n = new Books();
            n.Name = textBox1.Text;
            n.Author = textBox2.Text;
            n.Genre = textBox3.Text;
            n.Descr = textBox4.Text;
            bool kk = true;
            foreach (var k in textBox5.Text)
                if (!Char.IsDigit(k))
                {
                    kk = false; break;
                }
            if (kk)
            n.SeriesID = Convert.ToInt32(textBox5.Text);
            else
                n.SeriesID = 0;
            n.ID = BooksList.Count + 1;
            BooksList.Add(n);
            //MessageBox.Show(BooksList.Count.ToString()+" "+BooksList[BooksList.Count-1].ID.ToString());
            BooksToFTP.Add(n.ID.ToString());
            labelError.Text = "Книга добавлена!";
            dataGridView1.Rows.Clear();
            foreach (var j in BooksList)
            {
                dataGridView1.Rows.Add(j.Name,j.Author,j.Genre,j.Descr,j.ID,j.SeriesID);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool kk = true;
            foreach (var k in textBox6.Text)
                if (!Char.IsDigit(k))
                {
                    kk = false; break;
                }
            if (kk)
            {
                foreach (var j in BooksList)
                {
                    if (j.ID == Convert.ToInt32(textBox6.Text))
                    {
                        BooksList.Remove(j);
                        Books n = new Books();
                        n.Name = textBox1.Text;
                        n.Author = textBox2.Text;
                        n.Genre = textBox3.Text;
                        n.Descr = textBox4.Text;
                        n.SeriesID = Convert.ToInt32(textBox5.Text);
                        n.ID = Convert.ToInt32(textBox6.Text);
                        BooksList.Add(n);
                        break;
                    }
                }
                dataGridView1.Rows.Clear();
                foreach (var j in BooksList)
                {dataGridView1.Rows.Add(j.Name, j.Author, j.Genre, j.Descr, j.ID, j.SeriesID);}
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Series n = new Series();
            n.Name = textBox12.Text;
            n.Author = textBox11.Text;
            n.Genre = textBox10.Text;
            n.Descr = textBox9.Text;
            string[] d = textBox8.Text.Split(',');
            n.BooksIDs = new List<string>();
            bool isdigit = true;
            foreach (var ke in d)
                if (ke != "")
                {
                    foreach (char kke in ke)
                        if (!Char.IsDigit(kke)) { isdigit = false; break; }
                    if (isdigit)
                        n.BooksIDs.Add(ke);
                    else isdigit = true;
                }
            n.ID = SeriesList.Count + 1;
            SeriesList.Add(n);
            labelError.Text = "Серия добавлена!";
            dataGridView2.Rows.Clear();
            foreach (var j in SeriesList)
            {
                string BooksIDsString = "";
                foreach (var jjj in j.BooksIDs)
                    BooksIDsString += jjj + " ";
                dataGridView2.Rows.Add(j.Name, j.Author, j.Genre, j.Descr, j.ID, BooksIDsString);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool kk = true;
            foreach (var k in textBox7.Text)
                if (!Char.IsDigit(k))
                { kk = false; break;}
            if (kk)
            { foreach (var j in SeriesList)
                { if (j.ID == Convert.ToInt32(textBox7.Text))
                    { SeriesList.Remove(j);
                        Series n = new Series();
                        n.Name = textBox12.Text;
                        n.Author = textBox11.Text;
                        n.Genre = textBox10.Text;
                        n.Descr = textBox9.Text;
                        string[] d = textBox8.Text.Split(',');
                        n.BooksIDs = new List<string>();
                        n.BooksIDs.AddRange(d);
                        n.ID = SeriesList.Count + 1;
                        SeriesList.Add(n);
                        break;
                    }
                }
                dataGridView2.Rows.Clear();
                foreach (var j in SeriesList)
                { string BooksIDsString = "";
                    foreach (var jjj in j.BooksIDs)
                        BooksIDsString += jjj + " ";
                    dataGridView2.Rows.Add(j.Name, j.Author, j.Genre, j.Descr, j.ID, BooksIDsString); }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            BooksToFTP.Clear();
            foreach (var k in BooksList)
            BooksToFTP.Add(k.ID.ToString());
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            button1.Left = 463 * Form1.ActiveForm.Width / 1000; button1.Top = 254 * Form1.ActiveForm.Height / 700; button1.Width = 176 * Form1.ActiveForm.Width / 1000; button1.Height = 23 * Form1.ActiveForm.Height / 700;
            button2.Left = 771 * Form1.ActiveForm.Width / 1000; button2.Top = 251 * Form1.ActiveForm.Height / 700; button2.Width = 176 * Form1.ActiveForm.Width / 1000; button2.Height = 23 * Form1.ActiveForm.Height / 700;
            button3.Left = 771 * Form1.ActiveForm.Width / 1000; button3.Top = 196 * Form1.ActiveForm.Height / 700; button3.Width = 176 * Form1.ActiveForm.Width / 1000; button3.Height = 23 * Form1.ActiveForm.Height / 700;
            button4.Left = 28 * Form1.ActiveForm.Width / 1000; button4.Top = 150 * Form1.ActiveForm.Height / 700; button4.Width = 292 * Form1.ActiveForm.Width / 1000; button4.Height = 23 * Form1.ActiveForm.Height / 700;
            buttonAddBook.Left = 463 * Form1.ActiveForm.Width / 1000; buttonAddBook.Top = 199 * Form1.ActiveForm.Height / 700; buttonAddBook.Width = 176 * Form1.ActiveForm.Width / 1000; buttonAddBook.Height = 23 * Form1.ActiveForm.Height / 700;
            buttonDeserialize.Left = 186 * Form1.ActiveForm.Width / 1000; buttonDeserialize.Top = 208 * Form1.ActiveForm.Height / 700; buttonDeserialize.Width = 135 * Form1.ActiveForm.Width / 1000; buttonDeserialize.Height = 23 * Form1.ActiveForm.Height / 700;
            buttonDownload.Left = 28 * Form1.ActiveForm.Width / 1000; buttonDownload.Top = 208 * Form1.ActiveForm.Height / 700; buttonDownload.Width = 135 * Form1.ActiveForm.Width / 1000; buttonDownload.Height = 23 * Form1.ActiveForm.Height / 700;
            buttonSerialize.Left = 186 * Form1.ActiveForm.Width / 1000; buttonSerialize.Top = 179 * Form1.ActiveForm.Height / 700; buttonSerialize.Width = 135 * Form1.ActiveForm.Width / 1000; buttonSerialize.Height = 23 * Form1.ActiveForm.Height / 700;
            buttonUpload.Left = 28 * Form1.ActiveForm.Width / 1000; buttonUpload.Top = 179 * Form1.ActiveForm.Height / 700; buttonUpload.Width = 135 * Form1.ActiveForm.Width / 1000; buttonUpload.Height = 23 * Form1.ActiveForm.Height / 700;
            dataGridView1.Left = 115 * Form1.ActiveForm.Width / 1000; dataGridView1.Top = 307 * Form1.ActiveForm.Height / 700; dataGridView1.Width = 644 * Form1.ActiveForm.Width / 1000; dataGridView1.Height = 150 * Form1.ActiveForm.Height / 700;
            dataGridView2.Left = 115 * Form1.ActiveForm.Width / 1000; dataGridView2.Top = 475 * Form1.ActiveForm.Height / 700; dataGridView2.Width = 644 * Form1.ActiveForm.Width / 1000; dataGridView2.Height = 150 * Form1.ActiveForm.Height / 700;
            label1.Left = 25 * Form1.ActiveForm.Width / 1000; label1.Top = 15 * Form1.ActiveForm.Height / 700; label1.Width = 31 * Form1.ActiveForm.Width / 1000; label1.Height = 13 * Form1.ActiveForm.Height / 700;
            label2.Left = 25 * Form1.ActiveForm.Width / 1000; label2.Top = 41 * Form1.ActiveForm.Height / 700; label2.Width = 38 * Form1.ActiveForm.Width / 1000; label2.Height = 13 * Form1.ActiveForm.Height / 700;
            label3.Left = 25 * Form1.ActiveForm.Width / 1000; label3.Top = 67 * Form1.ActiveForm.Height / 700; label3.Width = 45 * Form1.ActiveForm.Width / 1000; label3.Height = 13 * Form1.ActiveForm.Height / 700;
            label4.Left = 371 * Form1.ActiveForm.Width / 1000; label4.Top = 17 * Form1.ActiveForm.Height / 700; label4.Width = 89 * Form1.ActiveForm.Width / 1000; label4.Height = 13 * Form1.ActiveForm.Height / 700;
            label5.Left = 371 * Form1.ActiveForm.Width / 1000; label5.Top = 43 * Form1.ActiveForm.Height / 700; label5.Width = 37 * Form1.ActiveForm.Width / 1000; label5.Height = 13 * Form1.ActiveForm.Height / 700;
            label6.Left = 371 * Form1.ActiveForm.Width / 1000; label6.Top = 69 * Form1.ActiveForm.Height / 700; label6.Width = 36 * Form1.ActiveForm.Width / 1000; label6.Height = 13 * Form1.ActiveForm.Height / 700;
            label7.Left = 371 * Form1.ActiveForm.Width / 1000; label7.Top = 95 * Form1.ActiveForm.Height / 700; label7.Width = 57 * Form1.ActiveForm.Width / 1000; label7.Height = 13 * Form1.ActiveForm.Height / 700;
            label8.Left = 371 * Form1.ActiveForm.Width / 1000; label8.Top = 167 * Form1.ActiveForm.Height / 700; label8.Width = 38 * Form1.ActiveForm.Width / 1000; label8.Height = 13 * Form1.ActiveForm.Height / 700;
            label9.Left = 371 * Form1.ActiveForm.Width / 1000; label9.Top = 231 * Form1.ActiveForm.Height / 700; label9.Width = 73 * Form1.ActiveForm.Width / 1000; label9.Height = 13 * Form1.ActiveForm.Height / 700;
            label10.Left = 679 * Form1.ActiveForm.Width / 1000; label10.Top = 228 * Form1.ActiveForm.Height / 700; label10.Width = 74 * Form1.ActiveForm.Width / 1000; label10.Height = 13 * Form1.ActiveForm.Height / 700;
            label11.Left = 679 * Form1.ActiveForm.Width / 1000; label11.Top = 164 * Form1.ActiveForm.Height / 700; label11.Width = 37 * Form1.ActiveForm.Width / 1000; label11.Height = 13 * Form1.ActiveForm.Height / 700;
            label12.Left = 679 * Form1.ActiveForm.Width / 1000; label12.Top = 92 * Form1.ActiveForm.Height / 700; label12.Width = 57 * Form1.ActiveForm.Width / 1000; label12.Height = 13 * Form1.ActiveForm.Height / 700;
            label13.Left = 679 * Form1.ActiveForm.Width / 1000; label13.Top = 66 * Form1.ActiveForm.Height / 700; label13.Width = 36 * Form1.ActiveForm.Width / 1000; label13.Height = 13 * Form1.ActiveForm.Height / 700;
            label14.Left = 679 * Form1.ActiveForm.Width / 1000; label14.Top = 40 * Form1.ActiveForm.Height / 700; label14.Width = 37 * Form1.ActiveForm.Width / 1000; label14.Height = 13 * Form1.ActiveForm.Height / 700;
            label15.Left = 679 * Form1.ActiveForm.Width / 1000; label15.Top = 14 * Form1.ActiveForm.Height / 700; label15.Width = 90 * Form1.ActiveForm.Width / 1000; label15.Height = 13 * Form1.ActiveForm.Height / 700;
            label16.Left = 22 * Form1.ActiveForm.Width / 1000; label16.Top = 307 * Form1.ActiveForm.Height / 700; label16.Width = 37 * Form1.ActiveForm.Width / 1000; label16.Height = 23 * Form1.ActiveForm.Height / 700;
            label17.Left = 22 * Form1.ActiveForm.Width / 1000; label17.Top = 475 * Form1.ActiveForm.Height / 700; label17.Width = 38 * Form1.ActiveForm.Width / 1000; label17.Height = 13 * Form1.ActiveForm.Height / 700;
            label18.Left = 25 * Form1.ActiveForm.Width / 1000; label18.Top = 92 * Form1.ActiveForm.Height / 700; label18.Width = 17 * Form1.ActiveForm.Width / 1000; label18.Height = 13 * Form1.ActiveForm.Height / 700;
            label19.Left = 25 * Form1.ActiveForm.Width / 1000; label19.Top = 117 * Form1.ActiveForm.Height / 700; label19.Width = 32 * Form1.ActiveForm.Width / 1000; label19.Height = 13 * Form1.ActiveForm.Height / 700;
            labelError.Left = 25 * Form1.ActiveForm.Width / 1000; labelError.Top = 254 * Form1.ActiveForm.Height / 700; labelError.Width = 65 * Form1.ActiveForm.Width / 1000; labelError.Height = 13 * Form1.ActiveForm.Height / 700;
            textBox1.Left = 463 * Form1.ActiveForm.Width / 1000; textBox1.Top = 14 * Form1.ActiveForm.Height / 700; textBox1.Width = 176 * Form1.ActiveForm.Width / 1000; textBox1.Height = 20 * Form1.ActiveForm.Height / 700;
            textBox2.Left = 463 * Form1.ActiveForm.Width / 1000; textBox2.Top = 40 * Form1.ActiveForm.Height / 700; textBox2.Width = 176 * Form1.ActiveForm.Width / 1000; textBox2.Height = 20 * Form1.ActiveForm.Height / 700;
            textBox3.Left = 463 * Form1.ActiveForm.Width / 1000; textBox3.Top = 66 * Form1.ActiveForm.Height / 700; textBox3.Width = 176 * Form1.ActiveForm.Width / 1000; textBox3.Height = 20 * Form1.ActiveForm.Height / 700;
            textBox4.Left = 463 * Form1.ActiveForm.Width / 1000; textBox4.Top = 92 * Form1.ActiveForm.Height / 700; textBox4.Width = 176 * Form1.ActiveForm.Width / 1000; textBox4.Height = 63 * Form1.ActiveForm.Height / 700;
            textBox5.Left = 463 * Form1.ActiveForm.Width / 1000; textBox5.Top = 164 * Form1.ActiveForm.Height / 700; textBox5.Width = 176 * Form1.ActiveForm.Width / 1000; textBox5.Height = 20 * Form1.ActiveForm.Height / 700;
            textBox6.Left = 463 * Form1.ActiveForm.Width / 1000; textBox6.Top = 228 * Form1.ActiveForm.Height / 700; textBox6.Width = 176 * Form1.ActiveForm.Width / 1000; textBox6.Height = 20 * Form1.ActiveForm.Height / 700;
            textBox7.Left = 771 * Form1.ActiveForm.Width / 1000; textBox7.Top = 225 * Form1.ActiveForm.Height / 700; textBox7.Width = 176 * Form1.ActiveForm.Width / 1000; textBox7.Height = 20 * Form1.ActiveForm.Height / 700;
            textBox8.Left = 771 * Form1.ActiveForm.Width / 1000; textBox8.Top = 161 * Form1.ActiveForm.Height / 700; textBox8.Width = 176 * Form1.ActiveForm.Width / 1000; textBox8.Height = 20 * Form1.ActiveForm.Height / 700;
            textBox9.Left = 771 * Form1.ActiveForm.Width / 1000; textBox9.Top = 89 * Form1.ActiveForm.Height / 700; textBox9.Width = 176 * Form1.ActiveForm.Width / 1000; textBox9.Height = 63 * Form1.ActiveForm.Height / 700;
            textBox10.Left = 771 * Form1.ActiveForm.Width / 1000; textBox10.Top = 63 * Form1.ActiveForm.Height / 700; textBox10.Width = 176 * Form1.ActiveForm.Width / 1000; textBox10.Height = 20 * Form1.ActiveForm.Height / 700;
            textBox11.Left = 771 * Form1.ActiveForm.Width / 1000; textBox11.Top = 37 * Form1.ActiveForm.Height / 700; textBox11.Width = 176 * Form1.ActiveForm.Width / 1000; textBox11.Height = 20 * Form1.ActiveForm.Height / 700;
            textBox12.Left = 771 * Form1.ActiveForm.Width / 1000; textBox12.Top = 11 * Form1.ActiveForm.Height / 700; textBox12.Width = 176 * Form1.ActiveForm.Width / 1000; textBox12.Height = 20 * Form1.ActiveForm.Height / 700;
            textBox13.Left = 107 * Form1.ActiveForm.Width / 1000; textBox13.Top = 89 * Form1.ActiveForm.Height / 700; textBox13.Width = 100 * Form1.ActiveForm.Width / 1000; textBox13.Height = 20 * Form1.ActiveForm.Height / 700;
            textBox14.Left = 107 * Form1.ActiveForm.Width / 1000; textBox14.Top = 114 * Form1.ActiveForm.Height / 700; textBox14.Width = 100 * Form1.ActiveForm.Width / 1000; textBox14.Height = 20 * Form1.ActiveForm.Height / 700;
            textBoxLogin.Left = 107 * Form1.ActiveForm.Width / 1000; textBoxLogin.Top = 38 * Form1.ActiveForm.Height / 700; textBoxLogin.Width = 100 * Form1.ActiveForm.Width / 1000; textBoxLogin.Height = 20 * Form1.ActiveForm.Height / 700;
            textBoxPassword.Left = 107 * Form1.ActiveForm.Width / 1000; textBoxPassword.Top = 64 * Form1.ActiveForm.Height / 700; textBoxPassword.Width = 100 * Form1.ActiveForm.Width / 1000; textBoxPassword.Height = 20 * Form1.ActiveForm.Height / 700;
            textBoxSite.Left = 107 * Form1.ActiveForm.Width / 1000; textBoxSite.Top = 12 * Form1.ActiveForm.Height / 700; textBoxSite.Width = 100 * Form1.ActiveForm.Width / 1000; textBoxSite.Height = 20 * Form1.ActiveForm.Height / 700;
            
            

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if ((dataGridView1.SelectedRows.Count > 0))
            {
                textBox1.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                textBox2.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                textBox3.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                textBox4.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
                textBox5.Text = dataGridView1.SelectedRows[0].Cells[5].Value.ToString();
            }
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                textBox12.Text = dataGridView2.SelectedRows[0].Cells[0].Value.ToString();
                textBox11.Text = dataGridView2.SelectedRows[0].Cells[1].Value.ToString();
                textBox10.Text = dataGridView2.SelectedRows[0].Cells[2].Value.ToString();
                textBox9.Text = dataGridView2.SelectedRows[0].Cells[3].Value.ToString();
                textBox8.Text = dataGridView2.SelectedRows[0].Cells[5].Value.ToString()[0].ToString();
                if (dataGridView2.SelectedRows[0].Cells[5].Value.ToString().Length > 1)
                for (int i = 2; i < dataGridView2.SelectedRows[0].Cells[5].Value.ToString().Length; i +=2)
                    textBox8.Text = textBox8.Text + "," + dataGridView2.SelectedRows[0].Cells[5].Value.ToString()[i].ToString();
            }
        }

    }
}
