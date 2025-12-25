using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WebApplication
{
    /// <summary>
    /// Trang chủ - Dashboard
    /// Hiển thị tổng quan chi tiêu và cảnh báo ngân sách
    /// </summary>
    public partial class Default : System.Web.UI.Page
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
                LoadDashboardData();
                CheckBudgetWarning();
                LoadRecentExpenses();
            }
        }

        /// <summary>
        /// Tải dữ liệu tổng quan cho dashboard
        /// </summary>
        private void LoadDashboardData()
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 1. Tổng chi tiêu tháng này
                string sqlTotal = @"SELECT ISNULL(SUM(Amount), 0) FROM Expenses 
                                   WHERE UserID = @UserID 
                                   AND MONTH(ExpenseDate) = @Month 
                                   AND YEAR(ExpenseDate) = @Year";
                using (SqlCommand cmd = new SqlCommand(sqlTotal, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Month", currentMonth);
                    cmd.Parameters.AddWithValue("@Year", currentYear);
                    decimal totalExpense = Convert.ToDecimal(cmd.ExecuteScalar());
                    lblTotalExpense.Text = totalExpense.ToString("N0");
                }

                // 2. Ngân sách tháng này
                string sqlBudget = @"SELECT ISNULL(LimitAmount, 0) FROM Budgets 
                                    WHERE UserID = @UserID 
                                    AND Month = @Month 
                                    AND Year = @Year";
                using (SqlCommand cmd = new SqlCommand(sqlBudget, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Month", currentMonth);
                    cmd.Parameters.AddWithValue("@Year", currentYear);
                    object result = cmd.ExecuteScalar();
                    decimal budget = result != null ? Convert.ToDecimal(result) : 0;
                    decimal totalExpense = decimal.Parse(lblTotalExpense.Text.Replace(",", ""));
                    decimal remaining = budget - totalExpense;
                    lblBudgetRemaining.Text = remaining.ToString("N0");
                }

                // 3. Số danh mục
                string sqlCategory = "SELECT COUNT(*) FROM Categories WHERE UserID = @UserID";
                using (SqlCommand cmd = new SqlCommand(sqlCategory, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    lblCategoryCount.Text = cmd.ExecuteScalar().ToString();
                }

                // 4. Số mục tiêu
                string sqlGoal = "SELECT COUNT(*) FROM Goals WHERE UserID = @UserID";
                using (SqlCommand cmd = new SqlCommand(sqlGoal, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    lblGoalCount.Text = cmd.ExecuteScalar().ToString();
                }
            }
        }

        /// <summary>
        /// Kiểm tra và hiển thị cảnh báo ngân sách
        /// </summary>
        private void CheckBudgetWarning()
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Lấy ngân sách tháng này
                string sqlBudget = @"SELECT LimitAmount FROM Budgets 
                                    WHERE UserID = @UserID 
                                    AND Month = @Month 
                                    AND Year = @Year";
                decimal budget = 0;
                using (SqlCommand cmd = new SqlCommand(sqlBudget, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Month", currentMonth);
                    cmd.Parameters.AddWithValue("@Year", currentYear);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        budget = Convert.ToDecimal(result);
                    }
                }

                // Nếu chưa thiết lập ngân sách
                if (budget == 0)
                {
                    pnlBudgetInfo.Visible = true;
                    lblBudgetInfo.Text = "Bạn chưa thiết lập ngân sách cho tháng này. <a href='Budget.aspx'>Thiết lập ngay</a>";
                    return;
                }

                // Lấy tổng chi tiêu
                string sqlTotal = @"SELECT ISNULL(SUM(Amount), 0) FROM Expenses 
                                   WHERE UserID = @UserID 
                                   AND MONTH(ExpenseDate) = @Month 
                                   AND YEAR(ExpenseDate) = @Year";
                decimal totalExpense = 0;
                using (SqlCommand cmd = new SqlCommand(sqlTotal, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Month", currentMonth);
                    cmd.Parameters.AddWithValue("@Year", currentYear);
                    totalExpense = Convert.ToDecimal(cmd.ExecuteScalar());
                }

                // Tính % chi tiêu
                decimal percent = (totalExpense / budget) * 100;

                // Hiển thị cảnh báo
                if (totalExpense > budget)
                {
                    // Đã vượt ngân sách
                    pnlBudgetWarning.Visible = true;
                    lblBudgetWarning.Text = string.Format("Bạn đã VƯỢT ngân sách {0:N0}đ! (Chi tiêu: {1:N0}đ / Ngân sách: {2:N0}đ)",
                        totalExpense - budget, totalExpense, budget);
                }
                else if (percent >= 80)
                {
                    // Sắp vượt ngân sách (>= 80%)
                    pnlBudgetInfo.Visible = true;
                    lblBudgetInfo.Text = string.Format("Bạn đã chi {0:N1}% ngân sách tháng này. Còn lại: {1:N0}đ",
                        percent, budget - totalExpense);
                }
            }
        }

        /// <summary>
        /// Tải danh sách chi tiêu gần đây
        /// </summary>
        private void LoadRecentExpenses()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"SELECT TOP 5 e.ExpenseDate, c.CategoryName, e.Amount, e.Note 
                              FROM Expenses e 
                              INNER JOIN Categories c ON e.CategoryID = c.CategoryID 
                              WHERE e.UserID = @UserID 
                              ORDER BY e.ExpenseDate DESC, e.ExpenseID DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    gvRecentExpenses.DataSource = dt;
                    gvRecentExpenses.DataBind();
                }
            }
        }
    }
}
