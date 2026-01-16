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
    public class GetAccountsResult
    {
        public List<SubaddressAccount> Subaddress_Accounts { get; set; }
        public ulong Total_Balance { get; set; }
        public ulong Total_Unlocked_Balance { get; set; }
    }
    public class SubaddressAccount
    {
        public uint Account_Index { get; set; }
        public ulong Balance { get; set; }
        public ulong Unlocked_Balance { get; set; }
        public string Label { get; set; }
        public string Tag { get; set; }
    }
}
