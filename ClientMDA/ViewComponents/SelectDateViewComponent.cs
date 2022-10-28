using ClientMDA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientMDA.ViewComponents
{
    public class SelectDateViewComponent:ViewComponent
    {
        private MDAContext _dbContext;

        public SelectDateViewComponent(MDAContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(int flag) //flag==1 沒有給電影CODE  flag==2有給電影CODE
        {
            #region 用來玩的

            int? theaterID = HttpContext.Session.GetInt32(CDictionary.SK_選擇的電影院ID);
            int? MovieCode = HttpContext.Session.GetInt32(CDictionary.SK_選擇的電影Code);
            List<DateTime> list = new List<DateTime>();

            if (flag == 1)
            {
                list = await this._dbContext.場次screenings
                             .Where(s => s.影廳編號cinema.電影院編號theaterId == theaterID)
                             .Select(s => s.放映日期playDate).Distinct().ToListAsync();
            }

            if (flag == 2)
            {
                list = await this._dbContext.場次screenings
                             .Where(s => s.影廳編號cinema.電影院編號theaterId == theaterID&&s.電影代碼movieCode==MovieCode)
                             .Select(s => s.放映日期playDate).Distinct().ToListAsync();
            }

            #endregion

            return View(list);
        }

    }
}
