using ToyCollection.Areas.Identity.Data;
using Microsoft.VisualBasic.FileIO;
using DevExpress.Xpo;

namespace ToyCollection.Models
{
    public class Collection : XPCustomObject 
    {
        public object this[string propertyName]
        {
            get { return ClassInfo.GetMember(propertyName).GetValue(this); }
            set { ClassInfo.GetMember(propertyName).SetValue(this, value); }
        }

        public Guid Id { get; set; }
        public UserModel? User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Theme { get; set; }
        public string? ImageUrl { get; set; }
        public List<Item> Items { get; set; } = new();

        public string? CustomString1 { get; set; }
        public string? CustomString2 { get; set; }
        public string? CustomString3 { get; set; }

        public string? CustomInt1 { get; set; }
        public string? CustomInt2 { get; set; }
        public string? CustomInt3 { get; set; }

        public string? CustomText1 { get; set; }
        public string? CustomText2 { get; set; }
        public string? CustomText3 { get; set; }

        public string? CustomBool1 { get; set; }
        public string? CustomBool2 { get; set; }
        public string? CustomBool3 { get; set; }

        public string? CustomDate1 { get; set; }
        public string? CustomDate2 { get; set; }
        public string? CustomDate3 { get; set; }

        //public List<Field> Fields { get; set; } //Поля каждого айтема
    }

    //public class Field
    //{
    //    public Guid Id { get; set; }
    //    public Guid CollectionId { get; set; }
    //    public string Name { get; set; }
    //    public FieldType Type { get; set; }
    //}

    //public enum FieldType
    //{
    //    Integer, //3 цч поля
    //    String, //3 строк поля
    //    Text, //3 многостр. текста
    //    Boolean, //3 да/нет чекбокса
    //    Date //3 поля даты
    //}
}
