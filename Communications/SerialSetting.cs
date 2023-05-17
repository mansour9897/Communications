using System.IO.Ports;

namespace Communications
{
    public class SerialSetting : ICommunicationSetting
    {
		public string? PortName { get; set; }
		public int BaudRate { get; set; }
		public int DataBits { get; set; }
		public Parity Parity { get; set; }
		public StopBits StopBits { get; set; }
		public Handshake Handshake { get; set; }
	}
}
