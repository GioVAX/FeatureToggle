
namespace FeatureToggle.Definitions
{
    public class FeatureConfiguration
    {
        public FeatureConfiguration(string feature, string value)
        {
            Feature = feature;
            Value = value;
        }

        public string Feature { get; }

        public string Value { get; set; }
    }
}
