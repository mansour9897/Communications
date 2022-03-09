namespace Communications
{
    public class SerialSetting : ICommunicationSetting
    {
        public string PortName { get; set; }
        public int Baudrate { get; set; }
    }
}
