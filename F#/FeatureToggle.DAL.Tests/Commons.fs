[<AutoOpen>]
module Test_Commons

open System
open FeatureToggle.Definitions

let featureIsValid fc = 
    not (String.IsNullOrWhiteSpace(fc.Feature) && String.IsNullOrWhiteSpace(fc.Value))

let featureNameStartsWith (pattern:string) fc  = fc.Feature.StartsWith pattern

let featureNameDoesNotStartWith (pattern:string) fc  = not (featureNameStartsWith pattern fc)


