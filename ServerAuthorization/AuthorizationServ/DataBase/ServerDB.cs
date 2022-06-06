using System.ComponentModel.DataAnnotations;

namespace AuthorizationServ
{
    public class ServerDB
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
    }
}