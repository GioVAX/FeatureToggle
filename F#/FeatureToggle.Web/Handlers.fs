namespace FeatureToggleWeb

open System
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging
open Giraffe
open Models
open Views

module Handlers =

    let indexHandler (greet, name) =
        let greetings = sprintf "%s %s, from Giraffe!" greet name
        let model     = { Text = greetings }
        let view      = index model
        htmlView view

    // ---------------------------------
    // Error handler
    // ---------------------------------

    let errorHandler (ex : Exception) (logger : ILogger) =
        logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
        clearResponse >=> setStatusCode 500 >=> text ex.Message