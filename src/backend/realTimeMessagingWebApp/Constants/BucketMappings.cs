namespace realTimeMessagingWebApp.Constants;

public static class BucketMappings
{
    static readonly Dictionary<BucketKeys, string> BucketKeyToValueMapping = new()
    {
        { BucketKeys.Public, "realTimeMessagingWebApp_public" },
        { BucketKeys.Private, "realTimeMessagingWebApp_private" },
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
