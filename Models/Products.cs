using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLSP.Models    
{
    
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tên sản phẩm")]
        public required string TenSanPham { get; set; }

        [Required]
        [Display(Name = "Giá")]
        public decimal Gia { get; set; }

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Ảnh sản phẩm")]
        public string? AnhSanPham { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
