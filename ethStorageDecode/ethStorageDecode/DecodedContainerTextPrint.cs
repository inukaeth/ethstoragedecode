using System;
using System.Collections.Generic;
using System.Text;

namespace ethStorageDecode
{
    public class DecodedContainerTextPrint
    {
        public static StringBuilder print(List<DecodedContainer> results,int spacesize=3)
        {
            StringBuilder output = new StringBuilder();
            foreach (var decd in results)
            {
                output.Append(printDecoded(decd,0,spacesize));
            }
            return output;
        }


        /*
         * vn1 = "somethting"
         * vn2 = 0x1200
         * map 
         *   [key1] struct demo1
         *      array
         *         [0]
         *         [1]
         *     uint a =b 
         *     uint c= 3
         *   [key2] */

        public static StringBuilder printDecoded(DecodedContainer cont, int depth, int spacesize = 3)
        {
            StringBuilder current = new StringBuilder();
            string keytext = "";
            string spc = "";
            string name = cont.solidityVar.name;
            string decoded = " "+cont.decodedValue;
            string structline = "";
            int depthinc = 1;
            if (!String.IsNullOrEmpty(cont.key))
            {
                keytext = String.Format("[{0}]", cont.key);
                name = "";
                if (cont.solidityVar is SolidityStruct)
                {
                    structline = pad(spacesize * (depth + 1)) + cont.solidityVar.name;
                    depthinc = 2;
                }            
                //decoded = "";
            }
            current.AppendLine(pad(spacesize*depth)+String.Format("{0}{1}{2}", keytext, name, decoded));
            if (!String.IsNullOrEmpty(structline))
                current.AppendLine(structline);
            
            foreach (var child in cont.children)
            {
                current.Append(printDecoded(child, depth + depthinc, spacesize));
            }
            return current;
        }

        private static string pad(int spc)
        {
            return new string(' ', spc);
        }

    }
}
