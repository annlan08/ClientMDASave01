using ClientMDA.Models;
using ClientMDA.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientMDA.Controllers
{
    public class MemberController : Controller
    {
        private readonly MDAContext _MDAcontext;
        private IWebHostEnvironment _enviro;
        public MemberController(MDAContext MDAcontext, IWebHostEnvironment p)  //相依性注入
        {
            _MDAcontext = MDAcontext;
            _enviro = p;
        }



        public IActionResult MemberMain()
        {
            if (HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER) == null)
                return RedirectToAction("Login");
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(CLoginViewModel vModel)
        {
            string jsonUserPhone = JsonSerializer.Serialize(vModel.txtphone);
            HttpContext.Session.SetString(CDictionary.SK_USER_PHONE, jsonUserPhone);

            bool isExist = _MDAcontext.會員members.Any(m => m.會員電話cellphone == vModel.txtphone);
            if (isExist)
                return RedirectToAction("Login2");
            else
                return RedirectToAction("SignUp");//, new { phone = vModel.txtphone }
        }

        public IActionResult checkExist(string phone)
        {
            bool isExist = _MDAcontext.會員members.Any(m => m.會員電話cellphone == phone);
            return Content(isExist.ToString(), "text/plain");
        }

        public IActionResult Login2(/*string phone*/)
        {
            //ViewBag.phone = phone;
            var a = HttpContext.Session.GetString(CDictionary.SK_USER_PHONE);
            ViewBag.phone = JsonSerializer.Deserialize<string>(a);
            //CLogin2ViewModel mem = new CLogin2ViewModel();
            //mem.txtphone = phone;
            return View();
        }
        [HttpPost]
        public IActionResult Login2(CLogin2ViewModel vModel)
        {
            會員member mem = _MDAcontext.會員members.FirstOrDefault(t => t.會員電話cellphone.Equals(vModel.txtPhone));
            if (mem != null && mem.密碼password.Equals(vModel.txtPassword))
            {
                //登入成功存入session
                string jsonUser = JsonSerializer.Serialize(mem);
                HttpContext.Session.SetString(CDictionary.SK_LOGINED_USER, jsonUser);

                //轉跳原頁面
                string page = HttpContext.Session.GetString(CDictionary.SK登後要前往的頁面);
                if (!string.IsNullOrEmpty(page))
                {
                    HttpContext.Session.SetString(CDictionary.SK登後要前往的頁面, "");
                    return Redirect(page);
                }
                else
                    return RedirectToAction("MemberMain");
            }

            else
            {
                ViewBag.phone = vModel.txtPhone;
                ViewBag.txtError = false;
                return View();
            }
        }
        public bool checkPsw(int id, string psw)
        {
            bool isPsw = _MDAcontext.會員members.Any(m => m.會員編號memberId == id && m.密碼password == psw);
            return isPsw;
        }
        public IActionResult SignUp()
        {
            var a = HttpContext.Session.GetString(CDictionary.SK_USER_PHONE);
            ViewBag.phone = JsonSerializer.Deserialize<string>(a);
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(CSignUpViewModel vModel)
        {
            會員member m = new 會員member();
            m.會員電話cellphone = vModel.txtPhone;
            m.電子信箱email = vModel.txtEmail;
            m.密碼password = vModel.txtPassword;
            m.會員權限permission = 0;
            m.正式會員formal = false;
            _MDAcontext.會員members.Add(m);
            _MDAcontext.SaveChanges();

            string jsonUser = JsonSerializer.Serialize(m);
            HttpContext.Session.SetString(CDictionary.SK_LOGINED_USER, jsonUser);

            string page = HttpContext.Session.GetString(CDictionary.SK登後要前往的頁面);
            if (!string.IsNullOrEmpty(page))
            {
                HttpContext.Session.SetString(CDictionary.SK登後要前往的頁面, "");
                return Redirect(page);
            }
            else
                return RedirectToAction("MemberMain");
        }
        public IActionResult MemberEdit()
        {
            if (HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER) == null)
                return RedirectToAction("Login");
            return View();
        }
        [HttpPost]
        public IActionResult MemberEdit(CMemberDemoViewModel inputMember)
        {
            會員member mem = _MDAcontext.會員members.FirstOrDefault(m => m.會員編號memberId == inputMember.會員編號memberId);
            if (mem != null)
            {
                if (inputMember.memberPhoto != null)
                {
                    string photoName = "member" + mem.會員編號memberId + ".jpg";
                    mem.會員照片image = photoName;
                    string path = _enviro.WebRootPath + "/images/Member/" + photoName;
                    inputMember.memberPhoto.CopyTo(new FileStream(path, FileMode.Create));
                }
                if (!string.IsNullOrEmpty(inputMember.birthDate))
                {
                    DateTime bd = DateTime.ParseExact(inputMember.birthDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    mem.生日birthDate = bd;
                }
                mem.暱稱nickName = inputMember.暱稱nickName;
                mem.名字fName = inputMember.名字fName;
                mem.姓氏lName = inputMember.姓氏lName;
                mem.性別gender = inputMember.性別gender;
                mem.地址address = inputMember.地址address;
                if (mem.正式會員formal == false && inputMember.地址address != null && inputMember.暱稱nickName != null
                    && inputMember.性別gender != null && inputMember.birthDate != null && inputMember.名字fName != null
                    && inputMember.姓氏lName != null)
                {
                    mem.正式會員formal = true;
                }

                _MDAcontext.SaveChanges();

                string jsonUser = JsonSerializer.Serialize(mem);
                HttpContext.Session.SetString(CDictionary.SK_LOGINED_USER, jsonUser);
            }
            return RedirectToAction("MemberEdit");
        }

        public IActionResult CommentList(CKeywordViewModel model)
        {
            if (HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER) == null)
                return RedirectToAction("Login");
            else
            {
                var a = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                會員member mem = JsonSerializer.Deserialize<會員member>(a);
                IEnumerable<電影評論movieComment> datas = null;
                var q = _MDAcontext.電影評論movieComments.Where(c => c.會員編號memberId == mem.會員編號memberId);
                if (string.IsNullOrEmpty(model.txtKeyword))
                {
                    datas = q;
                }
                else
                    datas = q.Where(c => c.評論標題commentTitle.Contains(model.txtKeyword));
                return View(datas);
            }

        }
        public IActionResult CommentEdit(int? id)
        {
            if (HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER) == null)
                return RedirectToAction("Login");
            else
            {
                var a = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                會員member mem = JsonSerializer.Deserialize<會員member>(a);
                if (mem.正式會員formal == false)
                {
                    return RedirectToAction("NotFormal");
                }
                return View();
            }
        }

        public IActionResult WatchList()
        {
            var a = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
            if (a == null)
                return RedirectToAction("Login");
            else
            {
                會員member mem = JsonSerializer.Deserialize<會員member>(a);

                var q = _MDAcontext.片單總表movieLists.Where(m => m.會員編號memberId == mem.會員編號memberId).Select(m => new CMovieListViewModel
                {
                    memberId = m.會員編號memberId,
                    listId = m.片單總表編號movieListId,
                    listName = m.片單總表名稱listName,
                    myLists = _MDAcontext.我的片單myMovieLists.Where(l => l.片單總表編號movieListId == m.片單總表編號movieListId).Select(m => new CMovieListSubViewModel
                    {
                        memberId = m.會員編號memberId,
                        myMovieListId = m.我的片單myMovieListId,
                        movieId = m.電影編號movieId,
                        movieTitle = m.電影編號movie.中文標題titleCht,
                        moviePic = m.電影編號movie.電影圖片movieIimagesLists.Select(c => c.圖片編號image.圖片雲端imageImdb).FirstOrDefault()
                    }).ToList(),


                }).ToList();

                return View(q);
            }

        }

        public IActionResult PasswordEdit()
        {
            if (HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER) == null)
                return RedirectToAction("Login");
            return View();
        }
        [HttpPost]
        public IActionResult PasswordEdit(CPasswordViewModel vModel)
        {
            會員member mem = _MDAcontext.會員members.FirstOrDefault(m => m.會員編號memberId == vModel.memberId);
            if (mem != null && mem.密碼password == vModel.txt_old_password)
            {
                mem.密碼password = vModel.txt_new_password;
                _MDAcontext.SaveChanges();

                string jsonUser = JsonSerializer.Serialize(mem);
                HttpContext.Session.SetString(CDictionary.SK_LOGINED_USER, jsonUser);

                ViewBag.txtSuccess = "s";
            }
            else
            {
                ViewBag.txtError = false;

            }
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(CDictionary.SK_LOGINED_USER);
            return View();
        }
        public IActionResult MemberBonusList()
        {
            var a = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
            if (HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER) == null)
                return RedirectToAction("Login");
            return View();
        }
        public IActionResult AddCoupon(CKeywordViewModel model)
        {
            var a = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
            會員member mem = JsonSerializer.Deserialize<會員member>(a);
            var q = _MDAcontext.優惠總表coupons.Where(c => c.優惠代碼couponCode == model.txtKeyword).FirstOrDefault();
            if (q != null)
            {
                優惠明細couponList coupon = new 優惠明細couponList
                {
                    會員編號memberId = mem.會員編號memberId,
                    優惠編號couponId = q.優惠編號couponId,
                    是否使用優惠oxCouponUsing = false,
                };
                _MDAcontext.優惠明細couponLists.Add(coupon);
                _MDAcontext.SaveChanges();
            }


            return RedirectToAction("MemberDiscount");
        }
        public IActionResult MemberDiscount()
        {
            var a = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
            if (a == null)
                return RedirectToAction("Login");
            else
            {
                會員member mem = JsonSerializer.Deserialize<會員member>(a);
                var q = _MDAcontext.優惠明細couponLists.Where(c => c.會員編號memberId == mem.會員編號memberId).Select(c => new CCouponListViewModel
                {
                    memberId = c.會員編號memberId,
                    couponListId = c.優惠明細編號couponListId,
                    couponName = c.優惠編號coupon.優惠名稱couponName,
                    dueDate = c.優惠編號coupon.優惠截止日期couponDueDate,
                    diccount = c.優惠編號coupon.優惠折扣couponDiscount,
                    used = c.是否使用優惠oxCouponUsing

                }).OrderByDescending(c => c.dueDate).ToList();


                return View(q);
            }

        }
        public IActionResult OrderList()
        {
            var a = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
            if (a == null)
                return RedirectToAction("Login");
            else
            {
                會員member mem = JsonSerializer.Deserialize<會員member>(a);
                //List<COrderListViewModel> vmList = new List<COrderListViewModel>();
                var q = _MDAcontext.訂單總表orders.Where(o => o.會員編號memberId == mem.會員編號memberId).Select(i => new COrderListViewModel
                {
                    memberId = i.會員編號memberId,
                    orderDate = i.訂單時間orderTime,
                    orderId = i.訂單編號orderId,
                    status = i.訂單狀態編號orderStatus.訂單狀態orderStatusName,
                    tickets = i.訂單明細orderDetails.Select(d => d.張數count).ToList(),
                    ticketPrice = i.訂單明細orderDetails.Select(d => d.票價明細ticket.價格ticketPrice).ToList(),
                }).ToList();


                //var qTest = _MDAcontext.訂單總表orders.Where(o => o.會員編號memberId == mem.會員編號memberId);
                //List<string> sta = new List<string> { "未付款", "已完成", "取消" };
                //foreach (var i in qTest)
                //{
                //    COrderListViewModel vm = new COrderListViewModel();
                //    decimal price = 0;
                //    vm.memberId = i.會員編號memberId;
                //    vm.orderDate = i.訂單時間orderTime;
                //    vm.orderId = i.訂單編號orderId;
                //    vm.status = sta[i.訂單狀態編號orderStatusId-1];

                //    foreach(var ii in i.訂單明細orderDetails)
                //    {
                //        price += (ii.張數count) * (ii.票價明細ticket.價格ticketPrice);
                //    }
                //    vm.orderPrice = price;

                //    vmList.Add(vm);
                //}

                return View(q);
            }

        }
        public IActionResult NotFormal()
        {
            return View();
        }
        public IActionResult test()
        {
            return View();
        }
        public IActionResult test2()
        {
            return View();
        }
        public IActionResult WriteComment()
        {
            if (HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER) == null)
                return RedirectToAction("Login");
            else
            {
                var a = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                會員member mem = JsonSerializer.Deserialize<會員member>(a);
                if (mem.正式會員formal == false)
                {
                    return RedirectToAction("NotFormal");
                }
                return View();
            }
        }
        public IActionResult WishList() //followList
        {
            var a = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
            if (a == null)
                return RedirectToAction("Login");
            else
            {
                會員member mem = JsonSerializer.Deserialize<會員member>(a);
                var q = _MDAcontext.我的追蹤清單myFollowLists.Where(f => f.會員編號memberId == mem.會員編號memberId && f.追讚倒編號actionTypeId == 0).Select(f => new CFollowListViewModel
                {
                    memberId = f.會員編號memberId,
                    targetName = f.對象target.對象名稱targetName,
                    targetId = f.對象targetId,
                    connectId = f.連接編號connectId,
                    followMemName = _MDAcontext.會員members.Where(m => m.會員編號memberId == f.連接編號connectId).Select(m => m.暱稱nickName).FirstOrDefault(),
                    comments = _MDAcontext.電影評論movieComments
                        .Where(m => m.會員編號memberId == f.連接編號connectId)
                        .OrderByDescending(c => c.發佈時間commentTime)
                        .Select(c => c.評論標題commentTitle)
                        .Take(3).ToList(),
                    followComTitle = _MDAcontext.電影評論movieComments.Where(c => c.評論編號commentId == f.連接編號connectId).Select(c => c.評論標題commentTitle).FirstOrDefault(),
                    replies = _MDAcontext.回覆樓數floors
                        .Where(c => c.評論編號commentId == f.連接編號connectId)
                        .OrderByDescending(r => r.發佈時間floorTime)
                        .Select(r => r.回覆內容floors)
                        .Take(3).ToList(),


                }).ToList();

                return View(q);
            }
        }
        public IActionResult queryFollowMember(int id)
        {
            var q = _MDAcontext.會員members.FirstOrDefault(m => m.會員編號memberId == id);
            return Content(q.暱稱nickName, "text/plain", System.Text.Encoding.UTF8);
        }

        //讀取所有城市資料
        public IActionResult City()
        {
            var cities = _MDAcontext.地址addresses.Select(a => a.City).Distinct();
            return Json(cities);

        }
        //依據城市名稱讀取鄉鎮區資料
        public IActionResult Site(string city)
        {
            var sites = _MDAcontext.地址addresses.Where(a => a.City == city).Select(a => a.SiteId).Distinct();
            return Json(sites);

        }
        //依據城市名稱讀取鄉鎮區資料
        public IActionResult Road(string site)
        {
            var roads = _MDAcontext.地址addresses.Where(a => a.SiteId == site).Select(a => a.Road).Distinct();
            return Json(roads);

        }
        public IActionResult autoCmpMovie(string movie)
        {
            var movies = _MDAcontext.電影movies.Where(m => m.中文標題titleCht.Contains(movie) || m.英文標題titleEng.ToUpper().Contains(movie.ToUpper())).Select(p => p.中文標題titleCht);
            return Json(movies);

        }


    }
}
