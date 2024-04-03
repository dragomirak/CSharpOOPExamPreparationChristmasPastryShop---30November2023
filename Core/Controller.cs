using ChristmasPastryShop.Core.Contracts;
using ChristmasPastryShop.Models.Booths;
using ChristmasPastryShop.Models.Cocktails;
using ChristmasPastryShop.Models.Delicacies;
using ChristmasPastryShop.Models.Delicacies.Contracts;
using ChristmasPastryShop.Repositories;
using ChristmasPastryShop.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChristmasPastryShop.Core
{
    public class Controller : IController
    {
        private BoothRepository booths;
        public Controller()
        {
            booths = new BoothRepository();
        }
        public string AddBooth(int capacity)
        {
            Booth booth = new Booth(booths.Models.Count + 1, capacity);
            booths.AddModel(booth);
            return string.Format(OutputMessages.NewBoothAdded, booth.BoothId, capacity);
        }

        public string AddCocktail(int boothId, string cocktailTypeName, string cocktailName, string size)
        {
            if (cocktailTypeName != "MulledWine" && cocktailTypeName != "Hibernation")
            {
                return string.Format(OutputMessages.InvalidCocktailType, cocktailTypeName);
            }

            if (size != "Large" && size != "Middle" && size != "Small")
            {
                return string.Format(OutputMessages.InvalidCocktailSize, size);
            }

            var booth = booths.Models.First(b => b.BoothId == boothId);
            var cocktail = booth.CocktailMenu.Models.FirstOrDefault(c => c.Name == cocktailName && c.Size == size);

            if (cocktail != null)
            {
                return string.Format(OutputMessages.CocktailAlreadyAdded, cocktailName, size);
            }

            if (cocktailTypeName == "MulledWine")
            {
                cocktail = new MulledWine(cocktailName, size);
            }
            else if (cocktailTypeName == "Hibernation")
            {
                cocktail = new Hibernation(cocktailName, size);
            }

            booth.CocktailMenu.AddModel(cocktail);

            return string.Format(OutputMessages.NewCocktailAdded, size, cocktailName, cocktailTypeName);
        }

        public string AddDelicacy(int boothId, string delicacyTypeName, string delicacyName)
        {
            if (delicacyTypeName != "Stolen" && delicacyTypeName != "Gingerbread")
            {
                return string.Format(OutputMessages.InvalidDelicacyType, delicacyTypeName);
            }

            var booth = booths.Models.First(b => b.BoothId == boothId);
            var delicacy = booth.DelicacyMenu.Models.FirstOrDefault(d => d.Name == delicacyName);

            if (delicacy != null)
            {
                return string.Format(OutputMessages.DelicacyAlreadyAdded, delicacyName);
            }

            if (delicacyTypeName == "Stolen")
            {
                delicacy = new Stolen(delicacyName);
            }
            else if (delicacyTypeName == "Gingerbread")
            {
                delicacy = new Gingerbread(delicacyName);
            }

            booth.DelicacyMenu.AddModel(delicacy);

            return string.Format(OutputMessages.NewDelicacyAdded, delicacyTypeName, delicacyName);
        }

        public string BoothReport(int boothId)
        {
            var booth = booths.Models.First(b => b.BoothId == boothId);
            return booth.ToString().TrimEnd();
        }

        public string LeaveBooth(int boothId)
        {
            var booth = booths.Models.FirstOrDefault(b => b.BoothId == boothId);
           
            var sb = new StringBuilder();
            sb.AppendLine($"Bill {booth.CurrentBill:f2} lv");
            sb.AppendLine($"Booth {boothId} is now available!");

            booth.Charge();
            booth.ChangeStatus();

            return sb.ToString().TrimEnd();
        }

        public string ReserveBooth(int countOfPeople)
        {
            var booth = booths.Models
                .Where(b => b.IsReserved == false && b.Capacity >= countOfPeople)
                .OrderBy(b => b.Capacity)
                .ThenByDescending(b => b.BoothId)
                .FirstOrDefault();

            if (booth == null)
            {
                return $"No available booth for {countOfPeople} people!";
            }

            booth.ChangeStatus();
            return $"Booth {booth.BoothId} has been reserved for {countOfPeople} people!";
        }

        public string TryOrder(int boothId, string order)
        { 
            var cmdArgs = order.Split("/");
            var itemTypeName = cmdArgs[0];
            var itemName = cmdArgs[1];
            int countOfItems = int.Parse(cmdArgs[2]);
            var size = string.Empty;

            if (itemTypeName == "Hibernation" || itemTypeName == "MulledWine")
            {
                size = cmdArgs[3];
            }

            var booth = booths.Models.First(b => b.BoothId == boothId);

            if (itemTypeName != "Hibernation" && itemTypeName != "MulledWine"
                && itemTypeName != "Stolen" && itemTypeName != "Gingerbread")
            {
                return $"{itemTypeName} is not recognized type!";
            }

            if(itemTypeName == "Hibernation" || itemTypeName == "MulledWine")
            {
                if (!booth.CocktailMenu.Models.Any(c => c.Name == itemName))
                {
                    return $"There is no {itemTypeName} {itemName} available!";
                }

                var cocktail = booth.CocktailMenu.Models.FirstOrDefault(c => c.Name == itemName && c.Size == size);
                if (cocktail == null)
                {
                    return $"There is no {size} {itemName} available!";
                }

                booth.UpdateCurrentBill(cocktail.Price * countOfItems);
                return $"Booth {boothId} ordered {countOfItems} {itemName}!";
            }
            else
            {
                if (!booth.DelicacyMenu.Models.Any(c => c.Name == itemName))
                {
                    return $"There is no {itemTypeName} {itemName} available!";
                }

                var delicacy = booth.DelicacyMenu.Models.First(d => d.Name == itemName);
                
                booth.UpdateCurrentBill(delicacy.Price * countOfItems);
                return $"Booth {boothId} ordered {countOfItems} {itemName}!";
            }
                       
        }
    }
}
