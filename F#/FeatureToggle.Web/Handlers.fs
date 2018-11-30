namespace FeatureToggleWeb

open System
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Logging
open Microsoft.FSharp.Core
open Giraffe
open Giraffe.Razor
open FeatureToggle.Definitions
open Models
open Views

module Handlers =

    let indexHandler (greet, name) =
        let greetings = sprintf "%s %s, from Giraffe!" greet name
        let model     = { Text = greetings }
        let view      = index model
        htmlView view


    let (razorHandler:HttpHandler) = 
        //let repo = FeatureToggle.DAL.DiskFeatureRepository()
        let model = { Title = "Razor View"; Text = "Hello"}
        razorHtmlView "Index" (Some model) None

    let ftListHandler (repo:IFeatureRepository) = 
        let handler =
            let features = repo.Select("")
            razorHtmlView "Index" (Some features) None
        handler
