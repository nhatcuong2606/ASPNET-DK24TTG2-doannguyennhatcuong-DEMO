<%@ Page Title="Mục tiêu Tài chính" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Goal.aspx.cs" Inherits="WebApplication.Goal" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <h1 class="page-title">
        <i class="fa-solid fa-bullseye"></i> Mục tiêu Tài chính
    </h1>

    <!-- Thông báo -->
    <asp:Label ID="lblMessage" runat="server" Visible="false"></asp:Label>

    <!-- Form thêm/sửa mục tiêu -->
    <div class="card">
        <div class="card-header">
            <i class="fa-solid fa-circle-plus"></i> 
            <asp:Label ID="lblFormTitle" runat="server" Text="Thêm mục tiêu mới"></asp:Label>
        </div>
        
        <div style="padding: 20px;">
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Tên mục tiêu <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtGoalName" runat="server" CssClass="form-control" 
                        placeholder="VD: Mua laptop, Du lịch..."></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvGoalName" runat="server" 
                        ControlToValidate="txtGoalName" ErrorMessage="Vui lòng nhập tên mục tiêu"
                        CssClass="text-danger" Display="Dynamic" ValidationGroup="GoalForm"></asp:RequiredFieldValidator>
                </div>
                <div class="form-group">
                    <label class="form-label">Số tiền mục tiêu (VNĐ) <span class="text-danger">*</span></label>
                    <asp:TextBox ID="txtTargetAmount" runat="server" CssClass="form-control" 
                        placeholder="Nhập số tiền cần đạt" TextMode="Number"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvTarget" runat="server" 
                        ControlToValidate="txtTargetAmount" ErrorMessage="Vui lòng nhập số tiền mục tiêu"
                        CssClass="text-danger" Display="Dynamic" ValidationGroup="GoalForm"></asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Số tiền hiện có (VNĐ)</label>
                    <asp:TextBox ID="txtCurrentAmount" runat="server" CssClass="form-control" 
                        placeholder="Nhập số tiền đã tiết kiệm" TextMode="Number" Text="0"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label class="form-label">Hạn chót</label>
                    <asp:TextBox ID="txtDeadline" runat="server" CssClass="form-control" 
                        TextMode="Date"></asp:TextBox>
                </div>
            </div>

            <!-- Hidden field để lưu GoalID khi sửa -->
            <asp:HiddenField ID="hfGoalID" runat="server" Value="0" />

            <div style="display: flex; gap: 10px;">
                <asp:Button ID="btnSave" runat="server" Text="Lưu mục tiêu" 
                    CssClass="btn btn-primary" OnClick="btnSave_Click" ValidationGroup="GoalForm" />
                <asp:Button ID="btnCancel" runat="server" Text="Hủy" 
                    CssClass="btn btn-outline" OnClick="btnCancel_Click" Visible="false" CausesValidation="false" />
            </div>
        </div>
    </div>

    <!-- Danh sách mục tiêu -->
    <div class="card">
        <div class="card-header">
            <i class="fa-solid fa-list-ul"></i> Danh sách mục tiêu
        </div>
        
        <div style="padding: 20px;">
            <asp:Repeater ID="rptGoals" runat="server" OnItemCommand="rptGoals_ItemCommand">
                <ItemTemplate>
                    <div class="card" style="border-left: 5px solid <%# GetProgressColor(Eval("CurrentAmount"), Eval("TargetAmount")) %>; margin-bottom: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.05);">
                        <div style="padding: 20px;">
                            <div style="display: flex; justify-content: space-between; align-items: flex-start; flex-wrap: wrap; gap: 15px;">
                                <div style="flex: 1; min-width: 200px;">
                                    <h3 style="margin: 0 0 5px 0; font-size: 1.25rem;"><i class="fa-solid fa-star" style="color: #f59e0b;"></i> <%# Eval("GoalName") %></h3>
                                    <p class="text-muted" style="margin: 0;">
                                        <i class="fa-regular fa-clock"></i> Hạn chót: <%# Eval("Deadline") != DBNull.Value ? Convert.ToDateTime(Eval("Deadline")).ToString("dd/MM/yyyy") : "Không đặt" %>
                                    </p>
                                </div>
                                <div style="text-align: right; min-width: 150px;">
                                    <div style="font-size: 1.5rem; font-weight: 700; color: var(--primary-color);">
                                        <%# String.Format("{0:N0}", Eval("CurrentAmount")) %>đ
                                    </div>
                                    <div class="text-muted">
                                        / <%# String.Format("{0:N0}", Eval("TargetAmount")) %>đ
                                    </div>
                                </div>
                            </div>
                            
                            <!-- Progress bar -->
                            <div style="margin: 15px 0;">
                                <div style="display: flex; justify-content: space-between; margin-bottom: 5px;">
                                    <span>Tiến độ</span>
                                    <span><strong><%# GetProgressPercent(Eval("CurrentAmount"), Eval("TargetAmount")) %>%</strong></span>
                                </div>
                                <div class="progress-container">
                                    <div class="progress-bar <%# GetProgressClass(Eval("CurrentAmount"), Eval("TargetAmount")) %>" 
                                         style="width: <%# GetProgressPercent(Eval("CurrentAmount"), Eval("TargetAmount")) %>%;">
                                    </div>
                                </div>
                            </div>

                            <!-- Action buttons -->
                            <div class="action-btns" style="justify-content: flex-end; margin-top: 15px; border-top: 1px solid #f3f4f6; padding-top: 15px;">
                                <asp:LinkButton ID="btnAddMoney" runat="server" CssClass="btn btn-success" Style="font-size: 0.9rem; padding: 6px 12px;"
                                    CommandName="AddMoney" CommandArgument='<%# Eval("GoalID") %>'
                                    CausesValidation="false" title="Thêm tiền">
                                    <i class="fa-solid fa-circle-dollar-to-slot"></i> Thêm tiền
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn btn-outline" Style="font-size: 0.9rem; padding: 6px 12px; color: var(--primary-color); border-color: var(--primary-color);"
                                    CommandName="EditGoal" CommandArgument='<%# Eval("GoalID") %>'
                                    CausesValidation="false" title="Sửa">
                                    <i class="fa-solid fa-pen"></i> Sửa
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn btn-outline" Style="font-size: 0.9rem; padding: 6px 12px; color: var(--danger-color); border-color: var(--danger-color);"
                                    CommandName="DeleteGoal" CommandArgument='<%# Eval("GoalID") %>'
                                    OnClientClick="return confirm('Bạn có chắc chắn muốn xóa mục tiêu này?');"
                                    CausesValidation="false" title="Xóa">
                                    <i class="fa-solid fa-trash"></i> Xóa
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Label ID="lblNoGoals" runat="server" Text="Chưa có mục tiêu nào. Hãy thêm mục tiêu mới!" 
                CssClass="text-muted" Visible="false" Style="display: block; text-align: center; padding: 20px; font-style: italic;"></asp:Label>
        </div>
    </div>

    <!-- Modal thêm tiền -->
    <asp:Panel ID="pnlAddMoney" runat="server" Visible="false">
        <div class="card" style="border: 2px solid var(--success-color);">
            <div class="card-header" style="background-color: #ecfdf5; color: var(--success-color);">
                <i class="fa-solid fa-money-bill-wave"></i> Thêm tiền vào mục tiêu: <asp:Label ID="lblAddMoneyGoal" runat="server"></asp:Label>
            </div>
            <div style="padding: 20px;">
                <div class="form-group">
                    <label class="form-label">Số tiền thêm vào (VNĐ)</label>
                    <asp:TextBox ID="txtAddAmount" runat="server" CssClass="form-control" 
                        placeholder="Nhập số tiền" TextMode="Number"></asp:TextBox>
                </div>
                <asp:HiddenField ID="hfAddMoneyGoalID" runat="server" />
                <div style="display: flex; gap: 10px;">
                    <asp:Button ID="btnConfirmAdd" runat="server" Text="Xác nhận" 
                        CssClass="btn btn-success" OnClick="btnConfirmAdd_Click" />
                    <asp:Button ID="btnCancelAdd" runat="server" Text="Hủy" 
                        CssClass="btn btn-outline" OnClick="btnCancelAdd_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>

</asp:Content>
