<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="WebApplication.Register" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Đăng ký - Quản Lý Chi Tiêu</title>
    <!-- FontAwesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
    <link href="Site.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <div class="login-box">
                <!-- Header -->
                <div class="login-header">
                    <div class="login-logo"><i class="fa-solid fa-user-plus"></i></div>
                    <h1 class="login-title">Đăng ký tài khoản</h1>
                    <p class="login-subtitle">Tạo tài khoản để bắt đầu quản lý chi tiêu</p>
                </div>

                <!-- Thông báo -->
                <asp:Label ID="lblMessage" runat="server" Visible="false"></asp:Label>

                <!-- Form đăng ký -->
                <div class="form-group">
                    <label class="form-label"><i class="fa-solid fa-user"></i> Tên đăng nhập</label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" 
                        placeholder="Nhập tên đăng nhập"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvUsername" runat="server" 
                        ControlToValidate="txtUsername" ErrorMessage="Vui lòng nhập tên đăng nhập"
                        CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>

                <div class="form-group">
                    <label class="form-label"><i class="fa-solid fa-envelope"></i> Email</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" 
                        placeholder="Nhập email" TextMode="Email"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
                        ControlToValidate="txtEmail" ErrorMessage="Vui lòng nhập email"
                        CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>

                <div class="form-group">
                    <label class="form-label"><i class="fa-solid fa-id-card"></i> Họ và tên</label>
                    <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" 
                        placeholder="Nhập họ và tên"></asp:TextBox>
                </div>

                <div class="form-group">
                    <label class="form-label"><i class="fa-solid fa-lock"></i> Mật khẩu</label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" 
                        TextMode="Password" placeholder="Nhập mật khẩu"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" 
                        ControlToValidate="txtPassword" ErrorMessage="Vui lòng nhập mật khẩu"
                        CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                </div>

                <div class="form-group">
                    <label class="form-label"><i class="fa-solid fa-check-double"></i> Xác nhận mật khẩu</label>
                    <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" 
                        TextMode="Password" placeholder="Nhập lại mật khẩu"></asp:TextBox>
                    <asp:CompareValidator ID="cvPassword" runat="server" 
                        ControlToValidate="txtConfirmPassword" ControlToCompare="txtPassword"
                        ErrorMessage="Mật khẩu xác nhận không khớp"
                        CssClass="text-danger" Display="Dynamic"></asp:CompareValidator>
                </div>

                <asp:Button ID="btnRegister" runat="server" Text="Đăng ký" 
                    CssClass="btn btn-success" OnClick="btnRegister_Click" 
                    Style="width: 100%; margin-top: 10px;" />

                <div class="text-center mt-2">
                    <p class="text-muted">Đã có tài khoản? 
                        <asp:HyperLink ID="lnkLogin" runat="server" NavigateUrl="~/Login.aspx">
                            Đăng nhập
                        </asp:HyperLink>
                    </p>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
