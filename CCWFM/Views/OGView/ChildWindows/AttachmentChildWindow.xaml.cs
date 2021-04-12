using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using CCWFM.ViewModel.OGViewModels;
using System.Windows.Input;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class AttachmentChildWindow
    {
        private readonly AttachmentModel _viewModel;

        public AttachmentChildWindow(string tableName,int masterId)
        {
            InitializeComponent();

            _viewModel = new AttachmentModel(tableName, masterId);
            DataContext = _viewModel;
        }

        private void btnAddGalaryAttachment_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Multiselect = true
            };
            if (dlg.ShowDialog().GetValueOrDefault(false))
            {
                foreach (var attachment in dlg.Files)
                {
                    _viewModel.MainRowList.Add(new AttachmentViewModel
                    {
                        AttachmentPerRow = attachment,
                        OrginalFileName = attachment.Name,
                        TableName = _viewModel.TableName,
                        RecordId = _viewModel.MasterId,
                    });
                }
            }
        }

        private void btnSaveGalary_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveAttachments();
        }

        private void BtnDownloadButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var btn = sender as Button;
            Storyboard.SetTarget(Storyboard1, btn);
            Storyboard1.Begin();
        }

        private void BtnDownloadButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Storyboard1.Stop();
        }

        private SaveFileDialog _dialog;
        private bool? _dialogResult;

        private void BtnDownloadButton_Click(object sender, RoutedEventArgs e)
        {
            _dialog = new SaveFileDialog
            {
                Filter = "All Files|*.*",
                DefaultFileName = _viewModel.SelectedMainRow.OrginalFileName
            };
            _dialogResult = _dialog.ShowDialog();
            var btn = sender as Button;
            var styleAttachment = btn.DataContext as AttachmentViewModel;

            var wClient = new WebClient();
            var uri = Application.Current.Host.Source.AbsoluteUri;
            var index = uri.IndexOf("/ClientBin");
            uri = uri.Substring(0, index);
            uri = uri +  styleAttachment.Path;
            wClient.OpenReadCompleted += wClient_OpenReadCompleted;
            wClient.DownloadProgressChanged += wClient_DownloadProgressChanged;

            wClient.OpenReadAsync(new Uri(uri), UriKind.Relative);
        }

        private void wClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _viewModel.SelectedMainRow.Percentage = e.ProgressPercentage;

            _viewModel.SelectedMainRow.FileMessage = e.ProgressPercentage + "% done " + e.BytesReceived + " bytes received and " + e.TotalBytesToReceive + " bytes still needed.";
        }

        private void wClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                var streams = e.Result;
                var ms = new MemoryStream();
                streams.CopyTo(ms);
                var bytes = ms.ToArray();

                if (_dialogResult != true) return;

                using (var stream = _dialog.OpenFile())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                }
            }
        }

        private void DataGrid_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                foreach (var item in datagrid.SelectedItems)
                {
                    var temp = item as AttachmentViewModel;
                    if (temp != null)
                        _viewModel.SelectedMainRows.Add(temp);
                }
                _viewModel.DeleteMainRow();
            }
        }
    }
}