namespace FeatureToggle.DAL

open System
open System.Collections.Generic
open System.IO
open FeatureToggle.Definitions
open Microsoft.Extensions.Options
open Newtonsoft.Json

type DiskFeatureRepository ( options : IOptions<FeaturesFileConfiguration> (*, logger : ILogger<DiskFeatureRepository>*) )  =

    let optionsFeaturesFileName = options |> function
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

        member this.Add featureName newValue = 
            match featureName with
                | "" | null -> raise (ArgumentException "FeatureName cannot be empty")
                | _ -> features <- FeatureConfiguration(featureName,newValue) :: features
            WriteConfigurationFile()
    
        member this.Update featureName newValue =
            let rec upd n v (fl:FeatureConfiguration list) = 
                match fl with
                | [] -> []
                | head::tail -> if head.Feature = n then
                                    FeatureConfiguration(head.Feature, newValue) :: tail
                                else
                                    head :: (upd n v tail)

            match featureName with
            | "" | null -> raise (ArgumentException "FeatureName cannot be empty")
            | _ -> if features |> List.exists (fun c -> c.Feature = featureName) then
                        features <- features |> upd featureName newValue
                        WriteConfigurationFile()
                    else
                        raise (ArgumentException "Feature not defined")
            features
