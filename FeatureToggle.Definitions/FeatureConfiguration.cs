using System;
using System.Collections.Generic;
using System.Text;

namespace FeatureToggle.Definitions
{
    public class FeatureConfiguration
    {
        public FeatureConfiguration(string feature, string value)
        {
            Feature = feature;
            Value = value;
        }

        public string Feature { get; set; }

        public string Value { get; set; }
    }
}
