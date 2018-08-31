namespace FeatureToggle.Definitions

type IFeatureRepository =
    abstract member Select : string -> FeatureConfiguration list
    abstract member Delete : string -> unit
    abstract member Update : string -> string -> FeatureConfiguration list
    abstract member Add : string -> string -> unit
