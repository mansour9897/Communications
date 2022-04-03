using System.Diagnostics;
using System.Threading;

namespace Communications.DeviceCommand
{
    public class DeviceCommand : IDeviceCommand
    {
        private readonly string _commandCode;
        private readonly string _confirmationCode;
        private readonly ICommunication _com;

        public delegate void CommandConfirmedHandler(string confirmationCode, string value);
        public event CommandConfirmedHandler? CommandConfirmed;

        public DeviceCommand(string code, string confirmCode, ICommunication com)
        {
            _com = com;
            _commandCode = code;
            _confirmationCode = confirmCode;
            _com.MessageReceived += _com_MessageReceived;
        }

        private void _com_MessageReceived(object sender, string message)
        {
            if (!(message.IndexOf(ConfirmationCode) < 0))
            {
                ExecuteConfirmed = true;
                
                string val = message.Split('\t')[1];
                CommandConfirmed?.Invoke(ConfirmationCode, val);
            }
        }

        public DeviceCommand(string code, ICommunication com)
        {
            _com = com;
            _commandCode = code;
            _confirmationCode = "";
        }

        public string CommandCode => _commandCode;

        public bool ExecuteConfirmed { get; set; }

        public string ConfirmationCode => _confirmationCode;

        public void Execute()
        {
            ExecuteConfirmed = false;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (!ExecuteConfirmed)
            {
                _com.Write(_commandCode);
                Thread.Sleep(10);

                if (stopwatch.Elapsed.TotalMilliseconds > 3000)
                    break;
            }
        }
    }
}
