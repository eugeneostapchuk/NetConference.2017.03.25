using System.Web.Mvc;

namespace PerformAspNetMcv.Tips
{
    public interface IRouteUrl
    {
        ActionResult Invoke();
    }

    public interface IRouteUrl<in T>
    {
        ActionResult Invoke(T value);
    }

    public interface IRouteUrl<in T1, in T2>
    {
        ActionResult Invoke(T1 value, T2 value2);
    }

    public interface IRouteUrl<in T1, in T2, in T3>
    {
        ActionResult Invoke(T1 value, T2 value2, T3 value3);
    }
}