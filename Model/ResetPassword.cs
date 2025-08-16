namespace PostSQLgreAPI.Model
{
    public class ResetPassword
    {
        public int UserId { get; set; }       // User to reset
        public string NewPassword { get; set; } // New password
    }
}
