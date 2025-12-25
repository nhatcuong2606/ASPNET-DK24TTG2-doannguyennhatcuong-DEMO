using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace WebApplication
{
    /// <summary>
    /// Trang quản lý chi tiêu - Expense.aspx
    /// Cho phép thêm, sửa, xóa các khoản chi tiêu
    /// </summary>
    public partial class Expense : System.Web.UI.Page
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
                // Đặt ngày mặc định là hôm nay
                txtExpenseDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                // Load danh sách năm
                LoadYears();

                // Load danh mục
                LoadCategories();

                // Load danh mục cho bộ lọc
                LoadFilterCategories();

                // Đặt tháng và năm hiện tại
                ddlMonth.SelectedValue = DateTime.Now.Month.ToString();
                ddlYear.SelectedValue = DateTime.Now.Year.ToString();

                // Load danh sách chi tiêu
                LoadExpenses();
            }
        }

        /// <summary>
        /// Load danh sách năm cho bộ lọc
        /// </summary>
        private void LoadYears()
        {
            ddlYear.Items.Clear();
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear - 5; year <= currentYear + 1; year++)
            {
                ddlYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
            }
            ddlYear.SelectedValue = currentYear.ToString();
        }

        /// <summary>
        /// Load danh sách danh mục cho form nhập
        /// </summary>
        private void LoadCategories()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT CategoryID, CategoryName FROM Categories WHERE UserID = @UserID ORDER BY CategoryName";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    conn.Open();
                    ddlCategory.DataSource = cmd.ExecuteReader();
                    ddlCategory.DataTextField = "CategoryName";
                    ddlCategory.DataValueField = "CategoryID";
                    ddlCategory.DataBind();
                }
            }

            ddlCategory.Items.Insert(0, new ListItem("-- Chọn danh mục --", ""));
        }

        /// <summary>
        /// Load danh sách danh mục cho bộ lọc
        /// </summary>
        private void LoadFilterCategories()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT CategoryID, CategoryName FROM Categories WHERE UserID = @UserID ORDER BY CategoryName";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlFilterCategory.Items.Clear();
                    ddlFilterCategory.Items.Add(new ListItem("-- Tất cả --", "0"));

                    while (reader.Read())
                    {
                        ddlFilterCategory.Items.Add(new ListItem(
                            reader["CategoryName"].ToString(),
                            reader["CategoryID"].ToString()
                        ));
                    }
                }
            }
        }

        /// <summary>
        /// Load danh sách chi tiêu theo bộ lọc
        /// </summary>
        private void LoadExpenses()
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            int month = Convert.ToInt32(ddlMonth.SelectedValue);
            int year = Convert.ToInt32(ddlYear.SelectedValue);
            int categoryId = Convert.ToInt32(ddlFilterCategory.SelectedValue);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"SELECT e.ExpenseID, e.ExpenseDate, c.CategoryName, e.Amount, e.Note 
                              FROM Expenses e 
                              INNER JOIN Categories c ON e.CategoryID = c.CategoryID 
                              WHERE e.UserID = @UserID";

                // Thêm điều kiện lọc
                if (month > 0)
                    sql += " AND MONTH(e.ExpenseDate) = @Month";
                if (year > 0)
                    sql += " AND YEAR(e.ExpenseDate) = @Year";
                if (categoryId > 0)
                    sql += " AND e.CategoryID = @CategoryID";

                sql += " ORDER BY e.ExpenseDate DESC, e.ExpenseID DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);
                    cmd.Parameters.AddWithValue("@CategoryID", categoryId);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    gvExpenses.DataSource = dt;
                    gvExpenses.DataBind();

                    // Tính tổng
                    decimal total = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        total += Convert.ToDecimal(row["Amount"]);
                    }
                    lblTotalAmount.Text = total.ToString("N0");
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện click nút Lưu
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            decimal amount = Convert.ToDecimal(txtAmount.Text);
            int categoryId = Convert.ToInt32(ddlCategory.SelectedValue);
            DateTime expenseDate = DateTime.Parse(txtExpenseDate.Text);
            string note = txtNote.Text.Trim();
            int expenseId = Convert.ToInt32(hfExpenseID.Value);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                if (expenseId == 0)
                {
                    // Thêm mới
                    string sql = @"INSERT INTO Expenses (Amount, ExpenseDate, CategoryID, Note, UserID) 
                                  VALUES (@Amount, @ExpenseDate, @CategoryID, @Note, @UserID)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@ExpenseDate", expenseDate);
                        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        cmd.Parameters.AddWithValue("@Note", note);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.ExecuteNonQuery();
                    }
                    ShowMessage("✅ Thêm chi tiêu thành công!", "success");
                }
                else
                {
                    // Cập nhật
                    string sql = @"UPDATE Expenses SET Amount = @Amount, ExpenseDate = @ExpenseDate, 
                                  CategoryID = @CategoryID, Note = @Note 
                                  WHERE ExpenseID = @ExpenseID AND UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Amount", amount);
                        cmd.Parameters.AddWithValue("@ExpenseDate", expenseDate);
                        cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                        cmd.Parameters.AddWithValue("@Note", note);
                        cmd.Parameters.AddWithValue("@ExpenseID", expenseId);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.ExecuteNonQuery();
                    }
                    ShowMessage("✅ Cập nhật chi tiêu thành công!", "success");
                }
            }

            // Reset form
            ResetForm();
            LoadExpenses();
        }

        /// <summary>
        /// Xử lý các lệnh từ GridView (Edit, Delete)
        /// </summary>
        protected void gvExpenses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int expenseId = Convert.ToInt32(e.CommandArgument);
            int userId = Convert.ToInt32(Session["UserID"]);

            if (e.CommandName == "EditExpense")
            {
                // Load thông tin chi tiêu để sửa
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"SELECT Amount, ExpenseDate, CategoryID, Note 
                                  FROM Expenses WHERE ExpenseID = @ExpenseID AND UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ExpenseID", expenseId);
                        cmd.Parameters.AddWithValue("@UserID", userId);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtAmount.Text = reader["Amount"].ToString();
                                txtExpenseDate.Text = Convert.ToDateTime(reader["ExpenseDate"]).ToString("yyyy-MM-dd");
                                ddlCategory.SelectedValue = reader["CategoryID"].ToString();
                                txtNote.Text = reader["Note"].ToString();
                                hfExpenseID.Value = expenseId.ToString();

                                lblFormTitle.Text = "Sửa chi tiêu";
                                btnSave.Text = "💾 Cập nhật";
                                btnCancel.Visible = true;
                            }
                        }
                    }
                }
            }
            else if (e.CommandName == "DeleteExpense")
            {
                // Xóa chi tiêu
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = "DELETE FROM Expenses WHERE ExpenseID = @ExpenseID AND UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ExpenseID", expenseId);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                ShowMessage("✅ Xóa chi tiêu thành công!", "success");
                LoadExpenses();
            }
        }

        /// <summary>
        /// Xử lý sự kiện thay đổi tháng lọc
        /// </summary>
        protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadExpenses();
        }

        /// <summary>
        /// Xử lý sự kiện thay đổi năm lọc
        /// </summary>
        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadExpenses();
        }

        /// <summary>
        /// Xử lý sự kiện thay đổi danh mục lọc
        /// </summary>
        protected void ddlFilterCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadExpenses();
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
            txtAmount.Text = "";
            txtExpenseDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            ddlCategory.SelectedIndex = 0;
            txtNote.Text = "";
            hfExpenseID.Value = "0";
            lblFormTitle.Text = "Thêm chi tiêu mới";
            btnSave.Text = "💾 Lưu chi tiêu";
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
