namespace EventSourcing;

public record ConfigurationKeyValue(string Key, string? Value, bool Readonly);