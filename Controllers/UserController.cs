using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostSQLgreAPI.Data;
//using Supabase.Gotrue;
using PostSQLgreAPI.Model;
using System.Text;
using System.Text.Json;

namespace PostSQLgreAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly Cloudinary _cloudinary;

        public UsersController(AppDbContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        ///// <summary>
        ///// Uploads a profile image and updates the user's image URL in the database.
        ///// </summary>
        //[HttpPost("{userId}/upload-image")]
        //public async Task<IActionResult> UploadProfileImage(int userId, IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file uploaded.");

        //    // Check user exists
        //    var user = await _context.Users.FindAsync(userId);
        //    if (user == null)
        //        return NotFound("User not found.");

        //    // Upload to Cloudinary
        //    var uploadParams = new ImageUploadParams
        //    {
        //        File = new FileDescription(file.FileName, file.OpenReadStream()),
        //        Folder = "profile_images"
        //    };

        //    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        //    if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
        //        return StatusCode((int)uploadResult.StatusCode, "Cloudinary upload failed.");

        //    // Save image URL in database
        //    user.profile_image_url = uploadResult.SecureUrl.ToString();
        //    await _context.SaveChangesAsync();

        //    return Ok(new { imageUrl = user.profile_image_url });
        //}

        [HttpPost("{userId}/upload-image")]
        public async Task<IActionResult> UploadProfileImage(int userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Check user exists
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            // Upload to Cloudinary as PRIVATE
     
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "profile_images",
                Type = "authenticated",  // or "private", depending on your needs
                AccessControl = new List<AccessControlRule>
                {
                    new AccessControlRule
                    {
                        AccessType = AccessType.Token // 👈 use enum, not string
                    }
                }
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                return StatusCode((int)uploadResult.StatusCode, "Cloudinary upload failed.");

            // Save only the PublicId, not the URL
            user.profile_image_url = uploadResult.PublicId;
            await _context.SaveChangesAsync();

            // Generate a signed URL to return immediately
            var signedUrl = _cloudinary.Api.UrlImgUp
                .Transform(new Transformation().Width(200).Height(200).Crop("fill"))
                .Signed(true) // 👈 required for private assets
                .BuildUrl(user.profile_image_url + ".jpg");

            return Ok(new { imageUrl = signedUrl });
        }



        [HttpGet("{userId}/profile-image")]
        public async Task<IActionResult> GetProfileImage(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.profile_image_url))
                return NotFound("Profile image not found.");

            var signedUrl = _cloudinary.Api.UrlImgUp
                .Transform(new Transformation().Width(200).Height(200).Crop("fill"))
                .Signed(true) // 👈 only signed links work for private
                .BuildUrl(user.profile_image_url+ ".jpg");

            return Ok(new { imageUrl = signedUrl });
        }




        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] Users user)
        //{
        //    try
        //    {
        //        // Check for existing email
        //        var emailExists = await _context.Users
        //            .AsNoTracking()
        //            .AnyAsync(u => u.email == user.email);

        //        if (emailExists)
        //            return Conflict(new { message = "Email already registered." });

        //        // Check for existing username
        //        var usernameExists = await _context.Users
        //            .AsNoTracking()
        //            .AnyAsync(u => u.username == user.username);

        //        if (usernameExists)
        //            return Conflict(new { message = "Username already taken." });

        //        // Set created date
        //        user.date_created = DateTime.UtcNow;

        //        // Save user
        //        _context.Users.Add(user);
        //        await _context.SaveChangesAsync();

        //        // 🔹 Trigger Pipedream webhook after successful save
        //        using var httpClient = new HttpClient();
        //        await httpClient.PostAsJsonAsync("https://eoy1schnghwcrpg.m.pipedream.net", new
        //        {
        //            action = "register",
        //            email = user.email,
        //            username = user.username,
        //            //message = $"Welcome {user.username}, your account has been successfully created."
        //        });

        //        return Ok(new
        //        {
        //            id = user.id,
        //            email = user.email,
        //            username = user.username
        //        });
        //    }
        //    catch (DbUpdateException dbEx) when (dbEx.InnerException?.Message?.Contains("duplicate key value") == true)
        //    {
        //        return Conflict(new
        //        {
        //            message = "A duplicate key error occurred. Either the email or username already exists.",
        //            details = dbEx.InnerException.Message
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            message = "An unexpected error occurred.",
        //            details = ex.Message
        //        });
        //    }
        //}




        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromForm] Model.RegisterRequest request)
        //{
        //    try
        //    {
        //        // Check for existing email
        //        if (await _context.Users.AnyAsync(u => u.email == request.Email))
        //            return Conflict(new { message = "Email already registered." });

        //        if (await _context.Users.AnyAsync(u => u.username == request.Username))
        //            return Conflict(new { message = "Username already taken." });

        //        // Create new user
        //        var user = new Users
        //        {
        //            email = request.Email,
        //            username = request.Username,
        //            date_created = DateTime.UtcNow
        //        };

        //        // If file (image) is provided, upload to Cloudinary
        //        if (request.ProfileImage != null && request.ProfileImage.Length > 0)
        //        {
        //            var uploadParams = new ImageUploadParams
        //            {
        //                File = new FileDescription(request.ProfileImage.FileName, request.ProfileImage.OpenReadStream()),
        //                Folder = "profile_images",
        //                Type = "authenticated",
        //                    AccessControl = new List<AccessControlRule>
        //                        {
        //                            new AccessControlRule
        //                            {
        //                                AccessType = AccessType.Token // 👈 use enum, not string
        //                            }
        //                        }
        //            };

        //            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        //            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
        //                return StatusCode((int)uploadResult.StatusCode, "Cloudinary upload failed.");

        //            // Save PublicId instead of URL
        //            user.profile_image_url = uploadResult.PublicId;
        //        }

        //        _context.Users.Add(user);
        //        await _context.SaveChangesAsync();

        //        // Trigger Pipedream webhook
        //        using var httpClient = new HttpClient();
        //        await httpClient.PostAsJsonAsync("https://eoy1schnghwcrpg.m.pipedream.net", new
        //        {
        //            action = "register",
        //            email = user.email,
        //            username = user.username
        //        });

        //        return Ok(new
        //        {
        //            id = user.id,
        //            email = user.email,
        //            username = user.username,
        //            profileImageId = user.profile_image_url
        //        });
        //    }
        //    catch (DbUpdateException dbEx) when (dbEx.InnerException?.Message?.Contains("duplicate key value") == true)
        //    {
        //        return Conflict(new { message = "Duplicate key error.", details = dbEx.InnerException.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Unexpected error.", details = ex.Message });
        //    }
        //}

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassword request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return NotFound("User not found");

            // Hash the new password
            user.password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password reset successfully." });
        }



        public static class PasswordHelper
        {
            public static string HashPassword(string password) =>
                BCrypt.Net.BCrypt.HashPassword(password);

            public static bool VerifyPassword(string password, string hash) =>
                BCrypt.Net.BCrypt.Verify(password, hash);
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] Model.RegisterRequest request)
        {
            try
            {
                // Check for existing email
                if (await _context.Users.AnyAsync(u => u.email == request.Email))
                    return Conflict(new { message = "Email already registered." });

                if (await _context.Users.AnyAsync(u => u.username == request.Username))
                    return Conflict(new { message = "Username already taken." });

                // Create new user with hashed password
                var user = new Users
                {
                    email = request.Email,
                    username = request.Username,
                    password = PasswordHelper.HashPassword(request.Password),
                    date_created = DateTime.UtcNow
                };

                if (request.ProfileImage != null && request.ProfileImage.Length > 0)
                {
                    // Upload profile image to Cloudinary
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(request.ProfileImage.FileName, request.ProfileImage.OpenReadStream()),
                        Folder = "profile_images",
                        Type = "authenticated",
                        AccessControl = new List<AccessControlRule>
                {
                    new AccessControlRule { AccessType = AccessType.Token }
                }
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                        return StatusCode((int)uploadResult.StatusCode, "Cloudinary upload failed.");

                    user.profile_image_url = uploadResult.PublicId; // Save PublicId
                }
                else
                {
                    // 👇 Save default avatar if no upload
                    user.profile_image_url = "images/default-avatar.png";
                    // ⚠️ if you uploaded default-avatar to Cloudinary, use its PublicId instead, e.g. "default-avatar"
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Trigger Pipedream webhook
                using var httpClient = new HttpClient();
                await httpClient.PostAsJsonAsync("https://eoy1schnghwcrpg.m.pipedream.net", new
                {
                    action = "register",
                    email = user.email,
                    username = user.username
                });

                return Ok(new
                {
                    id = user.id,
                    email = user.email,
                    username = user.username,
                    profileImageId = user.profile_image_url
                });
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException?.Message?.Contains("duplicate key value") == true)
            {
                return Conflict(new { message = "Duplicate key error.", details = dbEx.InnerException.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Unexpected error.", details = ex.Message });
            }
        }


        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] ForLogin login)
        //{
        //    var user = await _context.Users
        //        .FirstOrDefaultAsync(u => u.username == login.username && u.password == login.password);
        //    if (user == null) return Unauthorized("Invalid credentials");
        //    return Ok(1);
        //}


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ForLogin login)
        {
            // Find user by username (or email if you want to allow both)
            var user = await _context.Users.FirstOrDefaultAsync(u => u.username == login.username);
            if (user == null)
                return Unauthorized("Invalid credentials");

            // Verify password against hash
            bool validPassword = BCrypt.Net.BCrypt.Verify(login.password, user.password);
            if (!validPassword)
                return Unauthorized("Invalid credentials");

            // If valid, return success (later you can return JWT token)
            return Ok(new
            {
                id = user.id,
                email = user.email,
                username = user.username,
                profileImageId = user.profile_image_url
            });
        }



        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.id,
                    u.username,
                    u.email,
                    u.password
                })
                .ToListAsync();

            return Ok(users);
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Users updated)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.username = updated.username;
            user.email = updated.email;
            user.password = updated.password;
            await _context.SaveChangesAsync();

            // Send to Pipedream webhook
            using var httpClient = new HttpClient();
            await httpClient.PostAsJsonAsync("https://eoy1schnghwcrpg.m.pipedream.net", new
            {
                action = "update",
                email = user.email,
                username = user.username,
                //message = $"Hello {user.username}, your account information was successfully updated."
            });

            return Ok(user);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            // Send to Pipedream webhook
            using var httpClient = new HttpClient();
            await httpClient.PostAsJsonAsync("https://eoy1schnghwcrpg.m.pipedream.net", new
            {
                action = "delete",
                email = user.email,
                username = user.username,
                //message = $"Hello {user.username}, your account has been deleted."
            });

            return Ok(new { message = "User deleted successfully." });
        }


    }

}
