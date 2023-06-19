namespace ToyCollection.Models
{
    public class Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Like> Likes { get; set; }
        public List<Tag> Tags { get; set; }
        public List<ItemField> ItemFields { get; set; }
    }

    public class ItemField
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public Guid FieldId { get; set; }
        public string Value { get; set; }
        public Item Item { get; set; } //
    }
}
