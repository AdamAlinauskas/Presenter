using System;
using System.Threading.Tasks;
using DataAccess;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Service;

namespace Meetaroo
{
    public class Presentation : Hub
    {
        private readonly IUpdatePresentationCurrentPage updatePresentationCurrentPage;
        private readonly ICurrentSchema currentSchema;
        private readonly IPresentationCurrentPageQuery presentationCurrentPage;
        private readonly IUpdatePresentationStatusCommand updatePresentationStatusCommand;

        public Presentation(IUpdatePresentationCurrentPage updatePresentationCurrentPage, ICurrentSchema currentSchema, IPresentationCurrentPageQuery presentationCurrentPage, IUpdatePresentationStatusCommand updatePresentationStatusCommand)
        {
            this.updatePresentationCurrentPage = updatePresentationCurrentPage;
            this.currentSchema = currentSchema;
            this.presentationCurrentPage = presentationCurrentPage;
            this.updatePresentationStatusCommand = updatePresentationStatusCommand;
        }

        public async Task JoinPresentation(string schema, long presentationId, string presentationGroupId)
        {
            currentSchema.Name = schema;
            var currentPageNumber = await presentationCurrentPage.Fetch(presentationId);
            //Tell this client the current page number
            await Clients.Client(Context.ConnectionId).InvokeAsync("SetPage", currentPageNumber);

            await Groups.AddAsync(Context.ConnectionId, presentationGroupId);
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

        public async Task ChangePresentationStatusToStarted(string schema, long presentationId, string presentationGroupId)
        {
            currentSchema.Name = schema;
            await updatePresentationStatusCommand.Execute(presentationId, PresentationStatus.InProgress);
            await Clients.Group(presentationGroupId).InvokeAsync("StatusChangedTo", (int)PresentationStatus.InProgress);
        }
    }
}


