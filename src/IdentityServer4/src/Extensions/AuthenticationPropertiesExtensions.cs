// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IdentityServer4.Extensions
{
    /// <summary>
    /// Extensions for AuthenticationProperties
    /// </summary>
    public static class AuthenticationPropertiesExtensions
    {
        internal const string SessionIdKey = "session_id";
        internal const string ClientListKey = "client_list";

        /// <summary>
        /// Gets the user's session identifier.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static string GetSessionId(this AuthenticationProperties properties)
        {
            if (properties?.Items.ContainsKey(SessionIdKey) == true)
            {
                return properties.Items[SessionIdKey];
            }

            return null;
        }

        /// <summary>
        /// Sets the user's session identifier.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="sid">The session id</param>
        /// <returns></returns>
        public static void SetSessionId(this AuthenticationProperties properties, string sid)
        {
            properties.Items[SessionIdKey] = sid;
        }

        /// <summary>
        /// Gets the list of client ids the user has signed into during their session.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetClientList(this AuthenticationProperties properties)
        {
            if (properties?.Items.ContainsKey(ClientListKey) == true)
            {
                var value = properties.Items[ClientListKey];
                return DecodeList(value);
            }

            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Removes the list of client ids.
        /// </summary>
        /// <param name="properties"></param>
        public static void RemoveClientList(this AuthenticationProperties properties)
        {
            properties?.Items.Remove(ClientListKey);
        }

        /// <summary>
        /// Adds a client to the list of clients the user has signed into during their session.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="clientId"></param>
        public static void AddClientId(this AuthenticationProperties properties, string clientId)
        {
            if (clientId == null) throw new ArgumentNullException(nameof(clientId));

            var clients = properties.GetClientList();
            if (!clients.Contains(clientId))
            {
                var update = clients.ToList();
                update.Add(clientId);

                var value = EncodeList(update);
                if (value == null)
                {
                    properties.Items.Remove(ClientListKey);
                }
                else
                {
                    properties.Items[ClientListKey] = value;
                }
            }
        }


        private static IEnumerable<string> DecodeList(string value)
        {
            if (value.IsPresent())
            {
                var bytes = Base64Url.Decode(value);
                value = Encoding.UTF8.GetString(bytes);
                return ObjectSerializer.FromString<string[]>(value);
            }

            return Enumerable.Empty<string>();
        }

        private static string EncodeList(IEnumerable<string> list)
        {
            if (list != null && list.Any())
            {
                var value = ObjectSerializer.ToString(list);
                var bytes = Encoding.UTF8.GetBytes(value);
                value = Base64Url.Encode(bytes);
                return value;
            }

            return null;
        }
    }
}
