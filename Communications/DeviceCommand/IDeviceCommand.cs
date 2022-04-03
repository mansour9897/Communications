namespace Communications.DeviceCommand
{
    interface IDeviceCommand
    {
        string CommandCode { get; }
        bool ExecuteConfirmed { get; set; }
        bool ExecuteFinished { get; set; }
        string ConfirmationCode { get; }

        void Execute();
    }
}
