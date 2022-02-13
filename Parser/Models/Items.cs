using System.Text.Json.Serialization;

public class Current
{
    [JsonPropertyName("trend")]
    public string? Trend;

    [JsonPropertyName("price")]
    public double Price;
}

public class Today
{
    [JsonPropertyName("trend")]
    public string Trend = null!;

    [JsonPropertyName("price")]
    public double Price;
}

public class Item
{
    [JsonPropertyName("id")]
    public int Id;

    [JsonPropertyName("name")]
    public string Name = null!;

    [JsonPropertyName("icon")]
    public string? Icon;

    [JsonPropertyName("icon_large")]
    public string? IconLarge;

    [JsonPropertyName("descripton")]
    public string? Descripton;

    [JsonPropertyName("current")]
    public Current Current = null!;

    [JsonPropertyName("today")]
    public Today Today = null!;

    [JsonPropertyName("members")]
    public string? Members;
}