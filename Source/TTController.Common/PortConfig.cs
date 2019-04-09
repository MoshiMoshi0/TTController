namespace TTController.Common
{
    public class PortConfig
    {
        public string Name { get; private set; } = "Unknown";
        public int LedCount { get; private set; } = 12;
        public int LedRotation { get; private set; } = 0;
        public bool LedReverse { get; private set; } = false;
        
        public static readonly PortConfig Default = new PortConfig();
    }
}
