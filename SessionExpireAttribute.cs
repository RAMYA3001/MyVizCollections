using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;



public class SessionExpireAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        var session = filterContext.HttpContext.Session;

        string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
        string action = filterContext.ActionDescriptor.ActionName;

        // Allow login page
        if (controller == "Login" && action == "Index")
            return;

        // If session missing
        if (session == null || session["Username"] == null || session["LoginTime"] == null)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary {
                    { "controller", "Login" },
                    { "action", "Index" }
                });
            return;
        }



        // 🔴 STEP 1: SESSION NULL CHECK (FIRST)
        if (session == null || session["Username"] == null || session["LoginTime"] == null)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary {
                { "controller", "Login" },
                { "action", "Index" }
                });
            return;
        }

        // 🔴 STEP 2: TIMEOUT CHECK (SECOND)
        DateTime loginTime = Convert.ToDateTime(session["LoginTime"]);

        if (DateTime.Now > loginTime.AddMinutes(60)) // test
        {
            session.Clear();

            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary {
                { "controller", "Login" },
                { "action", "Index" }
                });
            return;
        }

        base.OnActionExecuting(filterContext);
    }
}