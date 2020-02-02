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

namespace ethStorageCli
{
    class Program
    {

        //TODO: json setting file
        //TODO: inheritance
        //TODO: GUI
        //TODO: associate actual memory storage to memory space mapping in 
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
            if(!File.Exists(inputfile))
            {
                Console.WriteLine("Error the input file does not exist :" + inputfile);
                return;
            }
            searchpath.Add(Path.GetDirectoryName(inputfile));
            RunDecodingAndPrint(inputfile, address, ethURL, searchpath);

        }


        static void RunDecodingAndPrint(string path, string address,string ethURL, List<string> searchpath)
        {
           
            Console.WriteLine(solidtyDecoder.Decode(path,address,ethURL, searchpath));
            Console.ReadKey();
        }

    }
}
