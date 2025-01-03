﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class WeatherForecastIdProvider : IRecordIdProvider<WeatherForecastId>
{
    public WeatherForecastId GetKey(object key)
    {
        if (key is Guid value)
            return new(value);

        throw new InvalidKeyProviderException("Object provided is not a WeatherForecastId Value");
    }
    public object GetValueObject(WeatherForecastId key)
    {
        return key.Value;
    }
}
