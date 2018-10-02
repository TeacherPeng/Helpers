using System.Web.UI;

namespace PengSW.WebHelper
{
    public static class PageHelper
    {
        public static void L(this Page aPage, string aLog)
        {
            if (aPage.Session["Log"] == null)
                aPage.Session["Log"] = aLog;
            else
                aPage.Session["Log"] += "/" + aLog;
        }
    }
}
