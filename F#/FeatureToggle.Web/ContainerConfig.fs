module ContainerConfig

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open FeatureToggle.DAL
open FeatureToggle.Definitions
open Microsoft.Extensions.Options

let registerAppServices (container:ServiceProvider) (services:IServiceCollection) =
    let configuration = container.GetService<IConfiguration>()
    
    services
        .AddTransient<IFeatureRepository>( 
            fun cnt -> 
                let options = cnt.GetService<IOptions<FeaturesFileConfiguration>>()
                DiskFeatureRepository.createRepository options.Value
        )
        //.AddSingleton<IFeatureRepository>( DiskFeatureRepository.createRepository )
        .Configure<FeaturesFileConfiguration>(configuration) |> ignore

