using System.ComponentModel.DataAnnotations;

namespace NProject.Web.ViewModels.Account
{
    public class SimpleRegistration
    {
        [Required]
        public string Email { get; set; }

        [Required, StringLength(255, MinimumLength = 2)]
        public string Password { get; set; }

        [Required, StringLength(10, MinimumLength = 2)]
        public string Name { get; set; }

        [Required, StringLength(50, MinimumLength = 1)]
        public string LastName { get; set; }

        [Required]
        [Range(-24, 24)]
        public byte TimeShiftFromUtc { get; set; }
    }
}