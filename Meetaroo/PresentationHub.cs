using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Meetaroo
{
    public class Presentation : Hub
    {
        public async Task JoinPresentation(string presentationId){
            Console.WriteLine("Join presentation " + presentationId);
            //Tell this client the current page number
            //Clients.Client(Context.ConnectionId).InvokeAsync("SetPage",currentPage);
            await Groups.AddAsync(Context.ConnectionId,presentationId);
        }

        public Task SetCurrentPage(int currentPage,string presentationId){
            Console.WriteLine("Set current page " + currentPage + " for "+ presentationId);
            
            //maybe need a permission check here...
            //save it in db.
            //tell all the clients
            return Clients.Group(presentationId).InvokeAsync("SetPage",currentPage); 
        }
    }
}