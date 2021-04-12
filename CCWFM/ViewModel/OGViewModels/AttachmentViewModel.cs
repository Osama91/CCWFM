using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;
using CCWFM.AssistanceService;

namespace CCWFM.ViewModel.OGViewModels
{
    public class AttachmentViewModel : ViewModelBase
    {
        private readonly string _baseUri;

        public AttachmentViewModel()
        {
            _synContext = UISynchronizationContext.Instance;

            var fullUri = Application.Current.Host.Source;
            _baseUri = fullUri.AbsoluteUri.Substring(0, fullUri.AbsoluteUri.IndexOf("/ClientBin", StringComparison.Ordinal));

            TableName = string.Empty;
            FileName = string.Empty;
            Path = string.Empty;
            CreationDate = DateTime.Now;
            LastUpdatedDate = DateTime.Now;
        }


        private int _iserialField;
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

        string tableName;
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; RaisePropertyChanged(nameof(TableName)); }
        }

        int recordId;
        public int RecordId
        {
            get { return recordId; }
            set { recordId = value; RaisePropertyChanged(nameof(RecordId)); }
        }

        string fileName;
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; RaisePropertyChanged(nameof(FileName)); }
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

        private string _pathField;
        public string Path
        {
            get
            {
                return _pathField;
            }
            set
            {
                if ((ReferenceEquals(_pathField, value) != true))
                {
                    _pathField = value;
                    RaisePropertyChanged("Attachment");
                }
            }
        }

        private string _noteField;
        public string Notes
        {
            get
            {
                return _noteField;
            }
            set
            {
                if ((ReferenceEquals(_noteField, value) != true))
                {
                    _noteField = value;
                    RaisePropertyChanged("AttachmentDescription");
                }
            }
        }

        private DateTime _creationDateField;
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

        private DateTime? _lastUpdateddateField;
        public DateTime? LastUpdatedDate
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


        private FileInfo _attachmentPerRow;
        public FileInfo AttachmentPerRow
        {
            get { return _attachmentPerRow; }
            set { _attachmentPerRow = value; RaisePropertyChanged(nameof(AttachmentPerRow)); }
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
            var pathofAttachment = Path.Split('/').LastOrDefault();
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

    public class AttachmentModel : ViewModelBase
    {
        AssistanceServiceClient assistanceClient = new AssistanceServiceClient();
        string tableName;
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; RaisePropertyChanged(nameof(TableName)); }
        }
        int recordId;
        public int MasterId
        {
            get { return recordId; }
            set { recordId = value; RaisePropertyChanged(nameof(MasterId)); }
        }
        public AttachmentModel(string TableName, int masterId)
        {
            this.TableName = TableName;
            this.MasterId = masterId;
            MainRowList = new SortableCollectionView<AttachmentViewModel>();
            SelectedMainRow = new AttachmentViewModel();

            assistanceClient.GetAttachmentCompleted += (s, sv) =>
            {
                MainRowList.Clear();
                foreach (var row in sv.Result)
                {
                    var newrow = new AttachmentViewModel();
                    newrow.InjectFrom(row);
                    MainRowList.Add(newrow);
                }
                Loading = false;
            };

            assistanceClient.UpdateOrInsertAttachmentCompleted += (s, sv) =>
            {
                FolderPath = "/Uploads" + "/" + sv.FolderPath;
                var folderName = FolderPath;// + "/" + Style.SeasonPerRow.Ename + "_" + Style.Brand + "_" + Style.SectionPerRow.Ename;

                //var counter = 0;
                foreach (var item in MainRowList)
                {
                    if (item.Iserial == 0)
                    {
                        var oldItem = sv.Result.FirstOrDefault(r => r.OrginalFileName == item.OrginalFileName);
                        if (oldItem != null)
                        {
                            item.Path = oldItem.Path;
                            item.FolderPath = folderName;
                            item.UploadFile(item.AttachmentPerRow);
                        }
                        //counter++;
                    }
                }
            };
            assistanceClient.GetAttachmentAsync(TableName, MasterId);
            //assistanceClient.UpdateOrInsertAttachmentCompleted += (s, x) => assistanceClient.GetAttachmentAsync(TableName);

            assistanceClient.DeleteAttachmentCompleted += (s, ev) =>
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
                        var record = new TblAttachment();
                        record.InjectFrom(row);
                        assistanceClient.DeleteAttachmentAsync(record, MainRowList.IndexOf(row));
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
                MainRowList.Insert(currentRowIndex + 1, new AttachmentViewModel
                {
                    TableName = this.TableName,
                    RecordId = MasterId,
                });
            }
        }

        #region Props

        private SortableCollectionView<AttachmentViewModel> _mainRowList;

        public SortableCollectionView<AttachmentViewModel> MainRowList
        {
            get { return _mainRowList; }
            set { _mainRowList = value; RaisePropertyChanged(nameof(MainRowList)); }
        }

        private ObservableCollection<AttachmentViewModel> _selectedMainRows;

        public ObservableCollection<AttachmentViewModel> SelectedMainRows
        {
            get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<AttachmentViewModel>()); }
            set { _selectedMainRows = value; RaisePropertyChanged(nameof(SelectedMainRows)); }
        }

        private AttachmentViewModel _selectedMainRow;

        public AttachmentViewModel SelectedMainRow
        {
            get { return _selectedMainRow; }
            set { _selectedMainRow = value; RaisePropertyChanged(nameof(SelectedMainRow)); }
        }

        private string _folderPath;

        public string FolderPath
        {
            get { return _folderPath; }
            set { _folderPath = value; RaisePropertyChanged(nameof(FolderPath)); }
        }

        #endregion Props

        internal void SaveAttachments()
        {
            var isvalid = false;

            foreach (var tblStyleAttachmentViewModel in MainRowList)
            {
                isvalid = Validator.TryValidateObject(tblStyleAttachmentViewModel, new ValidationContext(tblStyleAttachmentViewModel, null, null), null, true);

                if (!isvalid)
                {
                    return;
                }
            }
            if (isvalid)
            {
                var savelist = new ObservableCollection<TblAttachment>();
                GenericMapper.InjectFromObCollection(savelist, MainRowList);
                assistanceClient.UpdateOrInsertAttachmentAsync(savelist);
            }
        }
    }
}