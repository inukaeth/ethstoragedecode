
using Contracttest.Contracts.testInherit.ContractDefinition;
using ethStorageDecode;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using NUnit.Framework;
using storageDecodeTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ethStorageDecodeTest
{
    public class TestInherit : TestClassSimple
    {
        
        public override void DeployContract()
        {            
            Web3 web3 = new Web3(new Account(privateKey), (ganacheURL));
            //var testClassSimpleDeployTask =  TestClassSimpleService.DeployContractAsync(web3, new TestClassSimpleDeployment());

            var deploymentCallerHandler = web3.Eth.GetContractDeploymentHandler<TestInheritDeployment>();
            var deploymentReceiptCaller = deploymentCallerHandler.SendRequestAndWaitForReceiptAsync();
            deploymentReceiptCaller.Wait();
            testClassSimpleAddr = deploymentReceiptCaller.Result.ContractAddress;
            string relativePath = @"..\..\..\..\..\contracttest\contracts\testInherit.sol";
            Assert.IsTrue(File.Exists(relativePath), "Cannot file testInherit.sol");
            List<string> searchPath = new List<string>();
            searchPath.Add(@"..\..\..\..\..\contracttest\contracts\");
            variableList = solidtyDecoder.DecodeIntoContainerList(relativePath, testClassSimpleAddr, ganacheURL, searchPath, new Dictionary<string, string>());
            Assert.IsNotNull(variableList, "Variable list is null");
        }

        [Test]
        public void TestMainUint()
        {
            TestUtility.CheckVariable("mainclass", "52", 9 + startInd, variableList);
        }

    }
}
