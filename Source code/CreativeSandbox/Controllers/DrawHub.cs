using CreativeSandbox.Models;
using Microsoft.AspNet.SignalR;

namespace CreativeSandbox.Controllers
{
    public class DrawHub : Hub
    {
        public void Send(Data data)
        {
            Clients.AllExcept(Context.ConnectionId).addLine(data);
        }
    }
}