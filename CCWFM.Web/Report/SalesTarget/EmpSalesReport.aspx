<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="EmpSalesReport.aspx.cs" Inherits="CCWFM.Web.Report.SalesTarget.EmpSalesReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="plMainContent" runat="server">
    <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt"
        InteractiveDeviceInfos="(Collection)" Width="" Height="" WaitMessageFont-Names="Verdana"
        WaitMessageFont-Size="14pt" ZoomMode="PageWidth" ShowBackButton="true" SizeToReportContent="True">
        <LocalReport ReportPath="Report\SalesTarget\EmpSales.rdlc">
            <DataSources>
                <rsweb:ReportDataSource DataSourceId="SqlDataSource1" Name="EmpSalesDataSource" />
            </DataSources>
        </LocalReport>
    </rsweb:ReportViewer>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ccnewConnectionString %>"
        SelectCommand="SELECT * FROM [EmpSalesOsama]"></asp:SqlDataSource>
</asp:Content>