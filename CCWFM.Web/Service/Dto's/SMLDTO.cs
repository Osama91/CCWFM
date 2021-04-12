using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace CCWFM.Web.Service
{
    public class GlGroupsDtp
    {
        public int TblCurrency { get; set; }

        public decimal Cost { get; set; }

        public decimal CostWithoutMisc { get; set; }

        public decimal CostPercentage { get; set; }

        [DataMember]
        public int? GroupName { get; set; }

        public double ExchangeRate { get; set; }
    }

    public class GlGroupsDto
    {
        public int TblCurrency { get; set; }

        public double Cost { get; set; }

        public double CostWithoutMisc { get; set; }

        public double CostPercentage { get; set; }

        [DataMember]
        public string  GroupName { get; set; }
    }

    [DataContract]
    public class SMLDTO
    {
        private int _qty;

        [DataMember]
        public string StyleCode { get; set; }

        [DataMember]
        public string ColorCode { get; set; }

        [DataMember]
        public int Qty
        {
            get { return _qty; }
            set
            {
                _qty = value;
                if (value <= 0 || !SmlSizes.Any()) return;
                var ratiosSum = SmlSizes.Sum(x => x.Ratio);
                SmlSizes.ForEach(x => x.Qty = int.Parse(Math.Floor(value * (x.Ratio / ratiosSum)).ToString()));
                var max = SmlSizes.Max(x => x.Qty);
                var sum = SmlSizes.Sum(x => x.Qty);
                var firstOrDefault = SmlSizes.FirstOrDefault(x => x.Qty == max);
                if (firstOrDefault != null)
                    firstOrDefault.Qty += (value - sum);
            }
        }

        [DataMember]
        public string BrandCode { get; set; }

        [DataMember]
        public string Desc { get; set; }

        [DataMember]
        public string SeasonCode { get; set; }

        [DataMember]
        public string SizeRange { get; set; }

        [DataMember]
        public bool IsSampleRequested { get; set; }

        [DataMember]
        public int? RFSSerial { get; set; }

        [DataMember]
        public int SMLSerial { get; set; }

        [DataMember]
        public List<SMLSizesDTO> SmlSizes { get; set; }

        [DataMember]
        public decimal? Cost { get; set; }

        [DataMember]
        public decimal? ExchangeRate { get; set; }

        [DataMember]
        public decimal? LandedCost { get; set; }
    }

    [DataContract]
    public class SMLSizesDTO
    {
        [DataMember]
        public string SizeCode { get; set; }

        [DataMember]
        public float Ratio { get; set; }

        [DataMember]
        public int Qty { get; set; }
    }
}