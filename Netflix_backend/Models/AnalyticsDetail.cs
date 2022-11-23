using System;
namespace Netflix_backend.Models
{
    public class AnalyticsDetail
    {
        public float Revenue { get; set; }
        public float Sales { get; set; }
        public float Cost { get; set; }
        public float Revenue_P { get; set; }
        public float Sales_P { get; set; }
        public float Cost_P { get; set; }

        public AnalyticsDetail()
        {
        }

        public AnalyticsDetail(float revenue, float sales, float cost, float r_p, float s_p, float c_p) {
            this.Revenue = revenue;
            this.Sales = sales;
            this.Cost = cost;
            this.Revenue_P = r_p;
            this.Sales_P = s_p;
            this.Cost_P = c_p;
        }

    }
}
