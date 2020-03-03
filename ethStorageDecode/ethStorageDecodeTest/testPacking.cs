using Contracttest.Contracts.testPacking.ContractDefinition;
using ethStorageDecode;

using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ethStorageDecodeTest
{
    public class testPacking
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

            var deploymentCallerHandler = web3.Eth.GetContractDeploymentHandler<TestPackingDeployment>();
            var deploymentReceiptCaller = deploymentCallerHandler.SendRequestAndWaitForReceiptAsync();
            deploymentReceiptCaller.Wait();
            testClassSimpleAddr = deploymentReceiptCaller.Result.ContractAddress;
            string relativePath = @"..\..\..\..\..\contracttest\contracts\testPacking.sol";
            Assert.IsTrue(File.Exists(relativePath), "Cannot file testInherit.sol");
            variableList = solidtyDecoder.DecodeIntoContainerList(relativePath, testClassSimpleAddr, ganacheURL, new List<string>(), new Dictionary<string, string>());
            Assert.IsNotNull(variableList, "Variable list is null");
        }

        //Deploy the classes and grab the addresses
        [SetUp]
        public void Setup()
        {
            Web3 web3 = new Web3(new Account(privateKey), (ganacheURL));
            //var testClassSimpleDeployTask =  TestClassSimpleService.DeployContractAsync(web3, new TestClassSimpleDeployment());

            var deploymentCallerHandler = web3.Eth.GetContractDeploymentHandler<TestPackingDeployment>();
            var deploymentReceiptCaller = deploymentCallerHandler.SendRequestAndWaitForReceiptAsync();
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
            MultiKeyDecodeList.AddKey("structMap", "4");
            MultiKeyDecodeList.AddKey("structMap", "1");
            DeployContract();


        }

       
        /*
        
       
        struct SimpleStruct
        {
            uint8 slot0off0;
            uint128 slot0off8;
            uint128 slot1off130;
        }
        uint8[] array8;
        SimpleStruct[] arrayStruct;
        mapping(uint8=>uint8) simpleMap;
    mapping(uint8=>SimpleStruct) structMap; */
        [Test] //uint8 slot0off0;  //8  slot 0
        public void CheckUint0()
        {
            TestUtility.CheckVariable("slot0off0", "19", 0 + startInd, variableList);
        }

        [Test] //slot0off8=21;  //8  slot 0
        public void CheckUint1()
        {
            TestUtility.CheckVariable("slot0off8", "21", 1 + startInd, variableList);
        }

        [Test] //uint128 slot0off40; 
        public void CheckUint2()
        {
            TestUtility.CheckVariable("slot0off40", "54", 2 + startInd, variableList);
        }

        [Test] //uint128 slot1off0;  //not enough room move to slot 3
        public void CheckUint3()
        {
            TestUtility.CheckVariable("slot1off0", "57", 3 + startInd, variableList);
        }

       
        //uint32 slot1off129;   // slot 1 offset of 128 -3
        [Test] 
        public void CheckUint4()
        {
            TestUtility.CheckVariable("slot1off129", "96", 4 + startInd, variableList);
        }

       

        [Test] // uint8[] array8;   5
        public void CheckSimpleUintArray()
        {
            /* uintArray.push(12);
           uintArray.push(14);
           uintArray.push(15); */
            TestUtility.CheckArrayItem(0, TestUtility.CheckValueFunction, variableList[5 + startInd], new TestUtility.CheckValues
            {
                parentName = "array8",
                value = "12"
            });
            TestUtility.CheckArrayItem(1, TestUtility.CheckValueFunction, variableList[5 + startInd], new TestUtility.CheckValues
            {
                parentName = "array8",
                value = "14"
            });
            TestUtility.CheckArrayItem(2, TestUtility.CheckValueFunction, variableList[5 + startInd], new TestUtility.CheckValues
            {
                parentName = "array8",
                value = "15"
            });
        }

        //SimpleStruct[] arrayStruct; 5
        public void CheckArrayStruct()
        {
            TestUtility.CheckArrayItem(0, TestUtility.StructCheckFunction, variableList[6 + startInd], new TestUtility.CheckValues
            {
                parentName = "arrayStruct",
                nameValues = new List<TestUtility.NameValues>
                {
                    {
                        new TestUtility.NameValues{ Name="slot0off0", Value="23"}
                    },
                    {
                        new TestUtility.NameValues { Name="slot0off8", Value="14"}
                    },
                    {
                        new TestUtility.NameValues { Name="slot1off130", Value="25"}
                    }
                }

            });
            TestUtility.CheckArrayItem(1, TestUtility.StructCheckFunction, variableList[6 + startInd], new TestUtility.CheckValues
            {
                parentName = "arrayStruct",
                nameValues = new List<TestUtility.NameValues>
                {
                    {
                        new TestUtility.NameValues{ Name="slot0off0", Value="20"}
                    },
                    {
                        new TestUtility.NameValues { Name="slot0off8", Value="18"}
                    },
                    {
                        new TestUtility.NameValues { Name="slot1off130", Value="22"}
                    }
                }

            });
            TestUtility.CheckArrayItem(2, TestUtility.StructCheckFunction, variableList[6+ startInd], new TestUtility.CheckValues
            {
                parentName = "arrayStruct",
                nameValues = new List<TestUtility.NameValues>
                {
                    {
                        new TestUtility.NameValues{ Name="slot0off0", Value="30"}
                    },
                    {
                        new TestUtility.NameValues { Name="slot0off8", Value="33"}
                    },
                    {
                        new TestUtility.NameValues { Name="slot1off130", Value="34"}
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
            TestUtility.CheckMapItem("4", TestUtility.CheckValueFunction, variableList[7 + startInd], new TestUtility.CheckValues
            {
                parentName = "simpleMap",
                value = "43"
            });
            TestUtility.CheckMapItem("8", TestUtility.CheckValueFunction, variableList[7 + startInd], new TestUtility.CheckValues
            {
                parentName = "simpleMap",
                value = "59"
            });
            TestUtility.CheckMapItem("10", TestUtility.CheckValueFunction, variableList[7 + startInd], new TestUtility.CheckValues
            {
                parentName = "simpleMap",
                value = "42"
            });
            TestUtility.CheckMapItem("12", TestUtility.CheckValueFunction, variableList[7 + startInd], new TestUtility.CheckValues
            {
                parentName = "simpleMap",
                value = "45"
            });
        }


        [Test]// structMap //6
        public void TeststructMap()
        {
            TestUtility.CheckMapItem("4", TestUtility.StructCheckFunction, variableList[8 + startInd], new TestUtility.CheckValues
            {
                parentName = "structMap",
                nameValues = new List<TestUtility.NameValues>
                {
                    {
                        new TestUtility.NameValues { Name = "slot0off0", Value = "14" }
                    },
                    {
                        new TestUtility.NameValues { Name = "slot0off8", Value = "19" }
                    },
                    {
                        new TestUtility.NameValues { Name = "slot1off130", Value = "29" }
                    }
                }
            });
            TestUtility.CheckMapItem("1", TestUtility.StructCheckFunction, variableList[8 + startInd], new TestUtility.CheckValues
            {
                parentName = "structMap",
                nameValues = new List<TestUtility.NameValues>
                {
                    {
                        new TestUtility.NameValues { Name = "slot0off0", Value = "21" }
                    },
                    {
                        new TestUtility.NameValues { Name = "slot0off8", Value = "2" }
                    },
                    {
                        new TestUtility.NameValues { Name = "slot1off130", Value = "231" }
                    }
                }
            });
        }
            
            

    }
    
}
