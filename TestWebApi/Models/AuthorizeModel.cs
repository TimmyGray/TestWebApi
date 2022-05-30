using System.ComponentModel.DataAnnotations;
namespace TestWebApi.Models
{
    public class AuthorizeModel
    {
        [Required(ErrorMessage ="Неверные логин или пароль")]
        public string Login { get; set; }

        [Required(ErrorMessage ="Неверные логин или пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
