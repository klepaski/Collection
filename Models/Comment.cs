using ToyCollection.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToyCollection.Models
{
    public class Comment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }

        public string ItemId { get; set; }
        public Item Item { get; set; }

        public string UserId { get; set; }
        public UserModel User { get; set; }
    }
}
