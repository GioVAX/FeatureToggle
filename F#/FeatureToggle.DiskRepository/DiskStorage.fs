namespace FeatureToggle.DAL

open System.IO
open Newtonsoft.Json
open FeatureToggle.Definitions

module DiskStorage =
    
    type DiskStorage = {
        readFile: unit -> FeatureConfiguration list
        writeFile: FeatureConfiguration list -> unit
    }

    let private featuresPath (options:FeaturesFileConfiguration) = 
        match options.FeaturesConfigurationFile with
            | null | "" -> @".\Features.json"
            | file -> Path.GetFullPath(file)
    
    let private loadConfigurationFile options =
        let path = featuresPath options
        if not(File.Exists(path)) then
            []
        else
            let json = File.ReadAllText(path)
            List.ofSeq (JsonConvert.DeserializeObject<List<FeatureConfiguration>>(json))

    let private writeConfigurationFile options features =
        let json = JsonConvert.SerializeObject(features)
        let path = featuresPath options
        File.WriteAllText(path, json)

    let createDiskStoreage options = {
        readFile = (fun () -> loadConfigurationFile options)
        writeFile = writeConfigurationFile options
    }
