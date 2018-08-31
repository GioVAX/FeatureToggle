namespace FeatureToggle.Definitions

type FeatureConfiguration( feature:string, value: string) =
    member this.Feature = feature
    member this.Value = value
