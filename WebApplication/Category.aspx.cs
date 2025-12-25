using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace WebApplication
{
    /// <summary>
    /// Trang quản lý danh mục chi tiêu - Category.aspx
    /// Cho phép thêm, sửa, xóa danh mục chi tiêu
    /// </summary>
    public partial class Category : System.Web.UI.Page
    {
        // Lấy connection string từ Web.config
        private string connectionString = ConfigurationManager.ConnectionStrings["ExpenseDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Kiểm tra đăng nhập
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadCategories();
            }
        }

        /// <summary>
        /// Tải danh sách danh mục từ database
        /// </summary>
        private void LoadCategories()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Lấy danh mục kèm số lượng chi tiêu
                string sql = @"SELECT c.CategoryID, c.CategoryName, c.Description,
                              (SELECT COUNT(*) FROM Expenses WHERE CategoryID = c.CategoryID) AS ExpenseCount
                              FROM Categories c 
                              WHERE c.UserID = @UserID 
                              ORDER BY c.CategoryName";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    gvCategories.DataSource = dt;
                    gvCategories.DataBind();
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện click nút Lưu
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            string categoryName = txtCategoryName.Text.Trim();
            string description = txtDescription.Text.Trim();
            int categoryId = Convert.ToInt32(hfCategoryID.Value);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                if (categoryId == 0)
                {
                    // Thêm mới
                    string sql = @"INSERT INTO Categories (CategoryName, Description, UserID) 
                                  VALUES (@CategoryName, @Description, @UserID)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.ExecuteNonQuery();
                    }
                    ShowMessage("✅ Thêm danh mục thành công!", "success");
                }
                else
                {
                    // Cập nhật
                    string sql = @"UPDATE Categories SET CategoryName = @CategoryName, Description = @Description 
                                  WHERE CategoryID = @CategoryID AND UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryName", categoryName);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.ExecuteNonQuery();
                    }
                    ShowMessage("✅ Cập nhật danh mục thành công!", "success");
                }
            }

            // Reset form
            ResetForm();
            LoadCategories();
        }

        /// <summary>
        /// Xử lý các lệnh từ GridView (Edit, Delete)
        /// </summary>
        protected void gvCategories_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int categoryId = Convert.ToInt32(e.CommandArgument);
            int userId = Convert.ToInt32(Session["UserID"]);

            if (e.CommandName == "EditCategory")
            {
                // Load thông tin danh mục để sửa
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = "SELECT CategoryName, Description FROM Categories WHERE CategoryID = @CategoryID AND UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        cmd.Parameters.AddWithValue("@UserID", userId);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtCategoryName.Text = reader["CategoryName"].ToString();
                                txtDescription.Text = reader["Description"].ToString();
                                hfCategoryID.Value = categoryId.ToString();

                                lblFormTitle.Text = "Sửa danh mục";
                                btnSave.Text = "💾 Cập nhật";
                                btnCancel.Visible = true;
                            }
                        }
                    }
                }
            }
            else if (e.CommandName == "DeleteCategory")
            {
                // Kiểm tra xem danh mục có chi tiêu không
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlCheck = "SELECT COUNT(*) FROM Expenses WHERE CategoryID = @CategoryID";
                    using (SqlCommand cmd = new SqlCommand(sqlCheck, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            ShowMessage("⚠️ Không thể xóa danh mục này vì đã có " + count + " chi tiêu!", "danger");
                            return;
                        }
                    }

                    // Xóa danh mục
                    string sqlDelete = "DELETE FROM Categories WHERE CategoryID = @CategoryID AND UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(sqlDelete, conn))
                    {
                        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.ExecuteNonQuery();
                    }
                }

                ShowMessage("✅ Xóa danh mục thành công!", "success");
                LoadCategories();
            }
        }

        /// <summary>
        /// Xử lý sự kiện click nút Hủy
        /// </summary>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        /// <summary>
        /// Reset form về trạng thái ban đầu
        /// </summary>
        private void ResetForm()
        {
            txtCategoryName.Text = "";
            txtDescription.Text = "";
            hfCategoryID.Value = "0";
            lblFormTitle.Text = "Thêm danh mục mới";
            btnSave.Text = "💾 Lưu danh mục";
            btnCancel.Visible = false;
        }

        /// <summary>
        /// Hiển thị thông báo
        /// </summary>
        private void ShowMessage(string message, string type)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "alert alert-" + type;
            lblMessage.Visible = true;
        }
    }
}
