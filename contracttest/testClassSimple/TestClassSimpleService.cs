using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using Contracttest.Contracts.testClassSimple.ContractDefinition;

namespace Contracttest.Contracts.testClassSimple
{
    public partial class TestClassSimpleService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, TestClassSimpleDeployment testClassSimpleDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<TestClassSimpleDeployment>().SendRequestAndWaitForReceiptAsync(testClassSimpleDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, TestClassSimpleDeployment testClassSimpleDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<TestClassSimpleDeployment>().SendRequestAsync(testClassSimpleDeployment);
        }

        public static async Task<TestClassSimpleService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, TestClassSimpleDeployment testClassSimpleDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, testClassSimpleDeployment, cancellationTokenSource);
            return new TestClassSimpleService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public TestClassSimpleService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }


    }
}
