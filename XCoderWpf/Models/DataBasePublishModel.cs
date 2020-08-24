using System;

namespace XCoderWpf.Models
{
    public class DataBasePublishModel
    {

    }

    public class ConnectionStringModel
    {
        public Uri IconSource { get; set; }
        public string Title { get; set; }
        public string Server { get; set; }
        public string ShortName => Server?.Length > 15 ? Server.Substring(0, 15) : Server ?? "";

    }
}
