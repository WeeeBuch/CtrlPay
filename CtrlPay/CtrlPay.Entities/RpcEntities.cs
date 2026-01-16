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
    public class RpcAccount
    {
        public int Account_Index { get; set; }
        public decimal Balance { get; set; }
        public decimal Unlocked_Balance { get; set; }
        public string Label { get; set; }
        public string Tag { get; set; }
        public string Base_Address { get; set; }
    }
}
