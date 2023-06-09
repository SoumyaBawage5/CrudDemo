namespace CrudDemo.Models
{
    public class updatecontact
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public long Phone { get; set; } 
        public string Address { get; set; } = null!;

        public IFormFile PngFile { get; set; } = null!;
        public string filePath { get; internal set; }
    }
}
