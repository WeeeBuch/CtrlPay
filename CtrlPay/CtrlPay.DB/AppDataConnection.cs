using CtrlPay.Entities;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.AspNet;
using LinqToDB.Configuration; 

namespace CtrlPay.DB
{
    public class AppDataConnection : DataConnection
    {
        public AppDataConnection(DataOptions<AppDataConnection> options)
        : base(options.Options)
        {

        }

        public ITable<User> Users => this.GetTable<User>();
    }
}
