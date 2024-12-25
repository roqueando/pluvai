open System
open System.Threading.Tasks
open Dataset.Collect

[<EntryPoint>]
let main _ =
    printfn "downloading dataset..."
    getDataset
    |> Async.AwaitTask
    |> Async.RunSynchronously
     
    printfn "download dataset... finished!"
    Environment.Exit(0)
    0
