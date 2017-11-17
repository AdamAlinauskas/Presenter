using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using DataAccess;
using Service;
using System.Text.RegularExpressions;

public class RetrieveSchemaActionFilter : ActionFilterAttribute
{   
    private readonly IServiceProvider serviceProvider;
    // WARNING : This will only work on single dotted TLDs (e.g. .com but not .co.uk)
    // Doing this properly requires either knowing our hostname (e.g. findecks.com) or maintaining a list of TLDs
    // See github.com/peerigon/parse-domain for an example
    private readonly Regex hostnameRegex = new Regex(@"^(?<schema>[^\.]+)\.(?<domain>[^\.]+)\.(?<tld>[^\.]+)$");

    public RetrieveSchemaActionFilter(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }
    
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var confirmSchemaExists = serviceProvider.GetService<IConfirmSchemaExists>();
        var currentSchema = serviceProvider.GetService<ICurrentSchema>();
        
        var hostname = context.HttpContext.Request.Host.Host;
        var parts = hostnameRegex.Match(hostname);
        Console.WriteLine("--- hostname = " + hostname);
        Console.WriteLine("--- is match = " + parts.Success);
        

        var schemaName = parts.Success ? parts.Groups["schema"].Value : null;
        Console.WriteLine("--- schema = " + schemaName);
        var exists =  confirmSchemaExists.For(schemaName);

        //Really we should redirect some where... but this will do for now.
        if(!exists.Result) {
            throw new Exception("Schema missing or does not exist");
        }

        Console.WriteLine($"The schema is {schemaName}");
        currentSchema.Name = schemaName;
    }
}