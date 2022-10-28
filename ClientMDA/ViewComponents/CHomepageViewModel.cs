
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientMDA.ViewModels
{
    public class CHomepageViewModel
    {
        //rate
        public int 會員編號Member_ID { get; set; }
        public int 電影編號Movie_ID { get; set; }
        public DateTime 發佈時間Comment_Time { get; set; }
        public decimal 評分Rate { get; set; }
        //list
        public int 片單總表編號MovieList_ID { get; set; }
        public int 會員編號Member_IDbook { get; set; }
        public int 電影編號Movie_IDbook { get; set; }
    }

    
    
}
