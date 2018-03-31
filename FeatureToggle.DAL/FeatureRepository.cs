using FeatureToggle.Definitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FeatureToggle.DAL
{
    public class FeatureRepository : IFeatureRepository
    {
        public IEnumerable<KeyValuePair<string, string>> Select(string pattern)
        {
            throw new NotImplementedException();
        }
    }
}
