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

        public static List<DecodedContainer> DecodIntoContainer(List<SolidityVar> variableList, Web3 connect, 
            string address, BigInteger index, int offset = 0, string className="")
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
                    if ( var.getByteSize()>-1 && next.getByteSize()>-1 && (offset + var.getByteSize() + next.getByteSize() < 32))
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
                ethGlobal.DebugPrint(String.Format("[{0}]",i));
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


        public static List<DecodedContainer> DecodeIntoContainerList(string path, string address, string ethURL, List<string> searchpath,
            Dictionary<string, string> multiContracts, string className="")
        {
            StreamReader txt = new StreamReader(path);
            AntlrInputStream inputStream;
            if (String.IsNullOrEmpty(className))
                inputStream = new AntlrInputStream(txt.ReadToEnd());
            else
            {
                if (!multiContracts.ContainsKey(className))
                    throw new KeyNotFoundException("Error the className " + className + " Not in file");
                inputStream = new AntlrInputStream(multiContracts[className]);
            }
            SolidityLexer speakLexer = new SolidityLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(speakLexer);
            SolidityParser solParser = new SolidityParser(commonTokenStream);
            var lst = new SolList(searchpath,multiContracts );
            solParser.AddParseListener(lst);
            solParser.sourceUnit();
            Web3 connect = new Web3(ethURL);
            return DecodIntoContainer(lst.variableList, connect, address, 0);
        }



        public static StringBuilder Decode(string path, string address, string ethURL, List<string> searchpath, Dictionary<string, string> multiContracts, string className="")
        {
            List<DecodedContainer> decodeList = DecodeIntoContainerList(path, address, ethURL, searchpath, multiContracts, className);
            StringBuilder decodedoutput = DecodedContainerTextPrint.print(decodeList);
            return decodedoutput;
        }

        public static SolList DecodeInoSubDecoder(string txtContent, string address, string ethURL, List<string> searchpath, Dictionary<string,string> subcontracts)
        {

            AntlrInputStream inputStream = new AntlrInputStream(txtContent);
            SolidityLexer speakLexer = new SolidityLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(speakLexer);
            SolidityParser solParser = new SolidityParser(commonTokenStream);
            var lst = new SolList(searchpath, subcontracts);
            solParser.AddParseListener(lst);
            solParser.sourceUnit();
            return lst;
        }

        


    }
}
