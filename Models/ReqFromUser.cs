namespace CrudDemo.Models
{
    public class ReqFromUser
    {
       
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public long Phone { get; set; }
        public string Address { get; set; } = null!;

        public IFormFile PngFile { get; set; } = null!;
        
        //public Guid Id { get; set; }
        // public string filePath { get; internal set; }

    }
}
