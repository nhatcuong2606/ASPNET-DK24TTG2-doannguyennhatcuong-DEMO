<%@ Page Title="Báo cáo Chi tiêu" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="WebApplication.Report" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <h1 class="page-title">
        <i class="fa-solid fa-chart-pie"></i> Báo cáo Chi tiêu
    </h1>

    <!-- Bộ lọc báo cáo -->
    <div class="card">
        <div class="card-header">
            <i class="fa-solid fa-filter"></i> Lọc báo cáo
        </div>
        <div style="padding: 20px;">
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Năm</label>
                    <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
                <div class="form-group">
                    <label class="form-label">Loại báo cáo</label>
                    <asp:DropDownList ID="ddlReportType" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlReportType_SelectedIndexChanged">
                        <asp:ListItem Value="monthly">Theo tháng</asp:ListItem>
                        <asp:ListItem Value="category">Theo danh mục</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
        </div>
    </div>

    <!-- Tổng quan -->
    <div class="dashboard-grid">
        <div class="stat-card primary">
            <div class="stat-icon"><i class="fa-solid fa-scale-balanced"></i></div>
            <div class="stat-value"><asp:Label ID="lblTotalYear" runat="server" Text="0"></asp:Label>đ</div>
            <div class="stat-label">Tổng chi năm <asp:Label ID="lblYear1" runat="server"></asp:Label></div>
        </div>
        <div class="stat-card success">
            <div class="stat-icon"><i class="fa-regular fa-calendar-check"></i></div>
            <div class="stat-value"><asp:Label ID="lblAvgMonth" runat="server" Text="0"></asp:Label>đ</div>
            <div class="stat-label">Trung bình/tháng</div>
        </div>
        <div class="stat-card warning">
            <div class="stat-icon"><i class="fa-solid fa-crown"></i></div>
            <div class="stat-value" style="font-size: 1.25rem;"><asp:Label ID="lblTopCategory" runat="server" Text="-"></asp:Label></div>
            <div class="stat-label">Danh mục chi nhiều nhất</div>
        </div>
    </div>

    <!-- Biểu đồ -->
    <div class="card">
        <div class="card-header">
            <i class="fa-solid fa-chart-column"></i> 
            <asp:Label ID="lblChartTitle" runat="server" Text="Biểu đồ chi tiêu theo tháng"></asp:Label>
        </div>
        <div class="chart-container" style="text-align: center; padding: 20px; overflow-x: auto;">
            <asp:Chart ID="chartExpense" runat="server" Width="800" Height="400">
                <Series>
                    <asp:Series Name="Series1" ChartType="Column" Color="#3b82f6" 
                        IsValueShownAsLabel="true" LabelFormat="{0:N0}">
                    </asp:Series>
                </Series>
                <ChartAreas>
                    <asp:ChartArea Name="ChartArea1">
                        <AxisX Title="Tháng" Interval="1">
                            <MajorGrid Enabled="false" />
                        </AxisX>
                        <AxisY Title="Số tiền (VNĐ)">
                            <MajorGrid LineColor="#e5e7eb" />
                        </AxisY>
                    </asp:ChartArea>
                </ChartAreas>
            </asp:Chart>
        </div>
    </div>

    <!-- Biểu đồ tròn theo danh mục -->
    <asp:Panel ID="pnlPieChart" runat="server">
        <div class="card">
            <div class="card-header">
                <i class="fa-solid fa-chart-pie"></i> Phân bổ chi tiêu theo danh mục
            </div>
            <div class="chart-container" style="text-align: center; padding: 20px; overflow-x: auto;">
                <asp:Chart ID="chartPie" runat="server" Width="600" Height="400">
                    <Series>
                        <asp:Series Name="PieSeries" ChartType="Pie" 
                            IsValueShownAsLabel="true" LabelFormat="{0:N0}đ"
                            Label="#PERCENT{P1}">
                        </asp:Series>
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="ChartArea1">
                        </asp:ChartArea>
                    </ChartAreas>
                    <Legends>
                        <asp:Legend Name="Legend1" Docking="Right" Alignment="Center">
                        </asp:Legend>
                    </Legends>
                </asp:Chart>
            </div>
        </div>
    </asp:Panel>

    <!-- Bảng dữ liệu chi tiết -->
    <div class="card">
        <div class="card-header">
            <i class="fa-solid fa-table"></i> Chi tiết chi tiêu
        </div>
        <div class="grid-container">
            <asp:GridView ID="gvReport" runat="server" AutoGenerateColumns="False" 
                CssClass="data-grid" EmptyDataText="Không có dữ liệu">
                <Columns>
                    <asp:BoundField DataField="Label" HeaderText="Mục" />
                    <asp:BoundField DataField="Amount" HeaderText="Số tiền" DataFormatString="{0:N0}đ" 
                        ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                    <asp:BoundField DataField="Percent" HeaderText="Tỷ lệ" DataFormatString="{0:N1}%" 
                        ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
                </Columns>
            </asp:GridView>
        </div>
        <div class="mt-2" style="text-align: right; padding: 10px 20px;">
            <strong style="font-size: 1.1rem; color: var(--primary-color);">Tổng cộng: <asp:Label ID="lblGrandTotal" runat="server" Text="0"></asp:Label>đ</strong>
        </div>
    </div>

</asp:Content>
