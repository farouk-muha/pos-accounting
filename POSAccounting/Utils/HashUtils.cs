using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace POSAccounting.Utils
{
    public class HashUtils
    {
        public byte[] GenerateSaltedHash(string password)
        {
            byte[] salt = new byte[58];
            byte[] pass = Encoding.Default.GetBytes(password);

            HashAlgorithm algorithm = new SHA256Managed();

            byte[] plainTextWithSaltBytes =
              new byte[pass.Length + salt.Length];

            for (int i = 0; i < pass.Length; i++)
            {
                plainTextWithSaltBytes[i] = pass[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[pass.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        public bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }

}
