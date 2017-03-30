using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;

namespace MySqlDI
{
    /// <summary>
    /// a class to handle any and all server interactions
    /// </summary>
    public abstract class MySqlWebServer
    {

        protected RequestType _requestType;
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

        public delegate void _onReciveRequest(string body);
        public _onReciveRequest OnReciveRequest;
            
        /// <summary>
        /// handles all webrequests
        /// </summary>
        protected HttpClient _webRequest;

        /// <summary>
        /// the servers url
        /// </summary>
        protected string _serverAddress;
        public string ServerAddress { get { return _serverAddress; } }

        /// <summary>
        /// if we are connected to the server or not
        /// </summary>
        protected bool _isConnected = false;
        public bool IsConnected
        {
            get
            {
                if (_isConnected) //if we are connected return it
                    return _isConnected; 

                _log += "\n";
                _log += "Error not connected to server"; //otherwise show error msg and return false
                return false;
            }
        }

        /// <summary>
        /// create the database
        /// </summary>
        /// <param name="serverUrl"></param>
        public MySqlWebServer(string serverUrl, string ServerName)
        {
            _serverAddress = serverUrl; //set the url
            Name = ServerName;

            //set up httpclient
            _webRequest = new HttpClient();
            _webRequest.DefaultRequestHeaders.Accept.Clear();
            _webRequest.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/php"));

            _log += "Server " + Name + " Inizalized!";
        }

        /// <summary>
        /// creates a http request
        /// </summary>
        /// <param name="address">the web adress</param>
        /// <param name="method">the methord POST or GET</param>
        /// <returns></returns>
        private HttpRequestMessage CreateWebRequest(string address, HttpMethod method)
        {
            //make the web request
            _webRequest.BaseAddress = new Uri(address);
            return new HttpRequestMessage(method, address);
            //set the address
        }

        public void Connect()
        {
            if (_waitingForRequest) //make sure we can connect
            {
                _log += "\n"; //add new line
                _log += "Error a webrequest is already active cannot connect to " + Name;
                return;
            }
            _waitingForRequest = true; //set the flag
            _requestType = RequestType.Connect;
            _isConnected = true;
            SendData(_serverAddress, HttpMethod.Post, new Dictionary<string, string>());
        }

        
        /// <summary>
        /// this is run when the request was compleated sucessfully
        /// </summary>
        /// <param name="result"></param>
        protected void OnRequestFullfulled(HttpResponseMessage result)
        {
            try
            {
                result.EnsureSuccessStatusCode(); //make sure there is a result

                if (_requestType == RequestType.Connect) //if we were trying to connect to the server
                {
                    if (_isConnected) //if we are not connect
                    {
                        _log += "\n";
                        _log += "Already connect can not connect again";
                        if (OnErrorConnecting != null)
                            OnErrorConnecting(_log);
                    }
                    else
                    {
                        _isConnected = true; //sucess
                        _log += "\n";
                        _log += "Connected to " + Name;
                        if (OnErrorConnecting != null)
                            OnErrorConnecting(_log);
                    }
                    return; //exit as we dont need to decode the page we have connect to
                }
                PostDecodePage(result);
                
            }
            catch (Exception x)
            {
                _log += "\n";
                _log += "========UNHANDLED EXCEPTION=======";
                _log += x; //output to the log
                if (OnErrorConnecting != null)
                    OnErrorConnecting(_log);
            }

            _waitingForRequest = false;
        }

        private async void PostDecodePage(HttpResponseMessage result)
        {
            DecodePage(result);
            if (OnReciveRequest != null)
                OnReciveRequest(await result.Content.ReadAsStringAsync());
        }

        public abstract object DecodePage(HttpResponseMessage responce);
      
        
        protected async void SendData(string url, HttpMethod method, Dictionary<string, string> data)
        {
            if (!IsConnected)
                return; //dont send data
            
            HttpRequestMessage request = CreateWebRequest(_serverAddress, method);
            
            FormUrlEncodedContent encodedData = new FormUrlEncodedContent(data);
            request.Content = encodedData;
            OnRequestFullfulled(await _webRequest.SendAsync(request, HttpCompletionOption.ResponseContentRead)); //get the result
        }

    }


}
