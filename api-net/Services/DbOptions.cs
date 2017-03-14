namespace Todo.Services {
    public class DbOptions {
        public string DbName {get; set;} = "todo_dev";
        public string DbConnString {get; set;} = "localhost:27017";
    }
}