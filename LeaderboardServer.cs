using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MySqlDI
{

    public class LeaderboardServer : MySqlWebServer
    {
        public LeaderboardServer(string serverUrl, string ServerName) : base(serverUrl, ServerName)
        {
        }

        protected override void DecodePage(HttpResponseMessage responce)
        {
            throw new NotImplementedException();
        }
    }


}
