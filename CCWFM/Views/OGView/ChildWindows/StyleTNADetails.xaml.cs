using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.OGViewModels;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class StyleTNADetails
    {
        public int tblStyleTNADetail;

        public StyleTNADetails(int _tblStyleTNADetail)
        {
            InitializeComponent();

            tblStyleTNADetail = _tblStyleTNADetail;

            GetStyleTNADetailFiles(tblStyleTNADetail);
        }
  
        private void GetStyleTNADetailFiles(int tblStyleTNADetail)
        {
            LkpData.LkpDataClient _client = new LkpData.LkpDataClient();

            _client.GetTblStyleTNADetailAttachmentAsync(tblStyleTNADetail);
            _client.GetTblStyleTNADetailAttachmentCompleted += (s, sv) =>
            {
                List<FileAttachDetail> files = new List<FileAttachDetail>();

                foreach (var item in sv.Result)
                {
                    FileAttachDetail f = new FileAttachDetail();
                    f.Iserial = item.Iserial;
                    f.FileName = item.FileName;
                    f.Path = "http://192.168.1.23:222/" + item.FileName;
                    files.Add(f);
                }
                grdFilesDetails.ItemsSource = null;
                grdFilesDetails.ItemsSource = files;
            };
        }

        private void btnDeleteAttachment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedFileItem = grdFilesDetails.SelectedItem as FileAttachDetail;
                LkpData.LkpDataClient _client = new LkpData.LkpDataClient();
                _client.DeleteTblStyleSpecDetailAttachmentRowAsync(selectedFileItem.Iserial);
                _client.DeleteTblStyleSpecDetailAttachmentRowCompleted += (s, sv) => 
                {
                    GetStyleTNADetailFiles(tblStyleTNADetail);
                };

            }
            catch { }
        }

        public class FileAttachDetail
        {
            public int Iserial { get; set; }

            public string FileName { get; set; }

            public string Path { get; set; }
        }
    }
}