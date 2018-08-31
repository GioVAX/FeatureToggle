namespace FeatureToggle.Definitions

type FeaturesFileConfiguration (featuresConfigurationFile: string) = 
    member this.FeaturesConfigurationFile = featuresConfigurationFile

    new () = FeaturesFileConfiguration("")