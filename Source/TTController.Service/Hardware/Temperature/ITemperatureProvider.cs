namespace TTController.Service.Hardware.Temperature
{
    public interface ITemperatureProvider
    {
        void Update();
        float Value();
        float ValueOrDefault(float defaultValue);
    }
}
