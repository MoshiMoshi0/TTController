namespace TTController.Common.Config
{
    public class PortConfigData
    {
        public PortIdentifier Port { set; get; }
        public string Name { set; get; }
        public int LedCount { set; get; }
        public int LedRotation { set; get; }
        public bool LedReverse { set; get; }

        public PortConfigData(PortIdentifier port)
        {
            Port = port;
        }
    }
}
