namespace Communications.DeviceCommand
{
    interface IDeviceCommand
    {
        string CommandCode { get; }
        bool ExecuteConfirmed { get; set; }
        string ConfirmationCode { get; }

        void Execute();
    }
}
