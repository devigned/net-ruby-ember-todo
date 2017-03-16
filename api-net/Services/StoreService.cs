using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Todo.Models;
using Todo.Models.Atrributes;
using Humanizer;

namespace Todo.Services
{

    public interface IStoreService
    {
        T Save<T>(T item) where T : BaseModel;
        T Delete<T>(T item) where T : BaseModel;
        T Get<T>(ObjectId id) where T : BaseModel;

        IList<T> GetAll<T>() where T : BaseModel;

        IMongoQueryable<T> GetQueryable<T>() where T : BaseModel;
    }

    public class StoreException : Exception
    {
        public StoreException(string message) : base(message) { }
    }

    public class StoreService : IStoreService
    {

        private readonly DbOptions _options;
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _db;

        public StoreService(IOptions<DbOptions> options)
        {
            _options = options.Value;
            var connString = _options.DbConnString;
            if (!connString.Contains("mongodb://")) { connString = "mongodb://" + connString; }
            _client = new MongoClient(connString);
            _db = _client.GetDatabase(_options.DbName);
        }

        public IList<T> GetAll<T>() where T : BaseModel
        {
            return GetQueryable<T>().ToList().Select(i => FillHasMany(i)).ToList();
        }

        public T Delete<T>(T item) where T : BaseModel
        {
            try
            {
                if (item.Id != null)
                {
                    Builders<T>.Filter.Eq(i => i.Id, item.Id);
                    var deleteResult = GetCollection<T>().DeleteOne(Builders<T>.Filter.Eq(i => i.Id, item.Id));
                    if (deleteResult.DeletedCount > 1)
                    {
                        throw new StoreException($"Deleted {deleteResult.DeletedCount}!! This should only delete one record.");
                    }
                }
            }
            catch (StoreException ex)
            {
                item.Errors.Add(ex.Message);
            }
            catch (MongoException ex)
            {
                item.Errors.Add(ex.Message);
            }

            FillHasMany(item);
            return item;
        }

        public T Save<T>(T item) where T : BaseModel
        {
            try
            {
                if (item.Id == ObjectId.Empty)
                {
                    GetCollection<T>().InsertOne(item);
                }
                else
                {
                    Builders<T>.Filter.Eq(i => i.Id, item.Id);
                    var replaceResult = GetCollection<T>().ReplaceOne(Builders<T>.Filter.Eq(i => i.Id, item.Id), item);
                    if (replaceResult.ModifiedCount != 1)
                    {
                        throw new StoreException($"Failed to replace document with Id: {item.Id}");
                    }
                }
            }
            catch (StoreException ex)
            {
                item.Errors.Add(ex.Message);
            }
            catch (MongoException ex)
            {
                item.Errors.Add(ex.Message);
            }

            FillHasMany(item);
            return item;
        }

        public T Get<T>(ObjectId id) where T : BaseModel
        {
            return GetQueryable<T>().Where(item => item.Id == id).ToList().Select(i => FillHasMany(i)).FirstOrDefault();
        }

        public IMongoQueryable<T> GetQueryable<T>() where T : BaseModel
        {
            return GetCollection<T>().AsQueryable();
        }

        private IMongoCollection<T> GetCollection<T>() where T : BaseModel
        {
            var attr = typeof(T).GetTypeInfo().GetCustomAttribute<JsonApiTypeAttribute>();
            return _db.GetCollection<T>(attr.TypeName);
        }

        private T FillHasMany<T>(T item) where T : BaseModel
        {
            var typeInfo = typeof(T).GetTypeInfo();
            var fk = typeInfo.GetCustomAttribute<JsonApiTypeAttribute>().TypeName.Singularize() + "_id";
            var hasManyAttrs = typeInfo.GetCustomAttributes<HasManyAttribute>();
            if (hasManyAttrs.Any())
            {
                foreach (var hasManyAttr in hasManyAttrs)
                {
                    var hasManyTypeName = hasManyAttr.Type.GetTypeInfo().GetCustomAttribute<JsonApiTypeAttribute>().TypeName;
                    var prop = typeInfo.GetProperty(hasManyAttr.PropertyName);
                    var listType = typeof(List<>).MakeGenericType(hasManyAttr.Type);
                    var list = Activator.CreateInstance(listType);
                    var listAddMethod = listType.GetMethod("Add");
                    var children = _db.GetCollection<BsonDocument>(hasManyTypeName).AsQueryable().Where(doc => doc[fk] == item.Id);
                    var deserialized = new List<object>();
                    foreach (var child in children)
                    {
                        deserialized.Add(BsonSerializer.Deserialize(child, hasManyAttr.Type, null));
                    }
                    deserialized.ForEach(i =>
                    {
                        listAddMethod.Invoke(list, new object[] { Convert.ChangeType(i, hasManyAttr.Type) });
                    });
                    prop.SetValue(item, list);
                }
            }
            return item;
        }
    }
}