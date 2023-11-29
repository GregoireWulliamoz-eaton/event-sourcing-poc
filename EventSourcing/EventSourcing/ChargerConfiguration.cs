namespace EventSourcing;

public class ChargerConfiguration
{
    public Guid Id { get; set; }

    public Dictionary<string, ConfigurationValue> ConfigurationKeyValues { get; set; } = new();

    public void Apply(ConfigurationChanged configurationChanged)
    {
        foreach (ConfigurationKeyValue keyValue in configurationChanged.ConfigurationKeyValues)
        {
            var configurationValue = new ConfigurationValue(keyValue.Value, keyValue.Readonly);
            if (!ConfigurationKeyValues.TryAdd(keyValue.Key, configurationValue))
            {
                ConfigurationKeyValues[keyValue.Key] = configurationValue;
            }
        }
    }
}

public record ConfigurationValue(string? Value, bool Readonly);