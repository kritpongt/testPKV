using System;

namespace ConsoleApp_testPKV
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string Key = PKV_MakeKey(2725844759);
            bool Key_Validated = Validator.PKV_CheckKeyChecksum(Key);

            Console.WriteLine("KEYGEN\nLicense Key: " + Key + "\n");
            Console.WriteLine(Key_Validated.ToString());
            Console.ReadKey();
        }
        
        static byte PKV_GetKeyByte(Int64 Seed, byte a, byte b, byte c)
        {
            a = (byte)(a % 25);
            b = (byte)(b % 3);
            if (a % 2 == 0)
            {
                return (byte)(((Seed >> a) & 0xFF) ^ ((Seed >> b) | c));
            }
            else
            {
                return (byte)(((Seed >> a) & 0xFF) ^ ((Seed >> b) & c));
            }
        }

        public static string PKV_GetChecksum(string s)
        {
            short left, right, sum;
            left = 0x56;
            right = 0xAF;

            if(s.Length > 0)
            {
                for(int i = 0; i < s.Length; i++)
                {
                    right += Convert.ToByte(s[i]);
                    if (right > 0xFF)
                    {
                        right -= 0xFF;
                    }
                    left += right;
                    if(left > 0xFF)
                    {
                        left -= 0xFF;
                    }
                }
            }
            sum = (short)((left << 8) + right);
            return sum.ToString("X4");
        }

        static string PKV_MakeKey(Int64 Seed)
        {
            byte[] KeyBytes = new byte[4];
            string result;

            KeyBytes[0] = PKV_GetKeyByte(Seed, 24, 3, 200);
            KeyBytes[1] = PKV_GetKeyByte(Seed, 10, 0, 56);
            KeyBytes[2] = PKV_GetKeyByte(Seed, 1, 2, 91);
            KeyBytes[3] = PKV_GetKeyByte(Seed, 7, 1, 100);
            result = Seed.ToString("X8");
            for(int i = 0; i <= 3; i++)
            {
                result += KeyBytes[i].ToString("X2");
            }
            result += PKV_GetChecksum(result);
            //add "-"
            int n = result.Length - 4;
            while(n > 0)
            {
                result = result.Insert(n, "-");
                n -= 4;
            }
            return result;
        }
    }
    class Validator
    {
        const byte KEY_GOOD = 0;
        const byte KEY_INVALID = 1;
        const byte KEY_BLACKLISTED = 2;
        const byte KEY_PHONY = 3;

        public static bool PKV_CheckKeyChecksum(string key)
        {
            string s, c;

            s = key.Replace(@"-", "").ToUpper();
            if (s.Length != 20)
            {
                Console.WriteLine(s);
                return false;
            }
            else
            {
                c = String.Copy(s.Substring(16));
                s = s.Substring(0, 16);
                Console.WriteLine("Checksum1: "+ c + "\nChecksum2: " + Program.PKV_GetChecksum(s));
                return string.Equals(c, Program.PKV_GetChecksum(s));
            }
        }
    }
}