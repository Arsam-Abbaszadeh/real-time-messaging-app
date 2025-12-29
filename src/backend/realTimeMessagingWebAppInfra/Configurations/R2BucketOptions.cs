using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace realTimeMessagingWebAppInfra.Configurations;
public class R2BucketOptions
{
    public const string SectionName = "R2BucketOptions";
    public string PublicBucketUrlFormat { get; init; }
    public string RealTimeMessagingAppPublicBucketId { get; init; }
    public int BucketUploadExpirationInMinutes { get; init; }
    public int PrivateBucketDownloadExpirationInHours { get; init; }

    public string PublicBucketUrl => string.Format(PublicBucketUrlFormat, RealTimeMessagingAppPublicBucketId);
}
