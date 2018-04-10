using System.Collections.Generic;

namespace FeatureToggle.Definitions
{
    public interface IFeatureRepository
    {
        IEnumerable<FeatureConfiguration> Select( string pattern);
    }
}
