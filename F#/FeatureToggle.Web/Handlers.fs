namespace FeatureToggleWeb

open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.FSharp.Core
open Giraffe
open Giraffe.Razor
open FeatureToggle.Definitions
open Models

module Handlers =

    let indexHandler (repo:IFeatureRepository) = 
        let features = repo.Select("")
        razorHtmlView "Index" (Some features) None

    let updateFeature (repo:IFeatureRepository) = 
        fun (next: HttpFunc) (ctx : HttpContext) ->
            task {
                let model = ctx.BindFormAsync<FeatureModel>().Result
                (repo.Set model.Feature model.Value) |> ignore
            } |> ignore
            (indexHandler repo) next ctx

