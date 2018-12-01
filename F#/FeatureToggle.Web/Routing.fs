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
                    routeCi  "/index" 
                        >=> indexHandler repo
                    routeCi "/DeleteFeature" 
                        >=> setStatusCode 404 >=> text "Not Found"
                        //>=> deleteFeature repo
                        //>=> redirectTo false "/Index"
                ]
            POST >=>
                choose [
                    routeCi "/AddFeature" 
                        >=> setStatusCode 404 >=> text "Not Found"
                        //>=> addNewFeature repo
                        //>=> redirectTo false "/Index"

                    routeCi "/UpdateFeature"
                        >=> updateFeature repo
                        >=> redirectTo false "/Index"
                ]
            setStatusCode 404 >=> text "Not Found" ]    