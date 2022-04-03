namespace Communications.DeviceCommand
{
    class SetValueCommand : DeviceCommand
    {
        public SetValueCommand(string _opcode,string _confirmCode,double val,ICommunication com):
            base(GetCode(val,_opcode),GetConfirmCode(val,_confirmCode),com)
        {

        }

        private static string GetCode(double val,string opcode) => string.Format("{0}\t{1:0.00}\r", opcode, val);
        private static string GetConfirmCode(double val,string confirmCode) => string.Format("{0}\t{1:0.00}", confirmCode, val);
    }
}
