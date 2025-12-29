// I think ideally this should be a generic R2 cloud accessor library to be used generically.
// But that seems harder to implement, so I will couple our specific R2 usage here for now.
// At least both projects can reference is easier this way.

namespace realTimeMessagingWebAppInfra.Storage.Constants;

public static class BucketMappings
{
    static readonly Dictionary<BucketKeys, string> BucketKeyToValueMapping = new()
    {
        { BucketKeys.Public, "realtimemessagingwebapp-public" },
        { BucketKeys.Private, "realtimemessagingwebApp-private" },
    };

    static readonly Dictionary<string, BucketKeys> BucketValueToKeyMapping
        = BucketKeyToValueMapping.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    public static string GetBucketNameFromKey(BucketKeys bucketKey)
        => BucketKeyToValueMapping.TryGetValue(bucketKey, out var bucketName)
            ? bucketName
            : throw new KeyNotFoundException($"Bucket key '{bucketKey}' not found in mappings.");

    public static BucketKeys GetBucketKeyFromName(string bucketName)
        => BucketValueToKeyMapping.TryGetValue(bucketName, out var bucketKey)
            ? bucketKey
            : throw new KeyNotFoundException($"Bucket name '{bucketName}' not found in mappings.");
}
