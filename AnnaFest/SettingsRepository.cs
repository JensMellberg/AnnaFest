using System.Net;

namespace AnnaFest
{
    public class SettingsRepository
    {
        private static SettingsRepository instance;
        public static SettingsRepository Instance => instance ?? (instance = new SettingsRepository());

        private IList<IPAddress> blackList = new List<IPAddress>();

        public bool UploadEnabled = true;

        public void BlackListIp(IPAddress ip)
        {
            this.blackList.Add(ip);
        }

        public bool IsIpBlackListed(IPAddress ip) => this.blackList.Contains(ip);
    }
}
