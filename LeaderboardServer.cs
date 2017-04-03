using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MySqlDI
{
   
    public struct HighScore
    {
        public string Name, Id;
        public int Score;
    }

    public class LeaderboardServer : MySqlWebServer
    {
        #region bad word list enter you own words
        private static List<string> FilterList = new List<string>() {  };
        #endregion
        public enum SortOrder { Assending = 0, Desending = 1 };

        public string GetLeaderboardUrl;

        public LeaderboardServer(string serverUrl, string ServerName) : base(serverUrl, ServerName)
        {
        }

        public void GetLeaderBoard(string leaderboard, SortOrder sortby)
        {
            WriteLineToLog("Trying to connect to " + leaderboard + " leaderboard");
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                { "leaderboard", leaderboard },
                { "sortby" , Convert.ToString(((int)sortby)) }
            };

            ///this is fine we want senddata to run aysnc
            SendData(GetLeaderboardUrl, HttpMethod.Post, data, RequestType.SendScore);
        }

        

        public override object DecodePage(HttpResponseMessage responce, string htmlPage)
        {
            if (htmlPage.Length < 360) //make sure the page has some text
            {
                int start = htmlPage.IndexOf("<body>") + 6; 
                WriteLineToLog(htmlPage.Substring(start, htmlPage.Length - start)); //out error msg
            }

            Dictionary<string, List<string>> database = MySqlWebServer.DecodeMySqlDatabase(htmlPage);

            List<string> names, ids, score;
            names = database["name"];
            ids = database["id"];
            score = database["Score"];

            //now we decode the results
            List<HighScore> scores = new List<HighScore>();

            for (int i = 0; i < names.Count; i++)
                scores.Add(new HighScore()
                {
                    Name = names[i],
                    Id = ids[i],
                    Score = Convert.ToInt32(score[i]),
                });
                

            

            return scores;
        }

        public override void OnPageError(ErrorTypes error)
        {
            
        }
    }

}
