using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CCWFM.ViewModel.OGViewModels;

namespace CCWFM.Views.OGView.ChildWindows
{
    public partial class MarkerCalculationsChildWindow
    {
        private readonly MarkerViewModel _viewModel;

        public MarkerCalculationsChildWindow(MarkerViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = viewModel;
            viewModel.GetSalesOrderColor();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CbColorCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;
            if (combo != null)
            {
                var color = Convert.ToInt32(combo.SelectedValue);
                if (color==0)
                {
                    return;
                }
                _viewModel.SalesOrderColor.Where(x => x.TblColor == color);
                _viewModel.CalcList = new ObservableCollection<TblMarkerTempViewModel>();
                var templist = _viewModel.SalesOrderColor.Where(x => x.TblColor == color).Select(w => w.TblSalesOrderSizeRatios);

                var list = new ObservableCollection<TblMarkerTempViewModel>();
                foreach (var variable in templist)
                {
                    foreach (var row in variable)
                    {
                        var visible = Visibility.Collapsed;

                        if (row == variable.FirstOrDefault())
                        {
                            visible = Visibility.Visible;
                        }
                        list.Add(new TblMarkerTempViewModel
                        {
                            Size = row.Size,
                            Ratio = row.RatioForProduction,
                            Production = row.ProductionPerSizeForProduction,
                            Rem = row.ProductionPerSizeForProduction,
                            SizeVisible = visible
                        });
                    }
                }
                var smallestno = (int)list.Where(x => x.Production > 0).Select(x => x.Production / x.Ratio).OrderBy(x => x).FirstOrDefault();
                _viewModel.CalcList.Add(new TblMarkerTempViewModel
                {
                    MarkerNo = "Production Ratio",
                    NoOfLayersOrg = smallestno,
                    DetailsList = list,
                    TblColor = color,
                    
                });
                var listofproduction = list;
                var temp = _viewModel.MarkerHeader.MarkerListViewModelList.Where(x => x.StyleColorCode == color).Select(w => w.MarkerNo).OrderBy(x => x).Distinct();
                foreach (var variable in temp.Where(x => x != null))
                {
                    var listTemp =
                        _viewModel.MarkerHeader.MarkerListViewModelList.FirstOrDefault(x => x.MarkerNo == variable)
                            .MeterPerSizeList;

                    var temptemp = listTemp.Where(x => x.MeterPerSizeValue > 0);
                    smallestno = (int)temptemp.Select(x => listofproduction.Where(w => w.Size == x.MeterPerSizeCode).FirstOrDefault().Rem / x.MeterPerSizeValue).OrderBy(x => x).FirstOrDefault(qq=>qq.Value>0);
                    var Newlist = new ObservableCollection<TblMarkerTempViewModel>();
                    foreach (var VARIABLE in listTemp)
                    {
                        var visible = Visibility.Collapsed;

                        if (VARIABLE == listTemp.FirstOrDefault())
                        {
                            visible = Visibility.Visible;
                        }
                        if (VARIABLE != null)
                        {
                            VARIABLE.SizeVisible = visible;
                            var production =
                                CalcProduction(
                                    smallestno, listTemp, VARIABLE.MeterPerSizeCode);
                            var rem = listofproduction.FirstOrDefault(x => x.Size == VARIABLE.MeterPerSizeCode).Rem = listofproduction.FirstOrDefault(x => x.Size == VARIABLE.MeterPerSizeCode).Rem - production;
                            VARIABLE.SizecodeId = (int)rem;
                            VARIABLE.Iserial = production;

                            Newlist.Add(new TblMarkerTempViewModel
                            {
                                Ratio = VARIABLE.MeterPerSizeValue,
                                Size = VARIABLE.MeterPerSizeCode,
                                Rem = rem,
                                NoOfLayers = smallestno,
                                NoOfLayersOrg = smallestno,
                                Production = production,
                                SizeVisible = visible,
                            });
                        }
                    }

                    var rooow = new TblMarkerTempViewModel
                    {
                        MarkerNo = variable,
                        NoOfLayersOrg = smallestno,
                        NoOfLayers = smallestno,
                        DetailsList = Newlist
                    };

                    _viewModel.CalcList.Add(rooow);
                }
                //   _viewModel.saveCuttingTemp()
            }
        }

        public int CalcProduction(int number, ObservableCollection<MeterPerSize> List, string size)
        {
            var no = List.FirstOrDefault(x => x.MeterPerSizeCode == size).MeterPerSizeValue;
            return (int)(no * number);
        }

        private int CalcProduction(int noOfLayers, ObservableCollection<TblMarkerTempViewModel> detailsList, string size)
        {
            var no = detailsList.FirstOrDefault(x => x.Size == size).Ratio;
            return (int)(no * noOfLayers);
        }

        private void BtnCalculate_OnClick(object sender, RoutedEventArgs e)
        {
            var firstrow = _viewModel.CalcList.FirstOrDefault(x => x.MarkerNo == "Production Ratio");

            if (firstrow != null)
            {
                foreach (var variable in firstrow.DetailsList)
                {
                    variable.Rem = variable.Production;
                }
                var calclist = _viewModel.CalcList.Where(x => x.MarkerNo != "Production Ratio").ToList();
                var productionlist = new Dictionary<string, int>();
                foreach (var parentRow in calclist)
                {
                    foreach (var row in parentRow.DetailsList.ToList())
                    {
                        row.Production = 0;
                    }

                    foreach (var row in parentRow.DetailsList.ToList())
                    {
                        var total = firstrow.DetailsList.FirstOrDefault(x => x.Size == row.Size).Production;

                        var production =
                            CalcProduction(parentRow.NoOfLayers, parentRow.DetailsList, row.Size);

                        if (!productionlist.ContainsKey(row.Size))
                        {
                            productionlist.Add(row.Size, production);
                        }
                        else
                        {
                            productionlist[row.Size] = productionlist.FirstOrDefault(x => x.Key == row.Size).Value +
                                                       production;
                        }
                        parentRow.DetailsList.FirstOrDefault(x => x.Size == row.Size).Production = production;
                        var rem = total - productionlist[row.Size];
                        row.Rem = (int)rem;
                    }
                }
                foreach (var variable in firstrow.DetailsList)
                {
                    var total = variable.Production;
                    var rem = total - productionlist[variable.Size];
                    variable.Rem = (int)rem;
                }
                _viewModel.SaveCuttingTemp();
            }
        }
    }
}