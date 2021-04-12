//using System.ComponentModel;

//namespace Os.Controls.DataGrid
//{
//    public class CheckboxComboItem : INotifyPropertyChanged
//    {
//        private bool _isChecked;

//        public bool IsChecked
//        {
//            get { return _isChecked; }
//            set
//            {
//                if (_isChecked != value)
//                {
//                    _isChecked = value;
//                    OnPropertChanged("IsChecked");
//                }
//            }
//        }

//        private string _description;

//        public string Description
//        {
//            get { return _description; }
//            set
//            {
//                if (_description != value)
//                {
//                    _description = value;
//                    OnPropertChanged("Description");
//                }
//            }
//        }

//        private object _tag;

//        public object Tag
//        {
//            get { return _tag; }
//            set
//            {
//                if (_tag != value)
//                {
//                    _tag = value;
//                    OnPropertChanged("Tag");
//                }
//            }
//        }

//        public override string ToString()
//        {
//            return Description;
//        }

//        private void OnPropertChanged(string propertyName)
//        {
//            if (PropertyChanged != null)
//                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
//        }

//        public event PropertyChangedEventHandler PropertyChanged;
//    }
//}