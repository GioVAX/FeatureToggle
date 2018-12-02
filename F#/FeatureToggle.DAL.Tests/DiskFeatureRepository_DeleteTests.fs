namespace DiskFeatureRepository_UnitTests

open System
open Xunit
open System.IO
open FeatureToggle.Definitions
open FeatureToggle.DAL
open AutoFixture
open System.Collections.Generic
open FeatureToggle.DAL.DiskStorage
open FeatureToggle.DAL.DiskFeatureRepository

type Delete() =

    let fixture = Fixture()
    let srcFileName = "Test.json"
    let destFileName = fixture.Create<string>() + ".json"
    do File.Copy(srcFileName, destFileName, true)

    let initSUT filename =
        createRepository (createDiskStoreage  (FeaturesFileConfiguration(filename)))

    let sut = initSUT destFileName

    [<Fact>]
    let ``Delete exisiting feature SHOULD remove the feature``() =
        let featureName = "OtherRoot.Font"

        let fs = sut.Delete featureName

        Assert.Equal(fs.Length, 3)
        fs |> List.filter (fun fc -> fc.Feature.StartsWith featureName) 
           |> Assert.Empty

    [<Fact>]
    let ``Delete non exisiting feature SHOULD throw exception``() =
        let featureName = fixture.Create<string>()
        ( fun () -> sut.Delete featureName |> ignore )
            |> Assert.Throws<KeyNotFoundException>

    [<Fact>]
    let ``Delete feature SHOULD be persisted immediately``() =
        let featureName = "OtherRoot.Font"

        sut.Delete featureName |> ignore

        let repo = initSUT destFileName
        let fs = repo.Select ""

        Assert.Equal(fs.Length, 3)
        fs |> List.filter (fun fc -> fc.Feature.StartsWith featureName)
           |> Assert.Empty

    interface IDisposable with
        member this.Dispose() =
            File.Delete( destFileName )