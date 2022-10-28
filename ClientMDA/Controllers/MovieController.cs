using ClientMDA.Models;
using ClientMDA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientMDA.Controllers
{
    public class MovieController : Controller
    {
        private readonly MDAContext _MDAcontext;

        public MovieController(MDAContext MDAcontext)  //相依性注入
        {
            _MDAcontext = MDAcontext;
            _MDAcontext.電影圖片movieIimagesLists.ToList();
            _MDAcontext.電影圖片總表movieImages.ToList();
        }

        public ActionResult 排行首頁(CKeywordViewModel model)
        {
            List<CMovieViewModel> datas = null;
            var mPoster = _MDAcontext.電影圖片movieIimagesLists.Select(i => i);
            if (string.IsNullOrEmpty(model.txtKeyword))
                datas = _MDAcontext.電影movies.OrderByDescending(m => m.票房boxOffice).Select
                        (m => new CMovieViewModel
                        {
                            movie = m,
                            mImgFrList = _MDAcontext.電影圖片movieIimagesLists.Where(i => i.電影編號movieId == m.電影編號movieId)
                            .Select(c => c.圖片編號image.圖片雲端imageImdb).ToList()
                        }).Take(50).ToList();
            else
                datas = _MDAcontext.電影movies.Where(m => m.中文標題titleCht.Contains(model.txtKeyword) ||
                                                          m.英文標題titleEng.Contains(model.txtKeyword)).Select
                        (m => new CMovieViewModel
                        {
                            movie = m,
                            mImgFrList = _MDAcontext.電影圖片movieIimagesLists.Where(i => i.電影編號movieId == m.電影編號movieId)
                            .Select(c => c.圖片編號image.圖片雲端imageImdb).ToList()
                        }).ToList();

            return View(datas);
        }

        public ActionResult 電影排行新片(CKeywordViewModel model) //電影排行新片
        {
            List<CMovieViewModel> datas = null;
            var mPoster = _MDAcontext.電影圖片movieIimagesLists.Select(i => i);
            if (string.IsNullOrEmpty(model.txtKeyword))
                datas = _MDAcontext.電影movies.OrderByDescending(m => m.上映日期releaseDate).Select
                        (m => new CMovieViewModel
                        {
                            movie = m,
                            mImgFrList = _MDAcontext.電影圖片movieIimagesLists.Where(i => i.電影編號movieId == m.電影編號movieId)
                            .Select(c => c.圖片編號image.圖片雲端imageImdb).ToList()
                        }).Take(50).ToList();
            else
                datas = _MDAcontext.電影movies.Where(m => m.中文標題titleCht.Contains(model.txtKeyword) ||
                                                          m.英文標題titleEng.Contains(model.txtKeyword)).Select
                        (m => new CMovieViewModel
                        {
                            movie = m,
                            mImgFrList = _MDAcontext.電影圖片movieIimagesLists.Where(i => i.電影編號movieId == m.電影編號movieId)
                            .Select(c => c.圖片編號image.圖片雲端imageImdb).ToList()
                        }).ToList();

            return ViewComponent("電影排行新片", datas);
        }

        public IActionResult 電影介紹(int? id) //電影資訊/電影評論
        {
            CMovieViewModel datas = null;
            var q = _MDAcontext.電影圖片movieIimagesLists.Select(i => i);
            datas = _MDAcontext.電影movies.Where(m => m.電影編號movieId == id).Select
            (m => new CMovieViewModel
            {
                movie = m,
                //演員中文名字nameCht = m.電影主演casts,
                分級級數ratingLevel = m.電影分級編號rating.分級級數ratingLevel,
                分級圖片ratingImage = m.電影分級編號rating.分級圖片ratingImage,
                系列名稱seriesName = m.系列編號series.系列名稱seriesName,
                mCountryName = _MDAcontext.電影產地movieOrigins.Where(i => i.電影編號movieId == m.電影編號movieId)
                .Select(c => c.國家編號country.國家名稱countryName).ToList(),
                mCountryImg = _MDAcontext.電影產地movieOrigins.Where(i => i.電影編號movieId == m.電影編號movieId)
                .Select(c => c.國家編號country.國旗countryImage).ToList(),
                mImgFrList = _MDAcontext.電影圖片movieIimagesLists.Where(i => i.電影編號movieId == m.電影編號movieId)
                .Select(c => c.圖片編號image.圖片雲端imageImdb).ToList()

            }).FirstOrDefault();
            return View(datas);
        }

        public IActionResult 電影劇照牆(int? id)
        {
            CMovieViewModel datas = null;
            datas = _MDAcontext.電影movies.Where(m => m.電影編號movieId == id).Select
            (m => new CMovieViewModel
            {
                movie = m,
            }).FirstOrDefault();
            return View(datas);
        }

        public IActionResult 電影劇照()
        {
            return View();
        }


        public FileResult ShowPhoto(int id)
        {
            電影分級movieRating rating = _MDAcontext.電影分級movieRatings.Find(id);
            byte[] context = rating.分級圖片ratingImage;
            return File(context, "image/jpeg");
        }
    }
}
