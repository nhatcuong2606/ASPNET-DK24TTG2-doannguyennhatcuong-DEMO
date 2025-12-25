using System;

namespace WebApplication
{
    /// <summary>
    /// Trang đăng xuất - Logout.aspx
    /// Xóa Session và redirect về Login.aspx
    /// </summary>
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Xóa tất cả Session
            Session.Clear();
            Session.Abandon();

            // Redirect về trang đăng nhập
            Response.Redirect("~/Login.aspx");
        }
    }
}
