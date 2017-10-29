
using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Service;

public class RetrieveSchemaActionFilter : ActionFilterAttribute
{   
    private readonly IServiceProvider serviceProvider;

    public   RetrieveSchemaActionFilter(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var confirmSchemaExists = serviceProvider.GetService<IConfirmSchemaExists>();
        var currentSchema = serviceProvider.GetService<ICurrentSchema>();
        
        var schemaName = (string)context.RouteData.Values["schema"];
        var exists =  confirmSchemaExists.For(schemaName);
        //Really we should redirect some where... but this will do for now.
        if(!exists.Result){
            throw new Exception("Schema missing or does not exist");
        }
        Console.WriteLine($"The schema is {schemaName}");
        currentSchema.Name = schemaName;
    }
}