using System;
using System.Collections.Generic;
using System.Text;

namespace FeatureToggle.Definitions
{
    public interface IFeatureRepository
    {
        IEnumerable<FeatureConfiguration> Select( string pattern);
    }
}
