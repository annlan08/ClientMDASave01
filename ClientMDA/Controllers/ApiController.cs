using ClientMDA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace ClientMDA.Controllers
{
    public class ApiController : Controller
    {
        private readonly ILogger<ApiController> _logger;
        private readonly MDAContext _MDA;
        Random rd = new Random();

        public ApiController(ILogger<ApiController> logger, MDAContext MDA)
        {
            _logger = logger;
            _MDA = MDA;
            _MDA.電影圖片movieIimagesLists.ToList();
            _MDA.電影圖片總表movieImages.ToList();
            _MDA.電影movies.ToList();
            _MDA.我的片單myMovieLists.ToList();
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult getmemID()
        {
            string a = HttpContext.Session.GetString(CDictionary.SK_USER_PHONE);
            if (a != null)
            {
                string b = JsonSerializer.Deserialize<string>(a);
                var q = _MDA.會員members.Where(m => m.會員電話cellphone == b).Select(m => m.會員編號memberId);
                if (q != null)
                {

                    return Json(q);
                }
            }
            return Json(0);
        }

        public IActionResult getmemlist()
        {
            string a = HttpContext.Session.GetString(CDictionary.SK_USER_PHONE);
            if (a != null)
            {
                string b = JsonSerializer.Deserialize<string>(a);
                var q = _MDA.我的片單myMovieLists.Where(m => m.會員編號member.會員電話cellphone == b && m.片單總表編號movieList.片單總表名稱listName == "我的片單(預設)").Select(m => m.片單總表編號movieList.片單總表編號movieListId);
                if (q != null)
                {
                    return Json(q);
                }
            }
            return Json(0);
        }
        //尚未設定資料
        public IActionResult showbannernum()
        {
            var q = from a in _MDA.電影圖片movieIimagesLists
                    join b in _MDA.電影排行movieRanks on a.圖片編號image.電影名稱movieName equals b.電影movie
                    where a.圖片編號image.電影名稱movieName == b.電影movie
                    orderby b.排行編號rankId ascending
                    select a.電影編號movie.電影編號movieId;

            return Json(q);
        }
        public IActionResult checklogin()
        {
            string user = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
            if (user == null)
            {
                HttpContext.Session.SetString(CDictionary.SK登後要前往的頁面, "~/HomePage/Index");
                return Redirect("~/Member/Login");
            }
            return View();

        }

        public IActionResult showrankposter()
        {
            //抓不到圖片名稱為英文
            //var q = from a in _MDA.電影圖片總表movieImages
            //        join b in _MDA.電影排行movieRanks on a.電影名稱movieName equals b.電影movie
            //        where a.電影名稱movieName == b.電影movie
            //        orderby b.排行編號rankId ascending
            //        select new { a.圖片雲端imageImdb };
            var q = from a in _MDA.電影圖片movieIimagesLists
                    join b in _MDA.電影排行movieRanks on a.圖片編號image.電影名稱movieName equals b.電影movie
                    where a.圖片編號image.電影名稱movieName == b.電影movie
                    orderby b.排行編號rankId ascending
                    select a.圖片編號image.圖片雲端imageImdb;
            return Json(q);
        }

        public IActionResult showranknum()
        {
            var q = from a in _MDA.電影圖片movieIimagesLists
                    join b in _MDA.電影排行movieRanks on a.圖片編號image.電影名稱movieName equals b.電影movie
                    where a.圖片編號image.電影名稱movieName == b.電影movie
                    orderby b.排行編號rankId ascending
                    select a.電影編號movie.電影編號movieId;

            return Json(q);
        }

        public IActionResult showrankmovie()
        {
            var q1 = from a in _MDA.電影排行movieRanks
                     where a.電影排名movieRank != null
                     select a.電影movie;
            return Json(q1);
        }

        public IActionResult shownewmovie()
        {
            var q2 = _MDA.電影圖片movieIimagesLists.Where(m => m.電影編號movie.上映日期releaseDate >= (DateTime.Now.AddDays(-7)) && m.電影編號movie.上映日期releaseDate <= (DateTime.Now.AddDays(7))).OrderBy(d => d.電影編號movie.上映日期releaseDate).Select(v => v.電影編號movie.中文標題titleCht);
            return Json(q2);
        }

        public IActionResult shownewposter()
        {
            var q = _MDA.電影圖片movieIimagesLists.Where(m => m.電影編號movie.上映日期releaseDate >= (DateTime.Now.AddDays(-7)) && m.電影編號movie.上映日期releaseDate <= (DateTime.Now.AddDays(7))).OrderBy(d => d.電影編號movie.上映日期releaseDate).Select(u => u.圖片編號image.圖片雲端imageImdb);
            return Json(q);
        }

        public IActionResult shownewnum()
        {
            var q2 = _MDA.電影圖片movieIimagesLists.Where(m => m.電影編號movie.上映日期releaseDate >= (DateTime.Now.AddDays(-7)) && m.電影編號movie.上映日期releaseDate <= (DateTime.Now.AddDays(7))).OrderBy(d => d.電影編號movie.上映日期releaseDate).Select(v => v.電影編號movie.電影編號movieId);
            return Json(q2);
        }

        public IActionResult showrecommendposter()
        {
            List<string> list = new List<string>(0);
            List<string> list2 = new List<string>();
            List<string> list3 = new List<string>();
            List<int> listNumbers = new List<int>();
            int number;
            for (int i = 0; i < 20; i++)
            {
                do
                {
                    //資料庫要新增很多東西才可以跳更多推薦
                    number = rd.Next(1, 25);
                } while (listNumbers.Contains(number));
                listNumbers.Add(number);
            }
            for (int i = 0; i < listNumbers.Count(); i++)
            {
                var q3 = _MDA.電影圖片movieIimagesLists.AsEnumerable().Where(p => p.電影編號movieId == listNumbers[i]).FirstOrDefault().圖片編號image.圖片雲端imageImdb;
                list.Add(q3);
            }
            for (int i = 0; i < listNumbers.Count(); i++)
            {
                var q4 = _MDA.電影圖片movieIimagesLists.AsEnumerable().Where(o => o.電影編號movieId == listNumbers[i]).FirstOrDefault().圖片編號image.圖片編號imageId.ToString();
                list.Add(q4);
            }
            return Json(list);
        }

        public IActionResult showrecnum()
        {
            var q2 = _MDA.電影圖片movieIimagesLists.Where(m => m.電影編號movie.上映日期releaseDate >= (DateTime.Now.AddDays(-7)) && m.電影編號movie.上映日期releaseDate <= (DateTime.Now.AddDays(7))).OrderBy(d => d.電影編號movie.上映日期releaseDate).Select(v => v.電影編號movie.電影編號movieId);
            return Json(q2);
        }
    }
}
