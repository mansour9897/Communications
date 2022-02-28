using System;
using System.IO.Ports;

namespace Communications
{
    public class SerialComminucation : ICommunication
    {
        private readonly SerialPort _port;
        private readonly Action _dataRecieved;


        public SerialComminucation(string portName, int buad, Action dataRecieved)
        {
            _port = new SerialPort(portName, buad);

            _port.ReadTimeout = 1000;
            _port.WriteTimeout = 1000;

            _dataRecieved = dataRecieved;

            _port.DataReceived += _port_DataReceived;
        }

        private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _dataRecieved();
        }

        public bool IsConnected => _port.IsOpen;


        public bool Connect()
        {
            try
            {
                _port.Open();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
            }
        }

        public string Read()
        {
            string res = "";

            return res;
        }



        public void Write(string data)
        {
            if (_port.IsOpen == false) Connect();
            _port.WriteLine(data);
        }
    }
}