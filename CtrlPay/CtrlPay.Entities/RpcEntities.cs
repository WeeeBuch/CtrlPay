using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    public class RpcResponse<T>
    {
        public string Jsonrpc { get; set; }
        public string Id { get; set; }
        public T Result { get; set; }
        public RpcError Error { get; set; }
    }
    public class RpcError
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
    public class RpcAccountsResult
    {
        public List<RpcAccount> Subaddress_Accounts { get; set; }
        public decimal Total_Balance { get; set; }
        public decimal Total_Unlocked_Balance { get; set; }
    }
    public class RpcTransfersResult
    {
        public List<RpcTransfer> In { get; set; }
        public List<RpcTransfer> Out { get; set; }
    }
    public class RpcAccount
    {
        public int Account_Index { get; set; }
        public decimal Balance { get; set; }
        public decimal Unlocked_Balance { get; set; }
        public string Label { get; set; }
        public string Tag { get; set; }
        public string Base_Address { get; set; }
    }

    public class RpcTransfer
    {
        public string Address { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public long Timestamp { get; set; }
        public bool Locked { get; set; }
        public string Txid { get; set; }
        public string Type { get; set; }
        public SubaddrIndex Subaddr_Index { get; set; }
    }
    public class SubaddrIndex
    {
        public int Major { get; set; }
        public int Minor { get; set; }
    }
}
