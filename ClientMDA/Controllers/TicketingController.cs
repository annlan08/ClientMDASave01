using ClientMDA.Models;
using ClientMDA.ViewModels;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
using System.Text.Json;
using System.Collections.Specialized;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace ClientMDA.Controllers
{
    public class TicketingController : Controller
    {
        #region 建構子
        private MDAContext _dbContext;
        private readonly IWebHostEnvironment _host;

        public TicketingController(MDAContext dbContext, IWebHostEnvironment host)
        {
            _dbContext = dbContext;
            _host = host;
            _dbContext.場次screenings.ToList();
            _dbContext.電影movies.ToList();
            _dbContext.電影代碼movieCodes.ToList();
            _dbContext.電影院theaters.ToList();
            _dbContext.影廳cinemas.ToList();
            _dbContext.出售座位狀態seatStatuses.ToList();
            _dbContext.影城mainTheaters.ToList();
            _dbContext.票價資訊ticketPrices.ToList();
            _dbContext.票種ticketTypes.ToList();
            _dbContext.電影分級movieRatings.ToList();
            _dbContext.訂單總表orders.ToList();
            _dbContext.電影語言movieLanguages.ToList();
            _dbContext.電影主演casts.ToList();
            _dbContext.導演總表directors.ToList();
            _dbContext.電影導演movieDirectors.ToList();
            _dbContext.演員總表actors.ToList();
            _dbContext.會員members.ToList();

            _dbContext.商品資料products.ToList();
            _dbContext.購買商品明細receipts.ToList();
        }

        #endregion

        #region 普通Action
        public IActionResult MovieInfoIndex()
        {
            return View();
        }

        public IActionResult SelectMovie()
        {
            string user = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
            if (string.IsNullOrEmpty(user))
            {
                HttpContext.Session.SetString(CDictionary.SK登後要前往的頁面, "~/Ticketing/SelectMovie");
                return Redirect("~/Member/Login");
            }
            return View();
        }

        public IActionResult TheaterPartialView()
        {
            List<電影院theater> theaters = this._dbContext.電影院theaters.ToList();
            return PartialView($"~/Views/Ticketing/_TheaterPartialView.cshtml", theaters);
        }

        public IActionResult MoviePartialView()
        {
            List<電影代碼movieCode> movies = this._dbContext.電影代碼movieCodes.ToList();
            return PartialView($"~/Views/Ticketing/_MoviePartialView.cshtml", movies);
        }

        public IActionResult MovieInfoIndex2(int id)
        {
            HttpContext.Session.SetInt32(CDictionary.SK_選擇的電影Code, id);
            List<電影院theater> theater = this._dbContext.場次screenings.Where(s => s.電影代碼movieCode == id).Select(s => s.影廳編號cinema.電影院編號theater).Distinct().ToList();
            return View(theater);
        }

        public IActionResult SelectByMovie(int theaterID)
        {
            HttpContext.Session.SetInt32(CDictionary.SK_選擇的電影院ID, theaterID);
            int MovieID = (int)HttpContext.Session.GetInt32(CDictionary.SK_選擇的電影Code);
            CSelectByMovieViewModel view = new CSelectByMovieViewModel()
            {
                movie = this._dbContext.電影代碼movieCodes.FirstOrDefault(c => c.電影代碼編號movieCodeId == MovieID),
                theater = this._dbContext.電影院theaters.FirstOrDefault(t => t.電影院編號theaterId == theaterID),
                Delectors = this._dbContext.電影代碼movieCodes.FirstOrDefault(c => c.電影代碼編號movieCodeId == MovieID).電影編號movie.電影導演movieDirectors.Select(d => d.導演編號director.導演中文名字nameCht).ToList(),
                Actors = this._dbContext.電影代碼movieCodes.FirstOrDefault(c => c.電影代碼編號movieCodeId == MovieID).電影編號movie.電影主演casts.Select(a => a.演員編號actor.演員中文名字nameCht).ToList(),
            };
            return View(view);
        }

        public IActionResult SelectByTheater(int theaterID)
        {
            HttpContext.Session.SetInt32(CDictionary.SK_選擇的電影院ID, theaterID);

            電影院theater theater = this._dbContext.電影院theaters.FirstOrDefault(t => t.電影院編號theaterId == theaterID);
            List<場次screening> AllScreening = this._dbContext.場次screenings
                                                   .Where(s => s.影廳編號cinema.電影院編號theaterId == theaterID)
                                                   .ToList();

            CSelectByTheaterViewModel view = new CSelectByTheaterViewModel()
            {
                theaterName = theater.電影院名稱theaterName,
                address = theater.地址address,
                phone = theater.電話phone,
                theaterId = theaterID,
                Allscreen = AllScreening
            };
            return View(view);
        }

        public IActionResult SeatMap(CScreenIDAndCountViewModel info)
        {
            出售座位狀態seatStatus seatStaus = this._dbContext.出售座位狀態seatStatuses.Where(ss => ss.場次編號screeningId == info.ScreenID).FirstOrDefault();
            場次screening screening = this._dbContext.場次screenings.Where(s => s.場次編號screeningId == info.ScreenID).FirstOrDefault();
            CSeatMaoViewModels seatview = new CSeatMaoViewModels(seatStaus);
            seatview.seatCount選擇座位數量 = info.Count;
            seatview.MovieName電影名稱 = screening.電影代碼movieCodeNavigation.電影編號movie.中文標題titleCht;
            seatview.MovieID電影編號 = screening.電影代碼movieCodeNavigation.電影編號movieId;
            seatview.MovieCode電影代碼 = screening.電影代碼movieCode;
            seatview.TheaterName影城名稱 = screening.影廳編號cinema.電影院編號theater.電影院名稱theaterName;
            seatview.TheaterID影城編號 = screening.影廳編號cinema.電影院編號theaterId;
            seatview.Date日期 = screening.放映日期playDate.ToString("MMMM dd");
            seatview.fileVersion電影版本 = screening.影廳編號cinema.影廳名稱cinemaName;
            seatview.StartTime開始時間 = screening.放映開始時間playTime;

            return View(seatview);
        }

        public IActionResult SelectTicket(CGetInfoForTicketActionViewModel Info)
        {
            影城mainTheater currentMainTheater = this._dbContext.電影院theaters.Where(t => t.電影院編號theaterId == Info.theaterID).Select(t => t.影城編號mainTheater).FirstOrDefault();
            CSelectTicketViewModel ticketview = new CSelectTicketViewModel(Info);
            List<CTicketInfoViewModelcs> ALLticketInfo = new List<CTicketInfoViewModelcs>();
            var q = this._dbContext.票價資訊ticketPrices.Where(tp => tp.影城編號mainTheaterId == currentMainTheater.影城編號mainTheaterId);
            foreach (var item in q)
            {
                CTicketInfoViewModelcs ticketInfoitem = new CTicketInfoViewModelcs();
                ticketInfoitem.TicketID票價明細 = item.票價明細ticketId;
                ticketInfoitem.TicketName票種名稱 = item.票種編號ticketType.票種名稱ticketTypeName;
                ticketInfoitem.TicketPrice票價 = item.價格ticketPrice;
                ALLticketInfo.Add(ticketInfoitem);
            }
            ticketview.ALLticketInfo = ALLticketInfo;
            return View(ticketview);
        }

        public IActionResult PaymentWeb(CPaymentweb1ViewModel payment1) // 回傳字串 id:count#id:count#  id=票價明細 count=數量
        {
            CPaymentAndMovieInfoViewModel payview = new CPaymentAndMovieInfoViewModel(payment1);
            payview.theaterName電影院名稱 = this._dbContext.電影院theaters.FirstOrDefault(t => t.電影院編號theaterId == payment1.theaterID).電影院名稱theaterName;
            payview.Movieimage電影照片 = this._dbContext.電影圖片movieIimagesLists.Where(m => m.電影編號movieId == payment1.MovieID).Select(p => p.圖片編號image.圖片image).FirstOrDefault();
            payview.MovieName電影名稱 = this._dbContext.電影movies.Where(m => m.電影編號movieId == payment1.MovieID).Select(m => m.中文標題titleCht).FirstOrDefault();
            payview.MovieVersion電影版本 = this._dbContext.電影代碼movieCodes.Where(cd => cd.電影代碼編號movieCodeId == payment1.MovieCode).Select(l => l.語言編號language.語言名稱languageName).FirstOrDefault();
            payview.MovieInfo電影介紹 = this._dbContext.電影movies.Where(m => m.電影編號movieId == payment1.MovieID).Select(m => m.劇情大綱plot).FirstOrDefault();
            payview.Alltciket = fn_票種字串轉換List(payment1.Ticketstring);
            return View(payview);
        }

        public IActionResult PaymentWeb2(CInfoForMakeNewOrderViewModel infoview)
        {
            infoview.Alltciket = fn_票種字串轉換List(infoview.TicketInfo);
            return View(infoview);
        }

        public IActionResult PaymentWeb3(CCreateOrderViewModel order)
        {
            if (order.ScreenID != 0 && order.SeatInfo != "")
            {
                string SessionStr = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                會員member member = JsonSerializer.Deserialize<會員member>(SessionStr);

                fn_建立新訂單總表(order.ScreenID, member.會員編號memberId);
                fn_修改出售座位狀態(order.ScreenID, order.SeatInfo);
                int NewOrderID = this._dbContext.訂單總表orders.AsEnumerable().LastOrDefault().訂單狀態編號orderStatusId;
                fn_建立新訂單明細(NewOrderID, order.TicketInfo);
                fn_建立新出售座位明細(NewOrderID, order.ScreenID, order.SeatInfo);
                string MemberName = this._dbContext.會員members.Where(m => m.會員編號memberId == member.會員編號memberId).Select(n => (n.姓氏lName + n.名字fName)).FirstOrDefault();
                fn_寄送郵件("annlan08@gmail.com", MemberName, order.fullPrice);
            }
            else
            {
                string jsonstr = JsonSerializer.Serialize(order);
                HttpContext.Session.SetString(CDictionary.SK_ORDER_INFO, jsonstr);
                return View("OPayment", order);
            }
            return View();
        }
        
        public IActionResult PaymentWebO()
        {
            CCreateOrderViewModel order = new CCreateOrderViewModel();
            string json = HttpContext.Session.GetString(CDictionary.SK_ORDER_INFO);
            order = JsonSerializer.Deserialize<CCreateOrderViewModel>(json);
            return RedirectToAction("PaymentWeb3", order);
        }
            
        #endregion

        #region //歐付寶
        public IActionResult OPayment(CCreateOrderViewModel order)
        {
            string jsonstr = JsonSerializer.Serialize(order);
            HttpContext.Session.SetString(CDictionary.SK_ORDER_INFO, jsonstr);

            #region 金流支付
            string tradeNo = Guid.NewGuid().ToString();
            tradeNo = tradeNo.Substring(tradeNo.Length - 12, 12);
            ViewBag.tradeNo = tradeNo;
            string timenow = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            ViewBag.timenow = timenow;
            string moviename = this._dbContext.場次screenings.Where(s => s.場次編號screeningId == order.ScreenID).Select(s => s.電影代碼movieCodeNavigation.電影編號movie.中文標題titleCht).FirstOrDefault();
            List<CTicketItemViewModel> ticketlist = fn_票種字串轉換List(order.TicketInfo);
            int total = Convert.ToInt32(order.fullPrice);
            string ItemName = "";
            string filename = "";
            string ticketname ="";
            decimal ticketprice = 0;
            foreach (var item in ticketlist)
            {
                filename = this._dbContext.場次screenings.Where(s => s.場次編號screeningId == order.ScreenID).Select(s => s.電影代碼movieCodeNavigation.電影編號movie.中文標題titleCht).FirstOrDefault();
                ticketname = this._dbContext.票價資訊ticketPrices.Where(t => t.票價明細ticketId == item.TicketID).FirstOrDefault().票種編號ticketType.票種名稱ticketTypeName;
                ticketprice = this._dbContext.票價資訊ticketPrices.Where(t => t.票價明細ticketId == item.TicketID).FirstOrDefault().價格ticketPrice;
                ItemName += $"{filename}-{ticketname}({ticketprice}元)X{item.TicketCount}#";
            }
            ItemName = ItemName.Substring(0, ItemName.Length - 1);
            ViewBag.Total = total;
            ViewBag.ItemName = ItemName;
            NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["HashKey"] = "5294y06JbISpM5x9";
            parameters["ChoosePayment"] = "Credit";
            parameters["ClientBackURL"] = $"{Request.Scheme}://{Request.Host}/Ticketing/PaymentWebO";    //完成後跳回去的頁面
            parameters["CreditInstallment"] = "";
            parameters["EncryptType"] = "1";
            parameters["InstallmentAmount"] = "";
            parameters["ItemName"] = ItemName;
            parameters["MerchantID"] = "2000132";
            parameters["MerchantTradeDate"] = timenow;
            parameters["MerchantTradeNo"] = tradeNo;
            parameters["PaymentType"] = "aio";
            parameters["Redeem"] = "";
            parameters["ReturnURL"] = "https://developers.opay.tw/AioMock/MerchantReturnUrl";
            parameters["StoreID"] = "";
            parameters["TotalAmount"] = total.ToString();
            parameters["TradeDesc"] = "建立信用卡測試訂單";
            parameters["HashIV"] = "v77hoKGq4kWxNNIS";

            ViewBag.ClientBackURL = $"{Request.Scheme}://{Request.Host}/Ticketing/PaymentWebO";

            string checkMacValue = parameters.ToString();

            checkMacValue = checkMacValue.Replace("=", "%3d").Replace("&", "%26");

            using var hash = SHA256.Create();
            var byteArray = hash.ComputeHash(Encoding.UTF8.GetBytes(checkMacValue.ToLower()));
            checkMacValue = Convert.ToHexString(byteArray).ToUpper();
            ViewBag.checkMacValue = checkMacValue;
            #endregion
            return View();
        }

        [NonAction]
        public string Get_SHA256_Hash(string value)
        {
            using var hash = SHA256.Create();
            var byteArray = hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value));
            return Convert.ToHexString(byteArray).ToUpper();
        }
        #endregion

        #region ViewComponent區

        public IActionResult ComponentSelectMovie(string data)
        {
            HttpContext.Session.SetString(CDictionary.SK_選擇的放映日期, data);
            DateTime CurrentDate = fn_字串轉日期格式(data);
            return ViewComponent("SelectMovie", CurrentDate);
        }

        public IActionResult ComponentSelectScreen(int Code)
        {
            HttpContext.Session.SetInt32(CDictionary.SK_選擇的電影Code, Code);
            string data = HttpContext.Session.GetString(CDictionary.SK_選擇的放映日期);
            DateTime CurrentDate = fn_字串轉日期格式(data);
            return ViewComponent("SelectScreen", CurrentDate);
        }

        public IActionResult ComponentSelectScreen2(string data)
        {
            HttpContext.Session.SetString(CDictionary.SK_選擇的放映日期, data);
            DateTime CurrentDate = fn_字串轉日期格式(data);
            return ViewComponent("SelectScreen", CurrentDate);
        }

        public IActionResult ComponentMovieInfo(int Code)
        {
            int id = this._dbContext.電影代碼movieCodes
                         .Where(m => m.電影代碼編號movieCodeId == Code)
                         .FirstOrDefault().電影編號movieId;
            return ViewComponent("MovieInfo", id);
        }


        #endregion

        #region Ajax區




        #endregion

        #region 驗證信

        public IActionResult sendmail(string email)
        {
            MimeMessage message = new MimeMessage();
            BodyBuilder builder = new BodyBuilder();

            Random ran = new Random();
            int rannum = ran.Next(9999) + 1966728;
            HttpContext.Session.SetInt32(CDictionary.SK_購票驗證碼, rannum);
            builder.HtmlBody = $"<p>你好，你的驗證碼為{rannum}</p>" +
                              $"<div style='border: 2px solid black;text-align: center;'>      </div>" +
                              $"<p>當前時間:{DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>";

            message.From.Add(new MailboxAddress("MDA訂票官網", "annlan08@outlook.com"));
            message.To.Add(new MailboxAddress("親愛的顧客", email));
            message.Subject = "MDA訂票網付款資訊";
            message.Body = builder.ToMessageBody();

            using (SmtpClient client = new SmtpClient())
            {
                client.Connect("smtp.outlook.com", 25, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate("annlan08@outlook.com", "ssbb19970513");
                client.Send(message);
                client.Disconnect(true);
            }
            return Json("K");
        }

        public IActionResult checkcode(int code) //==> 1成功 0失敗
        {
            int? Code = HttpContext.Session.GetInt32(CDictionary.SK_購票驗證碼);

            if (Code != null)
            {
                if ((int)Code == code)
                {
                    return Json("1");
                }
            }

            return Json("0");
        }

        #endregion

        #region 內建方法區

        [NonAction]
        public List<CTicketItemViewModel> fn_票種字串轉換List(string ticketString) //==>將  (回傳字串 id:count#id:count#  id=票價明細 count=數量) 轉成 一個 List<CTicketItemViewModel>
        {
            List<CTicketItemViewModel> tciketList = new List<CTicketItemViewModel>();
            string[] ticketArr = ticketString.Split('#');
            for (int i = 0; i < (ticketArr.Length - 1); i++)
            {
                string[] itemArr = ticketArr[i].Split(':');
                int id = Convert.ToInt32(itemArr[0]);
                int count = Convert.ToInt32(itemArr[1]);
                CTicketItemViewModel Tickitem = new CTicketItemViewModel()
                {
                    TicketID = id,
                    TicketCount = count,
                    TicketName = this._dbContext.票價資訊ticketPrices.Where(t => t.票價明細ticketId == id).Select(t => t.票種編號ticketType.票種名稱ticketTypeName).FirstOrDefault(),
                    TicketPrice = this._dbContext.票價資訊ticketPrices.Where(t => t.票價明細ticketId == id).Select(t => t.價格ticketPrice).FirstOrDefault(),
                };
                tciketList.Add(Tickitem);
            }
            return tciketList;
        }

        [NonAction]
        public void fn_建立新訂單總表(int ScreenID, int MemberID)
        {
            訂單總表order order = new 訂單總表order()
            {
                場次編號screeningId = ScreenID,
                會員編號memberId = MemberID,
                訂單時間orderTime = DateTime.Now,
                訂單狀態編號orderStatusId = 2,
            };
            this._dbContext.訂單總表orders.Add(order);
            this._dbContext.SaveChanges();
        }

        [NonAction]
        public void fn_建立新訂單明細(int orderID, string TicketInfo)
        {
            string[] TickerArr = TicketInfo.Split('#');

            for (int i = 0; i < (TickerArr.Count() - 1); i++)
            {
                if (string.IsNullOrWhiteSpace(TickerArr[i]))
                    break;
                string[] InfoArr = TickerArr[i].Split(':');
                訂單明細orderDetail orderDetail = new 訂單明細orderDetail()
                {
                    訂單編號orderId = orderID,
                    票價明細ticketId = Convert.ToInt32(InfoArr[0]),
                    張數count = Convert.ToInt32(InfoArr[1]),
                };
                this._dbContext.訂單明細orderDetails.Add(orderDetail);
            }

            this._dbContext.SaveChanges();
        }

        [NonAction]
        public void fn_建立新出售座位明細(int orderID, int screenID, string seatInfo)
        {
            string[] seatArr = seatInfo.Split('#');
            for (int i = 0; i < seatArr.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(seatArr[i]))
                    break;
                出售座位明細seatSold seatSold = new 出售座位明細seatSold()
                {
                    訂單編號orderId = orderID,
                    場次編號screeningId = screenID,
                    座位表編號seatId = seatArr[i],
                };

                this._dbContext.出售座位明細seatSolds.Add(seatSold);
            }
            this._dbContext.SaveChanges();
        }

        [NonAction]
        public void fn_修改出售座位狀態(int screenID, string seatInfo)
        {
            string[] selectSeatArr = seatInfo.Split('#');
            出售座位狀態seatStatus seatStatus = this._dbContext.出售座位狀態seatStatuses.Where(seat => seat.場次編號screeningId == screenID).FirstOrDefault();
            string[] seatArr = seatStatus.出售座位資訊seatSoldInfo.Split('@');
            for (int i = 0; i < seatArr.Count(); i++)
            {
                foreach (string selectitem in selectSeatArr)
                {
                    if (seatArr[i].Trim() == selectitem.Trim())
                    {
                        seatArr[i] += "saled";
                    }
                }
            }
            seatStatus.出售座位資訊seatSoldInfo = "";
            for (int i = 0; i < seatArr.Count(); i++)
            {
                seatStatus.出售座位資訊seatSoldInfo += seatArr[i] + "@";
            }
            this._dbContext.SaveChanges();
        }

        [NonAction]
        public void fn_寄送郵件(string email, string name, decimal fullPrice)
        {
            MimeMessage message = new MimeMessage();
            BodyBuilder builder = new BodyBuilder();
            //var image = builder.LinkedResources.Add("C:\\Users\\Student\\Documents\\123\\ClientMDA\\wwwroot\\images\\Ticketing\\3.jpg");

            builder.HtmlBody = $"<p>{name}你好 ，不能退票</p>" +
                              $"<p>價錢一共是{fullPrice.ToString("0.00")}</p>" +
                              $"<div style='border: 1px solid black;text-align: center;'>一個大框框</div>" +
                              $"<p>當前時間:{DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>" +
                              $"<p>哈哈{name} 嫩</p>";

            message.From.Add(new MailboxAddress("MDA訂票官網", "annlan08@outlook.com"));
            message.To.Add(new MailboxAddress(name, email));
            message.Subject = "MDA訂票網付款資訊";
            message.Body = builder.ToMessageBody();


            using (SmtpClient client = new SmtpClient())
            {
                client.Connect("smtp.outlook.com", 25, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate("annlan08@outlook.com", "ssbb19970513");
                client.Send(message);
                client.Disconnect(true);
            }

        }

        [NonAction]
        public int fn_計算座位數(string seatInfo)
        {
            string[] seatArr = seatInfo.Split('@');
            int count = 0;
            foreach (string item in seatArr)
            {
                if (!(item.Contains("NA")) && !(item.Contains("saled")) && !(string.IsNullOrWhiteSpace(item)) && !(item.Contains("||")) && !(string.IsNullOrEmpty(item))) 
                    count++;
            }
            return count;
        }

        [NonAction]
        public DateTime fn_字串轉日期格式(string DateString)
        {
            string[] dateArr = DateString.Split('/');
            string[] dayArr = dateArr[2].Split(' ');
            DateTime Date = new DateTime(Convert.ToInt32(dateArr[0]), Convert.ToInt32(dateArr[1]), Convert.ToInt32(dayArr[0]));
            return Date;

        }
        #endregion
    }
}
