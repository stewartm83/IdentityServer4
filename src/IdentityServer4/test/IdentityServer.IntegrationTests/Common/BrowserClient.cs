// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Net.Http;

namespace IdentityServer.IntegrationTests.Common
{
    public class BrowserClient : HttpClient
    {
        public BrowserClient(BrowserHandler browserHandler)
            : base(browserHandler)
        {
            BrowserHandler = browserHandler;
        }

        public BrowserHandler BrowserHandler { get; private set; }

        public bool AllowCookies
        {
            get { return BrowserHandler.AllowCookies; }
            set { BrowserHandler.AllowCookies = value; }
        }
        public bool AllowAutoRedirect
        {
            get { return BrowserHandler.AllowAutoRedirect; }
            set { BrowserHandler.AllowAutoRedirect = value; }
        }
        public int ErrorRedirectLimit
        {
            get { return BrowserHandler.ErrorRedirectLimit; }
            set { BrowserHandler.ErrorRedirectLimit = value; }
        }
        public int StopRedirectingAfter
        {
            get { return BrowserHandler.StopRedirectingAfter; }
            set { BrowserHandler.StopRedirectingAfter = value; }
        }

        internal void RemoveCookie(string uri, string name)
        {
            BrowserHandler.RemoveCookie(uri, name);
        }

        internal System.Net.Cookie GetCookie(string uri, string name)
        {
            return BrowserHandler.GetCookie(uri, name);
        }
    }
}
