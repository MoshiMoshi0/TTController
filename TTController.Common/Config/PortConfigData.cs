using System.ComponentModel;

namespace TTController.Common.Config
{
    public class PortConfigData
    {
        public string Name { set; get; } = "Unknown";
        public int LedCount { set; get; } = 12;
        public int LedRotation { set; get; } = 0;
        public bool LedReverse { set; get; } = false;
        
        public static PortConfigData Default = new PortConfigData();
    }
}
