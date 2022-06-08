// Copyright 2022 Ringier SA

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Events;
using IdentityServer4.Services;

namespace IdentityServer.UnitTests.Common
{
    public class TestEventService : IEventService
    {
        private Dictionary<Type, object> _events = new Dictionary<Type, object>();

        public Task RaiseAsync(Event evt)
        {
            _events.Add(evt.GetType(), evt);
            return Task.CompletedTask;
        }

        public T AssertEventWasRaised<T>()
            where T : class
        {
            _events.ContainsKey(typeof(T)).Should().BeTrue();
            return (T)_events.Where(x => x.Key == typeof(T)).Select(x=>x.Value).First();
        }

        public bool CanRaiseEventType(EventTypes evtType)
        {
            return true;
        }
    }
}
