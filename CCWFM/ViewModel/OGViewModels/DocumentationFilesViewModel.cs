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
    public class TblDocumentationFilesViewModel : ViewModelBase
    {
        private readonly String _baseUri;

        public TblDocumentationFilesViewModel()
        {
            _synContext = UISynchronizationContext.Instance;

            var fullUri = Application.Current.Host.Source;
            _baseUri = fullUri.AbsoluteUri.Substring(0, fullUri.AbsoluteUri.IndexOf("/ClientBin", StringComparison.Ordinal));
        }

        private DateTime _creationDateField;

        private string _attachmentDescriptionField;

        private int _iserialField;

        private DateTime? _lastUpdateddateField;

        private int _tblStyleField;

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
                return _iserialField;
            }
            set
            {
                if ((_iserialField.Equals(value) != true))
                {
                    _iserialField = value;
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

        public int TblStyle
        {
            get
            {
                return _tblStyleField;
            }
            set
            {
                if ((_tblStyleField.Equals(value) != true))
                {
                    _tblStyleField = value;
                    RaisePropertyChanged("TblStyle");
                }
            }
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
        private System.IO.FileInfo _uploadedFile;
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

        public void UploadFile(System.IO.FileInfo fileToUpload)
        {
            _fileSize = GetFileSize(fileToUpload.Length);
            StartUpload(fileToUpload);
        }

        private void StartUpload(System.IO.FileInfo file)
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

    public class DocumentationFilesViewModel : ViewModelBase
    {

        public DocumentationFilesViewModel()
        {
            
            MainRowList = new SortableCollectionView<TblDocumentationFilesViewModel>();
            SelectedMainRow = new TblDocumentationFilesViewModel();

            Client.GetDocumentationFilesCompleted += (s, sv) =>
            {
                MainRowList.Clear();
                foreach (var row in sv.Result)
                {
                    var newrow = new TblDocumentationFilesViewModel();
                    
                    newrow.InjectFrom(row);
                    MainRowList.Add(newrow);
                }
                Loading = false;
            };

            Client.GetDocumentationFilesAsync();
           
        }
        
        #region Props

        private SortableCollectionView<TblDocumentationFilesViewModel> _mainRowList;

        public SortableCollectionView<TblDocumentationFilesViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
        }

        private ObservableCollection<TblDocumentationFilesViewModel> _selectedMainRows;

        public ObservableCollection<TblDocumentationFilesViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblDocumentationFilesViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
        }

        private TblDocumentationFilesViewModel _selectedMainRow;

        public TblDocumentationFilesViewModel SelectedMainRow
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
    }
}