using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientMDA.Models;
using ClientMDA.ViewModels;
using ClientMDA.ViewModel.WenViewModel;

namespace ClientMDA.Controllers
{
    public class WenShoppingCartController : Controller
    {
        private readonly MDAContext _context;

        public WenShoppingCartController(MDAContext context)
        {
            _context = context;
        }

        public IActionResult TestSearchKeyword(string keyword)
        {
            var product = _context.商品資料products
              .Where(p => p.商品名稱productName.Contains(keyword) ||
              p.商品介紹introduce.Contains(keyword) ||
              p.電影院編號theater.電影院名稱theaterName.Contains(keyword)
              )
              .Select(p => new
              {
                  p.電影院編號theater.電影院名稱theaterName,
                  p.商品名稱productName,
                  p.商品價格productPrice,
                  p.商品介紹introduce,
                  p.類別category,
                  p.商品圖片路徑imagePath
              });

            return Json(product);
        }

        public IActionResult SearchKeyword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SearchKeyword(string keyword)
        {
            return View();
        }


        public IActionResult CardProduct(int myTheaterId)
        {
            var product = _context.商品資料products
                .Where(p => p.電影院編號theaterId == myTheaterId)
                .Select(p => new
                {
                    p.商品名稱productName,
                    p.商品價格productPrice,
                    p.商品介紹introduce,
                    p.商品圖片路徑imagePath
                });
            return Json(product);
        }

        //public ActionResult AddToCard(int? id)
        //{
        //    商品資料product prod = _context.商品資料products.FirstOrDefault(p => p.商品編號productId == id);
        //    if (prod != null)
        //        return View(prod);
        //    return RedirectToAction("Index");
        //}
        public ActionResult AddToCard()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddToCard(WenCAddToCartItem c)
        {
            商品資料product prod = _context.商品資料products.FirstOrDefault(p => p.商品編號productId == c.prdId);
            if (prod != null)
            {
                return RedirectToAction("Index");
                //List<CAddToCartItem> cart = Session[WenDictionary.MY_PRODUCTS] as List<CAddToCartItem>;
            }
            return RedirectToAction("Index");

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CIndex()
        {
            var datas = from t in (new MDAContext()).商品資料products
                        select t;

            List<CShoppingProd> list = new List<CShoppingProd>();

            foreach (商品資料product t in datas)
            {
                CShoppingProd vm = new CShoppingProd();
                vm.product = t;
                list.Add(vm);
            }

            return View(list);
        }
        public IActionResult Theater()
        {
            var query = from m in _context.電影院theaters
                        select m;/*new { m.電影院名稱theaterName,m.電影院編號theaterId}*/  //object    
                       
            return Json(query);
        }
        public IActionResult Category(int TheaterId)
        {
            var q = _context.商品資料products
                .Where(p => p.電影院編號theaterId == TheaterId)
                .GroupBy(p=>new { p.類別category})
                .Select(p =>
                new { 
                ByCategory=p.Key                
                });

            return Json(q);
        }
        public IActionResult Prod(int categoryName)
        {
            var q = _context.商品資料products
                .Where(p => p.類別category.Equals(categoryName))               
                .Select(p =>
                new {
                   p.商品名稱productName,
                   p.商品介紹introduce,
                   p.商品價格productPrice
                });

            return Json(q);
        }

    }
}
