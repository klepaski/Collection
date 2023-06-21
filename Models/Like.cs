using ToyCollection.Areas.Identity.Data;

namespace ToyCollection.Models
{
    public class Like
    {
        public Guid Id { get; set; }

        public Guid ItemId { get; set; }
        public Item? Item { get; set; }

        //public Guid UserModelId { get; set; }
        public UserModel? User { get; set; }
    }
}
