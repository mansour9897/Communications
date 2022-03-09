using System;
using System.Diagnostics;
using System.IO.Ports;

namespace Communications
{
    public class SerialComminucation : ICommunication
    {
        private readonly SerialPort _port;

        public event ICommunication.MessageReceivedHandler? MessageReceived;

        public SerialComminucation()
        {
            string[]? ports = SerialPort.GetPortNames();
            _port = new SerialPort();
            if (ports.Length > 0)
                _port.PortName = ports[0];
            _port.BaudRate = 9600;
        }
        public SerialComminucation(string portName, int buad, Action dataRecieved)
        {
            _port = new SerialPort(portName, buad);

            _port.ReadTimeout = 1000;
            _port.WriteTimeout = 1000;

            _port.DataReceived += _port_DataReceived;
        }

        private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string msg = Read();
            MessageReceived?.Invoke(this, msg);
        }

        public bool IsConnected => _port.IsOpen;

        public bool Connect()
        {
            try
            {
                _port.Open();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            finally
            {
            }
        }

        public string Read()
        {
            if (_port.IsOpen == false)
            {
                if (!Connect())
                {
                    return "";
                }
            }

            return _port.ReadLine();
        }

        public void Write(string data)
        {
            if (_port.IsOpen == false)
            {
                if (!Connect()) return;
            }

            _port.WriteLine(data);
        }
    }
}