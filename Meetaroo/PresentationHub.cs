using System;
using System.Threading.Tasks;
using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Service;

namespace Meetaroo
{
    public class Presentation : Hub
    {
        private readonly IUpdatePresentationCurrentPage updatePresentationCurrentPage;
        private readonly ICurrentSchema currentSchema;

        public Presentation(IUpdatePresentationCurrentPage updatePresentationCurrentPage, ICurrentSchema currentSchema)
        {
            this.updatePresentationCurrentPage = updatePresentationCurrentPage;
            this.currentSchema = currentSchema;
        }

        public async Task JoinPresentation(string presentationId)
        {
            Console.WriteLine("Join presentation " + presentationId);
            //Tell this client the current page number
            //Clients.Client(Context.ConnectionId).InvokeAsync("SetPage",currentPage);
            await Groups.AddAsync(Context.ConnectionId, presentationId);
        }

        public async Task SetCurrentPage(string schema, long presentationId, int currentPage, string presentationGroupId)
        {
            currentSchema.Name = schema;
            await updatePresentationCurrentPage.Execute(presentationId, currentPage);

            //maybe need a permission check here...
            //save it in db.
            //tell all the clients
            await Clients.Group(presentationGroupId).InvokeAsync("SetPage", currentPage);
        }
    }
}


