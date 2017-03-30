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
        /// the name for this database
        /// </summary>
        public string Name;

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
        /// handles all webrequests
        /// </summary>
        protected HttpWebRequest _webRequest;

        /// <summary>
        /// the servers url
        /// </summary>
        protected string _serverUrl;
        public string ServerUrl;

        /// <summary>
        /// if we are connected to the server or not
        /// </summary>
        protected bool _isConnected = false;
        public bool IsConnected {  get { return _isConnected; } }

        /// <summary>
        /// create the database
        /// </summary>
        /// <param name="serverUrl"></param>
        public MySqlWebServer(string serverUrl)
        {
            _serverUrl = serverUrl; //set the url
        }

        

        public void Connect()
        {
            if (_waitingForRequest) //make sure we can connect
            {
                _log += "\n"; //add new line
                _log += "Error a webrequest is already active cannot connect to " + Name;
                return;
            }
            
            _webRequest = HttpWebRequest.Create(_serverUrl); //make the web request
            _webRequest.Method = "GET";
#if WINDOWS
            phpSender.Timeout = 1000; //windows has an oddity where we need to set the timeout otherwise we will have issues
#endif

            if (_webRequest.Headers == null)
                _webRequest.Headers = new WebHeaderCollection();

            _webRequest.BeginGetResponse(OnRequestFullfulled, _serverUrl);
        }

        
        /// <summary>
        /// this is run when the request was compleated sucessfully
        /// </summary>
        /// <param name="result"></param>
        protected void OnRequestFullfulled(IAsyncResult result, object o)
        {
            try
            {
                //get the webpage
                System.Net.HttpWebResponse response = phpSender.EndGetResponse(result) as System.Net.HttpWebResponse;

                if (response == null) //if no responce we have an error
                {
                    _log += "\n";
                    _log += "Error no responces from request to " + (string)o; //output error mesage
                    _waitingForRequest = false;
                    return;
                }

                if ((string)o == _serverUrl) //if we were trying to connect to the server
                {
                    if (_isConnected) //if we are not connect
                    {
                        _log += "\n";
                        Log += "Already connect can not connect again";
                    }
                    else
                    {
                        _isConnected = true; //sucess
                        _log += "\n";
                        _log += "Connected to " + Name;
                    }
                    return; //exit as we dont need to decode the page we have connect to
                }
                DecodePage(response, (string)o);
            }
            catch (Exception x)
            {
                _log += "\n";
                _log += "========UNHANDLED EXCEPTION=======";
                _log += x; //output to the log
            }

            _waitingForRequest = false;
        }

        private delegate string DecodePage(HttpWebResponse responce, object o);
       

    }


}
