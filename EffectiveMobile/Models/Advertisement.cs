namespace EffectiveMobile.Models;

public class Advertisement
{
    public string Name { get; set; } = string.Empty;
    public List<string> Locations { get; set; } = new List<string>();
}