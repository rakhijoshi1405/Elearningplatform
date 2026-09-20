using System.ComponentModel.DataAnnotations;

namespace Elearningplatform.Models // Or ElearningPlatform.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}