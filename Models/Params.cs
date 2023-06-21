namespace ToyCollection.Models
{
    public struct Params
    {
        public string name;
        public string description;
        public string theme;
        public string? customString1;
        public string? customString2;
        public string? customString3;
        public string? customInt1;
        public string? customInt2;
        public string? customInt3;
        public string? customText1;
        public string? customText2;
        public string? customText3;
        public string? customBool1;
        public string? customBool2;
        public string? customBool3;
        public string? customDate1;
        public string? customDate2;
        public string? customDate3;

        public Params(string name, string description, string theme,
                string customString1, string customString2, string customString3,
                string customInt1, string customInt2, string customInt3,
                string customText1, string customText2, string customText3,
                string customBool1, string customBool2, string customBool3,
                string customDate1, string customDate2, string customDate3)
        {
            this.name = name;
            this.description = description;
            this.theme = theme;
            this.customString1 = customString1;
            this.customString2 = customString2;
            this.customString3 = customString3;
            this.customInt1 = customInt1;
            this.customInt2 = customInt2;
            this.customInt3 = customInt3;
            this.customText1 = customText1;
            this.customText2 = customText2;
            this.customText3 = customText3;
            this.customBool1 = customBool1;
            this.customBool2 = customBool2;
            this.customBool3 = customBool3;
            this.customDate1 = customDate1;
            this.customDate2 = customDate2;
            this.customDate3 = customDate3;
        }
    }
}
