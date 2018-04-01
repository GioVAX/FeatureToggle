using FeatureToggle.Definitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FeatureToggle.DAL
{
    public class FeatureRepository : IFeatureRepository
    {
        List<KeyValuePair<string, string>> _features;

        public FeatureRepository(string filepath)
        {
            var json = File.ReadAllText(filepath);
            _features = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(json);
        }

        public IEnumerable<KeyValuePair<string, string>> Select(string pattern)
        {
            return _features;
        }
    }
}
