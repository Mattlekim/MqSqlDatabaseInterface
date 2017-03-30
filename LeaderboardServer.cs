using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MySqlDI
{
<<<<<<< HEAD
    

    public class LeaderboardServer : MySqlWebServer
    {
        public enum SortOrder { Assending = 0, Desending = 1 };
        

=======

    public class LeaderboardServer : MySqlWebServer
    {
>>>>>>> development
        public LeaderboardServer(string serverUrl, string ServerName) : base(serverUrl, ServerName)
        {
        }

<<<<<<< HEAD
        public void GetLeaderBoard(string id, SortOrder sortby)
        {
            
        }

        public override object DecodePage(HttpResponseMessage responce)
        {
            return responce;
=======
        protected override void DecodePage(HttpResponseMessage responce)
        {
>>>>>>> development
            throw new NotImplementedException();
        }
    }


}
