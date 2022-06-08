// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#pragma warning disable 1591

namespace IdentityServer4.Events
{
    public static class EventIds
    {
        //////////////////////////////////////////////////////
        /// Authentication related events
        //////////////////////////////////////////////////////
        private const int AuthenticationEventsStart = 1000;

        public const int UserLoginSuccess = AuthenticationEventsStart + 0;
        public const int UserLoginFailure = AuthenticationEventsStart + 1;
        public const int UserLogoutSuccess = AuthenticationEventsStart + 2;

        public const int ClientAuthenticationSuccess = AuthenticationEventsStart + 10;
        public const int ClientAuthenticationFailure = AuthenticationEventsStart + 11;
        
        public const int ApiAuthenticationSuccess = AuthenticationEventsStart + 20;
        public const int ApiAuthenticationFailure = AuthenticationEventsStart + 21;

        //////////////////////////////////////////////////////
        /// Token related events
        //////////////////////////////////////////////////////
        private const int TokenEventsStart = 2000;

        public const int TokenIssuedSuccess = TokenEventsStart + 0;
        public const int TokenIssuedFailure = TokenEventsStart + 1;

        public const int TokenRevokedSuccess = TokenEventsStart + 10;

        public const int TokenIntrospectionSuccess = TokenEventsStart + 20;
        public const int TokenIntrospectionFailure = TokenEventsStart + 21;
        
        //////////////////////////////////////////////////////
        /// Error related events
        //////////////////////////////////////////////////////
        private const int ErrorEventsStart = 3000;

        public const int UnhandledException = ErrorEventsStart + 0;
        public const int InvalidClientConfiguration = ErrorEventsStart + 1;

        //////////////////////////////////////////////////////
        /// Grants related events
        //////////////////////////////////////////////////////
        private const int GrantsEventsStart = 4000;

        public const int ConsentGranted = GrantsEventsStart + 0;
        public const int ConsentDenied = GrantsEventsStart + 1;
        public const int GrantsRevoked = GrantsEventsStart + 2;

        private const int DeviceFlowEventsStart = 5000;

        public const int DeviceAuthorizationSuccess = DeviceFlowEventsStart + 0;
        public const int DeviceAuthorizationFailure = DeviceFlowEventsStart + 1;
    }
}
