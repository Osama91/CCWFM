using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CCWFM.ViewModel.FabricToolsViewModels;
using _Proxy = CCWFM.CRUDManagerService;

namespace CCWFM.Views.FabricTools
{
    public partial class FabTool_SearchResultChild
    {
        public event EventHandler SubmitAction;

        public string FabricCode { get; set; }

        public FabTool_SearchResultChild()
        {
            InitializeComponent();
        }

        public bool IsSearchingToDye { get; set; }

        public void SearchFabricAttrs(FabricSetupsViewModel _SearchCriteria)
        {
            var client = new _Proxy.CRUD_ManagerServiceClient();
            client.GetFabAttributesByCategoryCompleted += (s, e) =>
            {
                switch (_SearchCriteria.FabricCategoryID)
                {
                    case 2:
                    case 3:
                        FabAttrResultDataGrid.ItemsSource =
                            (from x in e.Result.ToList()
                             select
                             new
                             {
                                 x.FabricID
                                 ,
                                 x.FabricDescription
                                 ,
                                 x.FabricCategoryName
                                 ,
                                 x.UoM
                                 ,
                                 x.WidthAsRaw
                                 ,
                                 x.WeightPerSquarMeterAsRaw
                                 ,
                                 x.ExpectedDyingLossMargin
                                 ,
                                 x.Notes
                             });
                        break;

                    case 4:
                    case 5:
                        FabAttrResultDataGrid.ItemsSource = (from x in e.Result.ToList()
                                                             select
                                                             new
                                                             {
                                                                 x.FabricID
                                                                 ,
                                                                 x.FabricDescription
                                                                 ,
                                                                 x.FabricCategoryName
                                                                 ,
                                                                 x.UoM
                                                                 ,
                                                                 x.HorizontalShrinkage
                                                                 ,
                                                                 x.VerticalShrinkage
                                                                 ,
                                                                 x.WeightPerSquarMeterAfterWash
                                                                 ,
                                                                 x.WeightPerSquarMeterBeforWash
                                                                 ,
                                                                 x.Twist
                                                                 ,
                                                                 x.Notes
                                                             }).ToList();
                        break;

                    case 1:
                        FabAttrResultDataGrid.ItemsSource = (from x in e.Result.ToList()
                                                             select
                                                             new
                                                             {
                                                                 x.FabricID
                                                                 ,
                                                                 x.FabricDescription
                                                                 ,
                                                                 x.FabricCategoryName
                                                                 ,
                                                                 x.UoM
                                                                 ,
                                                                 x.Notes
                                                             }).ToList();
                        break;
                }
            };
            client.GetFabAttributesByCategoryAsync(_SearchCriteria.FabricCategoryID);
        }

        public void SearchFabricAttrs(FabricSetupsWFViewModel _SearchCriteria)
        {
            var client = new _Proxy.CRUD_ManagerServiceClient();
            client.GetFabAttributesByCategoryCompleted += (s, e) =>
            {
                switch (_SearchCriteria.FabricCategoryID)
                {
                    case 2:
                    case 3:
                        FabAttrResultDataGrid.ItemsSource =
                            (from x in e.Result.ToList()
                             select
                             new
                             {
                                 x.FabricID
                                 ,
                                 x.FabricDescription
                                 ,
                                 x.FabricCategoryName
                                 ,
                                 x.UoM
                                 ,
                                 x.WidthAsRaw
                                 ,
                                 x.WeightPerSquarMeterAsRaw
                                 ,
                                 x.ExpectedDyingLossMargin
                                 ,
                                 x.Notes
                             });
                        break;

                    case 4:
                    case 5:
                        FabAttrResultDataGrid.ItemsSource = (from x in e.Result.ToList()
                                                             select
                                                             new
                                                             {
                                                                 x.FabricID
                                                                 ,
                                                                 x.FabricDescription
                                                                 ,
                                                                 x.FabricCategoryName
                                                                 ,
                                                                 x.UoM
                                                                 ,
                                                                 x.HorizontalShrinkage
                                                                 ,
                                                                 x.VerticalShrinkage
                                                                 ,
                                                                 x.WeightPerSquarMeterAfterWash
                                                                 ,
                                                                 x.WeightPerSquarMeterBeforWash
                                                                 ,
                                                                 x.Twist
                                                                 ,
                                                                 x.Notes
                                                             }).ToList();
                        break;

                    case 1:
                        FabAttrResultDataGrid.ItemsSource = (from x in e.Result.ToList()
                                                             select
                                                             new
                                                             {
                                                                 x.FabricID
                                                                 ,
                                                                 x.FabricDescription
                                                                 ,
                                                                 x.FabricCategoryName
                                                                 ,
                                                                 x.UoM
                                                                 ,
                                                                 x.Notes
                                                             }).ToList();
                        break;
                }
            };
            client.GetFabAttributesByCategoryAsync(_SearchCriteria.FabricCategoryID);
        }


        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FabAttrResultDataGrid.SelectedItems.Count > 0)
                {
                    if (SubmitAction != null)
                    {
                        var txtBlock = (TextBlock)FabAttrResultDataGrid.Columns[0]
                            .GetCellContent(FabAttrResultDataGrid.SelectedItem);
                        FabricCode = txtBlock.Text;
                        DialogResult = true;
                        SubmitAction(this, new EventArgs());
                    }
                }
                else
                {
                    MessageBox.Show("You must select a record!");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}