using System.Collections.Generic;

namespace Todo.Models {
    public class BaseModel {
        public List<string> Errors {get; set;} = new List<string>();
        public string Id {get; set;}
    }
}