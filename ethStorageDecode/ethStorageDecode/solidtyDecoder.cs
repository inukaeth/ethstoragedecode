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

            List<DecodedContainer> decodeList = new List<DecodedContainer>();
            BigInteger index = 0;
            foreach (SolidityVar var in lst.variableList)
            {
                decodeList.Add(var.DecodeIntoContainer(connect, address, index));
                index++;
            }
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
