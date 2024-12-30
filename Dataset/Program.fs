open System
open System.IO
open Dataset.Collect
open Deedle

[<EntryPoint>]
let main _ =

   // let path = "/home/osogyian/data/2019/INMET_S_SC_A898_CAMPOS NOVOS_15-02-2019_A_31-12-2019.CSV"

   // use fileStream = new FileStream(path, FileMode.Open, FileAccess.Read)
   // use reader = new StreamReader(fileStream, System.Text.Encoding.GetEncoding("ISO-8859-1"))
   // for _ in 0..7 do
   //     reader.ReadLine() |> ignore

   // use stream = new MemoryStream()
   // reader.BaseStream.CopyTo(stream)
   // stream.Position <- 0L

   // let f = Frame.ReadCsv (
   //     stream,
   //     hasHeaders=true,
   //     separators=";",
   //     inferTypes=false,
   //     culture="pt-BR",
   //     schema="R?*"
   // )
   // f.Print()
    getDataset
    |> Async.AwaitTask
    |> Async.RunSynchronously
    |> extractFile
    |> merge
     
    printfn "dataset... collected!"
    Environment.Exit(0)
    0
