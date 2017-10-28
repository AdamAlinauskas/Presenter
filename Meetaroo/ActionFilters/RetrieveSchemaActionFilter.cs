
using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Service;

public class RetrieveSchemaActionFilter : ActionFilterAttribute
{
    private readonly ICurrentSchema currentSchema;

    public   RetrieveSchemaActionFilter(ICurrentSchema currentSchema)
    {
        this.currentSchema = currentSchema;
    }
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var schemaName = (string)context.RouteData.Values["schema"];
        if(string.IsNullOrWhiteSpace(schemaName) ){
            throw new Exception("Schema expteced");
        }
        Console.WriteLine($"The schema is {schemaName}");
        currentSchema.Name = schemaName;
    }
}