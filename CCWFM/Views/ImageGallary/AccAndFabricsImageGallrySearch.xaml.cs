using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CCWFM.ViewModel.FabricToolsViewModels;

namespace CCWFM.Views.ImageGallary
{
    public partial class AccAndFabricsImageGallrySearch : INotifyPropertyChanged
    {
        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        private GImageViewModelCollection _imageCollectionViewModel;

        public GImageViewModelCollection ImageCollectionViewModel
        {
            get { return _imageCollectionViewModel; }
            set
            {
                _imageCollectionViewModel = value;
                RaisePropertyChanged("ImageCollectionViewModel");
            }
        }

        public event EventHandler SubmitResult;

        public AccAndFabricsImageGallrySearch()
        {
            InitializeComponent();
            ImageCollectionViewModel = new GImageViewModelCollection();
            SubLayoutRoot.DataContext = ImageCollectionViewModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (SubmitResult == null) return;
            DialogResult = true;
            SubmitResult(this, new EventArgs());
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void cmbFabric_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ImageCollectionViewModel.LoadImages(cmbFabric.SelectedValue.ToString());
        }

        private void ChildWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
            }
        }
    }
}