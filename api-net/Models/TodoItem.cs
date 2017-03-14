namespace Todo.Models {
    public class TodoItem : BaseModel {
        public string Title {get; set;}
        public string Url {get; set;}
        public string Content {get; set;}
        public bool Completed {get; set;} = false;
        public int order {get; set;}
    }
}