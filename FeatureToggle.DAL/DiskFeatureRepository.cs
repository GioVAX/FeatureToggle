using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FeatureToggle.Definitions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FeatureToggle.DAL
{
    public class DiskFeatureRepository : IFeatureRepository
    {
        private readonly string _filepath;
        private readonly List<FeatureConfiguration> _features;
        private readonly ILogger<DiskFeatureRepository> _logger;

        public DiskFeatureRepository(IOptions<FeaturesFileConfiguration> options, ILogger<DiskFeatureRepository> logger)
        {
            _logger = logger;
            _filepath = options.Value.FeaturesConfigurationFile;
            _features = LoadConfigurationFile();
        }

        private List<FeatureConfiguration> LoadConfigurationFile()
        {
            if (_filepath == null || !File.Exists(_filepath))
            {
                _logger.LogDebug($"File <{_filepath}> does not exist.");
                return new List<FeatureConfiguration>();
            }

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

        public IEnumerable<FeatureConfiguration> Update(string featureName, string newValue)
        {
            if (string.IsNullOrWhiteSpace(featureName))
                throw new ArgumentException("Feature name cannot be empty", nameof(featureName));

            var feature = _features.Find(feat =>
                feat.Feature.Equals(featureName, StringComparison.InvariantCultureIgnoreCase));

            if (feature == null)
                throw new KeyNotFoundException($"Feature <{featureName}> is not configured.");

            feature.Value = newValue;

            WriteConfigurationFile();

            return _features;
        }

        public void Add(string featureName, string newValue)
        {
            if (string.IsNullOrWhiteSpace(featureName))
                throw new InvalidDataException("parameter <featureName> cannot be empty");

            _features.Add(new FeatureConfiguration(featureName, newValue));

            WriteConfigurationFile();
        }
    }
}
