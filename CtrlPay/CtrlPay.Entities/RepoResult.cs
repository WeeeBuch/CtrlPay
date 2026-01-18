using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtrlPay.Entities
{
    public class RepoResult<T>
    {
        public string ErrorCode { get; set; }
        public T Result { get; set; }
    }
}
