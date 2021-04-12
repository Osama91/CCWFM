using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using CCWFM.CRUDManagerService;
using Omu.ValueInjecter.Silverlight;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.OGViewModels
{
	public static class ArgumentHelper
	{
		public static void AssertNotNull<T>(T arg, string argName)
			where T : class
		{
			if (arg == null)
			{
				throw new ArgumentNullException(argName);
			}
		}

		public static void AssertNotNull<T>(T? arg, string argName)
			where T : struct
		{
			if (!arg.HasValue)
			{
				throw new ArgumentNullException(argName);
			}
		}

		public static void AssertGenericArgumentNotNull<T>(T arg, string argName)
		{
			var type = typeof(T);

			if (!type.IsValueType || (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>))))
			{
				AssertNotNull((object)arg, argName);
			}
		}

		public static void AssertNotNull<T>(IEnumerable<T> arg, string argName, bool assertContentsNotNull)
		{
			//make sure the enumerable item itself isn't null
			AssertNotNull(arg, argName);

			if (assertContentsNotNull && typeof(T).IsClass)
			{
				//make sure each item in the enumeration isn't null
				foreach (var item in arg)
				{
					if (item == null)
					{
						throw new ArgumentException("An item inside the enumeration was null.", argName);
					}
				}
			}
		}

		public static void AssertNotNullOrEmpty(string arg, string argName)
		{
			AssertNotNullOrEmpty(arg, argName, false);
		}

		public static void AssertNotNullOrEmpty(string arg, string argName, bool trim)
		{
			if (string.IsNullOrEmpty(arg) || (trim && IsOnlyWhitespace(arg)))
			{
				throw new ArgumentException("Cannot be null or empty.", argName);
			}
		}

		[CLSCompliant(false)]
		public static void AssertEnumMember<TEnum>(TEnum enumValue, string argName)
				where TEnum : struct, IConvertible
		{
#if !SILVERLIGHT
				if (Attribute.IsDefined(typeof(TEnum), typeof(FlagsAttribute), false))
				{
					//flag enumeration - we can only get here if TEnum is a valid enumeration type, since the FlagsAttribute can
					//only be applied to enumerations
					bool throwEx;
					long longValue = enumValue.ToInt64(CultureInfo.InvariantCulture);

					if (longValue == 0)
					{
						//only throw if zero isn't defined in the enum - we have to convert zero to the underlying type of the enum
						throwEx = !Enum.IsDefined(typeof(TEnum), ((IConvertible)0).ToType(Enum.GetUnderlyingType(typeof(TEnum)), CultureInfo.InvariantCulture));
					}
					else
					{
						foreach (TEnum value in Enum.GetValues(typeof(TEnum)))
						{
							longValue &= ~value.ToInt64(CultureInfo.InvariantCulture);
						}

						//throw if there is a value left over after removing all valid values
						throwEx = (longValue != 0);
					}

					if (throwEx)
					{
						throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
							"Enum value '{0}' is not valid for flags enumeration '{1}'.",
							enumValue, typeof(TEnum).FullName), argName);
					}
				}
				else
				{
					//not a flag enumeration
					if (!Enum.IsDefined(typeof(TEnum), enumValue))
					{
						throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
								"Enum value '{0}' is not defined for enumeration '{1}'.",
								enumValue, typeof(TEnum).FullName), argName);
					}
				}
#else
			throw new NotSupportedException("AssertEnumMember not presently supported in Silverlight");
#endif
		}

		[CLSCompliant(false)]
		public static void AssertEnumMember<TEnum>(TEnum enumValue, string argName, params TEnum[] validValues)
			where TEnum : struct, IConvertible
		{
			AssertNotNull(validValues, "validValues");

			if (Attribute.IsDefined(typeof(TEnum), typeof(FlagsAttribute), false))
			{
				//flag enumeration
				bool throwEx;
				var longValue = enumValue.ToInt64(CultureInfo.InvariantCulture);

				if (longValue == 0)
				{
					//only throw if zero isn't permitted by the valid values
					throwEx = true;

					foreach (var value in validValues)
					{
						if (value.ToInt64(CultureInfo.InvariantCulture) == 0)
						{
							throwEx = false;
							break;
						}
					}
				}
				else
				{
					foreach (var value in validValues)
					{
						longValue &= ~value.ToInt64(CultureInfo.InvariantCulture);
					}

					//throw if there is a value left over after removing all valid values
					throwEx = (longValue != 0);
				}

				if (throwEx)
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
						"Enum value '{0}' is not allowed for flags enumeration '{1}'.",
						enumValue, typeof(TEnum).FullName), argName);
				}
			}
			else
			{
				//not a flag enumeration
				foreach (var value in validValues)
				{
					if (enumValue.Equals(value))
					{
						return;
					}
				}

				//at this point we know an exception is required - however, we want to tailor the message based on whether the
				//specified value is undefined or simply not allowed
				if (!Enum.IsDefined(typeof(TEnum), enumValue))
				{
					throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
							"Enum value '{0}' is not defined for enumeration '{1}'.",
							enumValue, typeof(TEnum).FullName), argName);
				}
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
					"Enum value '{0}' is defined for enumeration '{1}' but it is not permitted in this context.",
					enumValue, typeof(TEnum).FullName), argName);
			}
		}

		#region Private Methods

		private static bool IsOnlyWhitespace(string arg)
		{
			foreach (var c in arg)
			{
				if (!char.IsWhiteSpace(c))
				{
					return false;
				}
			}

			return true;
		}

		#endregion Private Methods
	}

	public class UISynchronizationContext : ISynchronizationContext
	{
		#region Data

		private DispatcherSynchronizationContext _context;
		private Dispatcher _dispatcher;
		private readonly object _initializationLock = new object();

		#endregion Data

		#region Singleton implementation

		private static readonly UISynchronizationContext instance = new UISynchronizationContext();

		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		/// <value>The singleton instance.</value>
		public static ISynchronizationContext Instance
		{
			get
			{
				return instance;
			}
		}

		#endregion Singleton implementation

		#region Public Methods

		public void Initialize()
		{
			EnsureInitialized();
		}

		#endregion Public Methods

		#region ISynchronizationContext Members

		public void Initialize(Dispatcher dispatcher)
		{
			ArgumentHelper.AssertNotNull(dispatcher, "dispatcher");

			lock (_initializationLock)
			{
				_dispatcher = dispatcher;
				_context = new DispatcherSynchronizationContext(dispatcher);
			}
		}

		public void InvokeAsynchronously(SendOrPostCallback callback, object state)
		{
			ArgumentHelper.AssertNotNull(callback, "callback");
			EnsureInitialized();

			_context.Post(callback, state);
		}

		public void InvokeAsynchronously(Action action)
		{
			ArgumentHelper.AssertNotNull(action, "action");
			EnsureInitialized();

			if (_dispatcher.CheckAccess())
			{
				action();
			}
			else
			{
				_dispatcher.BeginInvoke(action);
			}
		}

		public void InvokeSynchronously(SendOrPostCallback callback, object state)
		{
			ArgumentHelper.AssertNotNull(callback, "callback");
			EnsureInitialized();

			_context.Send(callback, state);
		}

		public void InvokeSynchronously(Action action)
		{
			ArgumentHelper.AssertNotNull(action, "action");
			EnsureInitialized();

			if (_dispatcher.CheckAccess())
			{
				action();
			}
			else
			{
				_context.Send(delegate { action(); }, null);
			}
		}

		public bool InvokeRequired
		{
			get
			{
				EnsureInitialized();
				return !_dispatcher.CheckAccess();
			}
		}

		#endregion ISynchronizationContext Members

		#region Private Methods

		private void EnsureInitialized()
		{
			if (_dispatcher != null && _context != null)
			{
				return;
			}

			lock (_initializationLock)
			{
				if (_dispatcher != null && _context != null)
				{
					return;
				}

				try
				{
					_dispatcher = Deployment.Current.Dispatcher;
					_context = new DispatcherSynchronizationContext(_dispatcher);
				}
				catch (InvalidOperationException)
				{
					throw new Exception("Initialised called from non-UI thread.");
				}
			}
		}

		#endregion Private Methods
	}

	public interface ISynchronizationContext
	{
		void Initialize(Dispatcher dispatcher);

		void InvokeAsynchronously(SendOrPostCallback callback, object state);

		void InvokeAsynchronously(Action action);

		void InvokeSynchronously(SendOrPostCallback callback, object state);

		void InvokeSynchronously(Action action);

		bool InvokeRequired { get; }
	}

	public class TblStyleImageViewModel : ViewModelBase
	{
		private readonly String _baseUri;

		public TblStyleImageViewModel()
		{
			_synContext = UISynchronizationContext.Instance;

			var fullUri = Application.Current.Host.Source;
			_baseUri = fullUri.AbsoluteUri.Substring(0, fullUri.AbsoluteUri.IndexOf("/ClientBin", StringComparison.Ordinal));
		}

		private DateTime _creationDateField;

		private string _imageDescriptionField;

		private bool _isActiveField;

		private bool _isPrintableField;

		private int _iserialField;

		private DateTime? _lastUpdateddateField;

		private int _tblStyleField;

		private TblColor _colorPerRow;

		public TblColor ColorPerRow
		{
			get { return _colorPerRow; }
			set
			{
				if (value != null)
				{
					_colorPerRow = value;
					RaisePropertyChanged("ColorPerRow");
				}
			}
		}

		private int? _tblColorField;

		public int? TblColor
		{
			get
			{
				return _tblColorField;
			}
			set
			{
				if ((Equals(_tblColorField, value) != true))
				{
					_tblColorField = value;
					RaisePropertyChanged("TblColor");
				}
			}
		}

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

		private string _orginalFilEname;

		public string OrginalFilEname
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
					RaisePropertyChanged("OrginalFilEname");
				}
			}
		}

		public string ImageDescription
		{
			get
			{
				return _imageDescriptionField;
			}
			set
			{
				if ((ReferenceEquals(_imageDescriptionField, value) != true))
				{
					_imageDescriptionField = value;
					RaisePropertyChanged("ImageDescription");
				}
			}
		}

		public bool IsActive
		{
			get
			{
				return _isActiveField;
			}
			set
			{
				if ((_isActiveField.Equals(value) != true))
				{
					_isActiveField = value;
					RaisePropertyChanged("IsActive");
				}
			}
		}

		public bool IsPrintable
		{
			get { return _isPrintableField; }
			set
			{
				if ((_isPrintableField.Equals(value) != true))
				{
					_isPrintableField = value;
					RaisePropertyChanged("IsPrintable");
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

		private string _imagePathField;

		public string ImagePath
		{
			get
			{
				return _imagePathField;
			}
			set
			{
				if ((ReferenceEquals(_imagePathField, value) != true))
				{
					_imagePathField = value;
					RaisePropertyChanged("ImagePath");
				}
			}
		}

		private byte[] _imagePathThumbnailField;

		public byte[] ImagePathThumbnail
		{
			get
			{
				return _imagePathThumbnailField;
			}
			set
			{
				if ((ReferenceEquals(_imagePathThumbnailField, value) != true))
				{
					_imagePathThumbnailField = value;
					RaisePropertyChanged("ImagePathThumbnail");
				}
			}
		}

		private int _tblRequestForSample;

		public int TblRequestForSample
		{
			get
			{
				return _tblRequestForSample;
			}
			set
			{
				if ((_tblRequestForSample.Equals(value) != true))
				{
					_tblRequestForSample = value;
					RaisePropertyChanged("TblRequestForSample");
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

		private FileInfo _imagePerRow;

		public FileInfo ImagePerRow
		{
			get { return _imagePerRow; }
			set { _imagePerRow = value; RaisePropertyChanged("ImagePerRow"); }
		}

		//exposed properties which are bound in XAML

		public bool IsBusy { get; set; }

		private double _percentage;

		/// <summary>
		/// Gets the percentage.
		/// </summary>
		/// <value>The percentage.</value>
		public double Percentage
		{
			get { return _percentage; }
			set
			{
				_percentage = value;
				RaisePropertyChanged("Percentage");
			}
		}

		private bool _default;

		public bool DefaultImage
		{
			get { return _default; }
			set { _default = value; RaisePropertyChanged("DefaultImage"); }
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
		private String _fileMessage = "";

		public String FileMessage
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
			string pathofImage = ImagePath.Split('/').LastOrDefault();
			var dataToSend = _dataLength - _dataSent;
			var isLastChunk = dataToSend <= ChunkSize;
			var isFirstChunk = _dataSent == 0;
			var httpHandlerUrlBuilder = new UriBuilder(string.Format("{0}/FileUpload.ashx", _baseUri));
			httpHandlerUrlBuilder.Query = string.Format("{4}file={0}&offset={1}&last={2}&first={3}&Folder={5}",
				pathofImage, _dataSent, isLastChunk, isFirstChunk,
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

	public class StyleImageViewModel : ViewModelBase
	{
		private TblStyleViewModel _style;

		public TblStyleViewModel Style
		{
			get { return _style; }
			set
			{
				_style = value;
				RaisePropertyChanged("Style");
			}
		}

		public StyleImageViewModel(TblStyleViewModel style)
		{
			Style = style;
			MainRowList = new SortableCollectionView<TblStyleImageViewModel>();

			GetColors();
			MainRowList.CollectionChanged += MainRowList_CollectionChanged;
			SelectedMainRow = new TblStyleImageViewModel();

			Client.GetTblStyleImageCompleted += (s, sv) =>
			{
				MainRowList.Clear();
				foreach (var row in sv.Result)
				{
					var newrow = new TblStyleImageViewModel();
					newrow.InjectFrom(row);
					newrow.ColorPerRow = row.TblColor1;
					MainRowList.Add(newrow);
				}
				Loading = false;
			};

			Client.GetTblColorLinkCompleted += (s, sv) =>
			{
				ColorList = new ObservableCollection<TblColor>();
				foreach (var row in sv.Result)
				{
					ColorList.Add(row.TblColor1);
				}
			};

			Client.MaxIserialOfStyleCompleted += (s, sv) =>
			{
				FolderPath = "Uploads" + "/" + sv.imagePath;

				string folderName = FolderPath + "/" + Style.SeasonPerRow.Ename + "_" + Style.Brand + "_" +
									 Style.SectionPerRow.Ename;

				var counter = 0;
				foreach (var item in MainRowList)
				{
					if (item.Iserial == 0)
					{
						var maxIserial = sv.Result;

						item.ImagePath = folderName + "/" + style.StyleCode + maxIserial + counter + ".png";
						item.FolderPath = folderName;
						item.UploadFile(item.ImagePerRow);

						counter++;
					}
				}

				var isvalid = false;

				foreach (var tblStyleImageViewModel in MainRowList)
				{
					isvalid = Validator.TryValidateObject(tblStyleImageViewModel, new ValidationContext(tblStyleImageViewModel, null, null), null, true);

					if (!isvalid)
					{
						return;
					}
				}

				if (isvalid)
				{
					var savelist = new ObservableCollection<TblStyleImage>();
					GenericMapper.InjectFromObCollection(savelist, MainRowList);

					Client.UpdateOrInsertTblStyleImageAsync(savelist);
				}
			};
			Client.GetTblStyleImageAsync(style.Iserial);
			Client.UpdateOrInsertTblStyleImageCompleted += (s, x) => Client.GetTblStyleImageAsync(style.Iserial);

			Client.DeleteTblStyleImageCompleted += (s, ev) =>
			{
				if (ev.Error != null)
				{
					throw ev.Error;
				}

				var oldrow = MainRowList.FirstOrDefault(x => x.Iserial == ev.Result);
				if (oldrow != null) MainRowList.Remove(oldrow);
			};
		}

		public void GetColors()
		{
			Client.GetTblColorLinkAsync(0, int.MaxValue, Style.Brand, (int)Style.TblLkpBrandSection, (int)Style.TblLkpSeason, "it.TblColor", null, null);
		}

		private void MainRowList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
				foreach (TblStyleImageViewModel item in e.NewItems)
					item.PropertyChanged += item_PropertyChanged;

			if (e.OldItems != null)
				foreach (TblStyleImageViewModel item in e.OldItems)
					item.PropertyChanged -= item_PropertyChanged;
		}

		private ObservableCollection<TblColor> _colorList;

		public ObservableCollection<TblColor> ColorList
		{
			get { return _colorList ?? (_colorList = new ObservableCollection<TblColor>()); }
			set { _colorList = value; RaisePropertyChanged("ColorList"); }
		}

		private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RaisePropertyChanged(e.PropertyName);
			if (e.PropertyName == "DefaultImage")
			{
				foreach (var tblStyleImageViewModel in MainRowList.Where(x => x != SelectedMainRow && x.DefaultImage && x.TblColor == SelectedMainRow.TblColor))
				{
					tblStyleImageViewModel.DefaultImage = false;
				}
			}
		}

		public void DeleteMainRow(TblStyleImageViewModel row)
		{
			var res = MessageBox.Show("Are You To Delete SelectedRecords From Database ?", "Delete",
				MessageBoxButton.OKCancel);
			if (res == MessageBoxResult.OK)
			{
				if (row.Iserial == 0)
				{
					MainRowList.Remove(row);
				}
				else
				{
					Client.DeleteTblStyleImageAsync(
					(TblStyleImage)new TblStyleImage().InjectFrom(row), MainRowList.IndexOf(row));
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

				MainRowList.Insert(currentRowIndex + 1, new TblStyleImageViewModel
				{
					TblStyle = Style.Iserial
				});
			}
		}

		#region Props

		private SortableCollectionView<TblStyleImageViewModel> _mainRowList;

		public SortableCollectionView<TblStyleImageViewModel> MainRowList
		{
			get { return _mainRowList; }
			set { _mainRowList = value; RaisePropertyChanged("MainRowList"); }
		}

		private ObservableCollection<TblStyleImageViewModel> _selectedMainRows;

		public ObservableCollection<TblStyleImageViewModel> SelectedMainRows
		{
			get { return _selectedMainRows ?? (_selectedMainRows = new ObservableCollection<TblStyleImageViewModel>()); }
			set { _selectedMainRows = value; RaisePropertyChanged("SelectedMainRows"); }
		}

		private TblStyleImageViewModel _selectedMainRow;

		public TblStyleImageViewModel SelectedMainRow
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

		internal void SaveImages()
		{
			Client.MaxIserialOfStyleAsync();
		}
	}
}