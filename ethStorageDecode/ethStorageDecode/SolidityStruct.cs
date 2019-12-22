using Nethereum.Util;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ethStorageDecode
{
    public class SolidityStruct : SolidityVar, ICloneable
    {
               
        public int _size = -1;
        bool once = false;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (!once)
                    name = value;
                once = true;
            }
        }
        List<SolidityVar> typesList = new List<SolidityVar>();

        public void AddType(SolidityVar type)
        {
            typesList.Add(type);
        }
       

        public override List<string> Decode(Web3 web, string address, BigInteger index, string key)
        {
            string val = getStorageAt(web, address, index);
            List<string> res = new List<string>();
            res.Add("(struct)" + name + "=");
            //var newkey = new Sha3Keccack().CalculateHash((i).ToString());
            for (int i = 0; i < typesList.Count; i++)
            {
                
                //pasdding 0 to prevent biginteger prase from making number negatives
                //BigInteger ind = BigInteger.Parse("0"+newkey, System.Globalization.NumberStyles.HexNumber);
                res.AddRange(typesList[i].Decode(web, address, i+index, ""));
            }
            return res;

        }

        public override DecodedContainer DecodeIntoContainer(Web3 web, string address, BigInteger index)
        {
            string val = getStorageAt(web, address, index);
            DecodedContainer cont = new DecodedContainer
            {
                rawValue = val,
                solidityVar = this                

            };
            for (int i = 0; i < typesList.Count; i++)
            {

                //pasdding 0 to prevent biginteger prase from making number negatives
                //BigInteger ind = BigInteger.Parse("0"+newkey, System.Globalization.NumberStyles.HexNumber);
                cont.children.Add(typesList[i].DecodeIntoContainer(web, address, i + index));
            }
            return cont;
        }

        public override object Clone()
        {
            SolidityStruct copy = new SolidityStruct();
            copy.Name = name;
            copy.index = index;
            copy.offset = offset;
            foreach(SolidityVar vars in typesList)
            {
                copy.AddType((SolidityVar)vars.Clone());
            }
            return copy;
        }

        public override int getSize()
        {
            if (_size > 0)
                return _size;
            else
            {
                _size = 0;
                foreach (SolidityVar v in typesList)
                {
                    _size += v.getSize();
                }
                return _size;
            }
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

