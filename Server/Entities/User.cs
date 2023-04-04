using System.ComponentModel.DataAnnotations;

namespace Server
{
    public class User
    {
        [Key]
        public string Login { get; set; }
        public string Password { get; set; }
    }
}