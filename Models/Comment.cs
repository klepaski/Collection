using ToyCollection.Areas.Identity.Data;

namespace ToyCollection.Models
{
    public class Comment
    {
        public Guid Id { get; set; }

        public Guid ItemId { get; set; } //внеш ключ
        public Item Item { get; set; } //навиг св-во

        //public Guid UserModelId { get; set; }
        public UserModel? User { get; set; }

        public DateTime Date { get; set; }
        public string Text { get; set; }
    }
}
