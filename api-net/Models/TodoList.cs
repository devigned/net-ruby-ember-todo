using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;
using Todo.Models.Atrributes;

namespace Todo.Models
{

    [JsonApiType("todo_lists")]
    [HasMany(typeof(TodoItem), "TodoItems")]
    public class TodoList : BaseModel
    {

        public TodoList() : base() { }
        public TodoList(JObject jList) : base(jList) { }

        [BsonElement("title")]
        public string Title { get; set; }
        [BsonElement("description")]
        public string Description { get; set; }

        public IList<TodoItem> TodoItems { get; set; } = new List<TodoItem>();

        public override JObject ToJson()
        {
            var attributes = new JObject();
            attributes.Add("title", Title);
            attributes.Add("description", Description);

            var data = new JArray();
            data.Add(TodoItems.Select(i =>
            {
                var jObj = new JObject();
                jObj.Add("id", i.Id.ToString());
                jObj.Add("type", "todo-items");
                return jObj;
            }));
            var dataObj = new JObject();
            dataObj.Add("data", data);
            var relationships = new JObject();
            relationships.Add("todo-items", dataObj);

            var obj = new JObject();
            obj.Add("id", Id.ToString());
            obj.Add("type", "todo_lists");
            obj.Add("attributes", attributes);
            obj.Add("relationships", relationships);
            return obj;
        }

        protected override void FromJson(JObject jList)
        {
            var data = jList["data"] as JObject;
            base.FromJson(data);
            var attributes = data["attributes"];
            Title = attributes["title"].Value<string>();
            Description = attributes["description"].Value<string>();
        }
    }
}