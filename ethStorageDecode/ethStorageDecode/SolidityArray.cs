﻿using Nethereum.Util;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace ethStorageDecode
{
    public class SolidityArray :SolidityVar
    {
        SolidityVar basevar = null;
        
        public SolidityArray(SolidityVar _baseVar, string _name)
        {
            basevar = _baseVar;
            name = _name;

        }


        public override DecodedContainer DecodeIntoContainer(Web3 web, string address, BigInteger index, int offset)
        {
            if(offset>0)
            {
                throw new NotSupportedException("Error the array should not have an offset");
            }
            string val = getStorageAt(web, address, index);
            //this is the lenth
            int len = Convert.ToInt32(val, 16);
            ethGlobal.DebugPrint("Decoding Arrray " + name+" with length "+len);
            List<string> res = new List<string>();
            //res.Add("(array)" + name + "=");
           
            String str = index.ToString("x64");
            var newkey = new Sha3Keccack().CalculateHashFromHex(str);//pad with zero to prevent BigIntegar prase from making number negative
            ethGlobal.DebugPrint("Array Decoding index  " + str + " hash is " + newkey);
            //var newkey = Web3.Sha3((index).ToString());//pad with zero to prevent BigIntegar prase from making number negative
            BigInteger ind = BigInteger.Parse("0" + newkey, System.Globalization.NumberStyles.HexNumber);//pad with zero to prevent BigIntegar prase from making number negative            
            DecodedContainer cont = new DecodedContainer
            {
                rawValue = val,
                solidityVar = this

            };
            /*for (int i = 0; i < len; i++)
            {
                // var newkey = new Sha3Keccack().CalculateHash((i).ToString());
                //BigInteger ind = new BigInteger(Encoding.ASCII.GetBytes(newkey));                
                var chld = basevar.DecodeIntoContainer(web, address, ind + (i * basevar.getIndexSize()),0);
                chld.key = i.ToString();
                cont.children.Add(chld);
            }*/
            cont.children.AddRange(solidtyDecoder.DecodIntoContainerInstances(basevar, web, address, len, ind, offset));
            return cont;
        }

        public override object Clone()
        {
            SolidityVar basecopy = (SolidityVar)basevar.Clone();

            SolidityArray copy = new SolidityArray(basecopy, name);
            return copy;

        }

        public override int getIndexSize()
        {

            return 1;
        }

        public override int getByteSize()
        {
            return -1; //not supported 
        }

        



        /*  public override int Size()
          {
              throw new NotImplementedException("Array should not support size, only base types should support it, since it is used for a");
          }*/
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

