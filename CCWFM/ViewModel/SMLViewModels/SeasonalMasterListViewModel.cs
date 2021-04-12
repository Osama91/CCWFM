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
using CCWFM.Views.StylePages;
using Os.Controls.DataGrid;

namespace CCWFM.ViewModel.SMLViewModels
{
    public class SeasonalMasterCollection : SortableCollectionView<SeasonalMasterListViewModel>
    {
    }

    public class SeasonalMasterListViewModel : ViewModelBase
    {
        #region [ Events ]

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
        private DateTime? _creationDate;
        private SmlHeaderViewModel _parentObj;
        private GenericViewModel _selectedStatus;
        private ObservableCollection<SmlSizeDetails> _sizes;
        private ObservableCollection<StylesDTO> _styles;
        #endregion [ Fields ]

        #region [ Properties ]

        private ObjectStatus _objStatus;

        public ObjectStatus ObjStatus
        {
            get { return _objStatus; }
            set
            {
                _objStatus = value;
                RaisePropertyChanged("ObjStatus");
            }
        }

        private int _Iserial;

        public int Iserial
        {
            get { return _Iserial; }
            set
            {
                _Iserial = value;
                RaisePropertyChanged("Iserial");
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

        [Display(ResourceType = typeof(strings), Name = "CreationDate")]
        public DateTime? CraetionDate
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

        public SmlHeaderViewModel ParentObj
        {
            get { return _parentObj; }
            set
            {
                _parentObj = value;
                RaisePropertyChanged("ParentObj");
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

        [Display(ResourceType = typeof(strings), Name = "Status")]
        public GenericViewModel SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                if (value == null) return;
                _selectedStatus = value;
                StatusName = value.Ename;
                RaisePropertyChanged("SelectedStatus");
            }
        }

        private string _statusName;

        [Display(ResourceType = typeof(strings), Name = "Status")]
        public string StatusName
        {
            get { return _statusName; }
            set { _statusName = value; RaisePropertyChanged("StatusName"); }
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

        public ObservableCollection<SmlSizeDetails> Sizes
        {
            get { return _sizes ?? (_sizes = new ObservableCollection<SmlSizeDetails>()); }
            set
            {
                _sizes = value;
                RaisePropertyChanged("Sizes");
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

        public SeasonalMasterListViewModel()
        {
            if (DesignerProperties.IsInDesignTool) return;
            Client = new CRUD_ManagerServiceClient();
            Client.UpdateOrInsertTblSmlCompleted += (s, e) =>
                {
                    if (e.Error != null) return;
                    ObjStatus.IsNew = false;
                    ObjStatus.IsSavedDBItem = true;
                    Iserial = e.Result.Iserial;
                   
                };
            Sizes.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                    foreach (SmlSizeDetails item in e.NewItems)
                    {
                        item.PropertyChanged
                            += (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }

                if (e.OldItems != null)
                    foreach (SmlSizeDetails item in e.OldItems)
                    {
                        // ReSharper disable once EventUnsubscriptionViaAnonymousDelegate
                        item.PropertyChanged
                            -= (s1, e1) => RaisePropertyChanged(e1.PropertyName);
                    }
            };
        }

        #endregion [ Constructors ]

        #region [ Commands ]

        private CommandsExecuter _editRowCommand;
        private List<SizesWithGroups> _sizeRageWithSizes;

        public CommandsExecuter EditRowCommand
        {
            get { return _editRowCommand ?? (_editRowCommand = new CommandsExecuter(EditRow) { IsEnabled = true }); }
            set { _editRowCommand = value; }
        }

        private CommandsExecuter _editRatiosCommand;

        public CommandsExecuter EditRatiosCommand
        {
            get { return _editRatiosCommand ?? (_editRatiosCommand = new CommandsExecuter(EditRatios) { IsEnabled = true }); }
        }

        #endregion [ Commands ]

        #region [ Commands bound methods ]

        private void EditRow()
        {
            var tmp = new SMLRowEditor
                {
                    LayoutRoot =
                        {
                            DataContext = new SmlRowEditorViewModel
                                {
                                    ColorCode = ColorCode,
                                    StyleCode = StyleCode,
                                    Description = Description,
                                    DeliveryDate = DeliveryDate,
                                    SizeRange = SizeRange,
                                    Qty = Qty,
                                    SeasonCode = SeasonCode,
                                    BrandCode = BrandCode,
                                    //Seasons = new List<TblLkpSeason>(ParentObj.Seasons),
                                    Brands = new List<Brand>(ParentObj.Brands),
                                    StatusList = new List<GenericViewModel>(ParentObj.StatusList),
                                    StatusSerial = StatusSerial,
                                    SelectedStatus = SelectedStatus
                                }
                        }
                };

            tmp.InitiateCustomeEvents();
            tmp.Submit += (s, e) =>
            {
                var vm = (tmp.LayoutRoot.DataContext as SmlRowEditorViewModel);
                if (vm == null) return;
                StyleCode = vm.StyleCode;
                ColorCode = vm.ColorCode;
                StyleCode = vm.StyleCode;
                Description = vm.Description;
                DeliveryDate = vm.DeliveryDate;
                SizeRange = vm.SizeRange;
                Qty = vm.Qty;
                SeasonCode = vm.SeasonCode;
                BrandCode = vm.BrandCode;
                SelectedStatus = vm.SelectedStatus;
                if (!Sizes.Any())
                {
                    if (vm.SizeRangeWithSizes != null)
                    {
                        var sze = (from x in vm.SizeRangeWithSizes
                                   select x.SizeCode).Distinct()
                            .Select(x => new SmlSizeDetails
                            {
                                ParentObject = this,
                                ParentSerial = Iserial,
                                SizeCode = x
                            }).ToList();
                        foreach (var item in sze)
                        {
                            Sizes.Add(item);
                        }
                    }
                }

                // if (ObjStatus.IsNew)
                //Client.UpdateOrInsertTblSmlAsync(new tblSeasonalMasterList
                //    {
                //        BrandCode = BrandCode,
                //        ColorCode = ColorCode,
                //        CreationDate = DateTime.Now,
                //        DelivaryDate = DeliveryDate,
                //        Description = Description,
                //        SeasonCode = SeasonCode,
                //        SizeRange = SizeRange,
                //        StyleCode = StyleCode,
                //        Qty = Qty,
                //        StatusID = SelectedStatus.Iserial
                //    }, true, 0);
                //    else if (ObjStatus.IsSavedDBItem)
                //        Client.UpdateOrInsertTblSmlAsync(new tblSeasonalMasterList
                //        {
                //            Iserial = Iserial,
                //            BrandCode = BrandCode,
                //            ColorCode = ColorCode,
                //            CreationDate = CraetionDate,
                //            DelivaryDate = DeliveryDate,
                //            Description = Description,
                //            SeasonCode = SeasonCode,
                //            SizeRange = SizeRange,
                //            StyleCode = StyleCode,
                //            Qty = Qty,
                //            StatusID = SelectedStatus.Iserial
                //        }, false, 0);
                //};
                //tmp.Show();
            };
        }

        private void EditRatios()
        {
            var tmp = new SMLRatioEditor
            {
                RatiosGrid =
                {
                    ItemsSource = Sizes
                }
            };
            tmp.SubmitRatios += (s, e) =>
                {
                    if (!Sizes.Any()) return;
                    SaveSizeRatios();
                };
            tmp.Show();
        }

        private void SaveSizeRatios()
        {
            //var mappedDetails = (from x in Sizes
            //                     select new TblSeasonalMasterListDetail
            //                         {
            //                             ParentSerial = Iserial,
            //                             Size = x.SizeCode,
            //                             Iserial = x.Iserial,
            //                             DeliveryDate = DeliveryDate,
            //                             Ratio = (double)x.SizeRatio
            //                         }).ToList();

            //Client.UpdateOrInsertTblSmlDetailsAsync
            //    (
            //        new ObservableCollection<TblSeasonalMasterListDetail>(mappedDetails)
            //    );
        }

        #endregion [ Commands bound methods ]
    }

    public class StylesDTO : ViewModelBase
    {
        private string _desc;
        private string _styleCode;

        public string Desc
        {
            get { return _desc; }
            set
            {
                _desc = value;
                RaisePropertyChanged("Desc");
            }
        }

        public string StyleCode
        {
            get { return _styleCode; }
            set
            {
                _styleCode = value;
                RaisePropertyChanged("StyleCode");
            }
        }
    }
}