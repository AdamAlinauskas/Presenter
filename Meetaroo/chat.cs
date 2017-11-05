using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Meetaroo
{
    public class Chat : Hub
    {
        public Task Send(string message)
        {
            return Clients.All.InvokeAsync("Send", message);
        }
    }
}