using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Drawing;

namespace WebApplication
{
    /// <summary>
    /// Trang báo cáo chi tiêu - Report.aspx
    /// Hiển thị thống kê và biểu đồ chi tiêu
    /// </summary>
    public partial class Report : System.Web.UI.Page
    {
        // Lấy connection string từ Web.config
        private string connectionString = ConfigurationManager.ConnectionStrings["ExpenseDB"].ConnectionString;

        // Mảng màu cho biểu đồ tròn
        private Color[] chartColors = {
            Color.FromArgb(102, 126, 234),  // Primary
            Color.FromArgb(72, 187, 120),   // Success
            Color.FromArgb(237, 137, 54),   // Warning
            Color.FromArgb(245, 101, 101),  // Danger
            Color.FromArgb(160, 174, 192),  // Gray
            Color.FromArgb(129, 140, 248),  // Indigo
            Color.FromArgb(251, 191, 36),   // Yellow
            Color.FromArgb(236, 72, 153),   // Pink
            Color.FromArgb(20, 184, 166),   // Teal
            Color.FromArgb(168, 85, 247)    // Purple
        };

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

                // Load báo cáo
                LoadReport();
            }
        }

        /// <summary>
        /// Load danh sách năm
        /// </summary>
        private void LoadYears()
        {
            ddlYear.Items.Clear();
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear - 5; year <= currentYear; year++)
            {
                ddlYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
            }
            ddlYear.SelectedValue = currentYear.ToString();
        }

        /// <summary>
        /// Load báo cáo theo loại đã chọn
        /// </summary>
        private void LoadReport()
        {
            int year = Convert.ToInt32(ddlYear.SelectedValue);
            lblYear1.Text = year.ToString();

            if (ddlReportType.SelectedValue == "monthly")
            {
                LoadMonthlyReport(year);
                lblChartTitle.Text = "Biểu đồ chi tiêu theo tháng năm " + year;
                pnlPieChart.Visible = false;
            }
            else
            {
                LoadCategoryReport(year);
                lblChartTitle.Text = "Biểu đồ chi tiêu theo danh mục năm " + year;
                pnlPieChart.Visible = true;
            }

            LoadSummary(year);
        }

        /// <summary>
        /// Load thống kê tổng quan
        /// </summary>
        private void LoadSummary(int year)
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Tổng chi năm
                string sqlTotal = @"SELECT ISNULL(SUM(Amount), 0) FROM Expenses 
                                   WHERE UserID = @UserID AND YEAR(ExpenseDate) = @Year";
                decimal totalYear = 0;
                using (SqlCommand cmd = new SqlCommand(sqlTotal, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Year", year);
                    totalYear = Convert.ToDecimal(cmd.ExecuteScalar());
                }
                lblTotalYear.Text = totalYear.ToString("N0");

                // Trung bình tháng
                decimal avgMonth = totalYear / 12;
                lblAvgMonth.Text = avgMonth.ToString("N0");

                // Danh mục chi nhiều nhất
                string sqlTop = @"SELECT TOP 1 c.CategoryName 
                                 FROM Expenses e 
                                 INNER JOIN Categories c ON e.CategoryID = c.CategoryID 
                                 WHERE e.UserID = @UserID AND YEAR(e.ExpenseDate) = @Year 
                                 GROUP BY c.CategoryName 
                                 ORDER BY SUM(e.Amount) DESC";
                using (SqlCommand cmd = new SqlCommand(sqlTop, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Year", year);
                    object result = cmd.ExecuteScalar();
                    lblTopCategory.Text = result != null ? result.ToString() : "-";
                }
            }
        }

        /// <summary>
        /// Load báo cáo theo tháng
        /// </summary>
        private void LoadMonthlyReport(int year)
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"SELECT MONTH(ExpenseDate) AS MonthNum, 
                              'Tháng ' + CAST(MONTH(ExpenseDate) AS VARCHAR) AS Label,
                              SUM(Amount) AS Amount
                              FROM Expenses 
                              WHERE UserID = @UserID AND YEAR(ExpenseDate) = @Year 
                              GROUP BY MONTH(ExpenseDate) 
                              ORDER BY MonthNum";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Year", year);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Tính tổng và % cho bảng
                    decimal total = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        total += Convert.ToDecimal(row["Amount"]);
                    }

                    // Thêm cột Percent
                    dt.Columns.Add("Percent", typeof(decimal));
                    foreach (DataRow row in dt.Rows)
                    {
                        decimal amount = Convert.ToDecimal(row["Amount"]);
                        row["Percent"] = total > 0 ? (amount / total) * 100 : 0;
                    }

                    // Bind to GridView
                    gvReport.DataSource = dt;
                    gvReport.DataBind();
                    lblGrandTotal.Text = total.ToString("N0");

                    // Bind to Chart
                    chartExpense.Series["Series1"].Points.Clear();
                    chartExpense.Series["Series1"].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Column;
                    
                    foreach (DataRow row in dt.Rows)
                    {
                        string label = row["Label"].ToString();
                        decimal amount = Convert.ToDecimal(row["Amount"]);
                        chartExpense.Series["Series1"].Points.AddXY(label, amount);
                    }
                }
            }
        }

        /// <summary>
        /// Load báo cáo theo danh mục
        /// </summary>
        private void LoadCategoryReport(int year)
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"SELECT c.CategoryName AS Label, SUM(e.Amount) AS Amount
                              FROM Expenses e 
                              INNER JOIN Categories c ON e.CategoryID = c.CategoryID 
                              WHERE e.UserID = @UserID AND YEAR(e.ExpenseDate) = @Year 
                              GROUP BY c.CategoryName 
                              ORDER BY SUM(e.Amount) DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Year", year);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Tính tổng và % cho bảng
                    decimal total = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        total += Convert.ToDecimal(row["Amount"]);
                    }

                    // Thêm cột Percent
                    dt.Columns.Add("Percent", typeof(decimal));
                    foreach (DataRow row in dt.Rows)
                    {
                        decimal amount = Convert.ToDecimal(row["Amount"]);
                        row["Percent"] = total > 0 ? (amount / total) * 100 : 0;
                    }

                    // Bind to GridView
                    gvReport.DataSource = dt;
                    gvReport.DataBind();
                    lblGrandTotal.Text = total.ToString("N0");

                    // Bind to Column Chart
                    chartExpense.Series["Series1"].Points.Clear();
                    chartExpense.Series["Series1"].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Bar;
                    
                    int colorIndex = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        string label = row["Label"].ToString();
                        decimal amount = Convert.ToDecimal(row["Amount"]);
                        var point = chartExpense.Series["Series1"].Points.AddXY(label, amount);
                        chartExpense.Series["Series1"].Points[point].Color = chartColors[colorIndex % chartColors.Length];
                        colorIndex++;
                    }

                    // Bind to Pie Chart
                    chartPie.Series["PieSeries"].Points.Clear();
                    colorIndex = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        string label = row["Label"].ToString();
                        decimal amount = Convert.ToDecimal(row["Amount"]);
                        var point = chartPie.Series["PieSeries"].Points.AddXY(label, amount);
                        chartPie.Series["PieSeries"].Points[point].Color = chartColors[colorIndex % chartColors.Length];
                        chartPie.Series["PieSeries"].Points[point].LegendText = label;
                        colorIndex++;
                    }
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện thay đổi năm
        /// </summary>
        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadReport();
        }

        /// <summary>
        /// Xử lý sự kiện thay đổi loại báo cáo
        /// </summary>
        protected void ddlReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadReport();
        }
    }
}
