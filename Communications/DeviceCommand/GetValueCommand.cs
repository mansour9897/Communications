namespace Communications.DeviceCommand
{
    public class GetValueCommand : DeviceCommand
    {
        private string? _val;
        public GetValueCommand(string Opcode, string ConfirmCode, ICommunication com) : base(Opcode, ConfirmCode, com)
        {
            this.CommandConfirmed += GetValueCommand_CommandConfirmed;
        }

        private void GetValueCommand_CommandConfirmed(string confirmationCode, string value)
        {
            _val = value;
        }

        public string GetValue()
        {
            _val = "";
            this.Execute();
            return _val;
        }
    }
}
