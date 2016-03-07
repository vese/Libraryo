using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace LibraryManager
{
    class FTP
    {
        public void Upload(string filename, string site, string login, string pass, ref string error/*, string ip, string port*/)
        {
            try
            {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + site + "/Books/" + filename);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.Credentials = new NetworkCredential(login, pass);
            /*request.Proxy = new WebProxy("http://"+ip+":"+port+"/");*/
            StreamReader sourceStream = new StreamReader(filename);
            byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            sourceStream.Close();
            request.ContentLength = fileContents.Length;
                
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            error = "Файл загружен на сайт!"/* + response.StatusDescription*/;

            response.Close();

            }
            catch (WebException wEx)
            {
                error = "WebОшибка " + wEx;
            }
            catch (Exception ex)
            {

                error = "Ошибка " + ex;

            }

        }
        public void Download(string filename, string site, string login, string pass, ref string error/*, string ip, string port*/)
        {
            try
            {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + site + "/Books/" + filename);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(login, pass);
            /*request.Proxy = new WebProxy(ip + ":" + port);*/
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            StreamWriter writ = new StreamWriter(filename);
            writ.Write(reader.ReadToEnd());
            writ.Close();

            error = "Файл загружен с сайта!"/* + response.StatusDescription*/;

            reader.Close();
            response.Close();  
            }
            catch (WebException wEx)
            {
                error = "WebОшибка " + wEx;
            }
            catch (Exception ex)
            {

                error = "Ошибка " + ex;

            }
        }

    }
}
