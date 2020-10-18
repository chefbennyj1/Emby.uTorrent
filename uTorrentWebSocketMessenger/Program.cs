using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uTorrentWebSocketMessenger
{
    class Program
    {
        static void Main(string[] args)
        {
            ApiClient.SendMessageUpdateTorrentData(args[0]);
        }
    }
}
