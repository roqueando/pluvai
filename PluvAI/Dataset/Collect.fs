module PluvAI.Dataset.Collect

open System.IO
open System.Net.Http

let getDataset =
    task {
        use client = new HttpClient()
        let! response =
            client.GetByteArrayAsync("https://portal.inmet.gov.br/uploads/dadoshistoricos/2019.zip")
        do! File.WriteAllBytesAsync("./data/2019.zip", response)
    }
    |> Async.AwaitTask
    |> Async.RunSynchronously