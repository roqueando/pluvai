module Dataset.Collect

open System
open System.IO
open System.Net.Http

let getDataset =
    task {
        let folderArray = [|Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); "data"; "2019.zip"|]
        
        let destination =
            folderArray
            |> Path.Combine
            
        use client = new HttpClient()
        let! response = client.GetByteArrayAsync("https://portal.inmet.gov.br/uploads/dadoshistoricos/2019.zip")
            
        do! File.WriteAllBytesAsync(destination, response)
       // if Directory.Exists(Path.Combine(folderArray[0..1])) then
       // else
       //     printf "\n data folder does not exists in your home! create one"
    }
