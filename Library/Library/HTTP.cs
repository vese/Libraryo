using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.ComponentModel;
using System.IO;

namespace Library
{
    class HTTP
    {
        //string fname = "";
        public void Download(string filename, string site, ref string error)
        {
            //fname = filename;
            Form1 f = new Form1();
            //f.downloadingname = filename;
            f.Text = "Библиотека Идет загрузка " + filename;
            f.progressBar1.Visible = true;
            WebClient web = new WebClient();
            web.Encoding = System.Text.Encoding.UTF8;
            web.DownloadFile/*Async*/(new Uri("http://" + site + "/Books/" + filename), filename);
            web.DownloadProgressChanged += DownloadProgressChanged;
            web.DownloadFileCompleted += DownloadFileCompleted;
        }
        public void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Form1 f = new Form1();
            f.progressBar1.Value = e.ProgressPercentage;
        }
        public void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Form1 f = new Form1();
            f.label1.Text = "Скачивание завершено";
            f.progressBar1.Visible = false;
            //f.downloadingname = "";
                f.Text = "Библиотека";
        }
    }
}
