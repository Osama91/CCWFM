using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SilverlightMenu.Library
{
    [ContentProperty("MenuItems")]
    public class MenuItem : ObservableCollection<MenuItem>
    {
        #region attributes

        private bool _isEnabled = true;
        private bool _isCheckable = false;
        private bool _isChecked = false;
        private int _level = 0;

        #endregion attributes

        #region Events

        #endregion Events

        #region Properties

        public IList<MenuItem> MenuItems
        {
            get { return Items; }
            set
            {
                foreach (MenuItem item in value)
                {
                    Items.Add(item);
                }
            }
        }

        public string MenuLink { get; set; }

        public string Name { get; set; }

        public string PermissionTyp { get; set; }

        public string ParentName { get; set; }

        public string Text { get; set; }

        public string Tag { get; set; }

        public string ImagePath { get; set; }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        public bool IsCheckable
        {
            get { return _isCheckable; }
            set { _isCheckable = value; }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; }
        }

        public Grid MenuGrid { get; set; }

        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return Name;
        }

        #endregion Methods
    }
}