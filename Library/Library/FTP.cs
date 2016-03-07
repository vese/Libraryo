using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Library
{
    class FTP
    {
        public void Download(string filename, string site, string login, string pass, ref string error)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + site + "/Books/" + filename);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(login, pass);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream,Encoding.UTF8);
            StreamWriter writ = new StreamWriter(filename);
            writ.Write(reader.ReadToEnd());
            writ.Close();

            error = "Файл загружен с сайта! "/* + response.StatusDescription*/;

            reader.Close();
            response.Close(); 
        }

    }
}
