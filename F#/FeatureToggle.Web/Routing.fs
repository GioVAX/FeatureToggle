namespace FeatureToggleWeb

module Routing =

    open Giraffe
    open FeatureToggle.DAL
    open FeatureToggle.Definitions
    open Handlers
    open Microsoft.Extensions.Configuration

    let (routingDefinitions:HttpHandler) =

        let ffConfig = FeaturesFileConfiguration("Features.json")
        //let config = Configuration.GetSection()

        choose [
            GET >=>
                choose [
                    route "/" >=> indexHandler ("hello", "world")
                    routef "/%s/%s" indexHandler
                    route  "/razor" >=> razorHandler
                    route  "/index" >=> ftListHandler (DiskFeatureRepository( IOptions<FeaturesFileConfiguration>("Features.json")))
                ]
            setStatusCode 404 >=> text "Not " ]