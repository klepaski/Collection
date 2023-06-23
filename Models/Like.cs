using ToyCollection.Areas.Identity.Data;

namespace ToyCollection.Models
{
    public class Like
    {
        public string ItemId { get; set; }
        public Item Item { get; set; }

        public string UserId { get; set; }
        public UserModel User { get; set; }
    }
}
