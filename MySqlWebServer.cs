using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;

namespace MySqlDI
{
    public enum RequestType { Connect = 0, SendScore = 1, RetrevieLeaderboard = 2 }

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


        public delegate void _onHTTPFailure(string log);
        public _onHTTPFailure OnHTTPFailure;

        public delegate void __onHTTPSuccesses(string body);
        public __onHTTPSuccesses OnHTTPSuccesses;
            
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
        /// debug option turn it on to output log to output window
        /// </summary>
        public bool WriteLogToOutputWindow = false;
        /// <summary>
        /// if we are connected to the server or not
        /// </summary>
        protected bool _isConnected = false;
        public bool IsConnected
        {
            get
            {
                return _isConnected;
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
            
            WriteLineToLog("Server " + Name + " Inizalized!");
        }

        /// <summary>
        /// creates a http request
        /// </summary>
        /// <param name="address">the web adress</param>
        /// <param name="method">the methord POST or GET</param>
        /// <returns></returns>
        public HttpRequestMessage CreateWebRequest(string address, HttpMethod method)
        {
            //make the web request
            _webRequest = new HttpClient(); //inizalize the http client

            _webRequest.BaseAddress = new Uri(address);
            return new HttpRequestMessage(method, address);
            //set the address
        }

        public async Task<bool> Connect()
        {
            
            return await SendData(_serverAddress, HttpMethod.Post, null, RequestType.Connect);
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
                        WriteLineToLog("Already connect can not connect again");
                        
                        if (OnHTTPFailure != null)
                            OnHTTPFailure(_log);
                    }
                    else
                    {
                        _isConnected = true; //sucess
                        WriteLineToLog("Connected to " + Name);
                        if (OnHTTPSuccesses != null)
                            OnHTTPFailure(_log);
                    }
                    _webRequest.Dispose();
                    _waitingForRequest = false;
                    return; //exit as we dont need to decode the page we have connect to
                }
                PostDecodePage(result);
                
            }
            catch (Exception x)
            {
                WriteLineToLog(" ========UNHANDLED EXCEPTION=======\n"+ x); //output to the log
                if (OnHTTPFailure != null)
                    OnHTTPFailure(_log);
            }
            _webRequest.Dispose();
            _waitingForRequest = false;
        }

        private async void PostDecodePage(HttpResponseMessage result)
        {
            DecodePage(result);
            if (OnHTTPSuccesses != null)
                OnHTTPSuccesses(await result.Content.ReadAsStringAsync());
        }

        public abstract void DecodePage(HttpResponseMessage responce);
      
        
        protected async Task<bool> SendData(string url, HttpMethod method, Dictionary<string, string> data, RequestType requesttype)
        {
            if (!IsConnected && requesttype != RequestType.Connect)
            {
                WriteLineToLog("Error you must be connected to server to send data");
                return false; //dont send data
            }

            if (_waitingForRequest)
            {
                WriteLineToLog("Server is bussy try again later");
                return false;
            }

            _waitingForRequest = true;

            _requestType = requesttype; //set the type of request
            HttpRequestMessage request = CreateWebRequest(url, method);

            if (data != null)
            {
                FormUrlEncodedContent encodedData = new FormUrlEncodedContent(data);
                request.Content = encodedData;
            }
            else
                request.Content = null;
            OnRequestFullfulled(await _webRequest.SendAsync(request, HttpCompletionOption.ResponseContentRead)); //get the result
            return true;
        }

        /// <summary>
        /// writes to the log
        /// </summary>
        /// <param name="msg">the text to add to the log</param>
        protected void WriteToLog(string msg)
        {
            if (WriteLogToOutputWindow)
                System.Diagnostics.Debug.WriteLine(msg);
            _log += msg;
        }

        /// <summary>
        /// writes to the log and creates a new line after it has finised
        /// </summary>
        /// <param name="msg">the text to add to the log</param>
        protected void WriteLineToLog(string msg)
        {
            if (WriteLogToOutputWindow)
                System.Diagnostics.Debug.WriteLine(msg);
            _log += msg;
            _log += "\n";
        }

        /// <summary>
        /// clears the log
        /// </summary>
        protected void ClearLog()
        {
            _log = "";
        }

    }


}
