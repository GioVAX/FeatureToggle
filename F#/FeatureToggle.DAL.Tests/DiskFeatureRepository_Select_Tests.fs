namespace DiskFeatureRepository_UnitTests

open System
open Xunit
open System.IO
open FeatureToggle.Definitions
open FeatureToggle.DAL.DiskStorage
open FeatureToggle.DAL.DiskFeatureRepository
open AutoFixture

type Select() =

    let fixture = Fixture()
    let srcFileName = "Test.json"
    let destFileName = fixture.Create<string>() + ".json"
    do File.Copy(srcFileName, destFileName, true)

    let initSUT filename =
        createRepository (createDiskStoreage (FeaturesFileConfiguration(filename)))

    let sut = initSUT destFileName

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

    interface IDisposable with
        member this.Dispose() =
            File.Delete( destFileName )