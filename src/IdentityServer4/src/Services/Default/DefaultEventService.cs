// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using IdentityServer4.Configuration;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;

namespace IdentityServer4.Events
{
    /// <summary>
    /// The default event service
    /// </summary>
    /// <seealso cref="IdentityServer4.Services.IEventService" />
    public class DefaultEventService : IEventService
    {
        /// <summary>
        /// The options
        /// </summary>
        protected readonly IdentityServerOptions Options;

        /// <summary>
        /// The context
        /// </summary>
        protected readonly IHttpContextAccessor Context;

        /// <summary>
        /// The sink
        /// </summary>
        protected readonly IEventSink Sink;

        /// <summary>
        /// The clock
        /// </summary>
        protected readonly ISystemClock Clock;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultEventService"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="context">The context.</param>
        /// <param name="sink">The sink.</param>
        /// <param name="clock">The clock.</param>
        public DefaultEventService(IdentityServerOptions options, IHttpContextAccessor context, IEventSink sink, ISystemClock clock)
        {
            Options = options;
            Context = context;
            Sink = sink;
            Clock = clock;
        }

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <param name="evt">The event.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">evt</exception>
        public async Task RaiseAsync(Event evt)
        {
            if (evt == null) throw new ArgumentNullException(nameof(evt));

            if (CanRaiseEvent(evt))
            {
                await PrepareEventAsync(evt);
                await Sink.PersistAsync(evt);
            }
        }

        /// <summary>
        /// Indicates if the type of event will be persisted.
        /// </summary>
        /// <param name="evtType"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public bool CanRaiseEventType(EventTypes evtType)
        {
            switch (evtType)
            {
                case EventTypes.Failure:
                    return Options.Events.RaiseFailureEvents;
                case EventTypes.Information:
                    return Options.Events.RaiseInformationEvents;
                case EventTypes.Success:
                    return Options.Events.RaiseSuccessEvents;
                case EventTypes.Error:
                    return Options.Events.RaiseErrorEvents;
                default:
                    throw new ArgumentOutOfRangeException(nameof(evtType));
            }
        }

        /// <summary>
        /// Determines whether this event would be persisted.
        /// </summary>
        /// <param name="evt">The evt.</param>
        /// <returns>
        ///   <c>true</c> if this event would be persisted; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanRaiseEvent(Event evt)
        {
            return CanRaiseEventType(evt.EventType);
        }

        /// <summary>
        /// Prepares the event.
        /// </summary>
        /// <param name="evt">The evt.</param>
        /// <returns></returns>
        protected virtual async Task PrepareEventAsync(Event evt)
        {
            evt.ActivityId = Context.HttpContext.TraceIdentifier;
            evt.TimeStamp = Clock.UtcNow.UtcDateTime;
            evt.ProcessId = Process.GetCurrentProcess().Id;

            if (Context.HttpContext.Connection.LocalIpAddress != null)
            {
                evt.LocalIpAddress = Context.HttpContext.Connection.LocalIpAddress.ToString() + ":" + Context.HttpContext.Connection.LocalPort;
            }
            else
            {
                evt.LocalIpAddress = "unknown";
            }

            if (Context.HttpContext.Connection.RemoteIpAddress != null)
            {
                evt.RemoteIpAddress = Context.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            else
            {
                evt.RemoteIpAddress = "unknown";
            }

            await evt.PrepareAsync();
        }
    }
}
