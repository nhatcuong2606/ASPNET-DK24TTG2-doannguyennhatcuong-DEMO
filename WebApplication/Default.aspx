<%@ Page Title="Trang chủ" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication.Default" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <h1 class="page-title">
        <i class="fa-solid fa-gauge-high"></i> Tổng quan Chi tiêu
    </h1>

    <!-- Cảnh báo ngân sách -->
    <asp:Panel ID="pnlBudgetWarning" runat="server" Visible="false">
        <div class="alert alert-danger">
            <i class="fa-solid fa-triangle-exclamation"></i>
            <div>
                <strong>Cảnh báo!</strong> 
                <asp:Label ID="lblBudgetWarning" runat="server"></asp:Label>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlBudgetInfo" runat="server" Visible="false">
        <div class="alert alert-warning">
            <i class="fa-solid fa-lightbulb"></i>
            <div>
                <asp:Label ID="lblBudgetInfo" runat="server"></asp:Label>
            </div>
        </div>
    </asp:Panel>

    <!-- Dashboard Cards -->
    <div class="dashboard-grid">
        <!-- Card: Tổng chi tiêu tháng này -->
        <div class="stat-card primary">
            <div class="stat-icon"><i class="fa-solid fa-hand-holding-dollar"></i></div>
            <div class="stat-value">
                <asp:Label ID="lblTotalExpense" runat="server" Text="0"></asp:Label>đ
            </div>
            <div class="stat-label">Chi tiêu tháng này</div>
        </div>

        <!-- Card: Ngân sách còn lại -->
        <div class="stat-card success">
            <div class="stat-icon"><i class="fa-solid fa-wallet"></i></div>
            <div class="stat-value">
                <asp:Label ID="lblBudgetRemaining" runat="server" Text="0"></asp:Label>đ
            </div>
            <div class="stat-label">Ngân sách còn lại</div>
        </div>

        <!-- Card: Số danh mục -->
        <div class="stat-card warning">
            <div class="stat-icon"><i class="fa-solid fa-layer-group"></i></div>
            <div class="stat-value">
                <asp:Label ID="lblCategoryCount" runat="server" Text="0"></asp:Label>
            </div>
            <div class="stat-label">Danh mục chi tiêu</div>
        </div>

        <!-- Card: Mục tiêu tiết kiệm -->
        <div class="stat-card danger">
            <div class="stat-icon"><i class="fa-solid fa-bullseye"></i></div>
            <div class="stat-value">
                <asp:Label ID="lblGoalCount" runat="server" Text="0"></asp:Label>
            </div>
            <div class="stat-label">Mục tiêu đang theo dõi</div>
        </div>
    </div>

    <!-- Chi tiêu gần đây -->
    <div class="card">
        <div class="card-header">
            <i class="fa-solid fa-clock-rotate-left"></i> Chi tiêu gần đây
        </div>
        <div class="grid-container">
            <asp:GridView ID="gvRecentExpenses" runat="server" AutoGenerateColumns="False" 
                CssClass="data-grid" EmptyDataText="Chưa có chi tiêu nào">
                <Columns>
                    <asp:BoundField DataField="ExpenseDate" HeaderText="Ngày" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="CategoryName" HeaderText="Danh mục" />
                    <asp:BoundField DataField="Amount" HeaderText="Số tiền" DataFormatString="{0:N0}đ" />
                    <asp:BoundField DataField="Note" HeaderText="Ghi chú" />
                </Columns>
            </asp:GridView>
        </div>
        <div class="mt-2" style="padding: 10px 20px;">
            <asp:HyperLink ID="lnkViewAll" runat="server" NavigateUrl="~/Expense.aspx" CssClass="btn btn-outline">
                Xem tất cả chi tiêu
            </asp:HyperLink>
        </div>
    </div>

    <!-- Quick Actions -->
    <div class="card">
        <div class="card-header">
            <i class="fa-solid fa-bolt"></i> Thao tác nhanh
        </div>
        <div style="display: flex; gap: 15px; flex-wrap: wrap; padding: 20px;">
            <asp:HyperLink ID="lnkAddExpense" runat="server" NavigateUrl="~/Expense.aspx" CssClass="btn btn-primary">
                <i class="fa-solid fa-plus"></i> Thêm chi tiêu
            </asp:HyperLink>
            <asp:HyperLink ID="lnkSetBudget" runat="server" NavigateUrl="~/Budget.aspx" CssClass="btn btn-success">
                <i class="fa-solid fa-sliders"></i> Thiết lập ngân sách
            </asp:HyperLink>
            <asp:HyperLink ID="lnkViewReport" runat="server" NavigateUrl="~/Report.aspx" CssClass="btn btn-outline">
                <i class="fa-solid fa-chart-line"></i> Xem báo cáo
            </asp:HyperLink>
        </div>
    </div>

</asp:Content>
