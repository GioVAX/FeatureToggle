[<AutoOpen>]
module Test_Commons

open System
open FeatureToggle.Definitions
open NHamcrest.Core

let private featureIsValid fc = 
    not (String.IsNullOrWhiteSpace(fc.Feature) && String.IsNullOrWhiteSpace(fc.Value))

let private startsWith (pattern:string) fc  = fc.Feature.StartsWith pattern

let private notStartWith pattern = startsWith pattern >> not

let private allMatch pred = 
    CustomMatcher<obj>(
        "All elements should match predicate",
        fun c -> match c with
                 | :? list<_> as l -> l |> List.forall pred
                 | :? array<_> as a -> a |> Array.forall pred
                 | :? seq<_> as s -> s |> Seq.forall pred
                 | :? System.Collections.IEnumerable as e -> e |> Seq.cast |> Seq.forall pred
                 | _ -> false)

let only = id

let containValidFeatures = allMatch (fun f -> featureIsValid f)

let containStartingWith str = allMatch (startsWith str)

let containNothingStartingWith str = allMatch (notStartWith str)
