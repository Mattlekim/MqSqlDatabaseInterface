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
        #region bad word list
        private static List<string> FilterList = new List<string>() { "fuking", "fuk", "fucking", "fuck", "shit", "bitch", "wancker" };
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

            SendData(GetLeaderboardUrl, HttpMethod.Post, data, RequestType.SendScore);
        }

        public override object DecodePage(HttpResponseMessage responce)
        {
            string htmlPage = responce.Content.ReadAsStringAsync().Result;

            if (htmlPage.Length < 360) //make sure the page has some text
            {
                int start = htmlPage.IndexOf("<body>") + 6; 
                WriteLineToLog(htmlPage.Substring(start, htmlPage.Length - start)); //out error msg
            }

            //now we decode the results
            List<HighScore> scores = new List<HighScore>();
            HighScore tmpScore;
            //now we need to get every score from the scoreboard and add them to the list
            bool addscore = false;
            tmpScore = new HighScore();
            for (int i = 0; i < htmlPage.Length; i++)
            {
                
                if (htmlPage[i] == '^')
                    for (int counter = i + 1; counter < i + 50 && counter < htmlPage.Length; counter++)
                        if (htmlPage[counter] == '^')//look for the end of the score
                        {
                            tmpScore.Score = Convert.ToInt32(htmlPage.Substring(i + 1, counter - i - 1));
                            i = counter + 6;
                            break;
                        }

                if (htmlPage[i] == '#')
                    for (int counter = i + 1; counter < i + 50 && counter < htmlPage.Length; counter++)
                        if (htmlPage[counter] == '#')//look for the end of the id
                        {

                            string tmp2 = htmlPage.Substring(i + 1, counter - i - 1).ToLower();
                            foreach (string s in FilterList)
                            {
                                if (tmp2.Contains(s))
                                    tmp2 = tmp2.Replace(s, "**");
                            }
                            tmpScore.Name = tmp2;
                            addscore = true;
                            i = counter + 6;
                            
                            break;
                        }

                if (addscore)
                {
                    scores.Add(tmpScore);
                    addscore = false;
                    tmpScore.Name = null;
                    tmpScore.Score = 0;
                }
                

            }

            return scores;
        }

        public override void OnPageError(ErrorTypes error)
        {
            
        }
    }

}
