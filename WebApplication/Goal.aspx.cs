using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace WebApplication
{
    /// <summary>
    /// Trang quản lý mục tiêu tài chính - Goal.aspx
    /// Cho phép đặt và theo dõi các mục tiêu tiết kiệm
    /// </summary>
    public partial class Goal : System.Web.UI.Page
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
                LoadGoals();
            }
        }

        /// <summary>
        /// Load danh sách mục tiêu
        /// </summary>
        private void LoadGoals()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"SELECT GoalID, GoalName, TargetAmount, CurrentAmount, Deadline 
                              FROM Goals WHERE UserID = @UserID ORDER BY CreatedDate DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        rptGoals.DataSource = dt;
                        rptGoals.DataBind();
                        lblNoGoals.Visible = false;
                    }
                    else
                    {
                        rptGoals.DataSource = null;
                        rptGoals.DataBind();
                        lblNoGoals.Visible = true;
                    }
                }
            }
        }

        /// <summary>
        /// Tính % tiến độ
        /// </summary>
        protected string GetProgressPercent(object current, object target)
        {
            decimal currentAmount = current != DBNull.Value ? Convert.ToDecimal(current) : 0;
            decimal targetAmount = target != DBNull.Value ? Convert.ToDecimal(target) : 1;

            if (targetAmount == 0) return "0";

            decimal percent = (currentAmount / targetAmount) * 100;
            return Math.Min(percent, 100).ToString("N1");
        }

        /// <summary>
        /// Lấy class CSS cho progress bar
        /// </summary>
        protected string GetProgressClass(object current, object target)
        {
            decimal currentAmount = current != DBNull.Value ? Convert.ToDecimal(current) : 0;
            decimal targetAmount = target != DBNull.Value ? Convert.ToDecimal(target) : 1;

            if (targetAmount == 0) return "";

            decimal percent = (currentAmount / targetAmount) * 100;

            if (percent >= 100) return "success";
            if (percent >= 50) return "warning";
            return "";
        }

        /// <summary>
        /// Lấy màu border cho card
        /// </summary>
        protected string GetProgressColor(object current, object target)
        {
            decimal currentAmount = current != DBNull.Value ? Convert.ToDecimal(current) : 0;
            decimal targetAmount = target != DBNull.Value ? Convert.ToDecimal(target) : 1;

            if (targetAmount == 0) return "#e2e8f0";

            decimal percent = (currentAmount / targetAmount) * 100;

            if (percent >= 100) return "#48bb78";
            if (percent >= 50) return "#ed8936";
            return "#667eea";
        }

        /// <summary>
        /// Xử lý sự kiện click nút Lưu
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            string goalName = txtGoalName.Text.Trim();
            decimal targetAmount = Convert.ToDecimal(txtTargetAmount.Text);
            decimal currentAmount = string.IsNullOrEmpty(txtCurrentAmount.Text) ? 0 : Convert.ToDecimal(txtCurrentAmount.Text);
            DateTime? deadline = string.IsNullOrEmpty(txtDeadline.Text) ? (DateTime?)null : DateTime.Parse(txtDeadline.Text);
            int goalId = Convert.ToInt32(hfGoalID.Value);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                if (goalId == 0)
                {
                    // Thêm mới
                    string sql = @"INSERT INTO Goals (GoalName, TargetAmount, CurrentAmount, Deadline, UserID) 
                                  VALUES (@GoalName, @TargetAmount, @CurrentAmount, @Deadline, @UserID)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@GoalName", goalName);
                        cmd.Parameters.AddWithValue("@TargetAmount", targetAmount);
                        cmd.Parameters.AddWithValue("@CurrentAmount", currentAmount);
                        cmd.Parameters.AddWithValue("@Deadline", deadline.HasValue ? (object)deadline.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.ExecuteNonQuery();
                    }
                    ShowMessage("✅ Thêm mục tiêu thành công!", "success");
                }
                else
                {
                    // Cập nhật
                    string sql = @"UPDATE Goals SET GoalName = @GoalName, TargetAmount = @TargetAmount, 
                                  CurrentAmount = @CurrentAmount, Deadline = @Deadline 
                                  WHERE GoalID = @GoalID AND UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@GoalName", goalName);
                        cmd.Parameters.AddWithValue("@TargetAmount", targetAmount);
                        cmd.Parameters.AddWithValue("@CurrentAmount", currentAmount);
                        cmd.Parameters.AddWithValue("@Deadline", deadline.HasValue ? (object)deadline.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@GoalID", goalId);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.ExecuteNonQuery();
                    }
                    ShowMessage("✅ Cập nhật mục tiêu thành công!", "success");
                }
            }

            ResetForm();
            LoadGoals();
        }

        /// <summary>
        /// Xử lý các lệnh từ Repeater
        /// </summary>
        protected void rptGoals_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int goalId = Convert.ToInt32(e.CommandArgument);
            int userId = Convert.ToInt32(Session["UserID"]);

            if (e.CommandName == "EditGoal")
            {
                // Load thông tin để sửa
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"SELECT GoalName, TargetAmount, CurrentAmount, Deadline 
                                  FROM Goals WHERE GoalID = @GoalID AND UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@GoalID", goalId);
                        cmd.Parameters.AddWithValue("@UserID", userId);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtGoalName.Text = reader["GoalName"].ToString();
                                txtTargetAmount.Text = reader["TargetAmount"].ToString();
                                txtCurrentAmount.Text = reader["CurrentAmount"].ToString();
                                if (reader["Deadline"] != DBNull.Value)
                                    txtDeadline.Text = Convert.ToDateTime(reader["Deadline"]).ToString("yyyy-MM-dd");
                                hfGoalID.Value = goalId.ToString();

                                lblFormTitle.Text = "Sửa mục tiêu";
                                btnSave.Text = "💾 Cập nhật";
                                btnCancel.Visible = true;
                            }
                        }
                    }
                }
            }
            else if (e.CommandName == "DeleteGoal")
            {
                // Xóa mục tiêu
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = "DELETE FROM Goals WHERE GoalID = @GoalID AND UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@GoalID", goalId);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                ShowMessage("✅ Xóa mục tiêu thành công!", "success");
                LoadGoals();
            }
            else if (e.CommandName == "AddMoney")
            {
                // Hiển thị panel thêm tiền
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = "SELECT GoalName FROM Goals WHERE GoalID = @GoalID AND UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@GoalID", goalId);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            lblAddMoneyGoal.Text = result.ToString();
                            hfAddMoneyGoalID.Value = goalId.ToString();
                            pnlAddMoney.Visible = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Xác nhận thêm tiền vào mục tiêu
        /// </summary>
        protected void btnConfirmAdd_Click(object sender, EventArgs e)
        {
            int goalId = Convert.ToInt32(hfAddMoneyGoalID.Value);
            int userId = Convert.ToInt32(Session["UserID"]);
            decimal addAmount = string.IsNullOrEmpty(txtAddAmount.Text) ? 0 : Convert.ToDecimal(txtAddAmount.Text);

            if (addAmount > 0)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"UPDATE Goals SET CurrentAmount = CurrentAmount + @AddAmount 
                                  WHERE GoalID = @GoalID AND UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@AddAmount", addAmount);
                        cmd.Parameters.AddWithValue("@GoalID", goalId);
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                ShowMessage("✅ Đã thêm " + addAmount.ToString("N0") + "đ vào mục tiêu!", "success");
            }

            pnlAddMoney.Visible = false;
            txtAddAmount.Text = "";
            LoadGoals();
        }

        /// <summary>
        /// Hủy thêm tiền
        /// </summary>
        protected void btnCancelAdd_Click(object sender, EventArgs e)
        {
            pnlAddMoney.Visible = false;
            txtAddAmount.Text = "";
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
            txtGoalName.Text = "";
            txtTargetAmount.Text = "";
            txtCurrentAmount.Text = "0";
            txtDeadline.Text = "";
            hfGoalID.Value = "0";
            lblFormTitle.Text = "Thêm mục tiêu mới";
            btnSave.Text = "💾 Lưu mục tiêu";
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
