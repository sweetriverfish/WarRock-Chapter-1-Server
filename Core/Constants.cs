using System.Text;
using System.Security.Cryptography;

namespace Core {
    public class Constants {
        public const byte Error_OK = 1;
        public const byte xOrKeySend = 0x96;
        public const byte xOrKeyReceive = 0xC3;

        public const byte xOrKeyServerSend = 0x23;
        public const byte xOrKeyServerReceive = 0xA3;

        public const sbyte maxChannelsCount = 3;

        public static bool isAlphaNumeric(string input) {
            System.Text.RegularExpressions.Regex objAlphaNumericPattern = new System.Text.RegularExpressions.Regex("[^a-zA-Z0-9]");
            return !objAlphaNumericPattern.IsMatch(input);
        }

        public static string GenerateSHAHash (string input)
            {
              SHA256 SHA = SHA256.Create();

              byte[] inputBytes = Encoding.ASCII.GetBytes(input);
              byte[] hash       = SHA.ComputeHash(inputBytes);

            StringBuilder stringbuilder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                stringbuilder.Append(hash[i].ToString("x2"));
            }
            return stringbuilder.ToString();

            }           
    
    }
}
