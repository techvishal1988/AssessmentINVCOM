namespace Framework.Configuration.Models
{
    using Framework.Configuration;    

    /// <summary>
    /// Defines the <see cref="ApplicationOptions" />.
    /// </summary>
    public class ApplicationOptions : ConfigurationOptions
    {
        public string CognitoAuthorityURL { get; set; }

        public AmazonSQSConfigurationOptions amazonSQSConfigurationOptions { get; set; }
        
        public AmazonS3ConfigurationOptions amazons3ConfigurationOptions { get; set; }
    }
}
