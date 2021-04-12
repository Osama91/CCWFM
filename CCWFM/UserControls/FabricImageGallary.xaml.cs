using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CCWFM.Helpers.Enums;
using CCWFM.ViewModel.FabricToolsViewModels;
using CCWFM.Views.ImageGallary;

namespace CCWFM.UserControls
{
    public partial class FabricImageGallary : INotifyPropertyChanged
    {
        #region Implement INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Implement INotifyPropertyChanged

        private AccAndFabricsImageGallrySearch _searchWindow;

        public AccAndFabricsImageGallrySearch SearchWindow
        {
            get { return _searchWindow; }
            set { _searchWindow = value; RaisePropertyChanged("SearchWindow"); }
        }

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

        public FabricImageGallary()
        {
            InitializeComponent();
            ImageCollectionViewModel = new GImageViewModelCollection();
            LayoutRoot.DataContext = ImageCollectionViewModel;
            SearchWindow = new AccAndFabricsImageGallrySearch();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Image Files (*.png, *.jpg, *.jpeg)|*.png;*.jpg;*.jpeg|All Files (*.*)|*.*",
                Multiselect = true
            };
            if (dlg.ShowDialog() == true)
            {
                if (dlg.Files.Any(x => x.Length > 1048576))
                {
                    MessageBox.Show("I have detected that one or more of the uploaded Images exceeds the allowed size which is 1 Megabyte...\nPlease note that any image larger than 1 MB will not be added to the galary!");
                }
                foreach (var item in dlg.Files)
                {
                    if (dlg.Files.Any())
                    {
                        if (item.Length <= 1048576)
                        {
                            var reader = item.OpenRead();
                            var byteArray = new byte[reader.Length];
                            reader.Read(byteArray, 0, Convert.ToInt32(reader.Length));
                            RaisePropertyChanged("ImageViewModel");
                            ImageCollectionViewModel
                                .ImageCollection
                                .Add
                                (new GImageViewModel(ImageCollectionViewModel.FabricCode)
                                    {
                                        G_Image = byteArray
                                        ,
                                        _ImageState = ImageCondition.NewAdded
                                    }
                                );
                            ImageCollectionViewModel.MyProperty
                                += Math.Round((item.Length / 1024.00) / 1024.00, 1);
                            ImageCollectionViewModel.RaisePropertyChanged("MyProperty");
                        }
                    }
                }
            }
        }

        private void btnAddGalaryImage_Click(object sender, RoutedEventArgs e)
        {
            btnBrowse.Visibility = Visibility.Visible;
            ImageCollectionViewModel = new GImageViewModelCollection();
            LayoutRoot.DataContext = ImageCollectionViewModel;
        }

        private void btnSaveGalary_Click(object sender, RoutedEventArgs e)
        {
            if (cmbFabric.SelectedValue != null)
            {
                ImageCollectionViewModel.SaveImages();
            }
            else
            {
                MessageBox.Show("You must select a fabric!");
            }
            if (lstImageThumbs.Items.Count < 1)
            {
                MessageBox.Show("No Images to save!");
            }
        }

        private void lblImageDesc_TextChanged(object sender, TextChangedEventArgs e)
        {
            (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchWindow.Show();
        }
    }
}