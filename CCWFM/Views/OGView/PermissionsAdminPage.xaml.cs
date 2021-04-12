using System;
using System.Windows;
using System.Windows.Controls;
using CCWFM.ViewModel.OGViewModels;
using CCWFM.Views.OGView.ChildWindows;

namespace CCWFM.Views.OGView
{
    public partial class PermissionsAdminPage
    {
        private readonly PermissionsPageViewModel _viewModel;

        public PermissionsAdminPage()
        {
            InitializeComponent();
            DataContext = _viewModel;
            _viewModel = (PermissionsPageViewModel)LayoutRoot.DataContext;
        }

        
        private void BtnNewJob_OnClick(object sender, RoutedEventArgs e)
        {
            var child = new JobChildWindow();
            child.Show();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SavePerm();
        }


        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CopyPerm(Convert.ToInt32( CmbJobs.SelectedValue));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cmb_Jobs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TreeViewPremissions.

            for (var i = 0; i < TreeViewPremissions.Items.Count; i++)
            {
                ExpandAllTreeViewItems((TreeViewItem)TreeViewPremissions.ItemContainerGenerator.ContainerFromIndex(i));
            }

            _viewModel.GetPermissionForJob(Convert.ToInt32(CmbJobs.SelectedValue), TreeViewPremissions);
        }

        private void ExpandAllTreeViewItems(TreeViewItem currentTreeViewItem)
        {
            if (currentTreeViewItem != null && !currentTreeViewItem.IsExpanded)
            {
                currentTreeViewItem.IsExpanded = true;
                currentTreeViewItem.Dispatcher.BeginInvoke(() => ExpandAllTreeViewItems(currentTreeViewItem));
            }
            else
            {
                try
                {
                    for (var i = 0; i < currentTreeViewItem.Items.Count; i++)
                    {
                        var child = (TreeViewItem)currentTreeViewItem.ItemContainerGenerator.ContainerFromIndex(i);
                        ExpandAllTreeViewItems(child);
                    }
                }
                catch (Exception)
                {

                    
                }
                
            }
        }
    }
}