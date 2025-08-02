
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostSQLgreAPI.Model
{
    [Table("users", Schema = "public")]  // Explicitly map to schema and table
    public class Users
    {
        [Key]
        public int id { get; set; }
        public string username { get; set; }
      
        public string password { get; set; }
      
    }
}

