using EffectiveMobile.Models;
using EffectiveMobile.Services;
using System.Text;

namespace EffectiveMobile.Services;

public class AdvertisementService : IAdvertisementService
{
    private readonly object _lockObject = new();
    private Dictionary<string, HashSet<string>> _locationAdvertisements = new();

    public void LoadAdvertisements(Stream fileStream)
    {
        var newData = new Dictionary<string, HashSet<string>>();

        using var reader = new StreamReader(fileStream);
        string? line;

        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(':', 2);
            if (parts.Length < 2) continue;

            var advertiser = parts[0].Trim();
            var locations = parts[1].Split(',')
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrEmpty(l))
                .Select(NormalizeLocation);

            foreach (var location in locations)
            {
                if (!newData.ContainsKey(location))
                    newData[location] = new HashSet<string>();

                newData[location].Add(advertiser);
            }
        }

        lock (_lockObject)
        {
            _locationAdvertisements = newData;
        }
    }

    public IEnumerable<string> FindAdvertisements(string location)
    {
        var normalizedLocation = NormalizeLocation(location);
        var result = new HashSet<string>();
        var prefixes = GetAllPrefixes(normalizedLocation);

        lock (_lockObject)
        {
            foreach (var prefix in prefixes)
            {
                if (_locationAdvertisements.TryGetValue(prefix, out var advertisers))
                {
                    foreach (var advertiser in advertisers)
                    {
                        result.Add(advertiser);
                    }
                }
            }
        }

        return result;
    }

    private IEnumerable<string> GetAllPrefixes(string location)
    {
        var prefixes = new List<string>();
        var segments = location.Split('/').Where(s => !string.IsNullOrEmpty(s)).ToArray();

        var currentPath = new StringBuilder();
        for (int i = 0; i < segments.Length; i++)
        {
            currentPath.Append('/').Append(segments[i]);
            prefixes.Add(currentPath.ToString());
        }

        return prefixes;
    }

    private static string NormalizeLocation(string location)
    {
        var segments = location.Split('/')
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s));

        return "/" + string.Join("/", segments);
    }
}