using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using Todo.Models;
using Todo.Services;

namespace Todo.Controllers
{
    [Route("api/v1/todo_lists")]
    public class TodoListController : Controller
    {
        private readonly IStoreService _store;

        public TodoListController(IStoreService store) {
            _store = store;
        }

        // GET api/v1/todo_lists
        [HttpGet]
        public JObject Get()
        {
            return ToData(new JArray(_store.GetAll<TodoList>().Select(i => i.ToJson())));
        }
        
        // GET api/v1/todo_lists/asldfkjasdklfaj
        [HttpGet("{id}")]
        public JObject Get(string id)
        {
            return ToData(_store.Get<TodoList>(new ObjectId(id)).ToJson());
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]JObject jList)
        {
            _store.Save(new TodoList(jList));
        }

        // Patch api/v1/todo_lists/asldfkjasdklfaj
        [HttpPatch("{id}")]
        public JObject Patch(string id, [FromBody]IDictionary<string, object> patch)
        {
            var item = _store.Get<TodoList>(new ObjectId(id));
            if(patch.ContainsKey("title")){
                item.Title = (String)patch["title"];
            }
            if(patch.ContainsKey("description")) {
                item.Description = (String)patch["description"];
            }
            _store.Save(item);
            return ToData(item.ToJson());
        }

        // DELETE api/v1/todo_lists/asldfkjasdklfaj
        [HttpDelete("{id}")]
        public JObject Delete(string id)
        {
            var item = _store.Get<TodoList>(new ObjectId(id));
            item = _store.Delete<TodoList>(item);
            return ToData(item.ToJson());
        }

        private JObject ToData(JToken token)
        {
            var data = new JObject();
            data.Add("data", token);
            return data;
        }
    }
}
