namespace FeatureToggle.Definitions

open System.Collections.Generic

type IFeatureRepository =
    abstract member Select : string -> IEnumerable<FeatureConfiguration>

