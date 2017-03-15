using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;

namespace Todo.Models
{
    public abstract class BaseModel
    {

        public BaseModel(){}
        public BaseModel(JObject jList){
            FromJson(jList);
        }

        public List<string> Errors { get; set; } = new List<string>();
        public ObjectId Id { get; set; }

        public T FromJson<T>(JObject jobject) where T : BaseModel
        {
            return default(T);
        }

        public JObject AsJson<T>(T item) where T : BaseModel
        {
            return default(JObject);
        }

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }

        public abstract JObject ToJson();
        protected virtual void FromJson(JObject jList){
            Id = new ObjectId(jList["id"].Value<string>());
        }
    }
}