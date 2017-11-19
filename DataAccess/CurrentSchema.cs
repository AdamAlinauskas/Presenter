using System;

namespace DataAccess{
    // TODO : This really has the wrong name now that we're storing more information
    public interface ICurrentSchema
    {
        string Name {get;set;}
        string Host {get;set;}
        bool HasSchema { get; }
    }

    public class CurrentSchema : ICurrentSchema
    {
        private string name;
        public string Name {
            get {
                if (name == null)
                {
                    throw new Exception("No schema selected or schema does not exist");
                }
                return name;
            }
            set { name = value; }
        }

        public string Host {get;set;}

        public bool HasSchema => name != null;
    }
}
