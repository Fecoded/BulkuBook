using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using BulkyBook.Utility;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            ProductPageModel productPageModel = new ProductPageModel()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            if(id == null)
            {
                //this is to create product
                return View(productPageModel);
            }
            //this is to edit
            productPageModel.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());

            if(productPageModel.Product == null)
            {
                return NotFound();
            }
            return View(productPageModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductPageModel productPageModel)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;

                var files = HttpContext.Request.Form.Files;

                if(files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var upload = Path.Combine(webRootPath, @"images\products");
                    var extension = Path.GetExtension(files[0].FileName);

                    if(productPageModel.Product.ImageUrl != null)
                    {
                        // this is an edit and we need to remove old image

                        var imagePath = Path.Combine(webRootPath, productPageModel.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using(var filesStreams = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }

                    productPageModel.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }
                else
                {
                    // Update when image is not Changed
                    if(productPageModel.Product.Id != 0)
                    {
                        Product objFromDb = _unitOfWork.Product.Get(productPageModel.Product.Id);
                        productPageModel.Product.ImageUrl = objFromDb.ImageUrl;
                    }
                }

                if (productPageModel.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productPageModel.Product);

                }
                else
                {
                    _unitOfWork.Product.Update(productPageModel.Product);
                }

                _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productPageModel.CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
                productPageModel.CoverTypeList = _unitOfWork.CoverType.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
            }
            if(productPageModel.Product.Id != 0)
            {
                productPageModel.Product = _unitOfWork.Product.Get(productPageModel.Product.Id);
            }
            return View(productPageModel);
        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
            return Json(new { data = result });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var product = _unitOfWork.Product.Get(id);
           
            if (product == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            string webRootPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, product.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _unitOfWork.Product.Remove(product);
            _unitOfWork.Complete();
            return Json(new { success = true, message = "Product was deleted " });
        }



        #endregion
    }
}
