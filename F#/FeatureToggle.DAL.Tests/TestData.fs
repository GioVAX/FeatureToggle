module TestData

open FeatureToggle.Definitions
open FeatureToggle.DAL.DiskStorage

let goodFeatures = [ 
    { Feature="FeatureToggle.Config1"; Value ="8" } 
    { Feature="FeatureToggle.Color"; Value ="black" } 
    { Feature="OtherFont.Root"; Value ="Helvetica" } 
    { Feature="Boolean.Value"; Value ="false" } 
]

let mutable private features = goodFeatures

let goodDataRepo = {
    readFile = fun () -> features
    writeFile = fun newFeatures -> features <- newFeatures  
}

let notExistingRepo = { 
    goodDataRepo with readFile = fun() -> [] 
}

