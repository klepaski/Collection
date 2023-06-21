namespace ToyCollection.Models
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Item> Items { get; set; } = new();
    }
    //ItemTag - промежут таблица сама сгенер (мн:мн)
}
