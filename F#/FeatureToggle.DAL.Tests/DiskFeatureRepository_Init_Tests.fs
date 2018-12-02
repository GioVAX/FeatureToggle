namespace DiskFeatureRepository_UnitTests

open Xunit
open FeatureToggle.Definitions
open FeatureToggle.DAL.DiskStorage
open FeatureToggle.DAL.DiskFeatureRepository
open AutoFixture

type Init() =

    let initSUT filename =
        createRepository (createDiskStoreage  (FeaturesFileConfiguration(filename)))

    let fixture = Fixture()

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
