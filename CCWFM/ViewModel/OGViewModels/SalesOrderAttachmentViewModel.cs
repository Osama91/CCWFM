using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using CCWFM.CRUDManagerService;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
    public class TblSalesOrderAttachmentViewModel : ViewModelBase
    {
        private readonly String _baseUri;

        public TblSalesOrderAttachmentViewModel()
        {
            _synContext = UISynchronizationContext.Instance;

            var fullUri = Application.Current.Host.Source;
            _baseUri = fullUri.AbsoluteUri.Substring(0, fullUri.AbsoluteUri.IndexOf("/ClientBin", StringComparison.Ordinal));
        }

        private DateTime _creationDateField;

        private string _attachmentDescriptionField;

        private int _IserialField;

        private DateTime? _lastUpdateddateField;

        private int _tblSalesOrderField;

        public DateTime CreationDate
        {
            get
            {
                return _creationDateField;
            }
            set
            {
                if ((_creationDateField.Equals(value) != true))
                {
                    _creationDateField = value;
                    RaisePropertyChanged("CreationDate");
                }
            }
        }

        public string AttachmentDescription
        {
            get
            {
                return _attachmentDescriptionField;
            }
            set
            {
                if ((ReferenceEquals(_attachmentDescriptionField, value) != true))
                {
                    _attachmentDescriptionField = value;
                    RaisePropertyChanged("AttachmentDescription");
                }
            }
        }

        public int Iserial
        {
            get
            {
                return _IserialField;
            }
            set
            {
                if ((_IserialField.Equals(value) != true))
                {
                    _IserialField = value;
                    RaisePropertyChanged("Iserial");
                }
            }
        }

        public DateTime? LastUpdateddate
        {
            get
            {
                return _lastUpdateddateField;
            }
            set
            {
                if ((_lastUpdateddateField.Equals(value) != true))
                {
                    _lastUpdateddateField = value;
                    RaisePropertyChanged("LastUpdateddate");
                }
            }
        }

        private string _attachmentPathField;

        public string AttachmentPath
        {
            get
            {
                return _attachmentPathField;
            }
            set
            {
                if ((ReferenceEquals(_attachmentPathField, value) != true))
                {
                    _attachmentPathField = value;
                    RaisePropertyChanged("Attachment");
                }
            }
        }

        public int TblSalesOrder
        {
            get
            {
                return _tblSalesOrderField;
            }
            set
            {
                if ((_tblSalesOrderField.Equals(value) != true))
                {
                    _tblSalesOrderField = value;
                    RaisePropertyChanged("TblSalesOrder");
                }
            }
        }

        private FileInfo _attachmentPerRow;

        public FileInfo AttachmentPerRow
        {
            get { return _attachmentPerRow; }
            set { _attachmentPerRow = value; RaisePropertyChanged("AttachmentPerRow"); }
        }

        private string _orginalFilEname;

        public string OrginalFileName
        {
            get
            {
                return _orginalFilEname;
            }
            set
            {
                if ((ReferenceEquals(_orginalFilEname, value) != true))
                {
                    _orginalFilEname = value;
                    RaisePropertyChanged("OrginalFileName");
                }
            }
        }

        //exposed properties which are bound in XAML

        public bool IsBusy { get; set; }

        private double _percentage;

        /// <summary>
        /// Gets the percentage.
        /// </summary>
        /// <value>The percentage.</value>
        /// [ReadOnly(true)]
        public double Percentage
        {
            get { return _percentage; }
            set
            {
                _percentage = value;
                RaisePropertyChanged("Percentage");
            }
        }

        private string _folderPath;

        public string FolderPath
        {
            get { return _folderPath; }
            set { _folderPath = value; RaisePropertyChanged("FolderPath"); }
        }

        private Stream _fileStream;
        private FileInfo _uploadedFile;
        private readonly ISynchronizationContext _synContext;
        private string _fileMessage;

        [ReadOnly(true)]
        public string FileMessage
        {
            get { return _fileMessage; }
            set
            {
                _fileMessage = value;
                RaisePropertyChanged("FileMessage");
            }
        }

        //internally used fields

        private string _fileSize; //to show while uploading
        private const int ChunkSize = 4194304;
        private long _dataLength;
        private long _dataSent;

        public void UploadFile(FileInfo fileToUpload)
        {
            _fileSize = GetFileSize(fileToUpload.Length);
            StartUpload(fileToUpload);
        }

        private void StartUpload(FileInfo file)
        {
            _uploadedFile = file;
            _fileStream = _uploadedFile.OpenRead();
            _dataLength = _fileStream.Length;
            var pathofAttachment = AttachmentPath.Split('/').LastOrDefault();
            var dataToSend = _dataLength - _dataSent;
            var isLastChunk = dataToSend <= ChunkSize;
            var isFirstChunk = _dataSent == 0;
            var httpHandlerUrlBuilder = new UriBuilder(string.Format("{0}/FileUpload.ashx", _baseUri));
            httpHandlerUrlBuilder.Query = string.Format("{4}file={0}&offset={1}&last={2}&first={3}&Folder={5}",
                pathofAttachment, _dataSent, isLastChunk, isFirstChunk,
                string.IsNullOrEmpty(httpHandlerUrlBuilder.Query) ? "" : httpHandlerUrlBuilder.Query.Remove(0, 1) + "&", FolderPath);

            var webRequest = (HttpWebRequest)WebRequest.Create(httpHandlerUrlBuilder.Uri);
            webRequest.Method = "POST";
            webRequest.BeginGetRequestStream(WriteToStreamCallback, webRequest);
        }

        private void WriteToStreamCallback(IAsyncResult asynchronousResult)
        {
            var webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            var requestStream = webRequest.EndGetRequestStream(asynchronousResult);

            var buffer = new Byte[4096];
            int bytesRead;
            var tempTotal = 0;

            //Set the start position
            _fileStream.Position = _dataSent;

            //Read the next chunk
            while ((bytesRead = _fileStream.Read(buffer, 0, buffer.Length)) != 0 && tempTotal + bytesRead < ChunkSize)
            {
                requestStream.Write(buffer, 0, bytesRead);
                requestStream.Flush();

                _dataSent += bytesRead;
                tempTotal += bytesRead;

                ////Show the progress change
                UpdateShowProgress(false);
            }

            requestStream.Close();

            //Get the response from the HttpHandler
            webRequest.BeginGetResponse(ReadHttpResponseCallback, webRequest);
        }

        private void ReadHttpResponseCallback(IAsyncResult asynchronousResult)
        {
            var webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            var webResponse = (HttpWebResponse)webRequest.EndGetResponse(asynchronousResult);
            var reader = new StreamReader(webResponse.GetResponseStream());

            reader.ReadToEnd();
            reader.Close();

            if (_dataSent < _dataLength)
            {
                //continue uploading the rest of the file in chunks
                StartUpload(_uploadedFile);

                //Show the progress change
                UpdateShowProgress(false);
            }
            else
            {
                _fileStream.Close();
                _fileStream.Dispose();

                //Show the progress change
                UpdateShowProgress(true);
            }
        }

        private void UpdateShowProgress(bool complete)
        {
            if (complete)
            {
                _synContext.InvokeSynchronously(delegate
                {
                    FileMessage = "complete";
                });

                //RESET ALL VALUES TO THEY CAN UPLOAD AGAIN

                _dataSent = 0;
                _dataLength = 0;
                _fileStream = null;
                _fileSize = "";
                IsBusy = false;
            }
            else
            {
                _synContext.InvokeSynchronously(delegate
                {
                    Percentage = Math.Round(_dataSent / (double)_dataLength * 100);
                    FileMessage = "Total file size: " + _fileSize + " Uploading: " +
                    string.Format("{0:###.00}%", _dataSent / (double)_dataLength * 100);

                    IsBusy = true;
                });
            }
        }

        private string GetFileSize(long length)
        {
            var bytes = (double)length;

            var fileSize = "0 KB";

            if (bytes >= 1073741824)
                fileSize = String.Format("{0:##.##}", bytes / 1073741824) + " GB";
            else if (bytes >= 1048576)
                fileSize = String.Format("{0:##.##}", bytes / 1048576) + " MB";
            else if (bytes >= 1024)
                fileSize = String.Format("{0:##.##}", bytes / 1024) + " KB";
            else if (bytes > 0 && bytes < 1024)
                fileSize = "1 KB";

            return fileSize;
        }
    }

    public class SalesOrderAttachmentViewModel : ViewModelBase
    {
        private TblSalesOrderViewModel _salesOrder;

        public TblSalesOrderViewModel SalesOrder
        {
            get { return _salesOrder; }
            set
            {
                _salesOrder = value;
                RaisePropertyChanged("SalesOrder");
            }
        }

        public SalesOrderAttachmentViewModel(TblSalesOrderViewModel salesOrder)
        {
            SalesOrder = salesOrder;
            MainRowList = new SortableCollectionView<TblSalesOrderAttachmentViewModel>();
            SelectedMainRow = new TblSalesOrderAttachmentViewModel();

            Client.GetTblSalesOrderAttachmentCompleted += (s, sv) =>
            {
                MainRowList.Clear();
                foreach (var row in sv.Result)
                {
                    var newrow = new TblSalesOrderAttachmentViewModel();
                    newrow.InjectFrom(row);
                    MainRowList.Add(newrow);
                }
                Loading = false;
            };

            Client.MaxIserialOfSalesOrderForAttachmentCompleted += (s, sv) =>
            {
                FolderPath = "Uploads" + "/" + sv.attachmentPath;
                var folderName = FolderPath + "/" + SalesOrder.SalesOrderCode;

                var counter = 0;
                foreach (var item in MainRowList)
                {
                    if (item.Iserial == 0)
                    {
                        var maxIserial = sv.Result;

                        item.AttachmentPath = folderName + "/" + SalesOrder.SalesOrderCode + maxIserial + counter + item.AttachmentPerRow.Extension;
                        item.FolderPath = folderName;
                        item.UploadFile(item.AttachmentPerRow);

                        //using (FileStream fs = File.Create("binary.dat", 2048, FileOptions.None))
                        //{
                        //    BinaryFormatter formatter = new BinaryFormatter();
                        //    string s = "this is a test";
                        //    formatter.Serialize(fs, Encoding.Unicode.GetBytes(s));
                        //}

                        //using (var stream = File.Create(item.AttachmentPathThumbnail))
                        //{
                        //    stream.Write(buffer, 0, buffer.Length);
                        //    stream.Flush();
                        //    stream.Close();

                        //}
                        //var temp = new FileInfo(item.AttachmentPathThumbnail);
                        //item.UploadFile(temp);

                        counter++;
                    }
                }

                var isvalid = false;

                foreach (var tblSalesOrderAttachmentViewModel in MainRowList)
                {
                    isvalid = Validator.TryValidateObject(tblSalesOrderAttachmentViewModel, new ValidationContext(tblSalesOrderAttachmentViewModel, null, null), null, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }

                if (isvalid)
                {
                    var savelist = new ObservableCollection<TblSalesOrderAttachment>();
                    GenericMapper.InjectFromObCollection(savelist, MainRowList);

                    Client.UpdateOrInsertTblSalesOrderAttachmentAsync(savelist);
                }
            };
            Client.GetTblSalesOrderAttachmentAsync(SalesOrder.Iserial);
            Client.UpdateOrInsertTblSalesOrderAttachmentCompleted += (s, x) => Client.GetTblSalesOrderAttachmentAsync(SalesOrder.Iserial);

            Client.DeleteTblSalesOrderAttachmentCompleted += (s, ev) =>
            {
                if (ev.Error != null)
                {
                    throw ev.Error;
                }

                var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
                if (oldrow != null) MainRowList.Remove(oldrow);
            };
        }

        public void DeleteMainRow()
        {
            if (SelectedMainRows != null)
            {
                var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
                    MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    foreach (var row in SelectedMainRows)
                    {
                        Client.DeleteTblSalesOrderAttachmentAsync(
                            (TblSalesOrderAttachment)new TblSalesOrderAttachment().InjectFrom(row), MainRowList.IndexOf(row));
                    }
                }
            }
        }

        public void AddNewMainRow(bool checkLastRow)
        {
            var currentRowIndex = (MainRowList.IndexOf(SelectedMainRow));
            if (!checkLastRow || currentRowIndex == (MainRowList.Count - 1))
            {
                if (checkLastRow)
                {
                    var valiationCollection = new List<ValidationResult>();

                    var isvalid = Validator.TryValidateObject(SelectedMainRow, new ValidationContext(SelectedMainRow, null, null), valiationCollection, true);

                    if (!isvalid)
                    {
                        return;
                    }
                }

                MainRowList.Insert(currentRowIndex + 1, new TblSalesOrderAttachmentViewModel
                {
                    TblSalesOrder = SalesOrder.Iserial
                });
            }
        }

        #region Props

        private SortableCollectionView<TblSalesOrderAttachmentViewModel> _mainRowList;

        public SortableCollectionView<TblSalesOrderAttachmentViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblSalesOrderAttachmentViewModel> _selectedMainRows;

        public ObservableCollection<TblSalesOrderAttachmentViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblSalesOrderAttachmentViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblSalesOrderAttachmentViewModel _selectedMainRow;

        public TblSalesOrderAttachmentViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged("SelectedMainRow"); }
        }

        private string _folderPath;

        public string FolderPath
        {
            get { return _folderPath; }
            set { _folderPath = value; RaisePropertyChanged("FolderPath"); }
        }

        #endregion Props

        internal void SaveAttachments()
        {
            Client.MaxIserialOfSalesOrderForAttachmentAsync();
        }
    }
}