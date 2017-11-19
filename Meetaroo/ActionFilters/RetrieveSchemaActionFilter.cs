using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using DataAccess;
using Service;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

public class RetrieveSchemaActionFilter : ActionFilterAttribute
{   
    private readonly IServiceProvider serviceProvider;
    // WARNING : This will only work on single dotted TLDs (e.g. .com but not .co.uk)
    // Doing this properly requires either knowing our hostname (e.g. findecks.com) or maintaining a list of TLDs
    // See github.com/peerigon/parse-domain for an example
    private readonly Regex hostnameRegex = new Regex(@"^(?<schema>[^\.]+)\.(?<host>[^\.]+\.[^\.]+)$");

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

        if (parts.Success)
        {
            var schemaName = parts.Groups["schema"].Value;
            var schemaExists = confirmSchemaExists.For(schemaName);
            
            if (!schemaExists.Result) {
                schemaName = null;
            }

            currentSchema.Name = schemaName;
            currentSchema.Host = parts.Groups["host"].Value;

            var controller = context.Controller as Controller;
            if (controller != null)
            {
                // TODO : This is a great candidate for caching
                var organizations = serviceProvider.GetService<IOrganizationRepository>();
                var organization = organizations.Fetch(schemaName);
                controller.ViewData["OrganizationName"] = organization.Name;
            }
        }
        else 
        {
            currentSchema.Host = hostname;
        }
    }
}