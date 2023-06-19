namespace ToyCollection.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
    }
}
