using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    // абстрактный класс загрузка
    abstract class Loading
    {
        public abstract string Load(string s);
    }

    //XML
    class XMLFile : Loading
    {
        public override string Load(string path)
        {
            DOM strat = new DOM();
            Students student = new Students();
            List<Students> result = strat.search(student, path);
            string res = String.Empty;
            foreach (Students s in result)
            {
                res += "Студент: " + s.Name + "\n";
                res += "Факультет: " + s.Faculty + "\n";
                res += "Група: " + s.Group + "\n";
                res += "Дисципліна: " + s.Subject + "\n";
                res += "Оцінка: " + s.Mark + "\n";
                res += "\n\n\n";
            }
            return res;
        }
    }

    //Txt or HTML
    class TxtOrHTMLFile : Loading
    {
        public override string Load(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string s = sr.ReadToEnd();
                    return s;
                }
            }
            catch (Exception ex)
            {
                return "0";
            }
        }
    }


    // класс абстрактной фабрики
    abstract class LoadFileFactory
    {
        public abstract Loading CreateLoading();
    }


    class XMLFactory : LoadFileFactory
    {
        public override Loading CreateLoading()
        {
            return new XMLFile();
        }
    }


    class TxtOrHTMLFactory : LoadFileFactory
    {
        public override Loading CreateLoading()
        {
            return new TxtOrHTMLFile();
        }
    }


    class File
    {
        private Loading file;
        public File(LoadFileFactory factory)
        {
            file = factory.CreateLoading();
            
        }
        public string Run(string s)
        {
            string result = file.Load(s);
            return result;
        }
    }
}
