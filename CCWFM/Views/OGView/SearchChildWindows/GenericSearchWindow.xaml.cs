using CCWFM.ViewModel.GenericViewModels;
using CCWFM.ViewModel.OGViewModels.ControlsOverride;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace CCWFM.Views.OGView.SearchChildWindows
{
    public partial class GenericSearchWindow : ChildWindowsOverride
    {
        public bool Loading { get; set; }

        public int FullCount { get; set; }

        public GenericSearchWindow(ObservableCollection<SearchColumnModel> SearchList)
        {
            InitializeComponent();
            // ده هيكون بدل تعبئة الخصائص لكن لازم تكون الداتا متوفرة فى الموديل باى شكل على الاغلب Attributes
            //  MainRowList.Search(t => new List<SearchColumnModel>(){
            //new SearchColumnModel() { PropertyPath = nameof(t.CodeFrom) }, new SearchColumnModel()});
            foreach (var item in SearchList)
            {
                MainGrid.Columns.Add(new DataGridTextColumn()
                {
                    Header = item.Header,
                    Binding = new System.Windows.Data.Binding(new System.Windows.Data.Binding()
                    {
                        Path = new System.Windows.PropertyPath(item.PropertyPath),
                        StringFormat = item.StringFormat,
                    }),
                    SortMemberPath = item.FilterPropertyPath,
                });
            }
        }
        
        private void MainGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var vm = DataContext as GenericSearchViewModelBase;
            if (vm.ItemsListCount < vm.PageSize)
            {
                return;
            }
            if (vm.ItemsListCount - 2 < e.Row.GetIndex() && !Loading && vm.ItemsListCount < FullCount)
            {
                Loading = true;
                if (vm.GetDataCommand != null)
                    if (vm.GetDataCommand.CanExecute(this))
                        vm.GetDataCommand.Execute(this);
            }
        }

        private void MainGrid_OnFilter(object sender, Os.Controls.DataGrid.Events.FilterEvent e)
        {
            var vm=DataContext as GenericSearchViewModelBase;
            if (vm != null)
                vm.OnFilter(e);          
        }

        private void DoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OkButton.Command != null)
                if (OkButton.Command.CanExecute(this))
                    OkButton.Command.Execute(this);
        }
    }
}
