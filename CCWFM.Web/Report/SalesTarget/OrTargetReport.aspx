<%@ Page Title="Or Sales Target Report" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="OrTargetReport.aspx.cs" Inherits="CCWFM.Web.Report.SalesTarget.OrTargerReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="plMainContent" runat="server">
    <center>
        <asp:Label ID="LbldATE" runat="server" Text="Label">Select Month</asp:Label>
        &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;
        <asp:DropDownList AutoPostBack="true" ID="DdlDate" runat="server" OnSelectedIndexChanged="DdlDate_SelectedIndexChanged">
        </asp:DropDownList>
        <br />
        <br />
        <asp:Label ID="LblYear" runat="server" Text="Label">Select year</asp:Label>
        &nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;
        <asp:DropDownList AutoPostBack="true" ID="DdlYear" runat="server" OnSelectedIndexChanged="DdlYear_SelectedIndexChanged">
        </asp:DropDownList>
        <br />
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt"
            InteractiveDeviceInfos="(Collection)" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt"
            DocumentMapWidth="100%" Width="" Height="" ZoomMode="PageWidth" ShowBackButton="true" SizeToReportContent="True">
            <LocalReport ReportPath="Report\SalesTarget\OrTarget.rdlc">
                <DataSources>
                    <rsweb:ReportDataSource DataSourceId="SqlDataSource1" Name="OrTargetDataSet" />
                </DataSources>
            </LocalReport>
        </rsweb:ReportViewer>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ccnewConnectionString %>"
            SelectCommand="ORSalesTargetnewOsama" SelectCommandType="StoredProcedure">
            <SelectParameters>
                <asp:ControlParameter ControlID="DdlDate" Name="date" PropertyName="SelectedValue"
                    Type="Int32" />
                <asp:ControlParameter ControlID="DdlYear" Name="ydate" PropertyName="SelectedValue"
                    Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        <br />
    </center>
</asp:Content>
