namespace FeatureToggle.Definitions

type IFeatureRepository =
    abstract member Select : string -> FeatureConfiguration list
    abstract member Delete : string -> FeatureConfiguration list
    abstract member Set : string -> string -> FeatureConfiguration list
