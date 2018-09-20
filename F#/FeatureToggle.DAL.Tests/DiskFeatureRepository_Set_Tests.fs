module DiskFeatureRepository_Set_UnitTests

open System
open Xunit
open System.IO
open Microsoft.Extensions.Options
open FeatureToggle.Definitions
open Moq
open FeatureToggle.DAL
open AutoFixture

type DiskFeatureRepository_Set_UnitTests() =
    
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

    [<Theory>]
    [<InlineData "">]
    [<InlineData null>]
    let ``Update with invalid feature name SHOULD throw exception`` featureName =
        let value = fixture.Create<string>()
        (fun () -> (sut.Update featureName value) |> ignore)
            |> Assert.Throws<ArgumentException>

    [<Fact>]
    let ``Update SHOULD not change number of features``() =
        let featureName = "OtherRoot.Font"
        let origCount = sut.Select("").Length
        
        sut.Update featureName (fixture.Create<string>()) |> ignore

        Assert.Equal(origCount, sut.Select("").Length)

    [<Fact>]
    let ``Update SHOULD contain the same features``() =
        let featureName = "OtherRoot.Font"

        let extractFeatureNames(list:FeatureConfiguration list) =
            list |> List.map (fun f -> f.Feature)
        
        let expectedFeatures = sut.Select("") |> extractFeatureNames

        sut.Update featureName (fixture.Create<string>()) |> ignore

        let actualFeatures = sut.Select("") |> extractFeatureNames
        actualFeatures |> List.except expectedFeatures
            |> Assert.Empty

    [<Fact>]
    let ``Update unknown feature name SHOULD throw exception``() =
        let feature = fixture.Create<string>()
        let value = fixture.Create<string>()
        (fun () -> (sut.Update feature value) |> ignore)
            |> Assert.Throws<ArgumentException>

    [<Fact>]
    let ``Update SHOULD be persisted immediately``() =
        let featureName = "OtherRoot.Font"
        let newValue = fixture.Create<string>()

        sut.Update featureName newValue |> ignore

        let repo = DiskFeatureRepository (mockOptions destFileName) :> IFeatureRepository
        let fs = repo.Select featureName

        Assert.Equal(fs.Length, 1)
        Assert.Equal(fs.Head.Value, newValue)
  
    [<Theory>]
    [<InlineData "">]
    [<InlineData null>]
    let ``Add with invalid feature name SHOULD throw exception`` featureName =
        let value = fixture.Create<string>()
        (fun () -> (sut.Add featureName value) |> ignore)
            |> Assert.Throws<ArgumentException>

    [<Fact>]
    let ``Add SHOULD add one new feature``() =
        let originalCount = sut.Select("").Length
        let featureName = fixture.Create<string>()
        let newValue = fixture.Create<string>()

        sut.Add featureName newValue

        Assert.Equal( originalCount + 1, sut.Select("").Length)

    [<Fact>]
    let ``Add SHOULD add the new feature``() = 
        let featureName = fixture.Create<string>()
        let newValue = fixture.Create<string>()

        sut.Add featureName newValue

        let actual = sut.Select(featureName)
        Assert.Equal( actual.Length, 1)
        (actual.Head.Feature.Equals(featureName) && actual.Head.Value.Equals(newValue))
            |> Assert.True

    [<Fact>]
    let ``Add SHOULD be persisted immediately``() =
        let featureName = fixture.Create<string>()
        let newValue = fixture.Create<string>()

        sut.Add featureName newValue

        let repo = DiskFeatureRepository (mockOptions destFileName) :> IFeatureRepository
        let fs = repo.Select featureName

        Assert.Equal(fs.Length, 1)
        (fs.Head.Feature.Equals(featureName) && fs.Head.Value.Equals(newValue))
            |> Assert.True

    interface IDisposable with
        member this.Dispose() =
            File.Delete( destFileName )