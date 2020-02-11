using Antlr4.Runtime;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace ethStorageDecode
{
    public class solidtyDecoder
    {

        public static List<DecodedContainer> DecodIntoContainer(List<SolidityVar> variableList, Web3 connect, string address, BigInteger index, int offset = 0)
        {
            List<DecodedContainer> decodeList = new List<DecodedContainer>();           
            
            for(int i=0;i<variableList.Count;i++)
            {                   
                SolidityVar var = variableList[i];
                var currentContainer = var.DecodeIntoContainer(connect, address, index, offset);
                currentContainer.key = i.ToString();
                decodeList.Add(currentContainer);
                if (i+1 < variableList.Count)
                {
                    SolidityVar next = variableList[i + 1];
                    if ( next.getByteSize()>-1 && (offset + var.getByteSize() + next.getByteSize() < 32))
                        offset += var.getByteSize(); 
                    else
                    {
                        index+=var.getIndexSize();
                        offset = 0;
                    }

                }
            }
            return decodeList;
        }

        public static List<DecodedContainer> DecodIntoContainerInstances(SolidityVar var, Web3 connect, string address,int numInstances, BigInteger index, int offset = 0)
        {
            List<DecodedContainer> decodeList = new List<DecodedContainer>();

            for (int i = 0; i < numInstances; i++)
            {
                var currContainer = var.DecodeIntoContainer(connect, address, index, offset);
                currContainer.key = i.ToString();
                decodeList.Add(currContainer);
                if (i + 1 < numInstances)
                {
                    SolidityVar next = var;
                    if (!(next is SolidityStruct)&& next.getByteSize() > -1 && (offset + var.getByteSize() + next.getByteSize() < 32))
                        offset += var.getByteSize();
                    else
                    {
                        index+=var.getIndexSize();
                        offset = 0;
                    }

                }
            }
            return decodeList;
        }






        public static StringBuilder Decode(string path, string address, string ethURL, List<string> searchpath)
        {
            StreamReader txt = new StreamReader(path);
            AntlrInputStream inputStream = new AntlrInputStream(txt.ReadToEnd());
            SolidityLexer speakLexer = new SolidityLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(speakLexer);
            SolidityParser solParser = new SolidityParser(commonTokenStream);
            var lst = new SolList(searchpath,address,ethURL);
            solParser.AddParseListener(lst);
            solParser.sourceUnit();
            Web3 connect = new Web3(ethURL);
            List<DecodedContainer> decodeList = DecodIntoContainer(lst.variableList, connect, address,0);
            StringBuilder decodedoutput = DecodedContainerTextPrint.print(decodeList);
            return decodedoutput;
        }

        public static SolList DecodeInoSubDecoder(string path, string address, string ethURL, List<string> searchpath)
        {
            StreamReader txt = new StreamReader(path);
            AntlrInputStream inputStream = new AntlrInputStream(txt.ReadToEnd());
            SolidityLexer speakLexer = new SolidityLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(speakLexer);
            SolidityParser solParser = new SolidityParser(commonTokenStream);
            var lst = new SolList(searchpath, address, ethURL);
            solParser.AddParseListener(lst);
            solParser.sourceUnit();
            return lst;
        }


    }
}
