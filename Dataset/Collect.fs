module Dataset.Collect

open System
open System.IO
open System.IO.Compression
open System.Net.Http
open Deedle
open Microsoft.Data.Analysis
open Microsoft.ML
open Microsoft.ML.Data

type Weather =
    {
        [<LoadColumn(0)>] date: string option
        [<LoadColumn(1)>] hour: string option
        [<LoadColumn(2)>] rain: float32 option
        [<LoadColumn(4)>] pmax: float32 option
        [<LoadColumn(5)>] pmin: float32 option
        [<LoadColumn(9)>] tmax: float32 option
        [<LoadColumn(10)>] tmin: float32 option
        [<LoadColumn(11)>] dpmax: float32 option
        [<LoadColumn(12)>] dpmin: float32 option
        [<LoadColumn(13)>] hmax: float32 option
        [<LoadColumn(14)>] hmin: float32 option
    }

let defaultDir =
    [|Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); "data"|]
    |> Path.Combine

let getDataset =
    task {
        // TODO: change 2019 for the year
        let destination =
            [|Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); "data"; "2019.zip"|]
            |> Path.Combine

        if File.Exists(destination) then
            printfn "dataset already exists!"
            return destination
        else
            printfn "downloading dataset..."
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

    let directoryLength =
        Directory.EnumerateFiles destination
        |> Seq.length

    if (Directory.Exists destination && directoryLength > 0) then
        printfn "files already extracted!"
        [|destination; "2019"|]
        |> Path.Combine
    else
        printfn "extracting file..."
        ZipFile.ExtractToDirectory (filePath, destination) |> ignore
        printfn "file extracted successfully!"

        [|destination; "2019"|]
        |> Path.Combine

let merge (dirPath: string) =

    let files =
        Directory.EnumerateFiles dirPath
        |> Seq.toList
        
    let df =
        Seq.map (fun x -> Path.Combine [|dirPath; x|]) files
        |> Seq.map (fun p ->
            use fileStream = new FileStream(p, FileMode.Open, FileAccess.Read)
            use reader = new StreamReader(fileStream, System.Text.Encoding.GetEncoding("ISO-8859-1"))
            
            for _ in 0..8 do
                reader.ReadLine() |> ignore
                
            use stream = new MemoryStream()
            reader.BaseStream.CopyTo(stream)
            stream.Position <- 0L
            
            Frame.ReadCsv (
                stream,
                hasHeaders=false,
                separators=";",
                inferTypes=false,
                missingValues=[|""|],
                culture="pt-BR",
                schema="R?*"
            )
            //DataFrame.LoadCsv (csvStream=stream, separator=';', header=false)
            )
       |> Frame.mergeAll
        //|> Seq.reduce (fun acc next ->
        //        let f1 = acc.IndexRowsOrdinally()
        //        let f2 = next.IndexRowsOrdinally()
        //        
        //        f1.Join(f2, kind=JoinKind.Inner, lookup=Lookup.Exact)
        //    )
        
    df.Print()
        
        
    //printfn $"%s{df.Preview().ToString()}"
        //|> Seq.reduce (fun acc next -> acc.Merge(next, leftJoinColumns=[||], rightJoinColumns=[||]))

    //printf $"%s{df.Preview().ToString()}"
    (*
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
    *)
    ()
