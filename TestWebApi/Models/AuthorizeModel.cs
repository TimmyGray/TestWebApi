using System.ComponentModel.DataAnnotations;
namespace TestWebApi.Models
{
    public class AuthorizeModel
    {
        [Required(ErrorMessage ="Поле не должно быть пустым")]
        [StringLength(20)]
        public string Login { get; set; }

        [Required(ErrorMessage ="Поле не должно быть пустым")]
        [StringLength(20)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
