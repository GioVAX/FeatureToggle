namespace FeatureToggleWeb

module Models =

    [<CLIMutable>]
    type FeatureModel = {
        Feature : string
        Value : string
    }
