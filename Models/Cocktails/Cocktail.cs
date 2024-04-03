using ChristmasPastryShop.Models.Cocktails.Contracts;
using ChristmasPastryShop.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ChristmasPastryShop.Models.Cocktails
{
    public abstract class Cocktail : ICocktail
    {
        private const double MiddleSizePriceModifier = 2.0 / 3.0;
        private const double SmallSizePriceModifier = 1.0 / 3.0;
        private string name;
        private double price;
        protected Cocktail(string name, string size, double price)
        {
            Name = name;
            Size = size;
            Price = price;
        }

        public string Name
        {
            get => name;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(ExceptionMessages.NameNullOrWhitespace);
                }

                name = value;
            }
        }

        public string Size { get; private set; }

        public double Price 
        {
            get => price;
            private set
            {
                if (Size == "Large")
                {
                    price = value;
                }
                else if (Size == "Middle")
                {
                    price = value * MiddleSizePriceModifier;
                }
                else if (Size == "Small")
                {
                    price = value * SmallSizePriceModifier;
                }
            }
        }

        public override string ToString()
        {
            return $"{Name} ({Size}) - {Price:F2} lv";
        }
    }
}
