namespace ToyCollection.Models
{
    public class Tag
    {
        public string Name { get; set; }
        public List<Item>? Items { get; set; } = new();
    }
}