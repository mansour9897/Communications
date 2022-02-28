using System;

namespace Communications
{
    public interface ICommunication
    {
        bool IsConnected { get; }
        bool Connect();
        void Write(string data);
        string Read();
    }
}