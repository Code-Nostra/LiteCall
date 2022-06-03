using System.ComponentModel.DataAnnotations;

namespace AuthorizationServ
{
    public class StandingRoomDB
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Password { get; set; }

        private bool _guard;
        public bool Guard {
            get
            {
                _guard = Password == string.Empty || Password == null ? false : true;
                return _guard ;
            }
            set
            {
                _guard = value;
            }
        }
    }
}