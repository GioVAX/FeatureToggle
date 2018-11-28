namespace DiskFeatureRepository_UnitTests

open System
open Xunit
open System.IO
open Microsoft.Extensions.Options
open FeatureToggle.Definitions
open Moq
open FeatureToggle.DAL
open AutoFixture
open System.Collections.Generic

type Delete() =

    let fixture = Fixture()
    let srcFileName = "Test.json"
    let destFileName = fixture.Create<string>() + ".json"
    do File.Copy(srcFileName, destFileName, true)

    let mockOptions filename =
        let mock = Mock<IOptions<FeaturesFileConfiguration>>()
        mock.Setup((fun cfg -> cfg.Value)).Returns(FeaturesFileConfiguration(filename)) |> ignore
        mock.Object

    let initSUT filename =
        DiskFeatureRepository (mockOptions filename) :> IFeatureRepository

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

        let repo = DiskFeatureRepository (mockOptions destFileName) :> IFeatureRepository
        let fs = repo.Select ""

        Assert.Equal(fs.Length, 3)
        fs |> List.filter (fun fc -> fc.Feature.StartsWith featureName)
           |> Assert.Empty

    interface IDisposable with
        member this.Dispose() =
            File.Delete( destFileName )