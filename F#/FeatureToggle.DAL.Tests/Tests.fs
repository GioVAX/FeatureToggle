module Tests

open System
open Xunit
open System.IO
open Microsoft.Extensions.Options
open FeatureToggle.Definitions
open Moq
open FeatureToggle.DAL
open System.Collections
open AutoFixture

type DiskFeatureRepositoryUnitTests() =
    let srcFileName = "Test.json"
    let destFileName = "Test_tmp.json"
    do File.Copy(srcFileName, destFileName, true)

    let mockOptions filename =
        let mock = Mock<IOptions<FeaturesFileConfiguration>>()
        mock.Setup((fun cfg -> cfg.Value)).Returns(FeaturesFileConfiguration(filename)) |> ignore
        mock.Object

    let sut = DiskFeatureRepository (mockOptions destFileName) :> IFeatureRepository
    let fixture = Fixture()

    //[<Fact>]
    //let ``DiskFeatureRepository Should Implement IFeatureRepository`` () =
    //    Assert.IsAssignableFrom( IFeatureRepository, sut)

    [<Fact>]
    let ``Init With Null String SHOULD Return Empty List Of Features``() =
        let sut = DiskFeatureRepository (mockOptions null) :> IFeatureRepository
        Assert.Empty(sut.Select(""))

    [<Fact>]
    let ``Init With Non Exisiting File SHOULD Return Empty List Of Features``() =
        let sut = DiskFeatureRepository (mockOptions (fixture.Create<string>())) :> IFeatureRepository
        Assert.Empty(sut.Select(""))

    //[<Fact>]
    //let ``Select Should return list of features`` () =
    //    Assert.IsAssignableFrom( FeatureConfiguration list, sut.Select(""))
    
    [<Fact>]
    let ``Select with no Pattern SHOULD return 4 valid features``() =
        let fs = sut.Select ""
        Assert.Equal(fs.Length, 4)
        Assert.Empty(fs |> List.filter (fun fc -> String.IsNullOrWhiteSpace(fc.Feature) || String.IsNullOrWhiteSpace(fc.Value)))

    [<Fact>]
    let ``Select with "FeatureToggle" SHOULD return 2 features starting with "FeatureToggle"``() =
        let pattern = "FeatureToggle"
        let fs = sut.Select pattern

        Assert.Equal(fs.Length, 2)
        Assert.Empty(fs |> List.filter (fun fc -> not(fc.Feature.StartsWith pattern)))

    [<Fact>]
    let ``Delete exisiting feature SHOULD remove the feature``() =
        let featureName = "OtherRoot.Font"

        sut.Delete featureName

        let fs = sut.Select ""

        Assert.Equal(fs.Length, 3)
        Assert.Empty(fs |> List.filter (fun fc -> fc.Feature.StartsWith featureName))

    //[<Fact>]
    //let ``Delete non exisiting feature SHOULD throw exception``() =
    //    let featureName = "adjflsdj.sdfsfsd"
        
    //    Assert.Throws( KeyNotFoundException, (fun _ -> sut.Delete featureName) )

    [<Fact>]
    let ``Delete feature SHOULD be persisted immediately``() =
        let featureName = "OtherRoot.Font"

        sut.Delete featureName

        let repo = DiskFeatureRepository (mockOptions destFileName) :> IFeatureRepository
        let fs = repo.Select ""

        Assert.Equal(fs.Length, 3)
        Assert.Empty(fs |> List.filter (fun fc -> fc.Feature.StartsWith featureName))

    //Update_EmptyFeatureName_ShouldThrow
    //Update_NullFeatureName_ShouldThrow

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
        Assert.Empty(actualFeatures |> List.except expectedFeatures ) 

    //UpdateUnknownFeature_ShouldThrowShowingFeatureName

    [<Fact>]
    let ``Update SHOULD be persisted immediately``() =
        let featureName = "OtherRoot.Font"
        let newValue = fixture.Create<string>()

        sut.Update featureName newValue |> ignore

        let repo = DiskFeatureRepository (mockOptions destFileName) :> IFeatureRepository
        let fs = repo.Select featureName

        Assert.Equal(fs.Length, 1)
        Assert.Equal(fs.Head.Value, newValue)

    // Add_InvalidFeatureName_ShouldThrow

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
        Assert.True(actual.Head.Feature.Equals(featureName) && actual.Head.Value.Equals(newValue))

    [<Fact>]
    let ``Add SHOULD be persisted immediately``() =
        let featureName = fixture.Create<string>()
        let newValue = fixture.Create<string>()

        sut.Add featureName newValue

        let repo = DiskFeatureRepository (mockOptions destFileName) :> IFeatureRepository
        let fs = repo.Select featureName

        Assert.Equal(fs.Length, 1)
        Assert.True(fs.Head.Feature.Equals(featureName) && fs.Head.Value.Equals(newValue))



    interface IDisposable with
        member this.Dispose() =
            File.Delete( destFileName )