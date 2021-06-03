using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CCWFM.GlService;
using CCWFM.Helpers.Enums;
using CCWFM.UserControls;
using CCWFM.ViewModel.Gl;
using CCWFM.Views;
using CCWFM.Views.AccessoryTools;
using CCWFM.Views.AttView;
using CCWFM.Views.DataSettingsForms;
using CCWFM.Views.FabricTools;
using CCWFM.Views.Gl;
using CCWFM.Views.OGView;
using CCWFM.Views.OGView.ChildWindows;
using CCWFM.Views.PrintPreviews;
using Os.Controls.DataGrid.Events;
using GlPosting = CCWFM.Views.Gl.GlPosting;

namespace CCWFM.Helpers.AuthenticationHelpers
{
    public static class GeneralFilter
    {
        public static void GeneralFilterMethod(out string Filter, out Dictionary<string, object> ValuesObjects, FilterEvent e)
        {
            var counter = 0;
            Filter = null;

            ValuesObjects = new Dictionary<string, object>();

            var filterRow =
                e.FiltersPredicate.FirstOrDefault(x => x.FilterColumnInfo.PropertyPath == "Tbl_fabricInspectionDetail.BatchNo");

            if (e.FiltersPredicate.Any(x => x.FilterColumnInfo.PropertyPath == "Tbl_fabricInspectionDetail.BatchNo"))
            {
                ValuesObjects.Add("BatchNo" + counter, filterRow.FilterText);
            }

            var filterFabricColorRow =
    e.FiltersPredicate.FirstOrDefault(x => x.FilterColumnInfo.PropertyPath == "Tbl_fabricInspectionDetail.ColorCode");

            if (e.FiltersPredicate.Any(x => x.FilterColumnInfo.PropertyPath == "Tbl_fabricInspectionDetail.ColorCode"))
            {
                ValuesObjects.Add("ColorCode" + counter, filterFabricColorRow.FilterText);
            }


            var filterStyleTnaStatusRow =
             e.FiltersPredicate.FirstOrDefault(x => x.FilterColumnInfo.PropertyPath == "TblStyleTNAHeaders.TblStyleTNAStatu.Iserial");

            if (e.FiltersPredicate.Any(x => x.FilterColumnInfo.PropertyPath == "TblStyleTNAHeaders.TblStyleTNAStatu.Iserial"))
            {
                ValuesObjects.Add("TblStyleTNAStatus" + counter, filterStyleTnaStatusRow.FilterText);
            }

            
            foreach (var f in e.FiltersPredicate.Where((x => x.FilterColumnInfo.PropertyPath != "Tbl_fabricInspectionDetail.ColorCode" && x.FilterColumnInfo.PropertyPath != "Tbl_fabricInspectionDetail.BatchNo"&&x.FilterColumnInfo.PropertyPath!= "TblStyleTNAHeaders.TblStyleTNAStatu.Iserial")))
            {
                string paramter = "(@" + f.FilterColumnInfo.PropertyPath.Replace(".", "_") + counter + ")";
                object myObject = null;
                try
                {
                    if (f.FilterColumnInfo.PropertyType == typeof(DateTime?) || f.FilterColumnInfo.PropertyType == typeof(DateTime))
                    {
                        f.FilterColumnInfo.PropertyType = typeof(DateTime);
                        myObject = f.SelectedDate;
                    }
                    else
                        myObject = Convert.ChangeType(f.FilterText, f.FilterColumnInfo.PropertyType, null);
                }
                catch (Exception)
                {
                    myObject = "";
                }
                switch (f.SelectedFilterOperation.FilterOption)
                {
                    case Os.Controls.DataGrid.Enums.FilterOperation.EndsWith:
                        myObject = "%" + f.FilterText;
                        break;

                    case Os.Controls.DataGrid.Enums.FilterOperation.StartsWith:
                        myObject = f.FilterText + "%";
                        break;

                    case Os.Controls.DataGrid.Enums.FilterOperation.Contains:
                        myObject = "%" + f.FilterText + "%";
                        break;
                }

                ValuesObjects.Add(f.FilterColumnInfo.PropertyPath.Replace(".", "_") + counter, myObject);

                if (counter > 0)
                {
                    Filter = Filter + " and ";
                }

                Filter = Filter + "it." + f.FilterColumnInfo.PropertyPath +
                                       f.SelectedFilterOperation.LinqUse + paramter;

                counter++;
            }
        }

        public static void NavigatToMenu(string menuName, string title, string menuLink)
        {
            var child = new FrameChildWindow();

            if (menuName == PermissionItemName.SeasonCurrenciesForm.ToString())
            {
                child.LayoutRoot.Children.Add(new SeasonCurrencies());
            }
            if (menuName == PermissionItemName.PurchaseBudget.ToString())
            {
                child.LayoutRoot.Children.Add(new PurchaseBudget());
            }
                
            if (menuName == PermissionItemName.VariableTermManual.ToString())
            {
                child.LayoutRoot.Children.Add(new VariableTermManual());
            }
            if (menuName == PermissionItemName.DyeingPlanForm.ToString())
            {
                child.LayoutRoot.Children.Add(new DyeingPlan());
            }
            if (menuName == PermissionItemName.IncomeStatment.ToString())
            {
                child.LayoutRoot.Children.Add(new IncomeStatmentPrintPreview(new IncomeStatmentViewModel()));
            }

            if (menuName == PermissionItemName.FactoryEmpLeavingTransaction.ToString())
            {
                child.LayoutRoot.Children.Add(new BehalfFiltered());
            }

            if (menuName == PermissionItemName.GlGenEntity.ToString())
            {
                child.LayoutRoot.Children.Add(new GlGenEntity());
            }

            if (menuName == PermissionItemName.GeneratePurchase.ToString())
            {
                child.LayoutRoot.Children.Add(new GeneratePurchase());
            }
            if (menuName == PermissionItemName.EmpLeavingTransactionForm.ToString())
            {
                child.LayoutRoot.Children.Add(new EmpLeavingTransaction());
            }
            if (menuName == PermissionItemName.EmpLeavingTransactionForManagment.ToString())
            {
                child.LayoutRoot.Children.Add(new EmpLeavingTransactioForManagment());
            }
            if (menuName == PermissionItemName.SalesOrderRequestInvoice.ToString())
            {
                child.LayoutRoot.Children.Add(new SalesOrderRequestInvoice());
            }
            
            if (menuName == PermissionItemName.RouteCardInvoice.ToString())
            {
                child.LayoutRoot.Children.Add(new RouteCardInvoice());
            }

            if (menuName == PermissionItemName.EmpWeeklyDayOff.ToString())
            {
                child.LayoutRoot.Children.Add(new EmpWeeklyDayOff());
            }

            if (menuName == PermissionItemName.NewUserRequestForm.ToString())
            {
                child.LayoutRoot.Children.Add(new CreateNewUser());
            }
            if (menuName == PermissionItemName.ConfirmNewUserRequestForm.ToString())
            {
                child.LayoutRoot.Children.Add(new ConfirmNewUserRequest());
            }

            if (menuName == PermissionItemName.StyleFabricComposition.ToString())
            {
                child.LayoutRoot.Children.Add(new GenericForm("TblStyleFabricComposition", "", PermissionItemName.StyleFabricComposition));
            }

            if (menuName == PermissionItemName.BrandSectionMail.ToString())
            {
                child.LayoutRoot.Children.Add(new BrandSectionMail());
            }

            if (menuName == PermissionItemName.RecInvProd.ToString())
            {
                child.LayoutRoot.Children.Add(new RecInvProd());
            }
            if (menuName == PermissionItemName.PeriodLock.ToString())
            {
                child.LayoutRoot.Children.Add(new PeriodLock());
            }

            
            if (menuName == PermissionItemName.BrandSectionMailSample.ToString())
            {
                child.LayoutRoot.Children.Add(new BrandSectionMailSample());
            }

            if (menuName == PermissionItemName.StandardBomForm.ToString())
            {
                child.LayoutRoot.Children.Add(new StandardBom());
            }
            if (menuName == PermissionItemName.AttendanceFileReason.ToString())
            {
                child.LayoutRoot.Children.Add(new AttendanceFileReason());
            }

            if (menuName == PermissionItemName.RouteCoding.ToString())
            {
                child.LayoutRoot.Children.Add(new RouteCoding());
            }
            if (menuName == PermissionItemName.FingerPrintTransaction.ToString())
            {
                child.LayoutRoot.Children.Add(new FingerPrintTransaction());
            }
            if (menuName == PermissionItemName.TransferMsg.ToString())
            {
                child.LayoutRoot.Children.Add(new TransferMsg());
            }
            if (menuName == PermissionItemName.AssetsStatus.ToString())
            {
                child.LayoutRoot.Children.Add(new GenericForm("TblAssetsStatus", "", PermissionItemName.AssetsStatus));
            }

            if (menuName == PermissionItemName.Reservation.ToString())
            {
                child.LayoutRoot.Children.Add(new Reservation());
            }

            if (menuName == PermissionItemName.DyeingPlanAccForm.ToString())
            {
                child.LayoutRoot.Children.Add(new DyeingPlanAcc());
            }

            if (menuName == PermissionItemName.GlobalRetailBusinessBudget.ToString())
            {
                child.LayoutRoot.Children.Add(new GlobalBudget(1));//RetailBudget
            }

            if (menuName == PermissionItemName.GlobalCCBusinessBudget.ToString())
            {
                child.LayoutRoot.Children.Add(new GlobalBudget(2));//CCBudget
            }

            if (menuName == PermissionItemName.CCBrandBudget.ToString())
            {
                child.LayoutRoot.Children.Add(new BrandBudget(2));//CCBudget
            }

            if (menuName == PermissionItemName.ClosingAdvanceVendorPayment.ToString())
            {
                child.LayoutRoot.Children.Add(new ClosingAdvanceVendorPayment());//CCBudget
            }


            if (menuName == PermissionItemName.AssetsType.ToString())
            {
                child.LayoutRoot.Children.Add(new GenericForm("TblAssetsType", "", PermissionItemName.AssetsType));
            }
            if (menuName == PermissionItemName.AssetsTransaction.ToString())
            {
                child.LayoutRoot.Children.Add(new AssetsTransaction());
            }
            if (menuName == PermissionItemName.VariableTermManualFactory.ToString())
            {
                child.LayoutRoot.Children.Add(new VariableTermManualFactory());
            }


            if (menuName == PermissionItemName.Depreciation.ToString())
            {
                child.LayoutRoot.Children.Add(new Depreciation());
            }
            if (menuName == PermissionItemName.ShopReqHeader.ToString())
            {
                child.LayoutRoot.Children.Add(new ShopReqHeader());
            }

            if (menuName == PermissionItemName.Processor.ToString())
            {
                child.LayoutRoot.Children.Add(new GenericForm("TblProcessor", "", PermissionItemName.Processor));
            }
            if (menuName == PermissionItemName.HardDisk.ToString())
            {
                child.LayoutRoot.Children.Add(new GenericForm("TblHardDisk", "", PermissionItemName.HardDisk));
            }
            if (menuName == PermissionItemName.Memory.ToString())
            {
                child.LayoutRoot.Children.Add(new GenericForm("TblMemory", "", PermissionItemName.Memory));
            }

            if (menuName == PermissionItemName.AssetsForm.ToString())
            {
                child.LayoutRoot.Children.Add(new Assets());
            }

            if (menuName == PermissionItemName.FixAtt.ToString())
            {
               //child.LayoutRoot.Children.Add(new SalaryTerms());
                child.LayoutRoot.Children.Add(new FixAtt());
            }

            
            if (menuName == PermissionItemName.DyeingOrderInvoice.ToString())
            {
                child.LayoutRoot.Children.Add(new DyeingOrderInvoice());
            }


            if (menuName == PermissionItemName.Gps.ToString())
            {
                const string url = "http://192.168.1.23:251/home/users";
                var absoluteUri = new Uri(url, UriKind.Absolute);
                System.Windows.Browser.HtmlPage.Window.Navigate(absoluteUri, "_blank");
                return;
            }
            if (menuName == PermissionItemName.MissionTracker.ToString())
            {
                const string url = "http://192.168.1.23:251/home/MissionTracking";
                var absoluteUri = new Uri(url, UriKind.Absolute);
                System.Windows.Browser.HtmlPage.Window.Navigate(absoluteUri, "_blank");
                return;
            }

            if (menuName == PermissionItemName.StyleCategory.ToString())
            {
                child.LayoutRoot.Children.Add(
                    new GenericForm
                        ("TblStyleCategory"

                            , PermissionItemName.StyleCategory
                        ));
            }
            if (menuName == PermissionItemName.BrandSectionPermissionForm.ToString())
            {
                child.LayoutRoot.Children.Add(new BrandSectionPermission(Convert.ToInt32(LoggedUserInfo.Iserial)));
            }
            else if (menuName == PermissionItemName.RouteCardForm.ToString())
            {
                child.LayoutRoot.Children.Add(new RouteCard());
            }
            else if (menuName == PermissionItemName.FabricInspectionForm.ToString())
            {
                child.LayoutRoot.Children.Add(new FabricDefects());
            }
            else if (menuName == PermissionItemName.BankDepositApproval.ToString())
            {
                child.LayoutRoot.Children.Add(new BankDepositApproval());
            }

            else if (menuName == PermissionItemName.MarkerDetailsForm.ToString())
            {
                child.LayoutRoot.Children.Add(new MarkersView());
            }
            else if (menuName == PermissionItemName.SizesForm.ToString())
            {
                child.LayoutRoot.Children.Add(new SizeGroup());
            }
            else if (menuName == PermissionItemName.ColorsForm.ToString())
            {
              
                
               child.LayoutRoot.Children.Add(new Color());
            }
            else if (menuName == PermissionItemName.TransferForm.ToString())
            {
                child.LayoutRoot.Children.Add(new TransferView(true));
            }
            else if (menuName == PermissionItemName.TransferTo.ToString())
            {
                child.LayoutRoot.Children.Add(new TransferView(false));
            }
            else if (menuName == PermissionItemName.Adjustment.ToString())
            {
                child.LayoutRoot.Children.Add(new AdjustmentView(false));
            }
            else if (menuName == PermissionItemName.OpeningBalance.ToString())
            {
                child.LayoutRoot.Children.Add(new AdjustmentView(true));
            }
            //Commented To Work On Stitch
            //else if (menuName == PermissionItemName.Contracts.ToString())
            //{
            //    child.LayoutRoot.Children.Add(new ContractView());
            //}
            else if (menuName == PermissionItemName.BankStatement.ToString())
            {
                child.LayoutRoot.Children.Add(new BankStatementView());
            }
            else if (menuName == PermissionItemName.CashDeposit.ToString())
            {
                child.LayoutRoot.Children.Add(new CashDepositView());
            }
            else if (menuName == PermissionItemName.FabricDesignsForm.ToString())
            {
                child.LayoutRoot.Children.Add(
                        new GenericForm
                                                ("Tbl_lkp_FabricDesignes"
                                                , ""
                                                , PermissionItemName.FabricDesignsForm
                                                )
                    );
            }
            else if (menuName == PermissionItemName.FabricFinishesForm.ToString())
            {
                child.LayoutRoot.Children.Add(
                        new GenericForm
                                                ("tbl_lkp_FabricFinish"
                                                , ""
                                                , PermissionItemName.FabricFinishesForm
                                                )
                    );
            }
            else if (menuName == PermissionItemName.FabricMaterialsForm.ToString())
            {
                child.LayoutRoot.Children.Add(
                        new GenericForm
                                                ("Tbl_lkp_FabricMaterials"
                                                , ""
                                                , PermissionItemName.FabricMaterialsForm
                                                )
                    );
            }

            else if (menuName == PermissionItemName.FabricStructuresForm.ToString())
            {
                child.LayoutRoot.Children.Add(new FabricStructure());
            }
            else if (menuName == PermissionItemName.FabricTypesForm.ToString())
            {
                child.LayoutRoot.Children.Add(
                        new GenericForm
                                                ("Tbl_lkp_FabricTypes"
                                                , ""
                                                , PermissionItemName.FabricTypesForm
                                                )
                    );
            }
            else if (menuName == PermissionItemName.YarnCountsForm.ToString())
            {
                child.LayoutRoot.Children.Add(
                        new GenericForm
                                                ("tbl_lkp_YarnCount"
                                                , ""
                                                , PermissionItemName.YarnCountsForm
                                                )
                    );
            }

            else if (menuName == PermissionItemName.YarnFinishesForm.ToString())
            {
                child.LayoutRoot.Children.Add(
                        new GenericForm
                                                ("tbl_lkp_YarnFinish"
                                                , ""
                                                , PermissionItemName.YarnFinishesForm
                                                )
                    );
            }
            else if (menuName == PermissionItemName.ContentsForm.ToString())
            {
                child.LayoutRoot.Children.Add(
                        new GenericForm
                                                ("Tbl_lkp_Contents"
                                                , ""
                                                , PermissionItemName.ContentsForm
                                                )
                    );
            }
            else if (menuName == PermissionItemName.GaugesForm.ToString())
            {
                child.LayoutRoot.Children.Add(
                        new GenericForm
                                                ("Tbl_lkp_Gauges"
                                                , ""
                                                , PermissionItemName.GaugesForm
                                                )
                    );
            }
            else if (menuName == PermissionItemName.StatusesForm.ToString())
            {
                child.LayoutRoot.Children.Add(
                        new GenericForm
                                                ("Tbl_lkp_Status"
                                                , ""
                                                , PermissionItemName.StatusesForm
                                                )
                    );
            }
            else if (menuName == PermissionItemName.UoMsForm.ToString())
            {
                child.LayoutRoot.Children.Add(
                        new GenericForm
                                                ("Tbl_lkp_UoM"
                                                , ""
                                                , PermissionItemName.UoMsForm
                                                )
                    );
            }
            else if (menuName == PermissionItemName.ThreadNumbersForm.ToString())
            {
                child.LayoutRoot.Children.Add(
                        new GenericForm
                                                ("Tbl_lkp_ThreadNumbers"
                                                , ""
                                                , PermissionItemName.ThreadNumbersForm
                                                )
                    );
            }
            else if (menuName == PermissionItemName.BarCodeSettingsForm.ToString())
            {
                child.LayoutRoot.Children.Add(new DisplayingBarcodeSetup());
            }
            else if (menuName == PermissionItemName.DefectsForm.ToString())
            {
                child.LayoutRoot.Children.Add(
                       new GenericForm
                                               ("tbl_WF_Defects", ""
                                               , PermissionItemName.DefectsForm)
                   );
            }
            else if (menuName == PermissionItemName.FabSetupForm.ToString())
            {
                child.LayoutRoot.Children.Add(new FabricSetups());
            }

            else if (menuName == PermissionItemName.FabSetupWFForm.ToString())
            {
                child.LayoutRoot.Children.Add(new FabricSetupsWF());
            }
            else if (menuName == PermissionItemName.YarnSourcesForm.ToString())
            {
                child.LayoutRoot.Children.Add(
                        new GenericForm
                                                ("tbl_lkp_YarnSource"
                                                , ""
                                                , PermissionItemName.YarnSourcesForm
                                                )
                    );
            }
            else if (menuName == PermissionItemName.Currencies.ToString())
            {
                child.LayoutRoot.Children.Add(
                        new GenericForm
                                                ("tbl_lkp_Currency"
                                                , ""
                                                , PermissionItemName.Currencies
                                                )
                    );
            }
            else if (menuName == PermissionItemName.CostTypes.ToString())
            {
                child.LayoutRoot.Children.Add(
                        new GenericForm
                                                ("tbl_lkp_CostTypes"
                                                , ""
                                                , PermissionItemName.CostTypes
                                                )
                    );
            }

            else if (menuName == PermissionItemName.CostCenter.ToString())
            {
                child.LayoutRoot.Children.Add(new CostCenter());
            }
            else if (menuName == PermissionItemName.ShopArea.ToString())
            {
                child.LayoutRoot.Children.Add(new ShopArea());
            }
            
            else if (menuName == PermissionItemName.CostDimSetup.ToString())
            {
                child.LayoutRoot.Children.Add(new CostDimSetup());
            }
            else if (menuName == PermissionItemName.UserJobsForm.ToString())
            {
                child.LayoutRoot.Children.Add(new UserJobs());
            }
            else if (menuName == PermissionItemName.UserBrandsForm.ToString())
            {
                child.LayoutRoot.Children.Add(new UserBrands());
            }
            else if (menuName == PermissionItemName.PermissionsForm.ToString())
            {
                child.LayoutRoot.Children.Add(new PermissionsAdminPage());
            }
            else if (menuName == PermissionItemName.SupplierFabric.ToString())
            {
                child.LayoutRoot.Children.Add(new SupplierFabric());
            }
            else if (menuName == PermissionItemName.StyleStatus.ToString())
            {
                child.LayoutRoot.Children.Add(new StyleStatus());
            }
            else if (menuName == PermissionItemName.FamilyForm.ToString())
            {
                child.LayoutRoot.Children.Add(new Family());
            }

            else if (menuName == PermissionItemName.ProductionOrder.ToString())
            {
                child.LayoutRoot.Children.Add(new ProductionOrder());
            }

            
            else if (menuName == PermissionItemName.PurchaseOrderRequest.ToString())
            {
                child.LayoutRoot.Children.Add(new PurchaseOrderRequest());
            }
            else if (menuName == PermissionItemName.SalesOrderRequest.ToString())
            {
                child.LayoutRoot.Children.Add(new SalesOrderRequest());
            }

            else if (menuName == PermissionItemName.GlExpensis.ToString())
            {
                child.LayoutRoot.Children.Add(new GlExpensis());
            }

            else if (menuName == PermissionItemName.SeasonForm.ToString())
            {
                child.LayoutRoot.Children.Add(new Season());
            }
            else if (menuName == PermissionItemName.DirectionForm.ToString())
            {
                child.LayoutRoot.Children.Add(

                        new GenericForm
                                                ("TblLkpDirection"
                                                , ""
                                                , PermissionItemName.DirectionForm
                                                )
                    );
            }
            else if (menuName == PermissionItemName.BrandSectionForm.ToString())
            {
                child.LayoutRoot.Children.Add(
                 new GenericForm
                     ("TblLkpBrandSection"
                         , PermissionItemName.BrandSection
                     ));
            }
            else if (menuName == PermissionItemName.ColorGroupForm.ToString())
            {
                child.LayoutRoot.Children.Add(new GenericForm
                    ("TblLkpColorGroup"
                        , ""
                        , PermissionItemName.ColorGroupForm
                    ));
            }
            else if (menuName == PermissionItemName.CheckListGroup.ToString())
            {
                child.LayoutRoot.Children.Add(new GenericForm
                    ("TblCheckListGroup"
                        , ""
                        , PermissionItemName.CheckListGroup
                    ));
            }

            else if (menuName == PermissionItemName.CheckListDesignGroupHeader1.ToString())
            {
                child.LayoutRoot.Children.Add(new GenericForm
                   ("TblCheckListDesignGroupHeader1"
                       , ""
                       , PermissionItemName.CheckListDesignGroupHeader1
                   ));
            }
            else if (menuName == PermissionItemName.CheckListDesignGroupHeader2.ToString())
            {
                child.LayoutRoot.Children.Add(new GenericForm
                    ("TblCheckListDesignGroupHeader2"
                        , ""
                        , PermissionItemName.CheckListDesignGroupHeader2
                    ));
            }
            else if (menuName == PermissionItemName.CheckListItem.ToString())
            {
                child.LayoutRoot.Children.Add(new GenericForm
                    ("TblCheckListItem"
                        , ""
                        , PermissionItemName.CheckListItem
                    ));
            }
            else if (menuName == PermissionItemName.CheckListLink.ToString())
            {
                child.LayoutRoot.Children.Add(new CheckListLink());
            }

            else if (menuName == PermissionItemName.CheckListTransaction.ToString())
            {
                child.LayoutRoot.Children.Add(new CheckListTransaction());
            }
            else if (menuName == PermissionItemName.ColorCodeForm.ToString())
            {
                //if (LoggedUserInfo.Iserial==5)
                //{
                //    child.LayoutRoot.Children.Add(new IntegrationPage());
                //}
                //else
                //{
                    child.LayoutRoot.Children.Add(new Color());
                //}

            }
            else if (menuName == PermissionItemName.ColorLinkForm.ToString())
            {
                child.LayoutRoot.Children.Add(new ColorLink());
            }
            else if (menuName == PermissionItemName.FamilyLinkForm.ToString())
            {

                child.LayoutRoot.Children.Add(new FamilyCategoryLink());
       
              //  child.LayoutRoot.Children.Add(new FamilyLink());

            }
            else if (menuName == PermissionItemName.DirectionLinkForm.ToString())
            {
                child.LayoutRoot.Children.Add(new DirectionLink());
            }
            else if (menuName == PermissionItemName.DesignForm.ToString())
            {
                child.LayoutRoot.Children.Add(
                        new GenericForm
                                                ("Tbl_lkp_FabricDesignes"
                                                , ""
                                                , PermissionItemName.DesignForm
                                                )
                    );
            }
            else if (menuName == PermissionItemName.ColorThemesForm.ToString())
            {
                child.LayoutRoot.Children.Add(new SalesOrderColorTheme());
            }
            else if (menuName == PermissionItemName.FactoryDelivery.ToString())
            {
                child.LayoutRoot.Children.Add(new FactoryDelivery());
            }

            else if (menuName == PermissionItemName.StyleCodingForm.ToString())
            {
                child.LayoutRoot.Children.Add(new StyleHeader(SalesOrderType.SalesOrderPo, true));
            }
            else if (menuName == PermissionItemName.ServiceCodingForm.ToString())
            {
                child.LayoutRoot.Children.Add(new ServiceCoding());
            }
            else if (menuName == PermissionItemName.fabImgs.ToString())
            {
                child.LayoutRoot.Children.Add(new FabricImageGallary());
            }
            else if (menuName == PermissionItemName.FactoryGroupForm.ToString())
            {
                child.LayoutRoot.Children.Add(
                    new GenericForm
                        ("TblFactoryGroup"
                            , ""
                            , PermissionItemName.FactoryGroupForm)
                        );
            }
            else if (menuName == PermissionItemName.SubContractor.ToString())
            {
                child.LayoutRoot.Children.Add(
                    new GenericForm
                        ("TblSubContractor"
                            , ""
                            , PermissionItemName.SubContractor)
                        );
            }
            else if (menuName == PermissionItemName.AccessoriesCodingForm.ToString())
            {
                child.LayoutRoot.Children.Add(new AccessorySetups());
            }            
            else if (menuName == PermissionItemName.AccessoriesGroupForm.ToString())
            {
                child.LayoutRoot.Children.Add(new AccessoryGroup());
            }

            else if (menuName == PermissionItemName.AccessoriesSizeGroupForm.ToString())
            {
                child.LayoutRoot.Children.Add(new AccSizeGroup());
            }
            else if (menuName == PermissionItemName.Documentation.ToString())
            {
                child.LayoutRoot.Children.Add(new DocumentationFiles());
            }

            else if (menuName == PermissionItemName.IssueJournalForm.ToString())
            {
                child.LayoutRoot.Children.Add(new IssueJournal());
            }
            else if (menuName == PermissionItemName.RetailPoForm.ToString())
            {
                child.LayoutRoot.Children.Add(new StyleHeader(SalesOrderType.RetailPo, false));
            }
            else if (menuName == PermissionItemName.CCPoForm.ToString())
            {
                child.LayoutRoot.Children.Add(new StyleHeader(SalesOrderType.SalesOrderPo, false));
            }
            else if (menuName == PermissionItemName.CCPoForm.ToString())
            {
                child.LayoutRoot.Children.Add(new StyleHeader(SalesOrderType.AdvancedSampleRequest, false));
            }
           
            else if (menuName == PermissionItemName.ProductionInvoice.ToString())
            {
                child.LayoutRoot.Children.Add(new ProductionInvoice());
            }
            else if (menuName == PermissionItemName.TradeAgreementFabricView.ToString())
            {
                child.LayoutRoot.Children.Add(new TradeAgreementFabricView());
            }
            else if (menuName == PermissionItemName.PaymentScheduleSettingsForm.ToString())
            {
                child.LayoutRoot.Children.Add(new PaymentScheduleSetting());
            }
            else if (menuName == PermissionItemName.PaymentScheduleForm.ToString())
            {
                child.LayoutRoot.Children.Add(new PaymentSchedule());
            }
            else if (menuName == PermissionItemName.BankDepositForm.ToString())
            {
                child.LayoutRoot.Children.Add(new BankDeposit());
            }
            else if (menuName == PermissionItemName.EmployeeShiftForm.ToString())
            {
                child.LayoutRoot.Children.Add(new EmployeeShift());
            }
            else if (menuName == PermissionItemName.EmployeeBehalfForm.ToString())
            {
                child.LayoutRoot.Children.Add(new EmployeeBehalf());
            }

            else if (menuName == PermissionItemName.ExcuseRulesForm.ToString())
            {
                child.LayoutRoot.Children.Add(new ExcuseRules());
            }
            else if (menuName == PermissionItemName.EmployeeInfoForm.ToString())
            {
                child.LayoutRoot.Children.Add(new EmployeeInfo());
            }

            else if (menuName == PermissionItemName.PromotionForm.ToString())
            {
                child.LayoutRoot.Children.Add(new PromotionViewModel());
            }
            else if (menuName == PermissionItemName.BrandStoreTarget.ToString())
            {
                child.LayoutRoot.Children.Add(new BrandStoreTarget());
            }
            else if (menuName == PermissionItemName.BrandStoreTargetForManagement.ToString())
            {
                child.LayoutRoot.Children.Add(new BrandStoreTargetManagment());
            }

            else if (menuName == PermissionItemName.CheckListMail.ToString())
            {
                child.LayoutRoot.Children.Add(new CheckListMail());
            }

            else if (menuName == PermissionItemName.BrandBudget.ToString())
            {
                child.LayoutRoot.Children.Add(new BrandBudget(1));
            }
            else if (menuName == PermissionItemName.CurrencyDailyExchangeForm.ToString())
            {
                child.LayoutRoot.Children.Add(new CurrencyDailyExchange());
            }
            else if (menuName == PermissionItemName.Bank.ToString())
            {
                child.LayoutRoot.Children.Add(new Bank());
            }
            else if (menuName == PermissionItemName.JournalSetting.ToString())
            {
                child.LayoutRoot.Children.Add(new JournalSetting());
            }
            
            else if (menuName == PermissionItemName.StoreCommission.ToString())
            {
                child.LayoutRoot.Children.Add(new StoreCommissionView());
            }
            else if (menuName == PermissionItemName.StoreVisaMachine.ToString())
            {
                child.LayoutRoot.Children.Add(new StoreVisaMachineView());
            }
            else if (menuName == PermissionItemName.Asset.ToString())
            {
                child.LayoutRoot.Children.Add(new Asset());
            }
            else if (menuName == PermissionItemName.GlPosting.ToString())
            {
                child.ImgClose.Visibility = Visibility.Collapsed;

                child.LayoutRoot.Children.Add(new GlPosting());
            }
            else if (menuName == PermissionItemName.BankGroup.ToString())
            {
                child.LayoutRoot.Children.Add((new GenericFormGl
                    ("TblBankGroup"
                        , ""
                        , PermissionItemName.BankGroup
                    )));
            }
            else if (menuName == PermissionItemName.AssetGroup.ToString())
            {
                child.LayoutRoot.Children.Add(new AssetGroup());
            }
            else if (menuName == PermissionItemName.CostCenterType.ToString())
            {
                child.LayoutRoot.Children.Add(new GenericFormGl
                    ("TblCostCenterType"
                        , ""
                        , PermissionItemName.CostCenterType
                    ));
            }

            else if (menuName == PermissionItemName.BankTransactionType.ToString())
            {
                child.LayoutRoot.Children.Add(new BankTransactionType());
            }
            else if (menuName == PermissionItemName.BankTransactionTypeGroup.ToString())
            {
                child.LayoutRoot.Children.Add(new GenericFormGl
                    ("TblBankTransactionTypeGroup"
                        , ""
                        , PermissionItemName.BankTransactionTypeGroup
                    ));
            }

            else if (menuName == PermissionItemName.CostCenterOption.ToString())
            {
                child.LayoutRoot.Children.Add(new GenericFormGl
                    ("TblCostCenterOption"
                        , ""
                        , PermissionItemName.CostCenterOption
                    ));
            }

            else if (menuName == PermissionItemName.ExpensesGroup.ToString())
            {
                child.LayoutRoot.Children.Add(new GenericFormGl
                    ("TblExpensisGroup"
                        , ""
                        , PermissionItemName.CostCenterOption
                    ));
            }
            else if (menuName == PermissionItemName.Expenses.ToString())
            {
                //   child.LayoutRoot.Children.Add(new Journal());
            }
            else if (menuName == PermissionItemName.Journal.ToString())
            {
                child.LayoutRoot.Children.Add(new Journal());
            }
            else if (menuName == PermissionItemName.Sequence.ToString())
            {
                child.LayoutRoot.Children.Add(new Sequence());
            }

            else if (menuName == PermissionItemName.CashdepositeTypeForm.ToString())
            {
                child.LayoutRoot.Children.Add(new CashDepositeType());
            }
            else if (menuName == PermissionItemName.VisaMachineForm.ToString())
            {
                child.LayoutRoot.Children.Add(new VisaMachine());
            }
            else if (menuName == PermissionItemName.CalliopeDaliySales.ToString())
            {
                child.LayoutRoot.Children.Add(new StoreDailySales());
            }
            else if (menuName == PermissionItemName.Account.ToString())
            {
                child.LayoutRoot.Children.Add(new Account());
            }
            else if (menuName == PermissionItemName.LedgerHeader.ToString())
            {
                child.LayoutRoot.Children.Add(new Ledger());
            }

            else if (menuName == PermissionItemName.PositionRoute.ToString())
            {
                child.LayoutRoot.Children.Add(new PositionRoute());
            }
            

            else if (menuName == PermissionItemName.PeriodsGl.ToString())
            {
                child.LayoutRoot.Children.Add(new PeriodsGl());
            }
            else if (menuName == PermissionItemName.PostingProfile.ToString())
            {
                child.LayoutRoot.Children.Add(new PostingProfile());
            }
            else if (menuName == PermissionItemName.RecInv.ToString())
            {
                child.LayoutRoot.Children.Add(new RecInv());
            }

            else if (menuName == PermissionItemName.Markup.ToString())
            {
                child.LayoutRoot.Children.Add(new Markup());
            }

            else if (menuName == PermissionItemName.MarkupGroup.ToString())
            {
                child.LayoutRoot.Children.Add(new MarkupGroup());
            }

            else if (menuName == PermissionItemName.InventPosting.ToString())
            {
                child.LayoutRoot.Children.Add(new InventPosting());
            }
            else if (menuName == PermissionItemName.SalaryApproval.ToString())
            {
                child.LayoutRoot.Children.Add(new SalaryApproval());
            }


            
            else if (menuName == PermissionItemName.GlRule.ToString())
            {
                child.LayoutRoot.Children.Add(new GlRule());
            }
            else if (menuName == PermissionItemName.GlRuleJob.ToString())
            {
                child.LayoutRoot.Children.Add(new GlRuleJob());
            }

            else if (menuName == PermissionItemName.CostAllocationMethod.ToString())
            {
                child.LayoutRoot.Children.Add(new CostAllocationMethod());
            }
            else if (menuName == PermissionItemName.MethodOfPayment.ToString())
            {
                child.LayoutRoot.Children.Add(new MethodOfPayment());
            }
            else if (menuName == PermissionItemName.IncomeStatmentDesign.ToString())
            {
                child.LayoutRoot.Children.Add(new IncomeStatmentDesign());
            }

            else if (menuName == PermissionItemName.IncomeStatmentDesign.ToString())
            {
                child.LayoutRoot.Children.Add(new IncomeStatmentDesign());
            }
            else if (menuName == PermissionItemName.CostCenterRouteGroup.ToString())
            {
                child.LayoutRoot.Children.Add(new CostCenterRouteGroup());
            }
            else if (menuName == PermissionItemName.CostCenterOrganizationUnit.ToString())
            {
                child.LayoutRoot.Children.Add(new CostCenterOrganizationUnit());
            }

            if (!string.IsNullOrWhiteSpace(menuLink)&& menuName.StartsWith(PermissionItemName.GlCashTransaction.ToString()))
            {
                var client = new GlServiceClient();
                client.GetTblCashTypeSettingsAsync(menuLink, LoggedUserInfo.DatabasEname);
                client.GetTblCashTypeSettingsCompleted += (s, sv) =>
                {
                    child.LayoutRoot.Children.Add(new GlCashTransaction(sv.Result, sv.entityList));
                    if (LoggedUserInfo.CurrLang == 0)
                    {
                        child.FlowDirection = FlowDirection.RightToLeft;
                    }
                    else
                    {
                        child.FlowDirection = FlowDirection.LeftToRight;
                    }
                    child.Title = title;
                    child.Show();
                };

                return;
            }

            if (!string.IsNullOrWhiteSpace(menuLink))
            {
                var client = new GlServiceClient();
                client.GetTblChequeTypeSettingsAsync(menuLink, LoggedUserInfo.DatabasEname);
                client.GetTblChequeTypeSettingsCompleted += (s, sv) =>
                {
                    child.LayoutRoot.Children.Add(new GlChequeTransaction(sv.Result, sv.entityList));
                    if (LoggedUserInfo.CurrLang == 0)
                    {
                        child.FlowDirection = FlowDirection.RightToLeft;
                    }
                    else
                    {
                        child.FlowDirection = FlowDirection.LeftToRight;
                    }
                    child.Title = title;
                    child.Show();
                };

                return;
            }
            if (LoggedUserInfo.CurrLang == 0)
            {
                child.FlowDirection = FlowDirection.RightToLeft;
            }
            else
            {
                child.FlowDirection = FlowDirection.LeftToRight;
            }
            if (menuName == PermissionItemName.BrandSectionFamily.ToString())
            {
                child.LayoutRoot.Children.Add(new BrandSectionFamily());
            }
            child.Title = title;
            child.Show();
        }
    }
}