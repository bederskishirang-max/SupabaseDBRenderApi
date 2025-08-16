namespace PostSQLgreAPI.Model
{
 
        public class RegisterRequest
        {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // 👈 add password
        public IFormFile? ProfileImage { get; set; }
         }
    
}
