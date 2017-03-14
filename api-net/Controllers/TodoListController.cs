using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
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
        public IEnumerable<TodoList> Get()
        {
            return _store.GetAll<TodoList>();
        }
        
        // GET api/v1/todo_lists/asldfkjasdklfaj
        [HttpGet("{id}")]
        public TodoList Get(string id)
        {
            return _store.Get<TodoList>(id);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]TodoList list)
        {
            _store.Save(list);
        }

        // Patch api/v1/todo_lists/asldfkjasdklfaj
        [HttpPatch("{id}")]
        public TodoList Patch(string id, [FromBody]IDictionary<string, object> patch)
        {
            var item = _store.Get<TodoList>(id);
            if(patch.ContainsKey("title")){
                item.Title = (String)patch["title"];
            }
            if(patch.ContainsKey("description")) {
                item.Description = (String)patch["description"];
            }
            _store.Save(item);
            return item;
        }

        // DELETE api/v1/todo_lists/asldfkjasdklfaj
        [HttpDelete("{id}")]
        public TodoList Delete(string id)
        {
            var item = _store.Get<TodoList>(id);
            item = _store.Delete<TodoList>(item);
            return item;
        }
    }
}
