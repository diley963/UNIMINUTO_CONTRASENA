using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PasswordChange.Filter
{
    public class Seguridad : ActionFilterAttribute 
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var IsValidUser = HttpContext.Current.Session["IsValidUser"];

            if (IsValidUser==null)
            {
                filterContext.Result = new RedirectResult("~/Home/Index");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}