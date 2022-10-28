using ClientMDA.Models;
using ClientMDA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ClientMDA.Controllers
{
    public class HomePageController : Controller
    {
        private readonly ILogger<HomePageController> _logger;
        private readonly MDAContext _MDA;

        public HomePageController(ILogger<HomePageController> logger, MDAContext MDA)
        {
            _logger = logger;
            _MDA = MDA;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(CHomepageViewModel p)
        {
            if (p.會員編號Member_ID != 0)
            {
                電影評論movieComment m = new 電影評論movieComment();
                m.會員編號memberId = p.會員編號Member_ID;
                m.電影編號movieId = p.電影編號Movie_ID;
                m.評分rate = p.評分Rate;
                m.發佈時間commentTime = p.發佈時間Comment_Time = DateTime.Now;
                _MDA.電影評論movieComments.Add(m);
                _MDA.SaveChanges();
            }
            if (p.會員編號Member_IDbook != 0)
            {
                我的片單myMovieList l = new 我的片單myMovieList();
                l.會員編號memberId = p.會員編號Member_IDbook;
                l.片單總表編號movieListId = p.片單總表編號MovieList_ID = 43;
                l.電影編號movieId = p.電影編號Movie_IDbook;
                _MDA.我的片單myMovieLists.Add(l);
                _MDA.SaveChanges();
            }
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult test()
        {
            return View();
        }

    }
}
