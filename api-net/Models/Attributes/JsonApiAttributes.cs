using System;

namespace Todo.Models.Atrributes {

    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class JsonApiTypeAttribute : System.Attribute
    {
        readonly string _typeName;
        
        public JsonApiTypeAttribute (string typeName)
        {
            _typeName = typeName;
        }
        
        public string TypeName
        {
            get { return _typeName; }
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class HasManyAttribute : System.Attribute
    {
        private readonly Type _type;
        private readonly string _propertyName;

        public HasManyAttribute (Type type, string propertyName)
        {
            _type = type;
            _propertyName = propertyName;
        }
        
        public Type Type
        {
            get { return _type; }
        }

        public String PropertyName 
        {
            get { return _propertyName; }
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class BelongsToAttribute : System.Attribute
    {
        private readonly Type _type;
        private readonly string _propertyName;
        
        public BelongsToAttribute (Type type, string propertyName)
        {
            _type = type;
            _propertyName = propertyName;
        }
        
        public Type Type
        {
            get { return _type; }
        }

        public String PropertyName 
        {
            get { return _propertyName; }
        }
    }
}