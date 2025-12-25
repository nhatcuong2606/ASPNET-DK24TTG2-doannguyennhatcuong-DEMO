<%@ Page Title="Quản lý Ngân sách" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Budget.aspx.cs" Inherits="WebApplication.Budget" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <h1 class="page-title">
        <i class="fa-solid fa-piggy-bank"></i> Quản lý Ngân sách
    </h1>

    <!-- Thông báo -->
    <asp:Label ID="lblMessage" runat="server" Visible="false"></asp:Label>

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

    <!-- Form thiết lập ngân sách -->
    <div class="card">
        <div class="card-header">
            <i class="fa-solid fa-calendar-check"></i> Thiết lập ngân sách tháng
        </div>
        
        <div style="padding: 20px;">
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Tháng <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control">
                        <asp:ListItem Value="1">Tháng 1</asp:ListItem>
                        <asp:ListItem Value="2">Tháng 2</asp:ListItem>
                        <asp:ListItem Value="3">Tháng 3</asp:ListItem>
                        <asp:ListItem Value="4">Tháng 4</asp:ListItem>
                        <asp:ListItem Value="5">Tháng 5</asp:ListItem>
                        <asp:ListItem Value="6">Tháng 6</asp:ListItem>
                        <asp:ListItem Value="7">Tháng 7</asp:ListItem>
                        <asp:ListItem Value="8">Tháng 8</asp:ListItem>
                        <asp:ListItem Value="9">Tháng 9</asp:ListItem>
                        <asp:ListItem Value="10">Tháng 10</asp:ListItem>
                        <asp:ListItem Value="11">Tháng 11</asp:ListItem>
                        <asp:ListItem Value="12">Tháng 12</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="form-group">
                    <label class="form-label">Năm <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control">
                    </asp:DropDownList>
                </div>
                <div class="form-group">
                    <label class="form-label">Hạn mức (VNĐ) <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtLimitAmount" runat="server" CssClass="form-control" 
                        placeholder="Nhập hạn mức chi tiêu" TextMode="Number"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvLimit" runat="server" 
                        ControlToValidate="txtLimitAmount" ErrorMessage="Vui lòng nhập hạn mức"
                        CssClass="text-danger" Display="Dynamic" ValidationGroup="BudgetForm"></asp:RequiredFieldValidator>
                </div>
            </div>

            <asp:Button ID="btnSave" runat="server" Text="Lưu ngân sách" 
                CssClass="btn btn-primary" OnClick="btnSave_Click" ValidationGroup="BudgetForm" />
        </div>
    </div>

    <!-- Thống kê tháng hiện tại -->
    <div class="card">
        <div class="card-header">
            <i class="fa-solid fa-chart-simple"></i> Thống kê tháng <asp:Label ID="lblCurrentMonth" runat="server"></asp:Label>
        </div>
        
        <div style="padding: 20px;">
            <div class="dashboard-grid">
                <div class="stat-card primary">
                    <div class="stat-icon"><i class="fa-solid fa-coins"></i></div>
                    <div class="stat-value"><asp:Label ID="lblBudgetAmount" runat="server" Text="0"></asp:Label>đ</div>
                    <div class="stat-label">Ngân sách đã đặt</div>
                </div>
                <div class="stat-card warning">
                    <div class="stat-icon"><i class="fa-solid fa-file-invoice-dollar"></i></div>
                    <div class="stat-value"><asp:Label ID="lblSpentAmount" runat="server" Text="0"></asp:Label>đ</div>
                    <div class="stat-label">Đã chi tiêu</div>
                </div>
                <div class="stat-card success">
                    <div class="stat-icon"><i class="fa-solid fa-vault"></i></div>
                    <div class="stat-value"><asp:Label ID="lblRemainingAmount" runat="server" Text="0"></asp:Label>đ</div>
                    <div class="stat-label">Còn lại</div>
                </div>
            </div>

            <!-- Progress bar -->
            <div style="margin-top: 20px;">
                <label class="form-label">Tiến độ chi tiêu: <asp:Label ID="lblPercent" runat="server" Text="0"></asp:Label>%</label>
                <div class="progress-container">
                    <asp:Panel ID="pnlProgress" runat="server" CssClass="progress-bar" Style="width: 0%;">
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>

    <!-- Danh sách ngân sách đã đặt -->
    <div class="card">
        <div class="card-header">
            <i class="fa-solid fa-clock-rotate-left"></i> Lịch sử ngân sách
        </div>
        <div class="grid-container">
            <asp:GridView ID="gvBudgets" runat="server" AutoGenerateColumns="False" 
                CssClass="data-grid" DataKeyNames="BudgetID"
                OnRowCommand="gvBudgets_RowCommand"
                EmptyDataText="Chưa có ngân sách nào được thiết lập.">
                <Columns>
                    <asp:TemplateField HeaderText="Tháng/Năm">
                        <ItemTemplate>
                            Tháng <%# Eval("Month") %>/<%# Eval("Year") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="LimitAmount" HeaderText="Hạn mức" DataFormatString="{0:N0}đ" 
                        ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                    <asp:TemplateField HeaderText="Đã chi">
                        <ItemTemplate>
                            <%# String.Format("{0:N0}đ", Eval("SpentAmount")) %>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Right" />
                        <HeaderStyle HorizontalAlign="Right" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Trạng thái">
                        <ItemTemplate>
                            <%# GetBudgetStatus(Eval("LimitAmount"), Eval("SpentAmount")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Thao tác">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn-delete"
                                CommandName="DeleteBudget" CommandArgument='<%# Eval("BudgetID") %>'
                                OnClientClick="return confirm('Bạn có chắc chắn muốn xóa ngân sách này?');"
                                CausesValidation="false" title="Xóa">
                                <i class="fa-solid fa-trash"></i> Xóa
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Content>
