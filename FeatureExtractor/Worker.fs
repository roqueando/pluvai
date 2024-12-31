namespace FeatureExtractor

open System
open System.Threading
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

// TODO: remove this gambiarra and put this into a Settings
open Core.Adapter

// INFO: if I want to change this I simply change for another External implementation
type Worker(logger: ILogger<Worker>, settings: Settings<External.Mongo.MongoImpl>) =
    inherit BackgroundService()

    override _.ExecuteAsync(_: CancellationToken) =
        task {
            logger.LogInformation("[{time}] -- feature_extractor starting", DateTimeOffset.Now)
            match runPipeline settings "2019/01/01" with
            | Error _ -> printfn "something goes wrong"
            | Ok x -> printfn $"%s{x.ToString()}"
        }
