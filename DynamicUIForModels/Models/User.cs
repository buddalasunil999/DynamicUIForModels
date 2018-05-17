namespace DynamicUIForModels.Models
{
    public class User
    {
        public int Id { set; get; }
        public string Name { set; get; }

        public Address AddressOfUser { set; get; }
    }

    public class Address
    {
        public string City { set; get; }
        public string Country { set; get; }
        public Zip Postal { set; get; }
    }

    public class Zip
    {
        public string Code { set; get; }
        public string Value { set; get; }
    }
}