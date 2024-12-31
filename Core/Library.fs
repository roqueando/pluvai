namespace Core

module Entity =
    type Weather =
        {
            date: string
            hour: string
            rain: float 
            pmax: float 
            pmin: float 
            tmax: float 
            tmin: float 
            dpmax: float 
            dpmin: float 
            hmax: float 
            hmin: float 
        }

    type EnrichedWeather =
        {
            date: string
            hour: string
            rain: float 
            pmax: float 
            pmin: float 
            tmax: float 
            tmin: float 
            dpmax: float 
            dpmin: float 
            hmax: float 
            hmin: float 
            pdiff: float
            tdiff: float
            dpdiff: float
            hdiff: float
        }

module Adapter =
    type QueryError =
        | QueryError = 0
        | ConnectionError = 1
        | AuthError = 2
        
    type IDataPipeline<'T> =
        abstract enrichData: date: string -> Result<'T, QueryError>

    type Settings<'a> =
        { 
            DataPipeline: IDataPipeline<'a>
        }
        
    let runPipeline (settings: Settings<'T>) (date: string): Result<'T, QueryError> =
        settings.DataPipeline.enrichData date
