﻿module DiskFeatureRepository_Select_UnitTests

open System
open Xunit
open System.IO
open Microsoft.Extensions.Options
open FeatureToggle.Definitions
open Moq
open FeatureToggle.DAL
open AutoFixture

type DiskFeatureRepository_Select_UnitTests() =

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

    interface IDisposable with
        member this.Dispose() =
            File.Delete( destFileName )