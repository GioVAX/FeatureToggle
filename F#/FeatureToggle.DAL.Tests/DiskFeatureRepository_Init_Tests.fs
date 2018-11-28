namespace DiskFeatureRepository_UnitTests

open Xunit
open Microsoft.Extensions.Options
open FeatureToggle.Definitions
open Moq
open FeatureToggle.DAL
open AutoFixture

type Init() =

    let mockOptions filename =
        let mock = Mock<IOptions<FeaturesFileConfiguration>>()
        mock.Setup((fun cfg -> cfg.Value)).Returns(FeaturesFileConfiguration(filename)) |> ignore
        mock.Object

    let initSUT filename =
        DiskFeatureRepository (mockOptions filename) :> IFeatureRepository

    let fixture = Fixture()
    let sut = initSUT (fixture.Create<string>())

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
