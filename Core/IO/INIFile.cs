using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Core.IO
{
    public class INIFile
    {
        public string Path { get; private set; }

        [DllImport("kernel32")]
        static extern long WritePrivateProfileString(string section, string key, string Value, string FilePath);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string section, string key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public INIFile(string path)
        {
            Path = new FileInfo(path + ".ini").FullName.ToString();
        }

        public bool Exists()
        {
            return File.Exists(this.Path);
        }

        public string Read(string section, string key)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public int ReadInt(string section, string key)
        {
            int result = 0;
            try
            {
                result = int.Parse(Read(section, key));
            }
            catch { result = 0; }
            return result;
        }

        public byte ReadByte(string section, string key)
        {
            byte result = 0;
            try
            {
                result = byte.Parse(Read(section, key));
            }
            catch { result = 0; }
            return result;
        }

        public void Write(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, Path);
        }

        public void Write(string section, string key, int value)
        {
            Write(section, key, value.ToString());
        }

        public void Write(string section, string key, byte value)
        {
            Write(section, key, value.ToString());
        }


        public void DeleteKey(string section, string key)
        {
            Write(key, null, section);
        }

        public void DeleteSection(string section)
        {
            Write(section, null, null);
        }

        public bool KeyExists(string section, string key)
        {
            return Read(section, key).Length > 0;
        }
    }
}
