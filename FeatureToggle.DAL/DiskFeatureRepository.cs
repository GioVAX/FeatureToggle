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
        private readonly string _filepath;
        private readonly List<FeatureConfiguration> _features;

        public DiskFeatureRepository(string filepath)
        {
            _filepath = filepath;
            _features = LoadConfigurationFile();
        }

        private List<FeatureConfiguration> LoadConfigurationFile()
        {
            if (_filepath == null || !File.Exists(_filepath))
                return new List<FeatureConfiguration>();

            var json = File.ReadAllText(_filepath);
            return JsonConvert.DeserializeObject<List<FeatureConfiguration>>(json);
        }

        private void WriteConfigurationFile()
        {
            var json = JsonConvert.SerializeObject(_features);
            File.WriteAllText(_filepath, json);
        }

        public IEnumerable<FeatureConfiguration> Select(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return _features;

            return _features.Where(pair => pair.Feature.StartsWith(pattern, StringComparison.InvariantCultureIgnoreCase));
        }

        public void Delete(string featureName)
        {
            if (_features.RemoveAll(configuration => string.Equals(configuration.Feature, featureName, StringComparison.InvariantCultureIgnoreCase)) == 0)
                throw new KeyNotFoundException($"Feature <{featureName}> is not configured.");

            WriteConfigurationFile();
        }
    }
}
