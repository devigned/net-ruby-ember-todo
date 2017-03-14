using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Todo.Models;
using Todo.Services;

namespace Todo.Controllers
{
    [Route("api/v1/[controller]")]
    public class TodoItemController : Controller
    {
        private readonly IStoreService _store;

        public TodoItemController(IStoreService store) {
            _store = store;
        }

        // GET api/v1/todo_lists
        [HttpGet]
        public IEnumerable<TodoItem> Get()
        {
            return _store.GetAll<TodoItem>();
        }

        // GET api/v1/todo_lists/asldfkjasdklfaj
        [HttpGet("{id}")]
        public TodoItem Get(string id)
        {
            return _store.Get<TodoItem>(id);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]TodoItem item)
        {
            _store.Save(item);
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
        public TodoItem Delete(string id)
        {
            return _store.Delete<TodoItem>(_store.Get<TodoItem>(id));
        }
    }
}
