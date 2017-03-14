using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Todo.Models;

namespace Todo.Services {

    public interface IStoreService{
        T Save<T>(T item) where T: BaseModel;
        T Delete<T>(T item) where T: BaseModel;
        T Get<T>(string id) where T: BaseModel;

        IList<T> GetAll<T>() where T: BaseModel;

        IMongoQueryable<T> GetQueryable<T>() where T: BaseModel;
    }

    public class StoreException : Exception {
        public StoreException(string message): base(message){}
    }

    public class StoreService : IStoreService {
        
        private readonly DbOptions _options;
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _db;

        public StoreService(IOptions<DbOptions> options) {
            _options = options.Value;
            var connString = _options.DbConnString;
            if (!connString.Contains("mongodb://")) { connString = "mongodb://" + connString; }
            _client = new MongoClient(connString);
            _db = _client.GetDatabase(_options.DbName);
        }

        public IList<T> GetAll<T>() where T: BaseModel 
        {
            return GetQueryable<T>().ToList();
        }

        public T Delete<T>(T item) where T: BaseModel
        {
            try {
                if(!String.IsNullOrEmpty(item.Id)) {
                    Builders<T>.Filter.Eq(i => i.Id, item.Id);
                    var deleteResult = GetCollection<T>().DeleteOne(Builders<T>.Filter.Eq(i => i.Id, item.Id));
                    if(deleteResult.DeletedCount > 1) {
                        throw new StoreException($"Deleted {deleteResult.DeletedCount}!! This should only delete one record.");
                    }
                }
            } catch(StoreException ex) {
                item.Errors.Add(ex.Message);
            } catch(MongoException ex) {
                item.Errors.Add(ex.Message);
            }
           
            return item;
        }

        public T Save<T>(T item)  where T: BaseModel {
            try {
                if(String.IsNullOrEmpty(item.Id)) {
                    GetCollection<T>().InsertOne(item);
                } else {
                    Builders<T>.Filter.Eq(i => i.Id, item.Id);
                    var replaceResult = GetCollection<T>().ReplaceOne(Builders<T>.Filter.Eq(i => i.Id, item.Id), item);
                    if(replaceResult.ModifiedCount != 1) {
                        throw new StoreException($"Failed to replace document with Id: {item.Id}");
                    }
                }
            } catch(StoreException ex) {
                item.Errors.Add(ex.Message);
            } catch(MongoException ex) {
                item.Errors.Add(ex.Message);
            }
           
            return item;
        }

        public T Get<T>(string id) where T: BaseModel {
            return GetQueryable<T>().Where(item => item.Id == id).FirstOrDefault();
        }

        public IMongoQueryable<T> GetQueryable<T>() where T: BaseModel {
            return GetCollection<T>().AsQueryable();
        }

        private IMongoCollection<T> GetCollection<T>() where T: BaseModel {
            return _db.GetCollection<T>(typeof(T).Name);
        }
    }
}