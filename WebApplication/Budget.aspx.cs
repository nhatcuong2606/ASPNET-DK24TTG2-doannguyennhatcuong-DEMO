using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace WebApplication
{
    /// <summary>
    /// Trang quản lý ngân sách - Budget.aspx
    /// Cho phép thiết lập hạn mức chi tiêu theo tháng
    /// </summary>
    public partial class Budget : System.Web.UI.Page
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
                // Load danh sách năm
                LoadYears();

                // Đặt tháng và năm hiện tại
                ddlMonth.SelectedValue = DateTime.Now.Month.ToString();
                ddlYear.SelectedValue = DateTime.Now.Year.ToString();

                // Load dữ liệu
                LoadCurrentMonthStats();
                LoadBudgets();
                CheckBudgetWarning();
            }
        }

        /// <summary>
        /// Load danh sách năm
        /// </summary>
        private void LoadYears()
        {
            ddlYear.Items.Clear();
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear - 2; year <= currentYear + 2; year++)
            {
                ddlYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
            }
            ddlYear.SelectedValue = currentYear.ToString();
        }

        /// <summary>
        /// Load thống kê tháng hiện tại
        /// </summary>
        private void LoadCurrentMonthStats()
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            lblCurrentMonth.Text = currentMonth + "/" + currentYear;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Lấy ngân sách tháng hiện tại
                decimal budget = 0;
                string sqlBudget = @"SELECT LimitAmount FROM Budgets 
                                    WHERE UserID = @UserID AND Month = @Month AND Year = @Year";
                using (SqlCommand cmd = new SqlCommand(sqlBudget, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Month", currentMonth);
                    cmd.Parameters.AddWithValue("@Year", currentYear);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        budget = Convert.ToDecimal(result);
                }

                // Lấy tổng chi tiêu tháng hiện tại
                decimal spent = 0;
                string sqlSpent = @"SELECT ISNULL(SUM(Amount), 0) FROM Expenses 
                                   WHERE UserID = @UserID AND MONTH(ExpenseDate) = @Month AND YEAR(ExpenseDate) = @Year";
                using (SqlCommand cmd = new SqlCommand(sqlSpent, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Month", currentMonth);
                    cmd.Parameters.AddWithValue("@Year", currentYear);
                    spent = Convert.ToDecimal(cmd.ExecuteScalar());
                }

                // Hiển thị thống kê
                lblBudgetAmount.Text = budget.ToString("N0");
                lblSpentAmount.Text = spent.ToString("N0");
                lblRemainingAmount.Text = (budget - spent).ToString("N0");

                // Tính % và hiển thị progress bar
                if (budget > 0)
                {
                    decimal percent = (spent / budget) * 100;
                    lblPercent.Text = percent.ToString("N1");

                    // Giới hạn width tối đa 100%
                    int progressWidth = (int)Math.Min(percent, 100);
                    pnlProgress.Style["width"] = progressWidth + "%";

                    // Đổi màu theo mức độ
                    if (percent >= 100)
                        pnlProgress.CssClass = "progress-bar danger";
                    else if (percent >= 80)
                        pnlProgress.CssClass = "progress-bar warning";
                    else
                        pnlProgress.CssClass = "progress-bar";
                }
                else
                {
                    lblPercent.Text = "0";
                    pnlProgress.Style["width"] = "0%";
                }
            }
        }

        /// <summary>
        /// Kiểm tra và hiển thị cảnh báo ngân sách
        /// </summary>
        private void CheckBudgetWarning()
        {
            decimal budget = decimal.Parse(lblBudgetAmount.Text.Replace(",", ""));
            decimal spent = decimal.Parse(lblSpentAmount.Text.Replace(",", ""));

            if (budget > 0 && spent > budget)
            {
                pnlBudgetWarning.Visible = true;
                lblBudgetWarning.Text = string.Format("Bạn đã VƯỢT ngân sách {0:N0}đ!", spent - budget);
            }
            else
            {
                pnlBudgetWarning.Visible = false;
            }
        }

        /// <summary>
        /// Load danh sách ngân sách đã thiết lập
        /// </summary>
        private void LoadBudgets()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"SELECT b.BudgetID, b.Month, b.Year, b.LimitAmount,
                              ISNULL((SELECT SUM(Amount) FROM Expenses 
                                     WHERE UserID = @UserID AND MONTH(ExpenseDate) = b.Month 
                                     AND YEAR(ExpenseDate) = b.Year), 0) AS SpentAmount
                              FROM Budgets b 
                              WHERE b.UserID = @UserID 
                              ORDER BY b.Year DESC, b.Month DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    gvBudgets.DataSource = dt;
                    gvBudgets.DataBind();
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện click nút Lưu
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            int month = Convert.ToInt32(ddlMonth.SelectedValue);
            int year = Convert.ToInt32(ddlYear.SelectedValue);
            decimal limitAmount = Convert.ToDecimal(txtLimitAmount.Text);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Kiểm tra xem đã có ngân sách cho tháng này chưa
                string sqlCheck = @"SELECT BudgetID FROM Budgets 
                                   WHERE UserID = @UserID AND Month = @Month AND Year = @Year";
                int existingId = 0;
                using (SqlCommand cmd = new SqlCommand(sqlCheck, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Year", year);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        existingId = Convert.ToInt32(result);
                }

                if (existingId > 0)
                {
                    // Cập nhật
                    string sql = @"UPDATE Budgets SET LimitAmount = @LimitAmount 
                                  WHERE BudgetID = @BudgetID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@LimitAmount", limitAmount);
                        cmd.Parameters.AddWithValue("@BudgetID", existingId);
                        cmd.ExecuteNonQuery();
                    }
                    ShowMessage("✅ Cập nhật ngân sách thành công!", "success");
                }
                else
                {
                    // Thêm mới
                    string sql = @"INSERT INTO Budgets (Month, Year, LimitAmount, UserID) 
                                  VALUES (@Month, @Year, @LimitAmount, @UserID)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Month", month);
                        cmd.Parameters.AddWithValue("@Year", year);
                        cmd.Parameters.AddWithValue("@LimitAmount", limitAmount);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.ExecuteNonQuery();
                    }
                    ShowMessage("✅ Thêm ngân sách thành công!", "success");
                }
            }

            // Reset form và reload
            txtLimitAmount.Text = "";
            LoadCurrentMonthStats();
            LoadBudgets();
            CheckBudgetWarning();
        }

        /// <summary>
        /// Xử lý lệnh từ GridView
        /// </summary>
        protected void gvBudgets_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteBudget")
            {
                int budgetId = Convert.ToInt32(e.CommandArgument);
                int userId = Convert.ToInt32(Session["UserID"]);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = "DELETE FROM Budgets WHERE BudgetID = @BudgetID AND UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@BudgetID", budgetId);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                ShowMessage("✅ Xóa ngân sách thành công!", "success");
                LoadCurrentMonthStats();
                LoadBudgets();
            }
        }

        /// <summary>
        /// Lấy trạng thái ngân sách (dùng trong GridView)
        /// </summary>
        protected string GetBudgetStatus(object limit, object spent)
        {
            decimal limitAmount = Convert.ToDecimal(limit);
            decimal spentAmount = Convert.ToDecimal(spent);

            if (limitAmount == 0)
                return "<span class='text-muted'>Chưa có</span>";

            decimal percent = (spentAmount / limitAmount) * 100;

            if (percent >= 100)
                return "<span class='text-danger'>⚠️ Vượt ngân sách</span>";
            else if (percent >= 80)
                return "<span class='text-warning'>⚡ Sắp hết</span>";
            else
                return "<span class='text-success'>✅ Bình thường</span>";
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
