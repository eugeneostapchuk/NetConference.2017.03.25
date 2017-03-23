using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PerformAspNetMcv.Tips
{
    public static class RouteUrlExtensions
    {
        private static readonly ConcurrentDictionary<Type, Delegate> _routeFuncs = new ConcurrentDictionary<Type, Delegate>();

        public static MvcHtmlString A<TController>(this HtmlHelper htmlHelper, string innerHtml, string cssClass) where TController : IRouteUrl
        {
            var url = Url<TController>();

            TagBuilder tagBuilder = new TagBuilder("a")
            {
                InnerHtml = !string.IsNullOrEmpty(innerHtml) ? HttpUtility.HtmlEncode(innerHtml) : string.Empty
            };

            tagBuilder.MergeAttribute("href", url);
            tagBuilder.MergeAttribute("class", cssClass);
            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString AWithDirectWriter<TController>(this HtmlHelper htmlHelper, string innerHtml, string cssClass) where TController : IRouteUrl
        {
            var url = Url<TController>();

            var writer = htmlHelper.ViewContext.Writer;

            writer.Write("<a href=\"");
            writer.Write(url);
            writer.Write("\"");

            writer.Write(" class=\"");
            writer.Write(cssClass);
            writer.Write("\"");

            writer.Write(">");
            writer.Write(HttpUtility.HtmlEncode(innerHtml));
            writer.Write("</a>");

            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString ImgWithDirectWriter(this HtmlHelper htmlHelper, string src) 
        {
            var writer = htmlHelper.ViewContext.Writer;

            writer.Write("<img src=\"");
            writer.Write(src);
            writer.Write("\">");

            return MvcHtmlString.Empty;
        }

        //public static MvcHtmlString A<TController, TValue>(this HtmlHelper htmlHelper, TValue value, string innerHtml, string cssClass) where TController : IRouteUrl<TValue>
        //{
        //    var url = Url<TController, TValue>(value);

        //    TagBuilder tagBuilder = new TagBuilder("a")
        //    {
        //        InnerHtml = !string.IsNullOrEmpty(innerHtml) ? HttpUtility.HtmlEncode(innerHtml) : string.Empty
        //    };

        //    tagBuilder.MergeAttribute("href", url);
        //    tagBuilder.MergeAttribute("class", cssClass);
        //    return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
        //}

        public static string Url<TRoute>() where TRoute : IRouteUrl
        {
           Type controllerType = typeof(TRoute);

            Delegate func = null;
            Func<string> concreteFunc = null;

            if (_routeFuncs.TryGetValue(controllerType, out func))
            {
                concreteFunc = func as Func<string>;

                return concreteFunc();
            }

            InterfaceMapping map = controllerType.GetInterfaceMap(typeof(IRouteUrl));

            foreach (var methodInfo in map.TargetMethods)
            {
                string route = GetRouteUri(methodInfo);

                route = route.ClearFixRoute();

                concreteFunc = () => route;

                _routeFuncs[controllerType] = concreteFunc;

                return concreteFunc();
            }

            return string.Empty;
        }


        private static string GetRouteUri(MethodInfo methodInfo)
        {
            var expectedControllerName = methodInfo.DeclaringType.Name.Replace("Controller", string.Empty);

            var url = RouteTable.Routes
                .ToArray()
                .Cast<Route>()
                .Where(i => i.Defaults != null && i.Defaults.ContainsKey("controller"))
                .Where(i => i.Defaults["controller"].ToString() == expectedControllerName)
                .Select(i => i.Url)
                .FirstOrDefault();

            return url;
        }

        private static string ClearFixRoute(this string route)
        {
            if (!route.StartsWith("/") || !route.StartsWith("\\"))
            {
                route = "/" + route;
            }

            return route;
        }
    }
}