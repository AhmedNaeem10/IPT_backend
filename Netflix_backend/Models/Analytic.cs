using System;
namespace Netflix_backend.Models
{
    public class Analytic
    {
        public float Revenue { get; set; }
        public float Sales { get; set; }
        public float Cost { get; set; }

        public Analytic()
        {
        }

        public Analytic(float revenue, float sales, float cost) {
            this.Revenue = revenue;
            this.Sales = sales;
            this.Cost = cost;
        }

        public void updateSales(float sales) {
            this.Sales += sales;
            this.Revenue += sales;
        }


    }
}
