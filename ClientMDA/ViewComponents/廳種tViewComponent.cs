using ClientMDA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientMDA.ViewConponents
{
    public class 廳種ViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<CTheater> datas) {


            return View(datas);

        }
    }
}
