module TestData

open FeatureToggle.Definitions
open FeatureToggle.DAL.DiskStorage

let goodFeatures = [ 
    { Feature="FeatureToggle.Config1"; Value ="8" } 
    { Feature="FeatureToggle.Color"; Value ="black" } 
    { Feature="OtherFont.Root"; Value ="Helvetica" } 
    { Feature="Boolean.Value"; Value ="false" } 
]

let goodDataRepo = {
    readFile = fun () -> goodFeatures
    writeFile = fun _ -> ()
}

let notExistingRepo = { 
    goodDataRepo with readFile = fun() -> [] 
}

