namespace FeatureToggle.API.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open FeatureToggle.Definitions

[<Produces("application/json")>]
[<Route( "[action]")>]
type ValuesController (repository : IFeatureRepository) =
    inherit Controller()

    [<HttpGet>]
    member this.GetFeatures( beginningWith : string) =
        OkObjectResult(repository.Select( beginningWith ))
        
