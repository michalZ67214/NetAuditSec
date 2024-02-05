using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace NetAuditSec
{
    internal class Net
    {
        static Ping PingSender = null;
        static PingReply PingReply;
        static TcpClient PortScan;

        public Net() { }

        public static bool SendPing(string ip)
        {
            if (Net.PingSender == null) Net.PingSender = new Ping();

            Net.PingReply = Net.PingSender.Send(IPAddress.Parse(ip), 250);

            return Net.PingReply.Status == IPStatus.Success;
        }

        public static bool IsPortOpen(string ip, int port)
        {
            Net.PortScan = new TcpClient();

            try
            {
                Net.PortScan.Connect(IPAddress.Parse(ip), port);
                Net.PortScan.Close();
                return true;
            }
            catch
            {
                Net.PortScan.Close();
                return false;
            }
        }
    }
}
