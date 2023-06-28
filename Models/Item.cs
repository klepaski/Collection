using ToyCollection.Areas.Identity.Data;
using DevExpress.Xpo;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToyCollection.Models
{
    public class Item : XPCustomObject
    {
        public object this[string propertyName]
        {
            get { return ClassInfo.GetMember(propertyName).GetValue(this); }
            set { ClassInfo.GetMember(propertyName).SetValue(this, value); }
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }    //

        public string CollectionId { get; set; }
        public Collection Collection { get; set; }

        public string UserId { get; set; }
        public UserModel User { get; set; }

        public List<Tag> Tags { get; set; } = new();
        public List<Comment>? Comments { get; set; } = new();
        public List<Like>? Likes { get; set; } = new();

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
    }
}
