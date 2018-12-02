namespace FeatureToggle.Definitions

type IFeatureRepository  = {
    Select : string -> FeatureConfiguration list
    Delete : string -> FeatureConfiguration list
    Set : string -> string -> FeatureConfiguration list
}
