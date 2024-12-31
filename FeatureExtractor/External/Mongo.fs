module FeatureExtractor.External.Mongo

open Core.Adapter

open MongoDB.Driver
open System.Collections.Generic
open System.Linq
open Core.Entity

type MongoImpl() =
    interface IDataPipeline<List<EnrichedWeather>> with
        member _.enrichData (_: string) =
            try
                // FIXME: get this by env vars
                let client = new MongoClient("mongodb://pluvius:local_password@localhost:27017")
                let database = client.GetDatabase("feature_store")
                let collection =
                    database
                        .GetCollection<Weather>("raw")
                        .AsQueryable()

                let query =
                    query {
                        for w in collection do
                            where (w.date = "2019/01/01") // FIXME: use date
                            select {
                                date = w.date
                                hour = w.hour
                                rain = w.rain
                                pmax = w.pmax / 10.0
                                pmin = w.pmin / 10.0
                                tmax = w.tmax
                                tmin = w.tmin
                                dpmax = w.dpmax
                                dpmin = w.dpmin
                                hmax = w.hmax
                                hmin = w.hmin
                                pdiff = w.pmax - w.pmin
                                tdiff = w.tmax - w.tmin
                                dpdiff = w.dpmax - w.dpmin
                                hdiff = w.hmax - w.hmin
                            }
                    }
                let results = query.ToList()
                let newCollection = database.GetCollection<EnrichedWeather>("enriched")

                if results.Count > 0 then
                    newCollection.InsertMany(results)

                Ok(results)
            with
            | :? MongoAuthenticationException as _ -> Error QueryError.AuthError

    member this.enrichData (param: string) =
        (this :> IDataPipeline<List<EnrichedWeather>>).enrichData param

