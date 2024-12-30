module FeatureExtractor.External.Mongo

open Core.Adapter

open MongoDB.Driver
open MongoDB.Bson

type MongoImpl() =
    interface IDataPipeline<BsonDocument> with
        member this.enrichData (_: string) =
            let client = new MongoClient("mongodb://pluvius:local_password@localhost:27017")
            let collection = client.GetDatabase("feature_store").GetCollection<BsonDocument>("raw")
            let filter = Builders<BsonDocument>.Filter.Eq("date", "2019/01/01")
            let l = collection.Find(filter).First()
            Ok(l)
    member this.enrichData (param: string) =
        (this :> IDataPipeline<BsonDocument>).enrichData param
