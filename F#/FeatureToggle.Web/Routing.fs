namespace FeatureToggleWeb

module Routing =

    open System
    open Microsoft.Extensions.DependencyInjection
    open Giraffe
    open FeatureToggle.Definitions
    open Handlers

    let routingDefinitions (services:IServiceProvider) : HttpHandler =

        choose [
            GET >=>
                choose [
                    route  "/index" >=> indexHandler (services.GetService<IFeatureRepository>())
                ]
            setStatusCode 404 >=> text "Not Found" ]