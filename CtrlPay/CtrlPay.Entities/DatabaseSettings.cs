namespace CtrlPay.Entities
{
    public class DatabaseSettings
    {
        public string Type { get; set; }           // "MySQL"
        public string ProviderIp { get; set; }
        public string ProviderPort { get; set; }
        public string DbName { get; set; }
        public string DbLogin { get; set; }
        public string DbPassword { get; set; }

    }
}
