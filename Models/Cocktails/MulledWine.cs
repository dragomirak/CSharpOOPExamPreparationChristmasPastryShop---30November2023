using System;
using System.Collections.Generic;
using System.Text;

namespace ChristmasPastryShop.Models.Cocktails
{
    public class MulledWine : Cocktail
    {
        private const double LargeSizePrice = 13.50;
        private double price;

        public MulledWine(string name, string size)
            : base(name, size, LargeSizePrice)
        {
        }
    }
}


