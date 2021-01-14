//HUUUUGE CREDIT TO magictrip https://github.com/magictripgames/X4MagicTripLogAnalyzer

using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Models
{
    public class Ware
    {
        public Ware()
        {
        }

        [Key]
        public string WareID { get; set; }
        public string Name { get; set; }
        public string TransportType { get; set; }
        public decimal MarketMinimumPrice { get; set; }

        public decimal MarketAveragePrice { get; set; }
        public decimal MarketMaximumPrice { get; set; }
        public decimal Volume { get; set; }

        public decimal MaxAndMinPriceDifferencePerVolume
        {
            get
            {
                return (MarketMaximumPrice - MarketMinimumPrice) / Volume;
            }
        }
    }
}
