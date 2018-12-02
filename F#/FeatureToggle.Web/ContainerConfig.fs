module ContainerConfig

open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open FeatureToggle.DAL
open FeatureToggle.Definitions
open Microsoft.Extensions.Options

let registerAppServices (container:ServiceProvider) (services:IServiceCollection) =
    let configuration = container.GetService<IConfiguration>()
    
    services
        .Configure<FeaturesFileConfiguration>(configuration)
        .AddScoped<DiskStorage.DiskStorage>(
            fun cnt ->
                let options = cnt.GetService<IOptions<FeaturesFileConfiguration>>()
                DiskStorage.createDiskStoreage options.Value
        )
        .AddScoped<IFeatureRepository>( 
            fun cnt -> 
                let storage = cnt.GetService<DiskStorage.DiskStorage>()
                DiskFeatureRepository.createRepository storage
        )
        |> ignore

