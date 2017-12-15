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

        public static string MD5(string Input)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(Input);
            byte[] hash = md5.ComputeHash(inputBytes);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }
    
    }
}
