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
using Contracttest.Contracts.testPacking.ContractDefinition;

namespace Contracttest.Contracts.testPacking
{
    public partial class TestPackingService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, TestPackingDeployment testPackingDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<TestPackingDeployment>().SendRequestAndWaitForReceiptAsync(testPackingDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, TestPackingDeployment testPackingDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<TestPackingDeployment>().SendRequestAsync(testPackingDeployment);
        }

        public static async Task<TestPackingService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, TestPackingDeployment testPackingDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, testPackingDeployment, cancellationTokenSource);
            return new TestPackingService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public TestPackingService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }


    }
}
