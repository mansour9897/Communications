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
			_port.BaudRate = 115200;
			_port.DtrEnable = true;

			_port.DataReceived += new SerialDataReceivedEventHandler(_port_DataReceived);

		}
		public SerialComminucation(string portName, int buad)
		{
			_port = new SerialPort(portName, buad)
			{
				ReadTimeout = 1000,
				WriteTimeout = 1000
			};
			_port.DtrEnable = true;
			_port.RtsEnable = true;
			_port.DataReceived += new SerialDataReceivedEventHandler(_port_DataReceived);

			//Thread trd = new Thread(backWork);
			//trd.IsBackground = true;
			////trd.Start();
		}

		private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			//Thread.Sleep(10);
			string msg = Read();
			// Console.WriteLine("Received: " + msg);
			//Debug.WriteLine("Received: " + msg);
			MessageReceived?.Invoke(this, msg);
		}

		//private void backWork()
		//{
		//    while (true)
		//    {
		//        string msg = "!00";//Read();
		//        MessageReceived?.Invoke(this, msg);
		//    }
		//}
		public bool IsConnected => _port.IsOpen;

		public bool Connect()
		{
			if (_port.IsOpen) return true;

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
			try
			{
				string msg = _port.ReadLine();
				//Debug.WriteLine("Read: " + msg);
				return msg.Trim();

			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return "";
			}
			finally
			{

			}
		}

		public void Write(string data)
		{
			if (_port.IsOpen == false)
			{
				if (!Connect()) return;
				Debug.WriteLine("Connected");
			}

			try
			{
				_port.Write(data);
				Debug.WriteLine("Sent: " + data);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			finally
			{

			}
			//_port.WriteLine(data);

		}

		public void ChangeSetting(ICommunicationSetting setting)
		{
			try
			{
				SerialSetting st = (SerialSetting)setting;
				_port.Close();

				if (st.PortName is not null)
					_port.PortName = st.PortName;

				_port.BaudRate = st.BaudRate;
				_port.Parity = st.Parity;
				_port.StopBits = st.StopBits;
				_port.DataBits = st.DataBits;
				_port.Handshake = st.Handshake;
				_port.DtrEnable = true;


				_port.DataReceived += new SerialDataReceivedEventHandler(_port_DataReceived);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		public void ChangePort(string portName)
		{
			if (portName is null) return;
			if (_port.IsOpen) _port.Close();

			_port.PortName = portName;
		}
	}
}