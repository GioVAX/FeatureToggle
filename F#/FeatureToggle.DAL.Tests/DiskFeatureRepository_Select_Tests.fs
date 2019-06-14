module SelectTests

open Xunit
open FeatureToggle.DAL.DiskFeatureRepository
open FsUnit.Xunit
open TestData

let private initSUT filename =
    createRepository goodDataRepo

let private sut = initSUT "Test.json"

[<Fact>]
let ``Select with no Pattern SHOULD return 4 valid features``() =
    let fs = sut.Select ""

    fs |> should haveLength 4
    fs |> should only containValidFeatures

[<Fact>]
let ``Select with "FeatureToggle" SHOULD return 2 features starting with "FeatureToggle"``() =
    let pattern = "FeatureToggle"
    
    let fs = sut.Select pattern

    fs |> should haveLength 2
    fs |> should only (containStartingWith pattern)