using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FeatureToggle.Definitions;
using Newtonsoft.Json;

namespace FeatureToggle.DAL
{
    public class DiskFeatureRepository : IFeatureRepository
    {
        List<FeatureConfiguration> _features;

        public DiskFeatureRepository(string filepath)
        {
            var json = File.ReadAllText(filepath);
            _features = JsonConvert.DeserializeObject<List<FeatureConfiguration>>(json);
        }

        public IEnumerable<FeatureConfiguration> Select(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return _features;

            return _features.Where(pair => pair.Feature.StartsWith(pattern, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
