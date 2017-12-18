﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace csharp_tutorial
{
    public class C_Event
    {
        // Events are not much used in web applications, but are real handy in real time systems

        [Fact]
        public void Hello_Event()
        {
            var hello = new HelloEvent();
            var vp = new ValueProcessor(hello);

            // Listen to ValueChanged Event with normal and anonymous function
            hello.ValueChanged += (s, e) => { Trace.WriteLine($"From anonymous: New value is {e}"); };
            hello.ValueChanged += Hello_ValueChanged;

            // Running for 5 seconds
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 5000) { }

            vp.Stop();

            // Problem with anonymous functions is that you can't remove those
            hello.ValueChanged -= Hello_ValueChanged;
            hello.CurrentValue = 8; // From anonymous function still prints
        }

        private void Hello_ValueChanged(object sender, int e)
        {
            Trace.WriteLine($"New value is {e}");
        }

        /// <summary>
        /// When CurrentValue is changed will invoke ValueChanged event
        /// </summary>
        public class HelloEvent
        {
            public event EventHandler<int> ValueChanged;

            private int _myValue;

            public int CurrentValue
            {
                get { return _myValue; }
                set
                {
                    if (_myValue != value)
                    {
                        _myValue = value;
                        ValueChanged?.Invoke(this, _myValue);
                    }
                }
            }
        }

        /// <summary>
        /// Updates HelloEvents value periodically
        /// </summary>
        public class ValueProcessor
        {
            private HelloEvent _helloEvent;
            private CancellationTokenSource _cts = new CancellationTokenSource();

            public ValueProcessor(HelloEvent helloEvent)
            {
                _helloEvent = helloEvent;
                Task.Factory.StartNew(UpdateValue, _cts.Token);
            }

            public void Stop()
            {
                _cts.Cancel();
            }

            private void UpdateValue(object ct)
            {
                var token = (CancellationToken)ct;

                while (token.IsCancellationRequested == false)
                {
                    var rand = new Random(DateTime.Now.Millisecond);
                    _helloEvent.CurrentValue = rand.Next(0, 100);
                    Thread.Sleep(100);
                }
            }
        }
    }
}