using System;
using System.Collections.Generic;
using System.Text;

namespace ethStorageDecode
{
    public class ethGlobal
    {
        public static bool IsDebug = false;
        public static void DebugPrint(string msg)
        {
            if (IsDebug)
                Console.WriteLine(msg);
        }
    }
}
