open System
open Dataset.Collect

[<EntryPoint>]
let main _ =
    printfn "downloading dataset..."
    
    getDataset
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> extractFile
    |> merge
     
    printfn "dataset... collected!"
    Environment.Exit(0)
    0
