namespace DeleteTests

open System
open Xunit
open System.IO
open FeatureToggle.Definitions
open AutoFixture
open System.Collections.Generic
open FeatureToggle.DAL.DiskStorage
open FeatureToggle.DAL.DiskFeatureRepository
open FsUnit.Xunit

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

        fs |> should haveLength 3
        fs |> should containNothingStartingWith featureName

    [<Fact>]
    let ``Delete non exisiting feature SHOULD throw exception``() =
        let featureName = fixture.Create<string>()
        ( fun () -> sut.Delete featureName |> ignore )
            |> should throw typeof<KeyNotFoundException>

    [<Fact>]
    let ``Delete feature SHOULD be persisted immediately``() =
        let featureName = "OtherRoot.Font"

        let _ = sut.Delete featureName

        let repo = initSUT destFileName
        let fs = repo.Select ""

        fs |> should haveLength 3
        fs |> should containNothingStartingWith featureName

    interface IDisposable with
        member this.Dispose() =
            File.Delete( destFileName )