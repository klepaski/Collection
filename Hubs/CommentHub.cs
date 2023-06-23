using Microsoft.AspNetCore.SignalR;
using System.Text;

namespace ToyCollection.Hubs
{
    public class CommentHub : Hub
    {
        public async Task SendComment(string userName, string text, string date, string itemId)
        {

            await Clients.Group(itemId).SendAsync("ReceiveComment", userName, text, date);
        }

        public async Task ChangeLikeCount(int likeCount, string itemId)
        {
            await Clients.OthersInGroup(itemId).SendAsync("ReceiveLikeCount", likeCount);
        }

        public override async Task OnConnectedAsync()
        {
            string? itemId = this.Context.GetHttpContext().Request.Query["itemId"];
            await Groups.AddToGroupAsync(Context.ConnectionId, itemId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string? itemId = this.Context.GetHttpContext().Request.Query["itemId"];
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, itemId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
