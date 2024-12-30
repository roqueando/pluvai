namespace FeatureExtractor

open System
open System.Threading
open System.Threading.Tasks
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

// TODO: remove this gambiarra and put this into a Settings
open External.Mongo

type Worker(logger: ILogger<Worker>) =
    inherit BackgroundService()

    override _.ExecuteAsync(ct: CancellationToken) =
        task {
            logger.LogInformation("[{time}] -- feature_extractor starting", DateTimeOffset.Now)
            while not ct.IsCancellationRequested do
                let m = MongoImpl()
                match m.enrichData "2019/01/01" with
                | Error _ -> printfn "something goes wrong"
                | Ok x -> printfn $"%s{x.ToString()}"
                do! Task.Delay(10000) // TODO: remove this
        }
