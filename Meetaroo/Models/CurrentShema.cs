public class CurrentSchema : ICurrentSchema {
    public string Name {get;set;}
}

public interface ICurrentSchema
{
    string Name {get;set;}
}