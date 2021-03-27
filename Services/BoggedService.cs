using System.Net.Http;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Web3;

namespace BoggedPrice.Services
{
    public class BoggedService
    {
        private const string BogBnbContractAddress = "0xb9A8e322aff57556a2CC00c89Fad003a61C5ac41";
        private const string BogContractAddress = "0xd7b729ef857aa773f47d37088a1181bb3fbf0099";

        private readonly Web3 _web3;
        private Contract _contract = null!;

        public BoggedService()
        {
            _web3 = new Web3("https://bsc-dataseed.binance.org/");
        }

        public async Task InitializeAsync()
        {
            _contract = await GetContract(BogBnbContractAddress);
        }

        public async Task<decimal> GetBalance(string address)
        {
            var balance = await _web3.Eth.GetContractQueryHandler<BalanceOfFunction>()
                .QueryAsync<BigInteger>(BogContractAddress, new BalanceOfFunction {Owner = address});

            return Web3.Convert.FromWei(balance, 18);
        }

        public async Task<decimal> GetUsdPrice()
        {
            var tokenSpot = await _contract.GetFunction("getSpotPrice").CallAsync<BigInteger>();
            var bnbSpot = await _contract.GetFunction("getBNBSpotPrice").CallAsync<BigInteger>();

            return 1000000 / (decimal) tokenSpot * (1000000 / (decimal) bnbSpot);
        }

        private async Task<Contract> GetContract(string address)
        {
            string url = $"https://api.bscscan.com/api?module=contract&action=getabi&address={address}&format=raw";

            using var client = new HttpClient();
            string abi = await client.GetStringAsync(url);

            return _web3.Eth.GetContract(abi, address);
        }
    }

    [Function("balanceOf", "uint256")]
    public class BalanceOfFunction : FunctionMessage
    {
        [Parameter("address", "_owner", 1)] public string Owner { get; set; }
    }
}