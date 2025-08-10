using System.ComponentModel.DataAnnotations;

namespace PostSQLgreAPI.Model
{
    public class ForLogin
    {

        [MaxLength(255)]
        public string username { get; set; }


        public string password { get; set; }
    }
}
