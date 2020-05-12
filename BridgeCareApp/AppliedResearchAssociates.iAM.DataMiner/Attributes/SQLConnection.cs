namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class SQLConnection
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public string DataSource { get; set; }

        public SQLConnection(string userName, string password, string server, string dataSource)
        {
            UserName = userName;
            Password = password;
            Server = server;
            DataSource = dataSource;
        }
    }
}
