namespace EffectiveMobile.Services;

public interface IAdvertisementService
{
    void LoadAdvertisements(Stream fileStream);
    IEnumerable<string> FindAdvertisements(string location);
}