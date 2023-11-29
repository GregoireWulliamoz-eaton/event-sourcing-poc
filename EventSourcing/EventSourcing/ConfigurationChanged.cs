namespace EventSourcing;

public record ConfigurationChanged(List<ConfigurationKeyValue> ConfigurationKeyValues);