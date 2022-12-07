using System;
using System.Net.Http;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Config;
using Microsoft.AspNetCore.Mvc;
using FireSharp.Interfaces;
using FireSharp;
using Netflix_backend.Models;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Netflix_backend.Controllers
{
    public class AnalyticController:  Controller
    {
        public AnalyticController()
        {
        }

        public JsonResult Compare() {
            try {
                DateTime dateTime = DateTime.UtcNow.Date;
                String date = dateTime.ToString().Split(" ")[0];
                String[] parts = date.Split("/");
                String key_current = parts[0] + "-" + parts[2];
                String key_prev = (Int64.Parse(parts[0]) - 1 % 12).ToString() + "-" + parts[2];
                IFirebaseConfig ifc = new FirebaseConfig()
                {
                    AuthSecret = "1Cwpr8Gn6GUoOYRxb3rS6OXzH3gX8v8SEuHzZWLi",
                    BasePath = "https://netflex-17f65-default-rtdb.asia-southeast1.firebasedatabase.app"
                };


                IFirebaseClient client = new FirebaseClient(ifc);
                FirebaseResponse resp = client.Get(@"Analytics/" + key_current);
                Analytic analytic1 = resp.ResultAs<Analytic>();
                resp = client.Get(@"Analytics/" + key_prev);
                Analytic analytic2 = resp.ResultAs<Analytic>();
                float revenue = analytic1.Revenue - analytic2.Revenue;
                float cost = analytic1.Cost - analytic2.Cost;
                float sales = analytic1.Sales - analytic2.Sales;
                float revenue_percentage = ((analytic1.Revenue - analytic2.Revenue) / analytic2.Revenue) * 100;
                float cost_percentage = ((analytic1.Cost - analytic2.Cost) / analytic2.Cost) * 100;
                float sales_percentage = ((analytic1.Sales - analytic2.Sales) / analytic2.Sales) * 100;
                AnalyticsDetail analytics_detail = new AnalyticsDetail(revenue, sales, cost, revenue_percentage, sales_percentage, cost_percentage);
                return Json(analytics_detail);

            }
            catch(Exception ex) {
                Response res = new Response(400, ex.Message);
                return new JsonResult(res);
            }
        }

    }
}
