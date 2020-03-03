using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ethStorageDecode
{
    public class KeyDecodeList
    {

        static Dictionary<string, List<BigInteger>> keyNameList = new Dictionary<string, List<BigInteger>>();

        public static void AddKey(string varname, BigInteger key)
        {
            if (!keyNameList.ContainsKey(varname))
            {
                keyNameList[varname] = new  List<BigInteger>();
            }
            keyNameList[varname].Add(key);           
        }

        public static void AddKey(string varname, string keyval)
        {
            keyval = keyval.Replace("0x", "");
            BigInteger val = BigInteger.Parse("0" + keyval, System.Globalization.NumberStyles.HexNumber);
            AddKey(varname, val);
        }

        public static bool Haskeys(string varname)
        {
            return keyNameList.ContainsKey(varname);
        }

        public static List<BigInteger> GetKeys(string varname)
        {
            if(keyNameList.ContainsKey(varname))
            {
                return keyNameList[varname];
            }
            return null;
        }
    }

    public class MultiKeyDecodeList
    {

        static Dictionary<string, List<string>> keyNameList = new Dictionary<string, List<string>>();

        public static void AddKey(string varname, string key)
        {
            if (!keyNameList.ContainsKey(varname))
            {
                keyNameList[varname] = new List<string>();
            }
            keyNameList[varname].Add(key);
        }

       
        public static bool Haskeys(string varname)
        {
            return keyNameList.ContainsKey(varname);
        }

        public static List<string> GetKeys(string varname)
        {
            if (keyNameList.ContainsKey(varname))
            {
                return keyNameList[varname];
            }
            return null;
        }

        public static void ClearAll()
        {
            keyNameList.Clear();
        }
    }
}
