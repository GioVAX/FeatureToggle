module Tests

open System
open Xunit
open System.IO
open Microsoft.Extensions.Options
open FeatureToggle.Definitions
open Moq
open FeatureToggle.DAL
open AutoFixture
open System.Collections.Generic

type DiskFeatureRepositoryUnitTests() =
    let srcFileName = "Test.json"
    let destFileName = "Test_tmp.json"
    do File.Copy(srcFileName, destFileName, true)

    let mockOptions filename =
        let mock = Mock<IOptions<FeaturesFileConfiguration>>()
        mock.Setup((fun cfg -> cfg.Value)).Returns(FeaturesFileConfiguration(filename)) |> ignore
        mock.Object

    let initSUT filename =
        DiskFeatureRepository (mockOptions filename) :> IFeatureRepository

    let sut = initSUT destFileName
    let fixture = Fixture()

    [<Fact>]
    let ``DiskFeatureRepository Should Implement IFeatureRepository`` () =
        Assert.True (sut :? IFeatureRepository)

    [<Theory>]
    [<InlineData "">]
    [<InlineData null>]
    let ``Init with no config SHOULD return empty list of features`` fileName =
        (initSUT fileName).Select "" 
            |> Assert.Empty

    [<Fact>]
    let ``Init With Non Exisiting File SHOULD Return Empty List Of Features``() =
        (initSUT (fixture.Create<string>())).Select "" 
            |> Assert.Empty

    //[<Fact>]
    //let ``Select Should return list of features`` () =
    //    Assert.True( sut.Select("") :? FeatureConfiguration list )
    
    [<Fact>]
    let ``Select with no Pattern SHOULD return 4 valid features``() =
        let fs = sut.Select ""
        Assert.Equal(fs.Length, 4)
        fs |> List.filter (fun fc -> String.IsNullOrWhiteSpace(fc.Feature) || String.IsNullOrWhiteSpace(fc.Value))
           |> Assert.Empty

    [<Fact>]
    let ``Select with "FeatureToggle" SHOULD return 2 features starting with "FeatureToggle"``() =
        let pattern = "FeatureToggle"
        let fs = sut.Select pattern

        Assert.Equal(fs.Length, 2)
        fs |> List.filter (fun fc -> not(fc.Feature.StartsWith pattern))
           |> Assert.Empty

    [<Fact>]
    let ``Delete exisiting feature SHOULD remove the feature``() =
        let featureName = "OtherRoot.Font"

        sut.Delete featureName

        let fs = sut.Select ""
        Assert.Equal(fs.Length, 3)
        fs |> List.filter (fun fc -> fc.Feature.StartsWith featureName) 
           |> Assert.Empty

    [<Fact>]
    let ``Delete non exisiting feature SHOULD throw exception``() =
        let featureName = fixture.Create<string>()
        ( fun () -> sut.Delete featureName )
            |> Assert.Throws<KeyNotFoundException>

    [<Fact>]
    let ``Delete feature SHOULD be persisted immediately``() =
        let featureName = "OtherRoot.Font"

        sut.Delete featureName

        let repo = DiskFeatureRepository (mockOptions destFileName) :> IFeatureRepository
        let fs = repo.Select ""

        Assert.Equal(fs.Length, 3)
        fs |> List.filter (fun fc -> fc.Feature.StartsWith featureName)
           |> Assert.Empty

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