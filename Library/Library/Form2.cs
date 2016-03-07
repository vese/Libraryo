using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Library
{
    public partial class Form2 : Form
    {
        public bool closed = false;
        public bool changed = false;
        List<string> site = new List<string>();
        public Form2()
        {
            InitializeComponent();
            StreamReader readftpinf = new StreamReader("Libraries.txtt");
            List<string> str = new List<string>();
            str.AddRange(readftpinf.ReadToEnd().Split('&'));
            for (int i = 0; i < str.Count; i++)
            {
                site.Add(str[i]);
            }
            str.Clear();
            readftpinf.Close();

            comboBox1.Items.Clear();
            foreach (string s in site)
            comboBox1.Items.Add(s);
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            closed = true;
            Close();
        }

        private void Form2_SizeChanged(object sender, EventArgs e)
        {
            button1.Left = 106 * Form2.ActiveForm.Width / 300; button1.Top = 180 * Form2.ActiveForm.Height / 220; button1.Width = 75 * Form2.ActiveForm.Width / 300; button1.Height = 23 * Form2.ActiveForm.Height / 220;
            button2.Left = 197 * Form2.ActiveForm.Width / 300; button2.Top = 36 * Form2.ActiveForm.Height / 220; button2.Width = 75 * Form2.ActiveForm.Width / 300; button2.Height = 23 * Form2.ActiveForm.Height / 220;
            button3.Left = 197 * Form2.ActiveForm.Width / 300; button3.Top = 97 * Form2.ActiveForm.Height / 220; button3.Width = 75 * Form2.ActiveForm.Width / 300; button3.Height = 23 * Form2.ActiveForm.Height / 220;
            button4.Left = 46 * Form2.ActiveForm.Width / 300; button4.Top = 150 * Form2.ActiveForm.Height / 220; button4.Width = 200 * Form2.ActiveForm.Width / 300; button4.Height = 23 * Form2.ActiveForm.Height / 220;
            textBox1.Left = 12 * Form2.ActiveForm.Width / 300; textBox1.Top = 100 * Form2.ActiveForm.Height / 220; textBox1.Width = 169 * Form2.ActiveForm.Width / 300; textBox1.Height = 20 * Form2.ActiveForm.Height / 220;
            comboBox1.Left = 12 * Form2.ActiveForm.Width / 300; comboBox1.Top = 36 * Form2.ActiveForm.Height / 220; comboBox1.Width = 169 * Form2.ActiveForm.Width / 300; comboBox1.Height = 21 * Form2.ActiveForm.Height / 220;
            label1.Left = 12 * Form2.ActiveForm.Width / 300; label1.Top = 20 * Form2.ActiveForm.Height / 220; label1.Width = 112 * Form2.ActiveForm.Width / 300; label1.Height = 13 * Form2.ActiveForm.Height / 220;
            label2.Left = 12 * Form2.ActiveForm.Width / 300; label2.Top = 81 * Form2.ActiveForm.Height / 220; label2.Width = 152 * Form2.ActiveForm.Width / 300; label2.Height = 13 * Form2.ActiveForm.Height / 220;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            site.RemoveAt(comboBox1.SelectedIndex);
            StreamWriter wr = new StreamWriter("Libraries.txtt");
            if (site.Count > 0)
            {
                wr.Write(site[0]);
                if (site.Count > 1)
                    for (int g = 1; g < site.Count; g++)
                        wr.Write("&" + site[g]);
                wr.Close();
                comboBox1.Items.Clear();
                foreach (string s in site)
                    comboBox1.Items.Add(s);
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                comboBox1.Items.Clear();
                comboBox1.Items.Add("libraryo.esy.es");
                wr.Write("libraryo.esy.es");
                wr.Close();
                site.Add("libraryo.esy.es");
                comboBox1.SelectedIndex = 0;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StreamReader re = new StreamReader("Libraries.txtt");
            string str = re.ReadToEnd();
            re.Close();
            StreamWriter wr = new StreamWriter("Libraries.txtt");
            wr.Write(str + "&" + textBox1.Text);
            wr.Close();
            site.Add(textBox1.Text);
            comboBox1.Items.Clear();
            foreach (string s in site)
                comboBox1.Items.Add(s);
            comboBox1.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StreamWriter wr = new StreamWriter("Libraries.txtt");
            wr.Write(site[comboBox1.SelectedIndex]);
                for (int g = 0; g < site.Count; g++)
                    if (comboBox1.SelectedIndex != g)
                    wr.Write("&" + site[g]);
            wr.Close();
            closed = true;
            changed = true;
            Close();
        }

    }
}
