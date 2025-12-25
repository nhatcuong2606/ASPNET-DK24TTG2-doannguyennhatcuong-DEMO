<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebApplication.Login" %>

<!DOCTYPE html>
<html lang="vi">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Đăng nhập - Quản Lý Chi Tiêu</title>
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
                    <div class="login-logo"><i class="fa-solid fa-wallet"></i></div>
                    <h1 class="login-title">Quản Lý Chi Tiêu</h1>
                    <p class="login-subtitle">Đăng nhập để quản lý tài chính cá nhân</p>
                </div>

                <!-- Thông báo lỗi -->
                <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-danger" Visible="false"></asp:Label>

                <!-- Form đăng nhập -->
                <div class="form-group">
                    <label class="form-label"><i class="fa-solid fa-user"></i> Tên đăng nhập</label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" 
                        placeholder="Nhập tên đăng nhập"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvUsername" runat="server" 
                        ControlToValidate="txtUsername" ErrorMessage="Vui lòng nhập tên đăng nhập"
                        CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
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
                    <asp:CheckBox ID="chkRemember" runat="server" Text=" Ghi nhớ đăng nhập" />
                </div>

                <asp:Button ID="btnLogin" runat="server" Text="Đăng nhập" 
                    CssClass="btn btn-primary" OnClick="btnLogin_Click" 
                    Style="width: 100%; margin-top: 10px;" />

                <div class="text-center mt-2">
                    <p class="text-muted">Chưa có tài khoản? 
                        <asp:HyperLink ID="lnkRegister" runat="server" NavigateUrl="~/Register.aspx">
                            Đăng ký ngay
                        </asp:HyperLink>
                    </p>
                </div>

                <!-- Thông tin demo -->
                <div class="alert alert-info mt-2">
                    <i class="fa-solid fa-circle-info"></i>
                    <div>
                        <strong>Tài khoản demo:</strong><br />
                        Username: admin | Password: 123456
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
