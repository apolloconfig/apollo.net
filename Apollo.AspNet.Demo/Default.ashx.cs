using System;
using System.Configuration;
using System.Web;

namespace Apollo.AspNet.Demo
{
    /// <summary>
    /// Default 的摘要说明
    /// </summary>
    public class Default : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Headers["Content-Type"] = "text/html; charset=utf-8";

            var key = context.Request.QueryString["key"];
            if (string.IsNullOrWhiteSpace(key))
                return;

            var value = DateTime.Now.Second % 2 == 1 ? ConfigurationManager.AppSettings[key] : Global.Configuration[key];
            if (value != null)
                context.Response.Write(value);
        }

        public bool IsReusable => false;
    }
}