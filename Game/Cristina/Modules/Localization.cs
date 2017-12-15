using System;
using System.IO;
using System.Xml.Linq;

namespace Game.Cristina.Modules
{
    public sealed class Localization
    {

        private XDocument LocFile = null;

        public Localization()
        {
            string _locFilePath = Path.Combine(Environment.CurrentDirectory, "Localization.xml");
            LocFile = GetLocFile(_locFilePath);

            if(LocFile == null)
                throw new System.IO.FileNotFoundException("Could not load Localization File... File not found");

        }

     
        private XDocument GetLocFile(string _path)
        {
           XDocument File = XDocument.Load(_path);
            return File;
        }

        public string GetLocMessageFrom(string _stringCode)
        {
            string _result = "STRING " + _stringCode + " NOT FOUND!!!!";

            if(LocFile != null)
            {
                try
                {
                    foreach(XElement Node in LocFile.Element("Localization").Elements("String"))
                    {
                        if (Node.Element("Code").Value == _stringCode)
                        {
                            _result = Node.Element("Message").Value.ToString();
                            break;
                        }
                           
                    }
                }
                catch
                {
                    throw new System.Exception("Could not load Localization nodes");
                }
            }

            return _result;
        }

    }
}
