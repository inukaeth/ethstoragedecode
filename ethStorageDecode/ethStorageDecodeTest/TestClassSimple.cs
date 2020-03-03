using Contracttest.Contracts.testClassSimple;
using Contracttest.Contracts.testClassSimple.ContractDefinition;
using Contracttest.Contracts.testInherit;
using Contracttest.Contracts.testInherit.ContractDefinition;
using Contracttest.Contracts.testPacking;
using Contracttest.Contracts.testPacking.ContractDefinition;
using ethStorageDecode;
using Nethereum.Web3;
using Nethereum.RPC;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using Nethereum.Web3.Accounts;
using System.Linq;
using ethStorageDecodeTest;

namespace storageDecodeTest
{
    public class TestClassSimple
    {
        //test config for Ganache.
        //test requires Ganache or the like to deploy the contract and test the decode
        //the constructor of the solidity class currently initializes the variables
        //TODO: Move test init section to to c#
        protected string ganacheURL = "HTTP://127.0.0.1:7545";
        protected string address = "0x7bC695aaccC55f228EF148D5bf6912C05C00E7D2";
        protected string privateKey = "867b369282d5dec76024b1f186fef0bce4198683895beeea9135cb9f36302ee0";
        protected string testClassSimpleAddr;
        protected string testClassInheritAddr;
        protected string testClassPack;
        protected List<DecodedContainer> variableList;
        protected int startInd = 0;

        public virtual void DeployContract()
        {
            Web3 web3 = new Web3(new Account(privateKey), (ganacheURL));
            //var testClassSimpleDeployTask =  TestClassSimpleService.DeployContractAsync(web3, new TestClassSimpleDeployment());

            var deploymentCallerHandler = web3.Eth.GetContractDeploymentHandler<TestClassSimpleDeployment>();
            var deploymentReceiptCaller = deploymentCallerHandler.SendRequestAndWaitForReceiptAsync();
            deploymentReceiptCaller.Wait();
            testClassSimpleAddr = deploymentReceiptCaller.Result.ContractAddress;            
            string relativePath = @"..\..\..\..\..\contracttest\contracts\TestClassSimple.sol";
            Assert.IsTrue(File.Exists(relativePath), "Cannot file testInherit.sol");
            variableList = solidtyDecoder.DecodeIntoContainerList(relativePath, testClassSimpleAddr, ganacheURL, new List<string>(), new Dictionary<string, string>());
            Assert.IsNotNull(variableList, "Variable list is null");
        }

        //Deploy the classes and grab the addresses
        [SetUp]
        public void Setup()
        {
           Web3 web3 = new Web3(new Account(privateKey),( ganacheURL));
            //var testClassSimpleDeployTask =  TestClassSimpleService.DeployContractAsync(web3, new TestClassSimpleDeployment());
           
            var deploymentCallerHandler = web3.Eth.GetContractDeploymentHandler<TestClassSimpleDeployment>();
            var deploymentReceiptCaller =  deploymentCallerHandler.SendRequestAndWaitForReceiptAsync();
            deploymentReceiptCaller.Wait();
            testClassSimpleAddr = deploymentReceiptCaller.Result.ContractAddress;
          
            MultiKeyDecodeList.ClearAll();
            /*  Json content if it was in the config file
             *     "simpleMap": [ "4", "8", "10","12" ],
                    "complexMap": [ "1,2", "2,2" ],
                    "structMap": [ "1", "4" ] */
            MultiKeyDecodeList.AddKey("simpleMap", "4");
            MultiKeyDecodeList.AddKey("simpleMap", "8");
            MultiKeyDecodeList.AddKey("simpleMap", "10");
            MultiKeyDecodeList.AddKey("simpleMap", "12");
            MultiKeyDecodeList.AddKey("complexMap", "1,2");
            MultiKeyDecodeList.AddKey("complexMap", "2,2");
            MultiKeyDecodeList.AddKey("structMap", "4");
            MultiKeyDecodeList.AddKey("structMap", "1");
            DeployContract();


        }

        [Test] //0-uint testUint;
        public void CheckUint()
        {
            TestUtility.CheckVariable("testUint", "8", 0 + startInd, variableList);
        }

        [Test] //1- testString="test string ";
        public void CheckString()
        {
            TestUtility.CheckVariable("testString", "test string ", 1 + startInd, variableList);
        }

        [Test] //2- uint[] uintArray;
        public void CheckSimpleUintArray()
        {
            /* uintArray.push(12);
           uintArray.push(14);
           uintArray.push(15); */
            TestUtility.CheckArrayItem(0, TestUtility.CheckValueFunction, variableList[2 + startInd], new TestUtility.CheckValues
            {
                parentName = "uintArray",
                value = "12"
            });
            TestUtility.CheckArrayItem(1, TestUtility.CheckValueFunction, variableList[2 + startInd], new TestUtility.CheckValues
            {
                parentName = "uintArray",
                value = "14"
            });
            TestUtility.CheckArrayItem(2, TestUtility.CheckValueFunction, variableList[2 + startInd], new TestUtility.CheckValues
            {
                parentName = "uintArray",
                value = "15"
            });
        }


        [Test]//3 string[] stringArray;
        public void CheckStringArray()
        {
            /*
           *         stringArray.push("test1");
      stringArray.push("rain");
      stringArray.push("in");
      stringArray.push("stays mainly");
      stringArray.push("in the plains"); 
          */
            TestUtility.CheckArrayItem(0, TestUtility.CheckValueFunction, variableList[3+ startInd], new TestUtility.CheckValues
            {
                parentName = "stringArray",
                value = "test1"
            });
            TestUtility.CheckArrayItem(1, TestUtility.CheckValueFunction, variableList[3+ startInd], new TestUtility.CheckValues
            {
                parentName = "stringArray",
                value = "rain"
            });
            TestUtility.CheckArrayItem(2, TestUtility.CheckValueFunction, variableList[3+ startInd], new TestUtility.CheckValues
            {
                parentName = "stringArray",
                value = "in"
            });
            TestUtility.CheckArrayItem(3, TestUtility.CheckValueFunction, variableList[3+ startInd], new TestUtility.CheckValues
            {
                parentName = "stringArray",
                value = "stays mainly"
            });
            TestUtility.CheckArrayItem(4, TestUtility.CheckValueFunction, variableList[3+ startInd], new TestUtility.CheckValues
            {
                parentName = "stringArray",
                value = "in the plains"
            });
        }

        

        [Test] ////4- string[] stringArray2; //Skipped second string at 4
        //SimpleStruct[] arrayStruct; 5
        public void TestArrayStruct()
        {
            TestUtility.CheckArrayItem(0, TestUtility.StructCheckFunction, variableList[5+ startInd], new TestUtility.CheckValues
            {
                parentName = "SimpleStruct",
                nameValues = new List<TestUtility.NameValues>
                {
                   {
                        new TestUtility.NameValues{ Name="simpleStructVal", Value="10"}
                    },
                    {
                        new TestUtility.NameValues { Name="simpleStructString", Value="Test struct 1 in array"}
                    }
                }
            });
            TestUtility.CheckArrayItem(1, TestUtility.StructCheckFunction, variableList[5+ startInd], new TestUtility.CheckValues
            {
                parentName = "SimpleStruct",
                nameValues = new List<TestUtility.NameValues>
                {
                   {
                        new TestUtility.NameValues{ Name="simpleStructVal", Value="20"}
                    },
                    {
                        new TestUtility.NameValues { Name="simpleStructString", Value="Test struct 2 in array"}
                    }
                }
            });
            TestUtility.CheckArrayItem(2, TestUtility.StructCheckFunction, variableList[5+ startInd], new TestUtility.CheckValues
            {
                parentName = "SimpleStruct",
                nameValues = new List<TestUtility.NameValues>
                {
                   {
                        new TestUtility.NameValues{ Name="simpleStructVal", Value="30"}
                    },
                    {
                        new TestUtility.NameValues { Name="simpleStructString", Value="Test struct 3 in array"}
                    }
                }
            });
        }

        [Test]// mapping(uint=>uint) simpleMap; //6
        public void TestSimpleMap()
        {
            /*
            *         simpleMap[4] = 42;
                simpleMap[8] = 42;
                simpleMap[10] = 42;*/
            TestUtility.CheckMapItem("4", TestUtility.CheckValueFunction, variableList[6+ startInd], new TestUtility.CheckValues
            {
                parentName = "simpleMap",
                value = "42"
            });
            TestUtility.CheckMapItem("8", TestUtility.CheckValueFunction, variableList[6+ startInd], new TestUtility.CheckValues
            {
                parentName = "simpleMap",
                value = "42"
            });
            TestUtility.CheckMapItem("10", TestUtility.CheckValueFunction, variableList[6+ startInd], new TestUtility.CheckValues
            {
                parentName = "simpleMap",
                value = "42"
            });
        }


        [Test] //mapping(uint=>SimpleStruct) structMap; //7
        public void TestStructMap()
        {
            TestUtility.CheckMapItem("4", TestUtility.StructCheckFunction, variableList[7+ startInd], new TestUtility.CheckValues
            {
                parentName = "structMap",
                nameValues = new List<TestUtility.NameValues>
                {
                    {
                        new TestUtility.NameValues{ Name="simpleStructVal", Value="14"}
                    },
                    {
                        new TestUtility.NameValues { Name="simpleStructString", Value="mapping struct 1"}
                    }
                }

            });
            TestUtility.CheckMapItem("1", TestUtility.StructCheckFunction, variableList[7+ startInd], new TestUtility.CheckValues
            {
                parentName = "structMap",
                nameValues = new List<TestUtility.NameValues>
                {
                    {
                        new TestUtility.NameValues{ Name="simpleStructVal", Value="21"}
                    },
                    {
                        new TestUtility.NameValues { Name="simpleStructString", Value="mapping struct 2"}
                    }
                }

            });
        }

        [Test] //mapping(uint=>mapping(uint=>SimpleStruct)) complexMap; //8
        public void TestComplexMap()
        {
            /*
            complexMap[1][2] = SimpleStruct(12, "complex struct 1 2");
            complexMap[2][2] = SimpleStruct(22, "complex struct 2 2"); */
            TestUtility.CheckMapItem("1,2", TestUtility.StructCheckFunction, variableList[8+ startInd], new TestUtility.CheckValues
            {
                parentName = "complexMap",
                nameValues = new List<TestUtility.NameValues>
                {
                    {
                        new TestUtility.NameValues{ Name="simpleStructVal", Value="12"}
                    },
                    {
                        new TestUtility.NameValues { Name="simpleStructString", Value="complex struct 1 2"}
                    }
                }

            });
            TestUtility.CheckMapItem("2,2", TestUtility.StructCheckFunction, variableList[8+ startInd], new TestUtility.CheckValues
            {
                parentName = "complexMap",
                nameValues = new List<TestUtility.NameValues>
                {
                    {
                        new TestUtility.NameValues{ Name="simpleStructVal", Value="22"}
                    },
                    {
                        new TestUtility.NameValues { Name="simpleStructString", Value="complex struct 2 2"}
                    }
                }

            });
        }
    }





            
    
}