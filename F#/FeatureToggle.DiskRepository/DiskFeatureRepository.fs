namespace FeatureToggle.DAL

open System
open System.Collections.Generic
open System.IO
open FeatureToggle.Definitions
open Newtonsoft.Json
open DiskStorage

module DiskFeatureRepository =

    let featuresPath (options:FeaturesFileConfiguration) = 
        match options.FeaturesConfigurationFile with
            | null | "" -> @".\Features.json"
            | file -> Path.GetFullPath(file)
    
    let loadConfigurationFile options =
        let path = featuresPath options
        if not(File.Exists(path)) then
            []
        else
            let json = File.ReadAllText(path)
            List.ofSeq (JsonConvert.DeserializeObject<List<FeatureConfiguration>>(json))

    let writeConfigurationFile options features =
        let json = JsonConvert.SerializeObject(features)
        let path = featuresPath options
        File.WriteAllText(path, json)

    let splitAt f =
        let rec loop acc = function
            | [] -> List.rev acc,[]
            | x::xs when f x -> List.rev acc, x::xs
            | x::xs -> loop (x::acc) xs
        loop []

    let matches fname = (fun feature -> feature.Feature = fname)

    let private IRepository_Select (storage:DiskStorage) pattern = 
        let features = storage.readFile()
        match pattern with
            | null | "" -> features
            | ptrn -> features |> List.filter (fun p -> p.Feature.StartsWith( ptrn, StringComparison.InvariantCultureIgnoreCase))
    
    let private IRepository_Delete storage featureName =
        let features = storage.readFile()
        
        let split = features |> splitAt (matches featureName)
        let newFeatures = match split with
                            | list,[] -> raise (KeyNotFoundException "FeatureName not found")
                            | first, last -> List.concat [first; List.tail last]

        storage.writeFile newFeatures
        newFeatures

    let private IRepository_Set storage featureName newValue =
        match featureName with
        | "" | null -> raise (ArgumentException "FeatureName cannot be empty")
        | _ ->  
            let features = storage.readFile()
            let split = features |> splitAt (matches featureName)
            let newFeatures = match split with
                                | list,[] -> {Feature = featureName; Value = newValue}::list
                                | first, last -> List.concat [first; [ {List.head last with Value = newValue}]; List.tail last]
            storage.writeFile newFeatures
            newFeatures

    let createRepository storage = {
        Select = IRepository_Select storage
        Delete = IRepository_Delete storage
        Set = IRepository_Set storage
    }
