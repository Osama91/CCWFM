using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using CCWFM.CRUDManagerService;
using CCWFM.Helpers.LocalizationHelpers;
using CCWFM.ViewModel.CommandsViewModelHelper;
using CCWFM.ViewModel.GenericViewModels;

namespace CCWFM.ViewModel.SMLViewModels
{
    public class SmlRowEditorViewModel : ViewModelBase
    {
        #region [ Events ]

        public event EventHandler StylesPupulated;

        protected virtual void OnStylesPupulated()
        {
            var handler = StylesPupulated;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler Colorspopulated;

        protected virtual void OnColorspopulated()
        {
            var handler = Colorspopulated;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion [ Events ]

        #region [ Fields ]

        private string _brandCode;
        private string _color;
        private DateTime _deliveryDate;
        private string _description;
        private int _qty;
        private string _seasonCode;
        private string _sizeRange;
        private int _statusSerial;
        private string _styleCode;
        private List<Brand> _brands;
        private DateTime _creationDate;
        private List<TblLkpSeason> _seasons;
        private GenericViewModel _selectedStatus;
        private StylesDTO _selectedStyle;
        private List<SizesWithGroups> _sizeRageWithSizes;
        private List<SmlSizeDetails> _sizes;
        private List<GenericViewModel> _statusList;
        private ObservableCollection<StylesDTO> _styles;
        #endregion [ Fields ]

        #region [ Properties ]

        [Display(ResourceType = typeof(strings), Name = "Brand")]
        public string BrandCode
        {
            get { return _brandCode; }
            set
            {
                _brandCode = value;
                RaisePropertyChanged("BrandCode");
            }
        }

        [Display(ResourceType = typeof(strings), Name = "Brand", Description = "Brand")]
        public List<Brand> Brands
        {
            get { return _brands ?? (_brands = new List<Brand>()); }
            set
            {
                _brands = value;
                RaisePropertyChanged("Brands");
            }
        }

        [Display(ResourceType = typeof(strings), Name = "Color")]
        public string ColorCode
        {
            get { return _color; }
            set
            {
                if (value == null || _color == value) return;
                _color = value;
                RaisePropertyChanged("ColorCode");
            }
        }

        private List<TblColor> _colorList;

        public List<TblColor> ColorList
        {
            get { return _colorList ?? (_colorList = new List<TblColor>()); }
            set
            {
                _colorList = value;
                RaisePropertyChanged("ColorList");
            }
        }

        [Display(ResourceType = typeof(strings), Name = "CreationDate")]
        public DateTime CraetionDate
        {
            get { return _creationDate; }
            set
            {
                _creationDate = value;
                RaisePropertyChanged("CraetionDate");
            }
        }

        [Display(ResourceType = typeof(strings), Name = "DeliveryDate")]
        public DateTime DeliveryDate
        {
            get
            {
                if (_deliveryDate == DateTime.MinValue)
                    _deliveryDate = DateTime.Now;
                return _deliveryDate;
            }
            set
            {
                _deliveryDate = value;
                RaisePropertyChanged("DeliveryDate");
            }
        }

        [Display(ResourceType = typeof(strings), Name = "Description")]
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged("Description");
            }
        }

        [Display(ResourceType = typeof(strings), Name = "Qty")]
        public int Qty
        {
            get { return _qty; }
            set
            {
                _qty = value;
                RaisePropertyChanged("Qty");
            }
        }

        [Display(ResourceType = typeof(strings), Name = "Season")]
        public string SeasonCode
        {
            get { return _seasonCode; }
            set
            {
                _seasonCode = value;
                RaisePropertyChanged("SeasonCode");
            }
        }

        [Display(ResourceType = typeof(strings), Name = "Season", Description = "Season")]
        public List<TblLkpSeason> Seasons
        {
            get { return _seasons ?? (_seasons = new List<TblLkpSeason>()); }
            set
            {
                _seasons = value;
                RaisePropertyChanged("Seasons");
            }
        }

        [Display(ResourceType = typeof(strings), Name = "Status", Description = "Status")]
        public GenericViewModel SelectedStatus
        {
            get { return _selectedStatus ?? (_selectedStatus = StatusList.FirstOrDefault()); }
            set
            {
                _selectedStatus = value;
                RaisePropertyChanged("SelectedStatus");
            }
        }

        public StylesDTO SelectedStyle
        {
            get { return _selectedStyle; }
            set
            {
                if (value == null || value == _selectedStyle) return;
                _selectedStyle = value;
                Description = value.Desc;
                var sizesWithGroups = SizeRangeWithSizes.FirstOrDefault(x => x.StyleCode == value.StyleCode);
                if (sizesWithGroups != null)
                    SizeRange = sizesWithGroups.SizeGroup;
                RaisePropertyChanged("SelectedStyle");
            }
        }

        [Display(ResourceType = typeof(strings), Name = "SizeRange")]
        public string SizeRange
        {
            get { return _sizeRange; }
            set
            {
                _sizeRange = value;
                RaisePropertyChanged("SizeRange");
            }
        }

        public List<SizesWithGroups> SizeRangeWithSizes
        {
            get { return _sizeRageWithSizes; }
            set
            {
                _sizeRageWithSizes = value;
                RaisePropertyChanged("SizeRangeWithSizes");
            }
        }

        public List<SmlSizeDetails> Sizes
        {
            get { return _sizes ?? (_sizes = new List<SmlSizeDetails>()); }
            set
            {
                _sizes = value;
                RaisePropertyChanged("Sizes");
            }
        }

        public List<GenericViewModel> StatusList
        {
            get { return _statusList ?? (_statusList = new List<GenericViewModel>()); }
            set
            {
                _statusList = value;
                RaisePropertyChanged("StatusList");
            }
        }

        public int StatusSerial
        {
            get { return _statusSerial; }
            set
            {
                _statusSerial = value;
                RaisePropertyChanged("StatusSerial");
            }
        }

        [Required]
        [Display(ResourceType = typeof(strings), Name = "Style")]
        public string StyleCode
        {
            get { return _styleCode; }
            set
            {
                _styleCode = value;
                RaisePropertyChanged("StyleCode");
            }
        }

        public ObservableCollection<StylesDTO> Styles
        {
            get { return _styles ?? (_styles = new ObservableCollection<StylesDTO>()); }
            set
            {
                _styles = value;
                RaisePropertyChanged("Styles");
            }
        }

        #endregion [ Properties ]

        #region [ Constructors ]

        public SmlRowEditorViewModel()
        {
            if (DesignerProperties.IsInDesignTool) return;
            Client = new CRUD_ManagerServiceClient();
            Client.SearchAXStylesCompleted += (s, ee) =>
                {
                    if (ee.Error != null) return;

                    SizeRangeWithSizes = new List<SizesWithGroups>(ee.sizesWithGroups);
                    Styles = new ObservableCollection<StylesDTO>((from x in ee.Result

                                                                  select new StylesDTO
                                                                      {
                                                                          StyleCode = x.StyleCode,
                                                                          Desc = x.StyleName
                                                                      }).ToList());
                    OnStylesPupulated();
                };

            Client.GetTblColorLinkCompleted += (s, ee) =>
            {
                if (ee.Error != null) return;
                ColorList = new List<TblColor>();
                foreach (var row in ee.Result)
                {
                    ColorList.Add(row.TblColor1);
                }

                OnColorspopulated();
            };
        }

        #endregion [ Constructors ]

        #region [ Commands ]

        private CommandsExecuter _filterStyleCommand;

        public CommandsExecuter FilterStyleCommand
        {
            get { return _filterStyleCommand ?? (_filterStyleCommand = new CommandsExecuter(PupulateStyles) { IsEnabled = true }); }
        }

        private CommandsExecuter _filterColorsCommand;

        public CommandsExecuter FilterColorsCommand
        {
            get { return _filterColorsCommand ?? (_filterColorsCommand = new CommandsExecuter(FilterColors) { IsEnabled = true }); }
            set { _filterColorsCommand = value; }
        }

        #endregion [ Commands ]

        #region [ Commands bound methods ]

        private void PupulateStyles()
        {
            Client.SearchAXStylesAsync("SML:" + StyleCode);
        }

        private void FilterColors()
        {
            if (string.IsNullOrEmpty(SeasonCode) || string.IsNullOrEmpty(BrandCode)) return;
            //      Client.GetColorsByBrandSeasonAsync(BrandCode, SeasonCode);
        }

        #endregion [ Commands bound methods ]
    }
}