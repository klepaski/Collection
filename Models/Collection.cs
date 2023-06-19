using Microsoft.VisualBasic.FileIO;

namespace ToyCollection.Models
{
    public class Collection
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Theme { get; set; } //Books, Signs, Silverware
        public string? ImageUrl { get; set; }

        public List<Item> Items { get; set; }
        public List<Field> Fields { get; set; } //Поля каждого айтема
    }

    public class Field
    {
        public Guid Id { get; set; }
        public Guid CollectionId { get; set; }
        public string Name { get; set; }
        public FieldType Type { get; set; }
    }

    public enum FieldType
    {
        Integer, //3 цч поля
        String, //3 строк поля
        Text, //3 многостр. текста
        Boolean, //3 да/нет чекбокса
        Date //3 поля даты
    }
}
