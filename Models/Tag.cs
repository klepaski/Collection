namespace ToyCollection.Models
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<ItemTag> ItemTags { get; set; }
    }

    public class ItemTag
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public Guid TagId { get; set; }
        public Item Item { get; set; }
        public Tag Tag { get; set; }
    }

}
