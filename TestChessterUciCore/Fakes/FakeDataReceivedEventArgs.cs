using System;
using System.Diagnostics;

namespace TestChessterUciCore.Fakes
{
    public class FakeDataReceivedEventArgs : EventArgs
    {
        public FakeDataReceivedEventArgs(string data)
        {

        }

        public string Data { get; set; }

        public static explicit operator DataReceivedEventArgs(FakeDataReceivedEventArgs v)
        {
            throw new NotImplementedException();
        }
    }
}
