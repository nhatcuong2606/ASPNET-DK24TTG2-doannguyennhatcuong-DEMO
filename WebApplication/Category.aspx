<%@ Page Title="Quản lý Danh mục" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Category.aspx.cs" Inherits="WebApplication.Category" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <h1 class="page-title">
        <i class="fa-solid fa-folder-tree"></i> Quản lý Danh mục Chi tiêu
    </h1>

    <!-- Thông báo -->
    <asp:Label ID="lblMessage" runat="server" Visible="false"></asp:Label>

    <!-- Form thêm/sửa danh mục -->
    <div class="card">
        <div class="card-header">
            <i class="fa-solid fa-pen-to-square"></i> 
            <asp:Label ID="lblFormTitle" runat="server" Text="Thêm danh mục mới"></asp:Label>
        </div>
        
        <div style="padding: 20px;">
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Tên danh mục <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtCategoryName" runat="server" CssClass="form-control" 
                        placeholder="Nhập tên danh mục"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvCategoryName" runat="server" 
                        ControlToValidate="txtCategoryName" ErrorMessage="Vui lòng nhập tên danh mục"
                        CssClass="text-danger" Display="Dynamic" ValidationGroup="CategoryForm"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label class="form-label">Mô tả</label>
                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" 
                        placeholder="Nhập mô tả (tùy chọn)"></asp:TextBox>
                </div>
            </div>

            <!-- Hidden field để lưu CategoryID khi sửa -->
            <asp:HiddenField ID="hfCategoryID" runat="server" Value="0" />

            <div style="display: flex; gap: 10px;">
                <asp:Button ID="btnSave" runat="server" Text="Lưu danh mục" 
                    CssClass="btn btn-primary" OnClick="btnSave_Click" ValidationGroup="CategoryForm" />
                <asp:Button ID="btnCancel" runat="server" Text="Hủy" 
                    CssClass="btn btn-outline" OnClick="btnCancel_Click" Visible="false" CausesValidation="false" />
            </div>
        </div>
    </div>

    <!-- Danh sách danh mục -->
    <div class="card">
        <div class="card-header">
            <i class="fa-solid fa-list"></i> Danh sách danh mục
        </div>
        <div class="grid-container">
            <asp:GridView ID="gvCategories" runat="server" AutoGenerateColumns="False" 
                CssClass="data-grid" DataKeyNames="CategoryID"
                OnRowCommand="gvCategories_RowCommand"
                EmptyDataText="Chưa có danh mục nào. Hãy thêm danh mục mới!">
                <Columns>
                    <asp:BoundField DataField="CategoryName" HeaderText="Tên danh mục" />
                    <asp:BoundField DataField="Description" HeaderText="Mô tả" />
                    <asp:TemplateField HeaderText="Số chi tiêu">
                        <ItemTemplate>
                            <span class="badge"><%# Eval("ExpenseCount") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Thao tác">
                        <ItemTemplate>
                            <div class="action-btns">
                                <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn-edit"
                                    CommandName="EditCategory" CommandArgument='<%# Eval("CategoryID") %>'
                                    CausesValidation="false" title="Sửa">
                                    <i class="fa-solid fa-pen"></i> Sửa
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn-delete"
                                    CommandName="DeleteCategory" CommandArgument='<%# Eval("CategoryID") %>'
                                    OnClientClick="return confirm('Bạn có chắc chắn muốn xóa danh mục này?');"
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
