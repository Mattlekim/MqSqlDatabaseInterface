using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
namespace MySqlDI
{
    /// <summary>
    /// a class to handle any and all server interactions
    /// </summary>
    public class MySqlWebServer
    {
        /// <summary>
        /// weather any webserver is currently waiting for a request or not
        /// </summary>
        private static bool _waitingForRequest;
        public static bool WatingForRequest { get { return _waitingForRequest; } }

        /// <summary>
        /// keeps a log of all activity
        /// </summary>
        protected string _log;
        public string Log { get { return _log; } }


        /// <summary>
        /// keeps a log of how much bandwidth this webserver has used
        /// </summary>
        protected int _bandwidthUsed;
        public int BandwidthUsed { get { return _bandwidthUsed; } }

        /// <summary>
        /// keeps a log of the total amount of bandwith used by the entire application 
        /// within all WebServers
        /// </summary>
        protected static int _appBandwidthUsed;
        public static int AppBandwidthUsed { get { return _appBandwidthUsed; } }


        public delegate void _onErrorConnecting(string log);
        public _onErrorConnecting OnErrorConnecting;


        /// <summary>
        /// the servers url
        /// </summary>
        protected string _serverUrl;
        public string ServerUrl;

        /// <summary>
        /// create the database
        /// </summary>
        /// <param name="serverUrl"></param>
        public MySqlWebServer(string serverUrl)
        {
            _serverUrl = serverUrl; //set the url
        }

        public bool Connect()
        {
            return false;
        }



    }


}
