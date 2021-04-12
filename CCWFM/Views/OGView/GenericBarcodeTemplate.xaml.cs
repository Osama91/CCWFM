using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.Helpers.InputValidators;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView
{
    public partial class GenericBarcodeTemplate : UserControl, INotifyPropertyChanged
   {      
        private string PropertyTypeField;

        private int? barcodePropertiesIserial;

     

        public Point clickPosition;
        public bool isDragging;

        private Nullable<bool> BoldPropertyField;

        private double CanvasLeftField;

        private double CanvasTopField;

        private string CodeField;

        private Nullable<double> FontSizeField;

        private Nullable<bool> ItalicPropertyField;

        private string PropertyNameField;

        private string FontFamilyField;

        private string PropertyNameArabicField;

        private Visibility itemsvisiblity;

        public string FontFamilyProp
        {
            get
            {
                return FontFamilyField;
            }
            set
            {
                if ((ReferenceEquals(FontFamilyField, value) != true))
                {
                    FontFamilyField = value;
                    RaisePropertyChanged("FontFamilyProp");
                }
            }
        }

        public Nullable<bool> BoldProperty
        {
            get
            {
                return BoldPropertyField;
            }
            set
            {
                if ((BoldPropertyField.Equals(value) != true))
                {
                    BoldPropertyField = value;
                    RaisePropertyChanged("BoldProperty");
                }
            }
        }

        public Visibility Itemsvisiblity
        {
            get
            {
                return itemsvisiblity;
            }
            set
            {
                if ((itemsvisiblity.Equals(value) != true))
                {
                    itemsvisiblity = value;
                    RaisePropertyChanged("Itemsvisiblity");
                }
            }
        }

        public double CanvasLeftPropperty
        {
            get
            {
                return CanvasLeftField;
            }
            set
            {
                if ((CanvasLeftField.Equals(value) != true))
                {
                    CanvasLeftField = value;
                    RaisePropertyChanged("CanvasLeftPropperty");
                }
            }
        }

        public double CanvasTopPropperty
        {
            get
            {
                return CanvasTopField;
            }
            set
            {
                if ((CanvasTopField.Equals(value) != true))
                {
                    CanvasTopField = value;
                    RaisePropertyChanged("CanvasTopPropperty");
                }
            }
        }

        public string Code
        {
            get
            {
                return CodeField;
            }
            set
            {
                if ((ReferenceEquals(CodeField, value) != true))
                {
                    CodeField = value;
                    RaisePropertyChanged("Code");
                }
            }
        }

        public Nullable<double> FontSizeProp
        {
            get
            {
                return FontSizeField;
            }
            set
            {
                if ((FontSizeField.Equals(value) != true))
                {
                    FontSizeField = value;
                    RaisePropertyChanged("FontSizeProp");
                }
            }
        }

        public Nullable<bool> ItalicProperty
        {
            get
            {
                return ItalicPropertyField;
            }
            set
            {
                if ((ItalicPropertyField.Equals(value) != true))
                {
                    ItalicPropertyField = value;
                    RaisePropertyChanged("ItalicProperty");
                }
            }
        }

        public string PropertyName
        {
            get
            {
                return PropertyNameField;
            }
            set
            {
                if ((ReferenceEquals(PropertyNameField, value) != true))
                {
                    PropertyNameField = value;
                    RaisePropertyChanged("PropertyName");
                }
            }
        }

        public string PropertyNameArabic
        {
            get
            {
                return PropertyNameArabicField;
            }
            set
            {
                if ((ReferenceEquals(PropertyNameArabicField, value) != true))
                {
                    PropertyNameArabicField = value;
                    RaisePropertyChanged("PropertyNameArabic");
                }
            }
        }

        public string PropertyType
        {
            get
            {
                return PropertyTypeField;
            }
            set
            {
                if ((ReferenceEquals(PropertyTypeField, value) != true))
                {
                    PropertyTypeField = value;
                    RaisePropertyChanged("PropertyType");
                }
            }
        }

        public int? BarcodePropertiesIserial
        {
            get
            {
                return barcodePropertiesIserial;
            }
            set
            {
                if ((barcodePropertiesIserial.Equals(value) != true))
                {
                    barcodePropertiesIserial = value;
                    RaisePropertyChanged("BarcodePropertiesIserial");
                }
            }
        }

        public BarcodeSettingsUiViewModel _ViewModel;

        public GenericBarcodeTemplate(BarcodeSettingsUiViewModel ViewModel)
        {
            
            FontFamilyProp = "Arial";
            BoldProperty = false;
            ItalicProperty = false;
            FontSizeProp = 12;
            Itemsvisiblity = Visibility.Collapsed;
            InitializeComponent();

            _ViewModel = ViewModel;
            DataContext = this;
            MouseLeftButtonDown += Control_MouseLeftButtonDown;
            MouseLeftButtonUp += Control_MouseLeftButtonUp;
            MouseMove += Control_MouseMove; 
        }

        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            var draggableControl = sender as UserControl;
            clickPosition = e.GetPosition(this);
            draggableControl.CaptureMouse();
        }

        private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            var draggable = sender as UserControl;
            draggable.ReleaseMouseCapture();
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            var draggableControl = sender as UserControl;

            if (isDragging && draggableControl != null)
            {
                Point currentPosition = e.GetPosition(Parent as UIElement);

                CanvasLeftPropperty = currentPosition.X;
                CanvasTopPropperty = currentPosition.Y;
            }
        }

        private void PropertyName_TextChanged(object sender, TextChangedEventArgs e)
        {
            (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            NumValidations.validateTextDouble(sender, e);
            (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void GenericBarcideTemplate_GotFocus(object sender, RoutedEventArgs e)
        {
            _ViewModel.SelectedTemplate = this as GenericBarcodeTemplate;
            Itemsvisiblity = Visibility.Visible;
        }

        private void GenericBarcideTemplate_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IsRelated(LayoutRoot, FocusManager.GetFocusedElement()))
            {
                Itemsvisiblity = Visibility.Collapsed;
            }
        }

        public bool IsRelated(Grid Parent, object Child)
        {
            FrameworkElement test = Child as FrameworkElement;
            if (test != null)
            {
                if (test.Parent == Parent || test.Parent == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }
    }
}