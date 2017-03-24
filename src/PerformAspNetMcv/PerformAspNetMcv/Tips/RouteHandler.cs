using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace PerformAspNetMcv.Tips
{
    public class RouteHandler : IRouteHandler, IHttpAsyncHandler
    {

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }

        public bool IsReusable { get; } = true;

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            var tcs = new TaskCompletionSource<object>(extraData);

            HandleRequest(context).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    tcs.SetException(t.Exception.InnerExceptions);
                }
                else
                {
                    context.Response.Flush();
                    tcs.SetResult(null);
                }

                cb(tcs.Task);
            });

            return tcs.Task;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            Task t = (Task)result;
            t.Wait();
        }

        protected Task HandleRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}