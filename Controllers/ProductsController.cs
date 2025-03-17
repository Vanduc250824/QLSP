using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QLSP.Models;

namespace QLSP.Controllers;

public class ProductsController : Controller
{
    private static List<Product> products = new List<Product>();
    public required ApplicationDbContext _context { get; init; }
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        var sanPhams = _context.Products.ToList();
        return View(sanPhams);
    }
    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Add(Product model, IFormFile file)
    {
        try
        {
            if (file != null && file.Length > 0)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                model.AnhSanPham = "/Images/" + uniqueFileName;
                Console.WriteLine("✅ Đường dẫn ảnh: " + model.AnhSanPham); // Debug xem đường dẫn có được gán không
            }
            else
            {
                Console.WriteLine("⚠ Không có file nào được chọn!");
            }

            Console.WriteLine("🛠 Dữ liệu trước khi lưu vào DB: " + model.TenSanPham + " - " + model.AnhSanPham);

            _context.Products.Add(model);
            await _context.SaveChangesAsync();
            Console.WriteLine("✅ Dữ liệu đã lưu vào database!");

            return RedirectToAction("Index", "Products");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Lỗi khi upload ảnh: " + ex.Message);
            return View(model);
        }
    }


    [HttpGet]
    public IActionResult Edit(int id)
    {
        var product = _context.Products.Find(id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }




    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Product product, IFormFile uploadedImage)
    {
        if (!ModelState.IsValid)
        {
            return View(product);
        }

        var existingProduct = _context.Products.FirstOrDefault(p => p.Id == product.Id);
        if (existingProduct == null)
        {
            return NotFound();
        }

        existingProduct.TenSanPham = product.TenSanPham;
        existingProduct.Gia = product.Gia;
        existingProduct.MoTa = product.MoTa;

        // Nếu có ảnh mới, lưu vào thư mục "/wwwroot/images/"
        if (uploadedImage != null && uploadedImage.Length > 0)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            // Tạo tên file duy nhất
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadedImage.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                uploadedImage.CopyTo(fileStream);
            }

            // Cập nhật đường dẫn ảnh
            existingProduct.AnhSanPham = "/images/" + uniqueFileName;
        }

        _context.SaveChanges();
        return RedirectToAction("Index");
    }


}
