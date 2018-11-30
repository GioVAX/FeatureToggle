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

    let indexHandler (repo:IFeatureRepository) = 
        let handler =
            let features = repo.Select("")
            razorHtmlView "Index" (Some features) None
        handler
