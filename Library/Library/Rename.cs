using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Library
{
    public class Rename
    {
        public void ren(string oldname, string newname, string type, ref string report)
        {
            string path = Application.StartupPath;
            DirectoryInfo dir = new DirectoryInfo(path);
            report = "Что-то пошло не так!";
            foreach (FileInfo file in dir.GetFiles())
            {
                if (file.Name == oldname + type)
                {
                    File.Move(file.Name, newname + type);
                    report = "Всё готово!";
                    break;
                }
            }
        }
        public void del(string name, ref string report)
        {
            string path = Application.StartupPath;
            DirectoryInfo dir = new DirectoryInfo(path);
            report = "Что-то пошло не так!";
            foreach (FileInfo file in dir.GetFiles())
            {
                if (file.Name == name)
                {
                    File.Delete(name);
                    report = "Всё готово!";
                    break;
                }
            }
        }
    }
}
