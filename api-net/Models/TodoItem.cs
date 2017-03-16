using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;
using Todo.Models.Atrributes;

namespace Todo.Models
{

    [JsonApiType("todo_items")]
    [BelongsTo(typeof(TodoList), "TodoList")]
    public class TodoItem : BaseModel
    {
        public TodoItem() : base() { }
        public TodoItem(JObject jList) : base(jList) { }
        [BsonElement("content")]
        public string Content { get; set; }

        [BsonElement("completed")]
        public bool Completed { get; set; } = false;

        [BsonElement("todo_list_id")]
        public ObjectId TodoListId { get; set; }

        public override JObject ToJson()
        {
            var attributes = new JObject();
            attributes.Add("content", Content);
            attributes.Add("completed", Completed);

            var data = new JObject();
            if (TodoListId != null)
            {
                data.Add("id", TodoListId.ToString());
                data.Add("type", "todo-lists");
            }
            var dataObj = new JObject();
            dataObj.Add("data", data);
            var relationships = new JObject();
            relationships.Add("todo-list", dataObj);

            var obj = new JObject();
            obj.Add("id", Id.ToString());
            obj.Add("type", "todo_items");
            obj.Add("attributes", attributes);
            obj.Add("relationships", relationships);
            return obj;
        }

        protected override void FromJson(JObject jObj)
        {
            var data = jObj["data"] as JObject;
            base.FromJson(data);
            var attributes = data["attributes"];
            Content = attributes["content"].Value<string>();
            Completed = attributes["completed"].Value<bool>();
            var relationships = data["relationships"]?["todo-list"]?["data"]?.HasValues;
            if (relationships.HasValue && relationships.Value)
            {
                TodoListId = new ObjectId(data["relationships"]["todo-list"]["data"]["id"].Value<string>());
            }
        }
    }
}