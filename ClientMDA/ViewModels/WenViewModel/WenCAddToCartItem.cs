using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientMDA.Models;
using ClientMDA.ViewModel;

namespace ClientMDA.ViewModel.WenViewModel
{
    public class WenCAddToCartItem
    {
        public int prdId { get; set; }
        public 商品資料product product { get; set; }
        public int Count { get; set; }
        public decimal price { get; set; }
        public decimal 小計
        {
            get
            {
                return this.price * this.Count;
            }
        }
    }
}
