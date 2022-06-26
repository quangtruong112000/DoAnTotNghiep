using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebBanThuoc
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
              name: "EditProduct",
              url: "EditProduct/{id}",
              defaults: new { controller = "Product", action = "Edit", id = UrlParameter.Optional }, namespaces: new[] { "WebBanThuoc.Controllers" }
          );
            routes.MapRoute(
      name: "EditService",
      url: "EditService/{id}",
      defaults: new { controller = "Service", action = "EditService", id = UrlParameter.Optional }, namespaces: new[] { "WebBanThuoc.Controllers" }
  );
            routes.MapRoute(
      name: "detailOrder",
      url: "detailOrder/{id}",
      defaults: new { controller = "Sendo", action = "detailOrder", id = UrlParameter.Optional }, namespaces: new[] { "WebBanThuoc.Controllers" }
  );
            routes.MapRoute(
name: "updateStatus",
url: "updateStatus/{id}",
defaults: new { controller = "Sendo", action = "updateStatus", id = UrlParameter.Optional }, namespaces: new[] { "WebBanThuoc.Controllers" }
);
            routes.MapRoute(
name: "EditOrder",
url: "EditOrder/{id}",
defaults: new { controller = "Order", action = "EditOrder", id = UrlParameter.Optional }, namespaces: new[] { "WebBanThuoc.Controllers" }
);
            
            routes.MapRoute(
name: "Transport",
url: "Transport/{id}",
defaults: new { controller = "Order", action = "Transport", id = UrlParameter.Optional }, namespaces: new[] { "WebBanThuoc.Controllers" }
);

            routes.MapRoute(
name: "CancellOrder",
url: "CancellOrder/{id}",
defaults: new { controller = "Order", action = "CancellOrder", id = UrlParameter.Optional }, namespaces: new[] { "WebBanThuoc.Controllers" }
);

                        routes.MapRoute(
name: "UpdateStatust",
url: "UpdateStatust/{id}",
defaults: new { controller = "Order", action = "UpdateStatust", id = UrlParameter.Optional }, namespaces: new[] { "WebBanThuoc.Controllers" }
);

            routes.MapRoute(
name: "Delete",
url: "Delete/{id}",
defaults: new { controller = "Order", action = "Delete", id = UrlParameter.Optional }, namespaces: new[] { "WebBanThuoc.Controllers" }
);






            routes.MapRoute(
name: "CancelOrderTransport",
url: "CancelOrderTransport/{id}",
defaults: new { controller = "Order", action = "CancelOrderTransport", id = UrlParameter.Optional }, namespaces: new[] { "WebBanThuoc.Controllers" }
);


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                  namespaces: new[] { "WebBanThuoc.Controllers" }
            );
        }
    }
}
