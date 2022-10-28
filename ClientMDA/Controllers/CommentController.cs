using ClientMDA.Models;
using ClientMDA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientMDA.Controllers
{
    public class CommentController : Controller
    {
        private readonly MDAContext _MDAcontext;

        public CommentController(MDAContext MDAcontext)  //相依性注入
        {
            _MDAcontext = MDAcontext;
        }

        public IActionResult 評論首頁() //最新/熱門/關注評論
        {
            List<CCommentViewModel> datas = null;
            var mPoster = _MDAcontext.評論圖片明細commentImagesLists.Select(i => i);
            datas = _MDAcontext.電影評論movieComments.OrderByDescending(c => c.發佈時間commentTime).Select
                    (c => new CCommentViewModel
                    {
                        comment = c,
                        //暱稱nickName = c.會員編號member.暱稱nickName,
                        cImgFrList = _MDAcontext.評論圖片明細commentImagesLists.Where(i => i.評論編號commentId == c.評論編號commentId)
                        .Select(c => c.評論圖庫編號commentImage.圖片image).ToList()
                    }).Take(5).ToList();
            return View(datas);
        }

        public IActionResult 最新評論() //最新評論
        {
            List<CCommentViewModel> datas = null;
            var mPoster = _MDAcontext.評論圖片明細commentImagesLists.Select(i => i);
            datas = _MDAcontext.電影評論movieComments.OrderByDescending(c => c.發佈時間commentTime).Select
                    (c => new CCommentViewModel
                    {
                        comment = c,
                        //暱稱nickName = c.會員編號member.暱稱nickName,
                        cImgFrList = _MDAcontext.評論圖片明細commentImagesLists.Where(i => i.評論編號commentId == c.評論編號commentId)
                        .Select(c => c.評論圖庫編號commentImage.圖片image).ToList()
                    }).Take(6).ToList();

            return ViewComponent("最新評論", datas);
        }

        public IActionResult 關注評論() //關注評論
        {
            List<CCommentViewModel> datas = null;
            var mPoster = _MDAcontext.評論圖片明細commentImagesLists.Select(i => i);
            datas = _MDAcontext.電影評論movieComments.OrderBy(c => c.會員編號memberId).Select
                    (c => new CCommentViewModel
                    {
                        comment = c,
                        //暱稱nickName = c.會員編號member.暱稱nickName,
                        cImgFrList = _MDAcontext.評論圖片明細commentImagesLists.Where(i => i.評論編號commentId == c.評論編號commentId)
                        .Select(c => c.評論圖庫編號commentImage.圖片image).ToList()
                    }).Take(6).ToList();

            return ViewComponent("關注評論", datas);
        }

        public IActionResult 電影評論(int? id) //電影單則評論
        {
            CCommentViewModel datas = null;
            datas = _MDAcontext.電影評論movieComments.Where(c => c.電影編號movieId == id).Select
            (c => new CCommentViewModel
            {
                comment = c,
                公開等級public = c.公開等級編號public.公開等級public,
                中文標題titleCht = c.電影編號movie.中文標題titleCht,
                //暱稱nickName = c.會員編號member.暱稱nickName,
                //會員照片image = c.會員編號member.會員照片image,
                //回覆內容floors = c.回覆樓數floors.
                //發佈時間floorTime = c.
                //被按讚次數thumbsUp
                //被倒讚次數thumbsDown
                //標籤明細編號chId =c.標籤明細hashtagsLists
                //標籤編號hashtagId
                //標籤hashtag
                //評論圖片編號ccId
                //評論圖庫編號commentImageId
                //圖片image
            }).FirstOrDefault();

            return View(datas);
        }

        public IActionResult 會員評論() //會員個別評論頁面
        {
            return View();
        }
    }
}