module Dataset.Collect

open System
open System.IO
open System.IO.Compression
open System.Net.Http
open Deedle

let defaultDir =
    [|Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); "data"|]
    |> Path.Combine

let getDataset =
    task {
        // TODO: change 2019 for the year
        let folderArray = [|Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); "data"; "2019.zip"|]

        let destination =
            folderArray
            |> Path.Combine
            
        use client = new HttpClient()
        let! response = client.GetByteArrayAsync("https://portal.inmet.gov.br/uploads/dadoshistoricos/2019.zip")
            
        do! File.WriteAllBytesAsync(destination, response)
        return destination
    }

let extractFile (filePath: string) =
    let destination =
        [|Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); "data"|]
        |> Path.Combine
        
    if not (File.Exists filePath) then
        failwithf $"The zip file %s{filePath} not exists"
    if not (Directory.Exists destination) then
        Directory.CreateDirectory destination |> ignore

    ZipFile.ExtractToDirectory (filePath, destination) |> ignore
    printfn "file extracted successfully!"

    [|destination; "2019"|]
    |> Path.Combine

let merge (dirPath: string) =

    let files = Directory.EnumerateFiles dirPath
    
    let df =
        Seq.map (fun x -> Path.Combine [|dirPath; x|]) files
        |> Seq.map (fun p ->
            use fileStream = new FileStream(p, FileMode.Open, FileAccess.Read)
            use reader = new StreamReader(fileStream)
            
            for _ in 0..7 do
                reader.ReadLine() |> ignore
                
            use stream = new MemoryStream()
            reader.BaseStream.CopyTo(stream)
            stream.Position <- 0L
            
            Frame.ReadCsv (
                stream,
                hasHeaders=true,
                separators=";",
                culture="pt-BR"
            )
        )
        |> Frame.mergeAll

    df.RenameColumns (fun colName ->
        match colName with
        | "Data" -> "date"
        | "Hora UTC" -> "hour"
        | "PRECIPITAÇÃO TOTAL, HORÁRIO (mm)" -> "rain"
        | "PRESSÃO ATMOSFERICA MAX.NA HORA ANT. (AUT) (mB)" -> "pmax"
        | "PRESSÃO ATMOSFERICA MIN. NA HORA ANT. (AUT) (mB)" -> "pmin"
        | "TEMPERATURA MÁXIMA NA HORA ANT. (AUT) (°C)" -> "tmax"
        | "TEMPERATURA MÍNIMA NA HORA ANT. (AUT) (°C)" -> "tmin"
        | "TEMPERATURA ORVALHO MAX. NA HORA ANT. (AUT) (°C)" -> "dpmax"
        | "TEMPERATURA ORVALHO MIN. NA HORA ANT. (AUT) (°C)" -> "dpmin"
        | "UMIDADE REL. MAX. NA HORA ANT. (AUT) (%)" -> "hmax"
        | "UMIDADE REL. MIN. NA HORA ANT. (AUT) (%)" -> "hmin"
        | other -> other
        )

    let convertFloat (value: string) =
        let corrected = value.Replace(",", ".")
        float corrected


    let mapped =
        Frame.mapCols (fun colName series ->
            match colName with
            | "date" -> series.As<float>() :> ISeries<int>
            | "hour" -> series.As<int>() :> ISeries<int>
            | _ -> Series.mapValues convertFloat (series.As<string>()) :> ISeries<int>
        ) df
        
    [|defaultDir; "2019.csv"|]
    |> Path.Combine
    |> mapped.SaveCsv
    ()
