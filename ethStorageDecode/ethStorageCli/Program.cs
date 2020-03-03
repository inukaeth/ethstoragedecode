using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.Binder;
using ethStorageDecode;
using Antlr4.Runtime;
using Nethereum.Web3;
using System.Numerics;
using System.Text.RegularExpressions;

namespace ethStorageCli
{
    class Program
    {

        
        //TODO: inheritance
        //TODO: GUI
        //TODO: associate actual memory storage to memory space mapping in ??
        //TODO: handles compaction of smaller types
        //TODO: types byte int
        //TODO: test array of vars less than 256bytes
        //TODO: test array of structs. 1. with same size, with varying size
        //TODO: add struct in the middle of variables and replicate issue that was causing antlr to hang
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
              .AddJsonFile("settings.json", true, true)
              .Build();

            /*"searchpath": [ "C:\\inuka_proj\\gitStorageDecodegit\\contracttest\\contracts" ],
               "inputfile": "C:\\inuka_proj\\gitStorageDecodegit\\contracttest\\contracts\\TestClassSimple.sol",
               "MapVariables": {
                    "simpleMap": [ "4", "5", "6", "7" ],
                    "var2": [ "7", "8", "9" ]
            }*/
            List<string> searchpath = config.GetSection("searchpath").Get<List<string>>();
            String inputfile = config.GetSection("inputfile").Value;
            var varsection = config.GetSection("MapVariables").GetChildren();
            foreach(var chld in varsection)
            {
                List<string> keys= chld.Get<List<string>>();
                foreach (string ky in keys)
                    MultiKeyDecodeList.AddKey(chld.Key, ky);
                //SolidityMap.
            }
            string address = config["address"];
            string ethURL = config["ethURL"];
            string className = config["className"]; //for the case where multiple contracts are in the same file.
            if (!File.Exists(inputfile))
            {
                Console.WriteLine("Error the input file does not exist :" + inputfile);
                return;
            }
            searchpath.Add(Path.GetDirectoryName(inputfile));
            Dictionary<string, string> subContracts = SplitContractFiles(inputfile);
            if(subContracts.Keys.Count>1 && String.IsNullOrEmpty(className))
            {
                Console.WriteLine("Error class name needs to be provied for file containing multiple contracts");
                return;
            }
            RunDecodingAndPrint(inputfile, address, ethURL, searchpath, subContracts, className);

        }


        static void RunDecodingAndPrint(string path, string address,string ethURL, List<string> searchpath, Dictionary<string,string> multiContracts, string className="")
        {
           
            Console.WriteLine(solidtyDecoder.Decode(path,address,ethURL, searchpath, multiContracts, className));
            Console.WriteLine("----Processing Complete---");
            Console.ReadKey();
        }

        static Dictionary<string,string> SplitContractFiles(string fname)
        {
            StreamReader reader = new StreamReader(fname);
            string inputfile = reader.ReadToEnd();
            List<string> contracts = SplitContracts(inputfile);
            Regex reg = new Regex(@"contract\s(\w +)\s", RegexOptions.IgnorePatternWhitespace);
            if (contracts.Count > 0)
            {
                Dictionary<string, string> store = new Dictionary<string, string>();
                foreach(string contractString in contracts)
                {
                    Match m = reg.Match(contractString);
                    if(m.Success)
                        store.Add(m.Groups[1].ToString(), contractString);
                }
                return store;

            }
            else
                return null;
        }

        static List<string> SplitContracts(string fileContents)
        {
            int startInd = 0;
            int endInd = fileContents.IndexOf("contract", startInd);
            bool first = true;
            //skip the first one
            List<string> contracts = new List<string>();
            string contractString = "contract";
            while(true)
            {
                if (first)
                {
                    endInd = fileContents.IndexOf(contractString, endInd+contractString.Length);
                    first = false;
                }
                else
                    endInd = fileContents.IndexOf(contractString, startInd+contractString.Length+1)-1;
                if(endInd>0)
                {
                    string substring = fileContents.Substring(startInd, endInd-startInd);
                    contracts.Add(substring);
                    startInd = fileContents.IndexOf("contract", endInd);
                   
                }
                else
                {
                    string substring = fileContents.Substring(startInd);
                    contracts.Add(substring);                   
                    break;
                }
            }
            return contracts;
            
        }

    }
}
