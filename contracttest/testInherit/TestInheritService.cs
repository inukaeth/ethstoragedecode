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
using Contracttest.Contracts.testInherit.ContractDefinition;

namespace Contracttest.Contracts.testInherit
{
    public partial class TestInheritService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, TestInheritDeployment testInheritDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<TestInheritDeployment>().SendRequestAndWaitForReceiptAsync(testInheritDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, TestInheritDeployment testInheritDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<TestInheritDeployment>().SendRequestAsync(testInheritDeployment);
        }

        public static async Task<TestInheritService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, TestInheritDeployment testInheritDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, testInheritDeployment, cancellationTokenSource);
            return new TestInheritService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public TestInheritService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }


    }
}
