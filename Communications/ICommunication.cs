using System;

namespace Communications
{
    public interface ICommunication
    {
        delegate void MessageReceivedHandler(object sender, string message);
        event MessageReceivedHandler MessageReceived;
        bool IsConnected { get; }
        bool Connect();
        void Write(string data);
        string Read();
    }
}