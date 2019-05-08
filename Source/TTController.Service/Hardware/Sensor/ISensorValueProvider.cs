namespace TTController.Service.Hardware.Sensor
{
    public interface ISensorValueProvider
    {
        void Update();
        float Value();
        float ValueOrDefault(float defaultValue);
    }
}
