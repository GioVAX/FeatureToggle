module SetTests

open System
open Xunit
open FeatureToggle.Definitions
open FeatureToggle.DAL.DiskFeatureRepository
open AutoFixture
open FsUnit.Xunit

let fixture = Fixture()
    
let initSUT = createRepository TestData.goodDataRepo
    
let sut = initSUT

[<Theory>]
[<InlineData "">]
[<InlineData null>]
let ``Set with invalid feature name SHOULD throw exception`` featureName =
    let value = fixture.Create<string>()
    (fun () -> (sut.Set featureName value) |> ignore)
        |> should throw typeof<ArgumentException>

[<Fact>]
let ``Set for an existing feature SHOULD not change number of features``() =
    let origCount = "" |> sut.Select |> List.length
        
    let fs = sut.Set "OtherFont.Root" (fixture.Create<string>())

    fs |> should haveLength origCount

[<Fact>]
let ``Set of an exisiting feature SHOULD contain the same features``() =
    let extractFeatureNames = List.map (fun f -> f.Feature)
    let expectedFeatures = "" |> sut.Select |> extractFeatureNames

    let fs = sut.Set "OtherFont.Root" (fixture.Create<string>())

    let actualFeatures = fs |> extractFeatureNames
    actualFeatures |> should equal expectedFeatures

[<Fact>]
let ``Set of an exisiting feature SHOULD change the value of the feature``() =
    let featureName = "OtherFont.Root"
    let newValue = fixture.Create<string>()

    let fs = sut.Set featureName newValue

    fs |> should contain {Feature = featureName; Value = newValue}

[<Fact>]
let ``Set SHOULD be persisted immediately``() =
    let featureName = "OtherRoot.Font"
    let newValue = fixture.Create<string>()

    sut.Set featureName newValue |> ignore

    let repo = initSUT
    let fs = repo.Select featureName

    fs |> should equal [{Feature = featureName; Value = newValue}]
  
[<Fact>]
let ``Set of a new feature SHOULD add one new feature``() =
    let originalCount = sut.Select("").Length
    let featureName = fixture.Create<string>()
    let newValue = fixture.Create<string>()

    let fs = sut.Set featureName newValue

    fs |> should haveLength (originalCount + 1)

[<Fact>]
let ``Set of a new feature SHOULD add the new feature``() = 
    let featureName = fixture.Create<string>()
    let newValue = fixture.Create<string>()

    let actual = sut.Set featureName newValue

    actual |> should contain {Feature = featureName; Value = newValue}
