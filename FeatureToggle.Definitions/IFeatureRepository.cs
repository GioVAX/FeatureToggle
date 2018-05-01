using System.Collections.Generic;

namespace FeatureToggle.Definitions
{
    public interface IFeatureRepository
    {
        IEnumerable<FeatureConfiguration> Select( string pattern);
        void Delete(string featureName);
        IEnumerable<FeatureConfiguration> Update(string featureName, string newValue);
    }
}
