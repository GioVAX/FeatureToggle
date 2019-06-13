module InitTests

open FeatureToggle.Definitions
open FeatureToggle.DAL.DiskFeatureRepository
open AutoFixture
open FsUnit.Xunit
open Xunit

let private initSUT filename =
    createRepository TestData.notExistingRepo

let private fixture = Fixture()

[<Theory>]
[<InlineData "">]
[<InlineData null>]
let ``Init with no config SHOULD return empty list of features`` fileName =
    (initSUT fileName).Select "" 
        |> should be Empty

[<Fact>]
let ``Init With Non Exisiting File SHOULD Return Empty List Of Features``() =
    (initSUT (fixture.Create<string>())).Select "" 
        |> should be Empty
