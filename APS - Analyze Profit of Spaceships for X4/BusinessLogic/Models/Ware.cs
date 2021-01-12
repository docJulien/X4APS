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
        public double MarketMinimumPrice { get; set; }

        public double MarketAveragePrice { get; set; }
        public double MarketMaximumPrice { get; set; }
        public double Volume { get; set; }

        public double MaxAndMinPriceDifferencePerVolume
        {
            get
            {
                return (MarketMaximumPrice - MarketMinimumPrice) / Volume;
            }
        }
    }
}
