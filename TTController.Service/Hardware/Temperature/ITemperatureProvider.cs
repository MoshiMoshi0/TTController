namespace TTController.Service.Hardware.Temperature
{
    public interface ITemperatureProvider
    {
        float Value();
        float ValueOrDefault(float defaultValue);
    }
}
