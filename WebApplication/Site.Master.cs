using System;
using System.Web.UI;

namespace WebApplication
{
    /// <summary>
    /// Master Page: Quản lý layout chung và kiểm tra đăng nhập
    /// </summary>
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Kiểm tra đăng nhập - nếu chưa đăng nhập thì redirect về Login.aspx
            // Bỏ qua kiểm tra nếu đang ở trang Login hoặc Register
            string currentPage = Request.Url.AbsolutePath.ToLower();
            if (!currentPage.Contains("login.aspx") && !currentPage.Contains("register.aspx"))
            {
                if (Session["IsLogin"] == null || !(bool)Session["IsLogin"])
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                // Hiển thị tên người dùng đã đăng nhập
                if (Session["Username"] != null)
                {
                    lblUsername.Text = "👤 " + Session["Username"].ToString();
                }
            }
        }
    }
}
