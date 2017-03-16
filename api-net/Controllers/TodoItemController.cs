using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using Todo.Models;
using Todo.Services;

namespace Todo.Controllers
{
    [Route("api/v1/todo_items")]
    public class TodoItemController : Controller
    {
        private readonly IStoreService _store;

        public TodoItemController(IStoreService store) {
            _store = store;
        }

        // GET api/v1/todo_lists
        [HttpGet]
        public JObject Get()
        {
            return ToData(_store.GetAll<TodoItem>().ToJson());
        }

        // GET api/v1/todo_lists/asldfkjasdklfaj
        [HttpGet("{id}")]
        public JObject Get(string id)
        {
            return ToData(_store.Get<TodoItem>(new ObjectId(id)).ToJson());
        }

        // POST api/values
        [HttpPost]
        public JObject Post([FromBody]JObject item)
        {
            return ToData(_store.Save(new TodoItem(item)).ToJson());
        }

        // Patch api/v1/todo_lists/asldfkjasdklfaj
        [HttpPatch("{id}")]
        public JObject Patch(string id, [FromBody]JObject patch)
        {
            // Really should be only patching, but really, I'm PUT'ing
            var updatedItem = new TodoItem(patch);
            _store.Save(updatedItem);
            return ToData(updatedItem.ToJson());
        }

        // DELETE api/v1/todo_lists/asldfkjasdklfaj
        [HttpDelete("{id}")]
        public JObject Delete(string id)
        {
            return ToData(_store.Delete<TodoItem>(_store.Get<TodoItem>(new ObjectId(id))).ToJson());
        }

        private JObject ToData(JToken token)
        {
            var data = new JObject();
            data.Add("data", token);
            return data;
        }
    }
}
