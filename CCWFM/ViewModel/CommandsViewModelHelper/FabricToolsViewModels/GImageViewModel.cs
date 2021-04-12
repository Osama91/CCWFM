using System.Collections.ObjectModel;
using CCWFM.Helpers.Enums;
using _Proxy = CCWFM.CRUDManagerService;

namespace CCWFM.ViewModel.FabricToolsViewModels
{
    public class GImageViewModel : Web.DataLayer.PropertiesViewModelBase
    {
        private ObservableCollection<_Proxy.tbl_FabricAttriputes> _fabricAttrCollection;

        private string _gFabricId;

        private byte[] _gImage;

        private string _gImageDescreption;

        private ImageCondition _gImageState;

        private int? _gIserial;

        public GImageViewModel(string _FabricCode)
        {
            G_FabricId = _FabricCode;
        }

        public ImageCondition _ImageState
        {
            get { return _gImageState; }
            set
            {
                if (_gImageState != value)
                {
                    _gImageState = value;
                    RaisePropertyChanged("_ImageState");
                }
            }
        }

        public ObservableCollection<_Proxy.tbl_FabricAttriputes> FabricAttrCollection
        {
            get { return _fabricAttrCollection; }
            set { _fabricAttrCollection = value; RaisePropertyChanged("FabricAttrCollection"); }
        }

        public string G_FabricId
        {
            get { return _gFabricId; }
            set
            {
                if (_gFabricId != value)
                {
                    _gFabricId = value;
                    RaisePropertyChanged("G_FabricId");
                }
            }
        }

        public byte[] G_Image
        {
            get { return _gImage; }
            set
            {
                if (_gImage != value)
                {
                    _gImage = value;
                    RaisePropertyChanged("G_Image");
                }
            }
        }

        public string G_ImageDescreption
        {
            get { return _gImageDescreption; }
            set
            {
                if (_gImageDescreption != value)
                {
                    _gImageDescreption = value;
                    RaisePropertyChanged("G_ImageDescreption");
                }
            }
        }

        public int? _GIserial
        {
            get { return _gIserial; }
            set { _gIserial = value; RaisePropertyChanged("G_Iserial"); }
        }
    }
}