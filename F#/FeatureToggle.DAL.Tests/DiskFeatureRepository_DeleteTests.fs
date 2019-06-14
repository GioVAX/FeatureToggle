namespace DeleteTests

open Xunit
open AutoFixture
open System.Collections.Generic
open FeatureToggle.DAL.DiskFeatureRepository
open FsUnit.Xunit
open TestData

type Delete() =

    let fixture = Fixture()

    let initSUT  = createRepository goodDataRepo
    let sut = initSUT

    [<Fact>]
    let ``Delete exisiting feature SHOULD remove the feature``() =
        let featureName = "OtherFont.Root"

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
        let featureName = "OtherFont.Root"

        let _ = sut.Delete featureName

        let repo = initSUT
        let fs = repo.Select ""

        fs |> should haveLength 3
        fs |> should containNothingStartingWith featureName
