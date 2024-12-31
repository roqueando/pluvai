namespace FeatureExtractor

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Core.Adapter
open Core.Entity
open External.Mongo

module Program =

    [<EntryPoint>]
    let main args =
        let builder = Host.CreateApplicationBuilder(args)
        builder.Services.AddSingleton<Settings<List<EnrichedWeather>>>(fun _ -> {
            DataPipeline = MongoImpl()
        }) |> ignore
        builder.Services.AddHostedService<Worker>() |> ignore

        builder.Build().Run()

        0 // exit code
