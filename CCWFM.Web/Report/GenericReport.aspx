<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GenericReport.aspx.cs"
    Inherits="CCWFM.Web.Report.GenericReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        #ParametersRowReportViewer1
        {
            text-align: left;
        }
    </style>
    <script src="../Js/jquery-1.9.1.js" type="text/javascript"></script>
  <%--   <script type="text/javascript" lang="javascript">

         $(document).ready(function () {
             if ($.browser.mozilla) {
                 try {
                     var ControlName = 'RptDespesas';
                     var innerScript = '<scr' + 'ipt type="text/javascript">document.getElementById("' + ControlName + '_print").Controller = new ReportViewerHoverButton("' + ControlName + '_print", false, "", "", "", "#ECE9D8", "#DDEEF7", "#99BBE2", "1px #ECE9D8 Solid", "1px #336699 Solid", "1px #336699 Solid");</scr' + 'ipt>';
                     var innerTbody = '<tbody><tr><td><input type="image" style="border-width: 0px; padding: 2px; height: 16px; width: 16px;" alt="Print" src="/Reserved.ReportViewerWebControl.axd?OpType=Resource&amp;Version=9.0.30729.1&amp;Name=Microsoft.Reporting.WebForms.Icons.Print.gif" title="Print"></td></tr></tbody>';
                     var innerTable = '<table title="Print" onmouseout="this.Controller.OnNormal();" onmouseover="this.Controller.OnHover();" onclick="PrintFunc(\'' + ControlName + '\'); return false;" id="' + ControlName + '_print" style="border: 1px solid rgb(236, 233, 216); background-color: rgb(236, 233, 216); cursor: default;">' + innerScript + innerTbody + '</table>'
                     var outerScript = '<scr' + 'ipt type="text/javascript">document.getElementById("' + ControlName + '_print").Controller.OnNormal();</scr' + 'ipt>';
                     var outerDiv = '<div style="display: inline; font-size: 8pt; height: 30px;" class=" "><table cellspacing="0" cellpadding="0" style="display: inline;"><tbody><tr><td height="28px">' + innerTable + outerScript + '</td></tr></tbody></table></div>';

                     $("#" + ControlName + " > div > div").append(outerDiv);

                 }
                 catch (e) { alert(e); }
             }
         });

         function PrintFunc(ControlName) {
             setTimeout('ReportFrame' + ControlName + '.print();', 100);
         }

         // Print Report function

         function PrintReport() {



             //get the ReportViewer Id

             var rv1 = $('#MyReportViewer_ctl09');

             var iDoc = rv1.parents('html');



             // Reading the report styles

             var styles = iDoc.find("head style[id$='ReportControl_styles']").html();

             if ((styles == undefined) || (styles == '')) {

                 iDoc.find('head script').each(function () {

                     var cnt = $(this).html();

                     var p1 = cnt.indexOf('ReportStyles":"');

                     if (p1 > 0) {

                         p1 += 15;

                         var p2 = cnt.indexOf('"', p1);

                         styles = cnt.substr(p1, p2 - p1);

                     }

                 });

             }

             if (styles == '') { alert("Cannot generate styles, Displaying without styles.."); }

             styles = '<style type="text/css">' + styles + "</style>";



             // Reading the report html

             var table = rv1.find("div[id$='_oReportDiv']");

             if (table == undefined) {

                 alert("Report source not found.");

                 return;

             }



             // Generating a copy of the report in a new window

             var docType = '<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/loose.dtd">';

             var docCnt = styles + table.parent().html();

             var docHead = '<head><style>body{margin:5;padding:0;}</style></head>';

             var winAttr = "location=yes, statusbar=no, directories=no, menubar=no, titlebar=no, toolbar=no, dependent=no, width=720, height=600, resizable=yes, screenX=200, screenY=200, personalbar=no, scrollbars=yes"; ;

             var newWin = window.open("", "_blank", winAttr);

             writeDoc = newWin.document;

             writeDoc.open();

             writeDoc.write(docType + '<html>' + docHead + '<body onload="window.print();">' + docCnt + '</body></html>');

             writeDoc.close();

             newWin.focus();

             // uncomment to autoclose the preview window when printing is confirmed or canceled.

             // newWin.close();

         };

     </script>
    <script>
        $(document).ready(function () {
            // Check if the current browser is IE (MSIE is not used since IE 11)
            var isIE = /MSIE/i.test(navigator.userAgent) || /rv:11.0/i.test(navigator.userAgent);

            function printReport() {

                var reportViewerName = 'ReportViewer'; //Name attribute of report viewer control.
                var src_url = $find(reportViewerName)._getInternalViewer().ExportUrlBase + 'PDF';

                var contentDisposition = 'AlwaysInline'; //Content Disposition instructs the server to either return the PDF being requested as an attachment or a viewable report.
                var src_new = src_url.replace(/(ContentDisposition=).*?(&)/, '$1' + contentDisposition + '$2');

                var iframe = $('<iframe>', {
                    src: src_new,
                    id: 'pdfDocument',
                    frameborder: 0,
                    scrolling: 'no'
                }).hide().load(function () {
                    var PDF = document.getElementById('pdfDocument');
                    PDF.focus();
                    try {
                        PDF.contentWindow.print();
                    }
                    catch (ex) {
                        //If all else fails, we want to inform the user that it is impossible to directly print the document with the current browser.
                        //Instead, let's give them the option to export the pdf so that they can print it themselves with a 3rd party PDF reader application.

                        if (confirm("ActiveX and PDF Native Print support is not supported in your browser. The system is unable to print your document directly. Would you like to download the PDF version instead? You may print the document by opening the PDF using your PDF reader application.")) {
                            window.open($find(reportViewerName)._getInternalViewer().ExportUrlBase + 'PDF');
                        }
                    }

                })

                //Bind the iframe we created to an invisible div.
                $('.pdf').html(iframe);


            }

            // 2. Add Print button for non-IE browsers
            if (!isIE) {

                $('#PrintButton').click(function (e) {
                    e.preventDefault();
                    debugger;
                    printReport();
                })
            }

        });

    </script>--%>


</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="360000">
        </asp:ScriptManager>
      <%--  <div class="pdf">
    </div>
        input id="PrintButton" title="Print" style="width: 16px; height: 16px;" type="image" alt="Print" runat="server" src="~/Reserved.ReportViewerWebControl.axd?OpType=Resource&amp;Version=11.0.3442.2&amp;Name=Microsoft.Reporting.WebForms.Icons.Print.gif" /><--%>

        <div style="text-align: center;">
            <rsweb:ReportViewer ClientIDMode="Static" ID="ReportViewer1" runat="server" Font-Names="Verdana"
                Font-Size="8pt" InteractiveDeviceInfos="(Collection)" ProcessingMode="Remote"
                WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" BorderWidth="0px"
                ShowFindControls="False" Height="100%" Width="100%" ShowBackButton="True" BackColor="#FFFBF7"
                ZoomMode="FullPage" ShowPromptAreaButton="False">
            </rsweb:ReportViewer>
        </div>
    </div>
    </form>
</body>
</html>