using Microsoft.AspNetCore.SignalR;
using System.Text;

namespace ToyCollection.Hubs
{
    public class CommentHub : Hub
    {
        public async Task SendComment(string userName, string text, string date)
        {
            await Clients.All.SendAsync("ReceiveComment", userName, text, date);
        }

        public async Task ChangeLikeCount(int likeCount)
        {
            await Clients.Others.SendAsync("ReceiveLikeCount", likeCount);
        }
    }
}
