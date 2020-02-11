using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ethStorageDecode
{
    public class SolidityUtils
    {
        public static BigInteger getAtOffset(string val, int offset, int size)
        {
            string cleanedHex = val.Replace("0x", "");            
            BigInteger num = BigInteger.Parse( cleanedHex, System.Globalization.NumberStyles.HexNumber);
            
            byte[] bytes = num.ToByteArray();
            byte[] curr = new byte[size];
            for (int i = 0; i < size; i++)
            {
                if(offset+i>bytes.Length-1)
                {
                    curr[i] = 0;
                }
                else
                    curr[i] = bytes[offset + i];
            }
            return new BigInteger(curr);
        }

       
    }
}
