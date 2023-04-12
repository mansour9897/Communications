namespace Communications
{
	public class SocketSetting : ICommunicationSetting
	{
		private readonly string _host;
		private readonly int _port;
		public SocketSetting(string ip, int port)
		{
			_host = ip;
			_port = port;
		}

		public string Ip { get { return _host; } }
		public int Port { get { return _port; } }

	}
}
