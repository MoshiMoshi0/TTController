namespace TTController.Service.Config
{
    public class PortConfigData
    {
        public string Name { get; private set; } = "Unknown";
        public int LedCount { get; private set; } = 12;
        public int LedRotation { get; private set; } = 0;
        public bool LedReverse { get; private set; } = false;
        
        public static PortConfigData Default = new PortConfigData();
    }
}
