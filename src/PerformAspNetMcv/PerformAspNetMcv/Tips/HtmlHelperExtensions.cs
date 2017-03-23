using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace PerformAspNetMcv.Tips
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Gets a link to a named route, you only have to supply the non default routeValues to get a link
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText">text shown in the link</param>
        /// <param name="routeName">name of the route to use</param>
        /// <param name="routeValues">extra routevalues</param>
        /// <returns></returns>

        public static MvcHtmlString ActionLink<T>(this HtmlHelper htmlHelper, string linkText,
            Expression<Func<T, ActionResult>> actionSelector
            ) where T : Controller
        {
            return htmlHelper.ActionLink<T>(linkText, actionSelector, null);
        }

        public static MvcHtmlString ActionLink<T>(this HtmlHelper htmlHelper, string linkText,
            Expression<Func<T, ActionResult>> actionSelector,
            RouteValueDictionary routeValues
            ) where T : Controller
        {
            return htmlHelper.ActionLink<T>(linkText, actionSelector, routeValues, null);
        }

        public static MvcHtmlString ActionLink<T>(this HtmlHelper htmlHelper, string linkText,
            Expression<Func<T, ActionResult>> actionSelector,
            RouteValueDictionary routeValues,
            IDictionary<string, object> htmlAttributes) where T : Controller
        {
            string controller;
            string action;

            routeValues = htmlHelper.GetRouteValues(actionSelector, routeValues, out action, out controller);
            return htmlHelper.ActionLink(linkText, action, controller, routeValues, htmlAttributes);
        }

        public static RouteValueDictionary GetRouteValues<T>(this HtmlHelper htmlHelper, Expression<Func<T, ActionResult>> actionSelector, RouteValueDictionary routeValues, out string action, out string controller)
        {
            return GetRouteValues(routeValues, actionSelector, out controller, out action);
        }


        /// <summary>
        /// Generates routevalues and controller and action strings from a strong typed lambda expression of a controllermethod.
        /// </summary>
        /// <typeparam name="T">Must be a Controller</typeparam>
        /// <param name="routeValues">Can be null or empty, but you can supply extra routevalues these win if generated values overlap</param>
        /// <param name="actionSelector">a lambda expression, must be a call to a ActionResult returning method</param>
        /// <param name="controller">the name of the controller</param>
        /// <param name="action">the name of the action</param>
        /// <returns></returns>
        public static RouteValueDictionary GetRouteValues<T>(RouteValueDictionary routeValues, Expression<Func<T, ActionResult>> actionSelector, out string controller, out string action)
        {
            Type controllerType = typeof(T);
            if (routeValues == null)
            {
                routeValues = new RouteValueDictionary();
            }

            //The body of the expression must be a call to a method
            MethodCallExpression call = actionSelector.Body as MethodCallExpression;
            if (call == null)
            {
                throw new ArgumentException("You must call a method of " + controllerType.Name, "actionSelector");
            }

            //the object being called must be the controller specified in <T>
            if (call.Object.Type != controllerType)
            {
                throw new ArgumentException("You must call a method of " + controllerType.Name, "actionSelector");
            }

            //Remove the controller part of the name ProductController --> Product
            if (controllerType.Name.EndsWith("Controller"))
            {
                controller = controllerType.Name.Substring(0, controllerType.Name.Length - "Controller".Length);
            }
            else
            {
                controller = controllerType.Name;
            }
            //The action is the name of the method being called
            action = call.Method.Name;

            //get all arguments from the lambda expression
            var args = call.Arguments;

            //Get all parameters from the Action Method
            ParameterInfo[] parameters = call.Method.GetParameters();

            //pair the lambda arguments with the param names
            var pairs = args.Select((a, i) => new
            {
                Argument = a,
                ParamName = parameters[i].Name
            });


            foreach (var argumentParameterPair in pairs)
            {
                string name = argumentParameterPair.ParamName;
                if (!routeValues.ContainsKey(name))
                {
                    //the argument could be a constant or a variable or a function and must be evaluated
                    object value;
                    //If it is a constant we can get the value immediately
                    if (argumentParameterPair.Argument.NodeType == ExpressionType.Constant)
                    {
                        var constant = argumentParameterPair.Argument as ConstantExpression;
                        value = constant.Value;
                    }
                    else //if not we have to evaluate the value
                    {

                        value = Expression.Lambda(argumentParameterPair.Argument).Compile().DynamicInvoke(null);
                    }
                    if (value != null)
                    {
                        //add routevalues with the name = method parameter name (productSlug) and value = the evaluated lambda value
                        routeValues.Add(name, value);
                    }
                }
            }

            return routeValues;
        }


        public static string Action<T>(this UrlHelper url,
            Expression<Func<T, ActionResult>> actionSelector) where T : Controller
        {
            return url.Action(actionSelector, null);
        }

        public static string Action<T>(this UrlHelper url,
            Expression<Func<T, ActionResult>> actionSelector,
            RouteValueDictionary routeValues) where T : Controller
        {
            string controller;
            string action;

            routeValues = GetRouteValues(routeValues, actionSelector, out controller, out action);
            return url.Action(action, controller, routeValues);
        }
    }
}