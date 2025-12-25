using System;
using System.Data.SqlClient;
using System.Configuration;

namespace WebApplication
{
    /// <summary>
    /// Trang đăng ký tài khoản - Register.aspx
    /// Cho phép người dùng tạo tài khoản mới
    /// </summary>
    public partial class Register : System.Web.UI.Page
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
        }

        /// <summary>
        /// Xử lý sự kiện click nút Đăng ký
        /// </summary>
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string fullName = txtFullName.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Kiểm tra username đã tồn tại chưa
            if (IsUsernameExists(username))
            {
                lblMessage.Text = "⚠️ Tên đăng nhập đã tồn tại!";
                lblMessage.CssClass = "alert alert-danger";
                lblMessage.Visible = true;
                return;
            }

            // Thêm user mới vào database
            if (CreateUser(username, password, fullName, email))
            {
                // Tạo các danh mục mặc định cho user mới
                int userId = GetUserIdByUsername(username);
                if (userId > 0)
                {
                    CreateDefaultCategories(userId);
                }

                lblMessage.Text = "✅ Đăng ký thành công! Đang chuyển hướng...";
                lblMessage.CssClass = "alert alert-success";
                lblMessage.Visible = true;

                // Redirect về trang đăng nhập sau 2 giây
                Response.AddHeader("REFRESH", "2;URL=Login.aspx");
            }
            else
            {
                lblMessage.Text = "⚠️ Có lỗi xảy ra, vui lòng thử lại!";
                lblMessage.CssClass = "alert alert-danger";
                lblMessage.Visible = true;
            }
        }

        /// <summary>
        /// Kiểm tra username đã tồn tại trong database chưa
        /// </summary>
        private bool IsUsernameExists(string username)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    conn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Tạo user mới trong database
        /// </summary>
        private bool CreateUser(string username, string password, string fullName, string email)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Users (Username, Password, FullName, Email) 
                              VALUES (@Username, @Password, @FullName, @Email)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@Email", email);

                    try
                    {
                        conn.Open();
                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Lấy UserID theo username
        /// </summary>
        private int GetUserIdByUsername(string username)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT UserID FROM Users WHERE Username = @Username";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
            }
        }

        /// <summary>
        /// Tạo các danh mục mặc định cho user mới
        /// </summary>
        private void CreateDefaultCategories(int userId)
        {
            string[] categories = { "Ăn uống", "Đi lại", "Học tập", "Giải trí", "Mua sắm", "Hóa đơn" };

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                foreach (string category in categories)
                {
                    string sql = "INSERT INTO Categories (CategoryName, UserID) VALUES (@CategoryName, @UserID)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryName", category);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
