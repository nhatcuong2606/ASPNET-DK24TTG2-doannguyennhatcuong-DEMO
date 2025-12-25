using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web;

namespace WebApplication
{
    /// <summary>
    /// Trang đăng nhập - Login.aspx
    /// Kiểm tra thông tin đăng nhập với database và lưu Session
    /// </summary>
    public partial class Login : System.Web.UI.Page
    {
        // Lấy connection string từ Web.config
        private string connectionString = ConfigurationManager.ConnectionStrings["ExpenseDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Nếu đã đăng nhập rồi thì redirect về trang chủ
            if (Session["IsLogin"] != null && (bool)Session["IsLogin"])
            {
                Response.Redirect("~/Default.aspx");
            }

            // Kiểm tra cookie ghi nhớ đăng nhập
            if (!IsPostBack)
            {
                if (Request.Cookies["Username"] != null)
                {
                    txtUsername.Text = Request.Cookies["Username"].Value;
                    chkRemember.Checked = true;
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện click nút Đăng nhập
        /// </summary>
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Kiểm tra thông tin đăng nhập trong database
            int userId = CheckLogin(username, password);

            if (userId > 0)
            {
                // Đăng nhập thành công - Lưu thông tin vào Session
                Session["UserID"] = userId;
                Session["Username"] = username;
                Session["IsLogin"] = true;

                // Xử lý ghi nhớ đăng nhập bằng Cookie
                if (chkRemember.Checked)
                {
                    HttpCookie cookie = new HttpCookie("Username");
                    cookie.Value = username;
                    cookie.Expires = DateTime.Now.AddDays(30); // Cookie tồn tại 30 ngày
                    Response.Cookies.Add(cookie);
                }
                else
                {
                    // Xóa cookie nếu không chọn ghi nhớ
                    if (Request.Cookies["Username"] != null)
                    {
                        HttpCookie cookie = new HttpCookie("Username");
                        cookie.Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies.Add(cookie);
                    }
                }

                // Chuyển hướng về trang chủ
                Response.Redirect("~/Default.aspx");
            }
            else
            {
                // Đăng nhập thất bại
                lblMessage.Text = "⚠️ Tên đăng nhập hoặc mật khẩu không đúng!";
                lblMessage.Visible = true;
            }
        }

        /// <summary>
        /// Kiểm tra thông tin đăng nhập trong database
        /// Trả về UserID nếu đúng, -1 nếu sai
        /// </summary>
        private int CheckLogin(string username, string password)
        {
            int userId = -1;

            // Sử dụng ADO.NET để truy vấn database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Câu lệnh SQL kiểm tra username và password
                string sql = "SELECT UserID FROM Users WHERE Username = @Username AND Password = @Password";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Thêm parameters để tránh SQL Injection
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            userId = Convert.ToInt32(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log lỗi (trong thực tế nên dùng logging framework)
                        lblMessage.Text = "⚠️ Lỗi kết nối database: " + ex.Message;
                        lblMessage.Visible = true;
                    }
                }
            }

            return userId;
        }
    }
}
