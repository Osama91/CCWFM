<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IssueFabricReport.aspx.cs" Inherits="CCWFM.Web.Report.IssueFabricReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <br />
       <br />
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt"
        InteractiveDeviceInfos="(Collection)" ProcessingMode="Remote" WaitMessageFont-Names="Verdana"
        WaitMessageFont-Size="14pt" Width="" Height="">
        <ServerReport ReportServerUrl="http://apptest/ReportServer/" ReportPath="/Report/TransferToVendorWarehouseView" />
     
    </rsweb:ReportViewer>
      
    </div>
    </form>
</body>
</html>
