namespace FeatureToggle.DAL

open System
open System.Collections.Generic
open FeatureToggle.Definitions
open DiskStorage

module DiskFeatureRepository =

    let private splitWhen pred =
        let rec loop acc = function
            | [] -> None
            | x::xs when pred x -> Some (List.rev acc, x::xs)
            | x::xs -> loop (x::acc) xs
        loop []

    let private matches fname = (fun feature -> feature.Feature = fname)

    let private IRepository_Select (storage:DiskStorage) pattern = 
        let features = storage.readFile()
        match pattern with
            | null | "" -> features
            | ptrn -> features |> List.filter (fun p -> p.Feature.StartsWith( ptrn, StringComparison.InvariantCultureIgnoreCase))
    
    let private IRepository_Delete storage featureName =
        let features = storage.readFile()
        
        let split = features |> splitWhen (matches featureName)
        let newFeatures = match split with
                            | None -> raise (KeyNotFoundException "FeatureName not found")
                            | Some (first, last) -> List.concat [first; List.tail last]

        storage.writeFile newFeatures
        newFeatures

    let private IRepository_Set storage featureName newValue =
        match featureName with
        | "" | null -> raise (ArgumentException "FeatureName cannot be empty")
        | _ ->  
            let features = storage.readFile()
            let split = features |> splitWhen (matches featureName)
            let newFeatures = match split with
                                | None -> {Feature = featureName; Value = newValue}::features
                                | Some (first, head::tail) -> List.concat [first; [ {head with Value = newValue}]; tail]
            storage.writeFile newFeatures
            newFeatures

    let createRepository storage = {
        Select = IRepository_Select storage
        Delete = IRepository_Delete storage
        Set = IRepository_Set storage
    }
