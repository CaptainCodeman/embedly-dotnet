using System;
using System.Net;

namespace Embedly.Http
{
    /// <summary>
    /// This class is used to pass on "state" between each Begin/End call
    /// It also carries the user supplied "state" object all the way till
    /// the end where is then hands off the state object to the
    /// HttpWebRequestCallbackState object.
    /// </summary>
    internal class HttpWebRequestAsyncState
    {
        public byte[] RequestBytes { get; set; }
        public HttpWebRequest HttpWebRequest { get; set; }
        public Action<HttpWebRequestCallbackState> ResponseCallback { get; set; }
        public Object State { get; set; }
    }
}