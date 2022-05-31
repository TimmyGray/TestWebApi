using System.Text.Json.Serialization;
namespace TestWebApi.Models
{
    public class DbFile
    {
        public int Id { get; set; }
        public string? Name { get; set; }    
        public float? Size { get; set; }

       //public byte[]? Data { get; set; }
        public string? Type { get; set; }

        public string? DataFileId { get; set; }

        public DataFile? File { get; set; }
        
        //public int? UserId { get; set; }
        
        //public User? User { get; set; }

        public string? User { get; set; }
    }
}
