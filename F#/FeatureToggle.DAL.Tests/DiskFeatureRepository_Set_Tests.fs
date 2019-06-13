module SetTests

open System
open Xunit
open System.IO
open FeatureToggle.Definitions
open FeatureToggle.DAL.DiskFeatureRepository
open AutoFixture
open FsUnit.Xunit
open FeatureToggle.DAL

type Set() =
    
    let fixture = Fixture()
    
    let srcFileName = "Test.json"
    let destFileName = fixture.Create<string>() + ".json"
    do File.Copy(srcFileName, destFileName, true)

    let initSUT filename =
        //createRepository TestData.goodDataRepo
        createRepository (DiskStorage.createDiskStoreage  (FeaturesFileConfiguration(filename)))
    
    let sut = initSUT destFileName

    [<Theory>]
    [<InlineData "">]
    [<InlineData null>]
    let ``Set with invalid feature name SHOULD throw exception`` featureName =
        let value = fixture.Create<string>()
        (fun () -> (sut.Set featureName value) |> ignore)
            |> should throw typeof<ArgumentException>

    [<Fact>]
    let ``Set for an existing feature SHOULD not change number of features``() =
        let featureName = "OtherRoot.Font"
        let origCount = sut.Select("").Length
        
        let fs = sut.Set featureName (fixture.Create<string>())

        fs |> should haveLength origCount

    [<Fact>]
    let ``Set of an exisiting feature SHOULD contain the same features``() =
        let featureName = "OtherRoot.Font"

        let extractFeatureNames(list:FeatureConfiguration list) =
            list |> List.map (fun f -> f.Feature)
        
        let expectedFeatures = sut.Select("") |> extractFeatureNames

        let fs = sut.Set featureName (fixture.Create<string>())

        let actualFeatures = fs |> extractFeatureNames

        actualFeatures |> should equal expectedFeatures

    [<Fact>]
    let ``Set SHOULD be persisted immediately``() =
        let featureName = "OtherRoot.Font"
        let newValue = fixture.Create<string>()

        sut.Set featureName newValue |> ignore

        let repo = initSUT destFileName
        let fs = repo.Select featureName

        fs |> should haveLength 1
        fs.Head.Value
            |> should equal newValue
  
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

        let fs = sut.Set featureName newValue

        let actual = sut.Select(featureName)

        actual |> should haveLength 1
        actual.Head |> should equal {Feature = featureName; Value = newValue}

    interface IDisposable with
        member this.Dispose() =
            File.Delete( destFileName )