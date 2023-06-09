namespace CrudDemo.Settings
{
    public class AwsConfig
    {
        /// <summary>
        /// S3Bucket
        /// </summary>
        public string Region { get; set; } = null!;
        /// <summary>
        /// S3Bucket
        /// </summary>
        public string Access_Key { get; set; } = null!;
        /// <summary>
        /// S3BucketFolder
        /// </summary>
        public string Secret_Key { get; set; } = null!;
        /// <summary>
        /// S3Bucket
        /// </summary>
        public string S3Bucket { get; set; } = null!;
        /// <summary>
        /// S3BucketFolder
        /// </summary>
        public string S3BucketFolder { get; set; } = null!;
        /// <summary>
        /// S3PreSignedUrlExpirationInMin
        /// </summary>
        public string S3PreSignedUrlExpirationInMin { get; set; } = null!;
    }
}
