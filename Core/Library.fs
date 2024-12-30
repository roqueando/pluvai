namespace Core

module Adapter =
    type QueryError =
        | QueryError = 0
        | ConnectionError = 1
        | AuthError = 2
        
    type IDataPipeline<'T> =
        abstract enrichData: date: string -> Result<'T, QueryError>
        
    let runPipeline (dataPipe: IDataPipeline<'T>) (date: string): Result<'T, QueryError> =
        dataPipe.enrichData date
