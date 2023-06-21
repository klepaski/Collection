using ToyCollection.Areas.Identity.Data;
using DevExpress.Xpo;

namespace ToyCollection.Models
{
    public class Item : XPCustomObject
    {
        public object this[string propertyName]
        {
            get { return ClassInfo.GetMember(propertyName).GetValue(this); }
            set { ClassInfo.GetMember(propertyName).SetValue(this, value); }
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public UserModel? User { get; set; }
        public List<Tag> Tags { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
        public List<Like> Likes { get; set; } = new();
        public Guid CollectionId { get; set; }
        public Collection Collection { get; set; }

        public string? CustomString1 { get; set; }
        public string? CustomString2 { get; set; }
        public string? CustomString3 { get; set; }

        public int? CustomInt1 { get; set; }
        public int? CustomInt2 { get; set; }
        public int? CustomInt3 { get; set; }

        public string? CustomText1 { get; set; }
        public string? CustomText2 { get; set; }
        public string? CustomText3 { get; set; }

        public Boolean? CustomBool1 { get; set; }
        public Boolean? CustomBool2 { get; set; }
        public Boolean? CustomBool3 { get; set; }

        public DateTime? CustomDate1 { get; set; }
        public DateTime? CustomDate2 { get; set; }
        public DateTime? CustomDate3 { get; set; }
        //public List<ItemField> ItemFields { get; set; }
    }

    //public class ItemField
    //{
    //    public Guid Id { get; set; }
    //    public Guid ItemId { get; set; }
    //    public Guid FieldId { get; set; }
    //    public string Value { get; set; }
    //    public Item Item { get; set; } //
    //}
}
