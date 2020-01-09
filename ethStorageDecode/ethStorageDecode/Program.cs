using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Nethereum.ABI;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace ethStorageDecode
{







    public class Program
    {
        //TODO:
        // read json file for settings
        // inheriance.
        // nested maps
        // document
        static void Main(string[] args)
        {
            // StreamReader txt = new StreamReader(@"C:\dlt2\lenderDAO\contracts\contracts\LoanManager.sol");
            StreamReader txt = new StreamReader(@"C:\inuka_proj\gitStorageDecodegit\contracttest\contracts\TestClassSimple.sol");            
             AntlrInputStream inputStream = new AntlrInputStream(txt.ReadToEnd());
            SolidityLexer speakLexer = new SolidityLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(speakLexer);
            SolidityParser solParser = new SolidityParser(commonTokenStream);
            var lst = new SolList();
            solParser.AddParseListener(lst);
            solParser.sourceUnit();
            Web3 connect = new Web3("HTTP://127.0.0.1:7545");
            string address = "0x32525207DbF25168E6Cc2E466b49D254f2f671De";
            // address = "0x"+address.Replace("0x", "0x").ToLower();
            //var newkey = new Sha3Keccack().CalculateHashFromHex("0000000000000000000000000000000000000000000000000000000000000002");
            BigInteger num = new BigInteger(10);
            string hexval = num.ToString("x64");
            var newkey = new Sha3Keccack().CalculateHashFromHex(num.ToString("x64"));
            var newkey2 = new Sha3Keccack().CalculateHashFromHex("2");
            BigInteger ind = BigInteger.Parse("0" + newkey, System.Globalization.NumberStyles.HexNumber);
            var curr = connect.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            curr.Wait();
            var tsk = connect.Eth.GetStorageAt.SendRequestAsync(address, new HexBigInteger(newkey) );
            tsk.Wait();
            string res=  tsk.Result;
            BigInteger index = 0; //starting with 3 because first 3 are the inherited ERC20 cont
            List<DecodedContainer> decodeList = new List<DecodedContainer>();
            KeyDecodeList.AddKey("simpleMap", 4);
            KeyDecodeList.AddKey("simpleMap", 5);
            KeyDecodeList.AddKey("simpleMap", 8);
            KeyDecodeList.AddKey("simpleMap", 10);
            KeyDecodeList.AddKey("structMap", 1);
            KeyDecodeList.AddKey("structMap", 4);
            //complex mapo
            MultiKeyDecodeList.AddKey("complexMap", "1,2");
            MultiKeyDecodeList.AddKey("complexMap", "2,2");

            foreach (SolidityVar var in lst.variableList)
            {
                decodeList.Add(var.DecodeIntoContainer(connect, address, index));
                index++;
            }
            StringBuilder decodedoutput = DecodedContainerTextPrint.print(decodeList);
            Console.WriteLine(decodedoutput.ToString());
            //Console.ReadKey();

            //todo enum
            //solParser.AddParseListener()
            //solParser.Context 
            //solParser.ChatContext chatContext = speakParser.chat();
            //SpeakVisitor visitor = new SpeakVisitor();
            //visitor.Visit(chatContext);




        }
    }




 /*   public abstract class SolidityVar
    {

        public abstract long Lenght();

        public abstract DecodedOutput decode(object obj);

        public abstract DecodedOutput decoded(object obj, object key);

        public abstract bool IsKeyNeeded();
        



        
    }*/

  /*  public class SolidityStruct : SolidityVar
    {
        List<SolidityVar> internalVars;
        long len = 0;

        public SolidityStruct(List<SolidityVar> vars)
        {
            internalVars = vars;
            foreach(var v in internalVars )
            {
                len += v.Lenght();
            }
        }

        public override 


        public override DecodedOutput decode(object obj)
        {
           
        }

        public override DecodedOutput decoded(object obj, object key)
        {
            throw new NotImplementedException();
        }

        public override bool IsKeyNeeded()
        {
            throw new NotImplementedException();
        }

        public override long Lenght()
        {
            return len
        }
    }

    public class SolidityUint : SolidityVar
    {
        string Name { get; }
        long len = 0;

        public SolidityUint(string name)
        {
            Name = name;
        }

        public override


        public override DecodedOutput decode(object obj)
        {

        }

        public override DecodedOutput decoded(object obj, object key)
        {
            throw new NotImplementedException();
        }

        public override bool IsKeyNeeded()
        {
            throw new NotImplementedException();
        }

        public override long Lenght()
        {
            return len
        }
    }



    public class global
    {
        //List<SolidityVar> structDefs;
        //struct regex ($\s*struct(.+?)\{([^\}]+?)}) 
        static Regex structmatch = new Regex(@"($\s*struct(.+?)\{([^\}]+?)}");
        static Regex globalMatch = new Regex(@"((struct.+?\{(.+?)\})|(function.+?\{.+?\})|(?<v1>^\s*(?<v2>\w+)(?<v3>.+?)\;))", RegexOptions.Multiline | RegexOptions.Singleline);
        public static Dictionary<string, SolidityVar> structDefs = new Dictionary<string,SolidityVar>();
        public static List<SolidityVar> VarFactory(string input, Dictionary<string,SolidityVar> currentStructs)
        {
            List<SolidityVar> currentVariables = new List<SolidityVar>();
            MatchCollection coll = globalMatch.Matches(input);
            foreach(Match m in coll) 
            {
                if (m.Groups[1].ToString().Contains("function"))
                    continue;
                if(m.Groups[1].ToString().Contains("struct"))
                {
                    Match m2 = structmatch.Match(m.Groups[1].ToString());
                    currentVariables.Add(DecodeStruct(m2.Groups[3].ToString(), currentStructs));
                }
                if(m.Groups[1].ToString().Contains("uint"))
                {

                }
                else if(m.Groups[1].ToString().Contains("byes32"))
                { 

                }
                
            }
        }

        public static SolidityStruct DecodeStruct(string contents,Dictionary<string,SolidityVar> currentStruct)
        {
            return new SolidityStruct(VarFactory(contents, currentStruct));
        }

    }

    //uint, long, bytes32, struct ,map, array

    public class DecodedOutput
    {
        string val
        public DecodedOutput(string _val)
        {
            val = _val;
        }
        public bool IsSimple()
        {
            return true;
        }

        public string GetDecode()
        {
            return val;
        }

        public string GetDecode(string key)
        {
            throw new NotSupportedException("Simple Decoded output does not have keys");
        }
    }

    public class ComplexeDecodedOutput
    {
        int ind;
        public ComplexeDecodedOutput(int startInd)
        {
            ind = startInd;
        }
        public bool IsSimple()
        {
            return true;
        }

        public string GetDecode()
        {
            throw new NotSupportedException("Must have keys for complex decoded type");
        }
    }

        public string GetDecode(string key)
        {
             return "";
        }
    }

    public class SolidityVariableDecoder
    {

        //Regex expr = new Regex(@".?+(\w+)\s(\w+\s)?(\w+).*;");
       

        public SolidityVariableDecoder(string file)
        {
           MatchCollection matches=  globalMatch.Matches(file);
           List<SolidityVar> varList;
           foreach(Match m in matches)
           {
                varList.Add(SolidityVar.Create(m));
           }
        }



    } */


}

