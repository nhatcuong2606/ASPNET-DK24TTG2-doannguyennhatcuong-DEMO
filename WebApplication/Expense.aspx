<%@ Page Title="Quản lý Chi tiêu" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Expense.aspx.cs" Inherits="WebApplication.Expense" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <h1 class="page-title">
        <i class="fa-solid fa-money-bill-transfer"></i> Quản lý Chi tiêu
    </h1>

    <!-- Thông báo -->
    <asp:Label ID="lblMessage" runat="server" Visible="false"></asp:Label>

    <!-- Form thêm/sửa chi tiêu -->
    <div class="card">
        <div class="card-header">
            <i class="fa-solid fa-circle-plus"></i> 
            <asp:Label ID="lblFormTitle" runat="server" Text="Thêm chi tiêu mới"></asp:Label>
        </div>
        
        <div style="padding: 20px;">
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Số tiền (VNĐ) <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" 
                        placeholder="Nhập số tiền" TextMode="Number"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvAmount" runat="server" 
                        ControlToValidate="txtAmount" ErrorMessage="Vui lòng nhập số tiền"
                        CssClass="text-danger" Display="Dynamic" ValidationGroup="ExpenseForm"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label class="form-label">Danh mục <span class="text-danger">*</span></label>
                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvCategory" runat="server" 
                        ControlToValidate="ddlCategory" InitialValue="" ErrorMessage="Vui lòng chọn danh mục"
                        CssClass="text-danger" Display="Dynamic" ValidationGroup="ExpenseForm"></asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Ngày chi <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtExpenseDate" runat="server" CssClass="form-control" 
                        TextMode="Date"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDate" runat="server" 
                        ControlToValidate="txtExpenseDate" ErrorMessage="Vui lòng chọn ngày"
                        CssClass="text-danger" Display="Dynamic" ValidationGroup="ExpenseForm"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label class="form-label">Ghi chú</label>
                    <asp:TextBox ID="txtNote" runat="server" CssClass="form-control" 
                        placeholder="Nhập ghi chú (tùy chọn)"></asp:TextBox>
                </div>
            </div>

            <!-- Hidden field để lưu ExpenseID khi sửa -->
            <asp:HiddenField ID="hfExpenseID" runat="server" Value="0" />

            <div style="display: flex; gap: 10px;">
                <asp:Button ID="btnSave" runat="server" Text="Lưu chi tiêu" 
                    CssClass="btn btn-primary" OnClick="btnSave_Click" ValidationGroup="ExpenseForm" />
                <asp:Button ID="btnCancel" runat="server" Text="Hủy" 
                    CssClass="btn btn-outline" OnClick="btnCancel_Click" Visible="false" CausesValidation="false" />
            </div>
        </div>
    </div>

    <!-- Bộ lọc -->
    <div class="card">
        <div class="card-header">
            <i class="fa-solid fa-filter"></i> Lọc chi tiêu
        </div>
        <div style="padding: 20px;">
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Tháng</label>
                    <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlMonth_SelectedIndexChanged">
                        <asp:ListItem Value="0">-- Tất cả --</asp:ListItem>
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
                    <label class="form-label">Năm</label>
                    <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
                <div class="form-group">
                    <label class="form-label">Danh mục</label>
                    <asp:DropDownList ID="ddlFilterCategory" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlFilterCategory_SelectedIndexChanged">
                        <asp:ListItem Value="0">-- Tất cả --</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
        </div>
    </div>

    <!-- Danh sách chi tiêu -->
    <div class="card">
        <div class="card-header">
            <i class="fa-solid fa-list-check"></i> Danh sách chi tiêu
            <span style="margin-left: auto; font-size: 1rem; color: var(--success-color);">
                Tổng: <strong><asp:Label ID="lblTotalAmount" runat="server" Text="0"></asp:Label>đ</strong>
            </span>
        </div>
        <div class="grid-container">
            <asp:GridView ID="gvExpenses" runat="server" AutoGenerateColumns="False" 
                CssClass="data-grid" DataKeyNames="ExpenseID"
                OnRowCommand="gvExpenses_RowCommand"
                EmptyDataText="Chưa có chi tiêu nào. Hãy thêm chi tiêu mới!">
                <Columns>
                    <asp:BoundField DataField="ExpenseDate" HeaderText="Ngày" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="CategoryName" HeaderText="Danh mục" />
                    <asp:BoundField DataField="Amount" HeaderText="Số tiền" DataFormatString="{0:N0}đ" 
                        ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="Note" HeaderText="Ghi chú" />
                    <asp:TemplateField HeaderText="Thao tác">
                        <ItemTemplate>
                            <div class="action-btns">
                                <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn-edit"
                                    CommandName="EditExpense" CommandArgument='<%# Eval("ExpenseID") %>'
                                    CausesValidation="false" title="Sửa">
                                    <i class="fa-solid fa-pen"></i> Sửa
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn-delete"
                                    CommandName="DeleteExpense" CommandArgument='<%# Eval("ExpenseID") %>'
                                    OnClientClick="return confirm('Bạn có chắc chắn muốn xóa chi tiêu này?');"
                                    CausesValidation="false" title="Xóa">
                                    <i class="fa-solid fa-trash"></i> Xóa
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

</asp:Content>
