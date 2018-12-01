namespace FeatureToggleWeb

module Routing =

    open System
    open Microsoft.Extensions.DependencyInjection
    open Giraffe
    open FeatureToggle.Definitions
    open Handlers

    let routingDefinitions (services:IServiceProvider) : HttpHandler =

        let repo = services.GetService<IFeatureRepository>()
        choose [
            GET >=>
                choose [
                    route  "/index" >=> indexHandler repo
                    route "/DeleteFeature" >=> setStatusCode 404 >=> text "Not Found" //deleteFeature
                ]
            POST >=>
                choose [
                    route "/AddFeature" >=> setStatusCode 404 >=> text "Not Found" //addNewFeature
                    route "/UpdateFeature" >=> updateFeature repo
                ]
            setStatusCode 404 >=> text "Not Found" ]    