namespace FeatureToggle.DAL

open System
open System.Collections.Generic
open System.IO
open FeatureToggle.Definitions
open Microsoft.Extensions.Options
open Newtonsoft.Json

type DiskFeatureRepository ( options : IOptions<FeaturesFileConfiguration> (*, logger : ILogger<DiskFeatureRepository>*) )  =

    let optionsFeaturesFileName = 
        match options with
        | null -> null
        | opt -> opt.Value.FeaturesConfigurationFile
    
    let featuresPath = 
        match optionsFeaturesFileName with
            | null | "" -> @".\Features.json"
            | file -> Path.GetFullPath(file)
    
    let LoadConfigurationFile() = 
        //_logger.LogInformation($"Reading configuration from <{_featuresPath}>."); 
        if not(File.Exists(featuresPath)) then
            []
        else
            let json = File.ReadAllText(featuresPath)
            List.ofSeq (JsonConvert.DeserializeObject<List<FeatureConfiguration>>(json))

    let mutable features = LoadConfigurationFile()

    let WriteConfigurationFile() =
        let json = JsonConvert.SerializeObject(features)
        File.WriteAllText(featuresPath, json)

    interface IFeatureRepository with
        member this.Select pattern = 
            match pattern with
                | null | "" -> features
                | ptrn -> features |> List.filter (fun p -> p.Feature.StartsWith( ptrn, StringComparison.InvariantCultureIgnoreCase))

        member this.Delete featureName =
            features |> List.find (fun fc -> fc.Feature = featureName) |> ignore
            features <- (features |> List.filter (fun fc -> not (fc.Feature.StartsWith( featureName, StringComparison.InvariantCultureIgnoreCase))))
            WriteConfigurationFile()
            features

        member this.Set featureName newValue =
            let splitAt f =
                let rec loop acc = function
                    | [] -> List.rev acc,[]
                    | x::xs when f x -> List.rev acc, xs
                    | x::xs -> loop (x::acc) xs
                loop []

            match featureName with
            | "" | null -> raise (ArgumentException "FeatureName cannot be empty")
            | _ ->  let first, last = features |> splitAt (fun feature -> featureName = feature.Feature)
                    features <- List.concat [first; [FeatureConfiguration(featureName,newValue)]; last]
                    WriteConfigurationFile()
            features
