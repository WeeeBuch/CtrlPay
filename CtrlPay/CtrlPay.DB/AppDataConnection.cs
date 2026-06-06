using LinqToDB;
using LinqToDB.Data;

namespace CtrlPay.DB
{
    public class AppDataConnection : DataConnection
    {
        public AppDataConnection(DataOptions<AppDataConnection> options)
        : base(options.Options)
        {

        }
    }
}
