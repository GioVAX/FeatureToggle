namespace FeatureToggleWeb

module Routing =

    open System
    open Microsoft.Extensions.DependencyInjection
    open Microsoft.Extensions.Options
    open Giraffe
    open FeatureToggle.DAL
    open FeatureToggle.Definitions
    open Handlers

    let routingDefinitions (services: IServiceProvider) : HttpHandler =

        choose [
            GET >=>
                choose [
                    route "/" >=> indexHandler ("hello", "world")
                    routef "/%s/%s" indexHandler
                    route  "/razor" >=> razorHandler
                    route  "/index" >=> ftListHandler (services.GetService<IFeatureRepository>())
                ]
            setStatusCode 404 >=> text "Not Found" ]