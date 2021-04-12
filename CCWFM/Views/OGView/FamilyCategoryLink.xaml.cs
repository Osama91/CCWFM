using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using CCWFM.Helpers.AuthenticationHelpers;
using CCWFM.CRUDManagerService;

namespace CCWFM.Views.OGView
{
    public partial class FamilyCategoryLink 
    {
        

        private readonly FamilyCategoryLinkViewModel _viewModel;
        CRUD_ManagerServiceClient Client = new CRUD_ManagerServiceClient();
        public LkpData.LkpDataClient lkpClient = new LkpData.LkpDataClient();
        public List<SectionLinkModel> BrandSectionList = new List<SectionLinkModel>();
        public List<DirectionLinkModel> DirectionList = new List<DirectionLinkModel>(); 
        public List<CategoryLinkModel> CategoryList = new List<CategoryLinkModel>();
        public List<FamilyLinkModel> FamilyList = new List<FamilyLinkModel>();
        public List<SubFamilyLinkModel> SubFamilyList = new List<SubFamilyLinkModel>();

        public FamilyCategoryLink()
        {
            InitializeComponent();
            
            LoadControls();
        }

        public void LoadControls()
        {
            LoadBrands();
            LoadSections();
            LoadDirection();
            LoadCategory();
            LoadFamily();
           // LoadSubFamily();
        }

        #region LoadMainData

        private void LoadBrands()
        {
            Client.GetAllBrandsCompleted += (s, sv) =>
            {
                CbBrand.ItemsSource = null;
                CbBrand.ItemsSource = sv.Result;
            };
            Client.GetAllBrandsAsync(LoggedUserInfo.Iserial);
        }

        private void LoadSections()
        {

            Client.GetGenericCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    var newrow = new SectionLinkModel
                    {
                        Aname = row.Aname,
                        Ename = row.Ename,
                        Iserial = row.Iserial,
                        Code = row.Code
                    };
                    BrandSectionList.Add(newrow);

                }
                BrandSectionGrid.ItemsSource = null;
                BrandSectionGrid.ItemsSource = BrandSectionList;
            };
            Client.GetGenericAsync("TblLkpBrandSection", "%%", "%%", "%%", "Iserial", "ASC");

        }

        private void LoadDirection()
        {
            lkpClient.FamilyCategory_GetTblDirectionCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    var newrow = new DirectionLinkModel
                    {
                        Aname = row.Aname,
                        Ename = row.Ename,
                        Iserial = row.Iserial,

                        Code = row.Code
                    };
                    DirectionList.Add(newrow);

                }
                DirectionGrid.ItemsSource = null;
                DirectionGrid.ItemsSource = DirectionList;
            };


            lkpClient.FamilyCategory_GetTblDirectionAsync(DirectionList.Count, 1000, "", "", null);

        }

        private void LoadCategory()
        {
            lkpClient.FamilyCategory_GetTblStyleCategoryCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    var newrow = new CategoryLinkModel
                    {
                        Aname = row.Aname,
                        Ename = row.Ename,
                        Iserial = row.Iserial,
                        Code = row.Code
                    };
                    CategoryList.Add(newrow);

                }
                CategoryGrid.ItemsSource = null;
                CategoryGrid.ItemsSource = CategoryList;
            };
            lkpClient.FamilyCategory_GetTblStyleCategoryAsync(CategoryList.Count, 1000, "", "", null);
        }

        private void LoadFamily()
        {
            lkpClient.FamilyCategory_GetTblFamilyCompleted += (s, sv) =>
            {
                foreach (var row in sv.Result)
                {
                    var newrow = new FamilyLinkModel
                    {
                        Aname = row.Aname,
                        Ename = row.Ename,
                        Iserial = row.Iserial,
                        Code = row.Code
                    };
                    FamilyList.Add(newrow);

                }
                FamilyGrid.ItemsSource = null;
                FamilyGrid.ItemsSource = FamilyList;
            };
            lkpClient.FamilyCategory_GetTblFamilyAsync(FamilyList.Count, 1000, "", "", null);
        }

        private void LoadSubFamily()
        {
            try
            {
                var familyselectedItem = FamilyGrid.SelectedItem as FamilyLinkModel;
                
                lkpClient.FamilyCategory_GetTblSubFamilyCompleted += (s, sv) =>
                {
                    SubFamilyList = new List<SubFamilyLinkModel>();
                    foreach (var row in sv.Result)
                    {
                        var newrow = new SubFamilyLinkModel
                        {
                            Aname = row.Aname,
                            Ename = row.Ename,
                            Iserial = row.Iserial,
                            Code = row.Code
                        };
                        SubFamilyList.Add(newrow);

                    }
                    SubFamilyGrid.ItemsSource = null;
                    SubFamilyGrid.ItemsSource = SubFamilyList;
                };
                lkpClient.FamilyCategory_GetTblSubFamilyAsync(familyselectedItem.Iserial);
            }
            catch { }
           
        }

        #endregion

        #region Loading DataLink

        private void CbBrand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSectionsByBrand();
            LoadDirectionBySection();
            LoadCategoryByDirection();
            LoadFamilyByCategory();
           // LoadSubFamilyByFamily();
        }

        private void BrandSectionGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadDirectionBySection();
            LoadCategoryByDirection();
            LoadFamilyByCategory();
           // LoadSubFamilyByFamily();
        }

        private void DirectionGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadCategoryByDirection();
            LoadFamilyByCategory();
            //LoadSubFamilyByFamily();
        }

        private void CategoryGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadFamilyByCategory();
           // LoadSubFamilyByFamily();
        }

        private void FamilyGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSubFamily();
            LoadSubFamilyByFamily();
        }


        public void LoadSectionsByBrand()
        {
            
            lkpClient.GetTblBrandSectionLinkCompleted += (s, sv) =>
            {
                try
                {
                    foreach (var row in BrandSectionList)
                    {
                        row.Checked = false;
                    }

                    foreach (var row in sv.Result)
                    {
                        var brandSectionRow = BrandSectionList.SingleOrDefault(x => x.Iserial == row.TblLkpBrandSection);
                        if (brandSectionRow != null)
                        {
                            brandSectionRow.Checked = true;
                        }
                    }
                   
                    BrandSectionGrid.ItemsSource = null;
                    BrandSectionGrid.ItemsSource = BrandSectionList;
                }
                catch { }

            };
            lkpClient.GetTblBrandSectionLinkAsync(CbBrand.SelectedValue.ToString(), LoggedUserInfo.Iserial);
        }

        public void LoadDirectionBySection()
        {
            try
            {
                var selectedItem = BrandSectionGrid.SelectedItem as SectionLinkModel;
                if (selectedItem == null || selectedItem.Checked == false)
                {
                    DirectionGrid.IsEnabled = false;
                    CategoryGrid.IsEnabled = false;
                    FamilyGrid.IsEnabled = false;
                    SubFamilyGrid.IsEnabled = false;
                }
                else
                {
                    DirectionGrid.IsEnabled = true;
                }
                    lkpClient.FamilyCategory_GetTblDirectionLinkCompleted += (s, sv) =>
                    {
                        foreach (var row in DirectionList)
                        {
                            row.Checked = false;
                        }

                        foreach (var row in sv.Result)
                        {
                            var directionRow = DirectionList.SingleOrDefault(x => x.Iserial == row.TblLkpDirection);
                            if (directionRow != null)
                            {
                                directionRow.Checked = true;
                            }
                        }
                        DirectionGrid.ItemsSource = null;
                        DirectionGrid.ItemsSource = DirectionList;
                    };
                    lkpClient.FamilyCategory_GetTblDirectionLinkAsync(CbBrand.SelectedValue.ToString(), selectedItem.Iserial);
                
            }
            catch { }

        }

        public void LoadCategoryByDirection()
        {
            try
            {
                var sectionselectedItem = BrandSectionGrid.SelectedItem as SectionLinkModel;
                var directionselectedItem = DirectionGrid.SelectedItem as DirectionLinkModel;

                if (directionselectedItem == null || directionselectedItem.Checked == false)
                {
                    CategoryGrid.IsEnabled = false;
                    FamilyGrid.IsEnabled = false;
                    SubFamilyGrid.IsEnabled = false;
                }
                else
                {
                    CategoryGrid.IsEnabled = true;
                }
                    lkpClient.FamilyCategory_GetTblCategoryLinkCompleted += (s, sv) =>
                    {
                        try
                        {
                            foreach (var row in CategoryList)
                            {
                                row.Checked = false;
                            }

                            foreach (var row in sv.Result)
                            {
                                var categoryRow = CategoryList.SingleOrDefault(x => x.Iserial == row.TblStyleCategory);
                                if (categoryRow != null)
                                {
                                    categoryRow.Checked = true;
                                }
                            }
                        }
                        catch { }
                        CategoryGrid.ItemsSource = null;
                        CategoryGrid.ItemsSource = CategoryList;
                    };
                    if (sectionselectedItem != null && directionselectedItem != null)
                        lkpClient.FamilyCategory_GetTblCategoryLinkAsync(CbBrand.SelectedValue.ToString(), sectionselectedItem.Iserial, directionselectedItem.Iserial);
                
            
            }
            catch { }
        }

        public void LoadFamilyByCategory()
        {

            try
            {
                var sectionselectedItem = BrandSectionGrid.SelectedItem as SectionLinkModel;
                var directionselectedItem = DirectionGrid.SelectedItem as DirectionLinkModel;
                var categoryselectedItem = CategoryGrid.SelectedItem as CategoryLinkModel;
                if (categoryselectedItem == null || categoryselectedItem.Checked == false)
                {
                    FamilyGrid.IsEnabled = false;
                    SubFamilyGrid.IsEnabled = false;
                }
                else
                {
                    FamilyGrid.IsEnabled = true;
                }
                    lkpClient.FamilyCategory_GetTblFamilyCategoryLinkCompleted += (s, sv) =>
                    {
                        foreach (var row in FamilyList)
                        {
                            row.Checked = false;
                        }

                        foreach (var row in sv.Result)
                        {
                            var familyRow = FamilyList.SingleOrDefault(x => x.Iserial == row.TblFamily);
                            if (familyRow != null)
                            {
                                familyRow.Checked = true;
                            }
                        }
                        FamilyGrid.ItemsSource = null;
                        FamilyGrid.ItemsSource = FamilyList;
                    };
                    if (sectionselectedItem != null && directionselectedItem != null && categoryselectedItem != null)
                        lkpClient.FamilyCategory_GetTblFamilyCategoryLinkAsync(CbBrand.SelectedValue.ToString(), sectionselectedItem.Iserial, directionselectedItem.Iserial, categoryselectedItem.Iserial);
                
        
            }
            catch { }
        }

        public void LoadSubFamilyByFamily()
        {
            try
            {
                var sectionselectedItem = BrandSectionGrid.SelectedItem as SectionLinkModel;
                var directionselectedItem = DirectionGrid.SelectedItem as DirectionLinkModel;
                var categoryselectedItem = CategoryGrid.SelectedItem as CategoryLinkModel;
                var familyselectedItem = FamilyGrid.SelectedItem as FamilyLinkModel;
                
                if (familyselectedItem == null || familyselectedItem.Checked == false)
                {
                    SubFamilyGrid.IsEnabled = false;
                }
                else
                {
                    SubFamilyGrid.IsEnabled = true;
                }
                    lkpClient.FamilyCategory_GetTblSubFamilyCategoryLinkCompleted += (s, sv) =>
                    {
                        foreach (var row in SubFamilyList)
                        {
                            row.Checked = false;
                        }

                        foreach (var row in sv.Result)
                        {
                            var subFamilyRow = SubFamilyList.SingleOrDefault(x => x.Iserial == row.TblSubFamily);
                            if (subFamilyRow != null)
                            {
                                subFamilyRow.Checked = true;
                            }
                        }
                        SubFamilyGrid.ItemsSource = null;
                        SubFamilyGrid.ItemsSource = SubFamilyList;
                    };

                    if (sectionselectedItem != null && directionselectedItem != null && categoryselectedItem != null && familyselectedItem != null)
                        lkpClient.FamilyCategory_GetTblSubFamilyCategoryLinkAsync(CbBrand.SelectedValue.ToString(), sectionselectedItem.Iserial, directionselectedItem.Iserial, categoryselectedItem.Iserial, familyselectedItem.Iserial);
                
            }
            catch { }
        }
        #endregion

        #region SaveUpdate
        
        private void BrandSectionCheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = BrandSectionGrid.SelectedItem as SectionLinkModel;

                if (selectedItem.Checked == true)
                    DirectionGrid.IsEnabled = true;
                else DirectionGrid.IsEnabled = false;

                lkpClient.FamilyCategory_UpdateOrDeleteTblLkpBrandSectionLinkAsync(CbBrand.SelectedValue.ToString(), selectedItem.Iserial, selectedItem.Checked);

            } catch { }
        }

        private void DirectionCheckBox_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                var SectionselectedItem =   BrandSectionGrid.SelectedItem as SectionLinkModel;
                var directionselectedItem = DirectionGrid.SelectedItem as DirectionLinkModel;

                if (directionselectedItem.Checked == true)
                    CategoryGrid.IsEnabled = true;
                else CategoryGrid.IsEnabled = false;

                lkpClient.FamilyCategory_UpdateOrDeleteTblLkpDirectionLinkAsync (CbBrand.SelectedValue.ToString(), SectionselectedItem.Iserial, directionselectedItem.Iserial, directionselectedItem.Checked);

            }
            catch { }
        }

        private void CategoryCheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var SectionselectedItem = BrandSectionGrid.SelectedItem as SectionLinkModel;
                var directionselectedItem = DirectionGrid.SelectedItem as DirectionLinkModel;
                var categoryelectedItem = CategoryGrid.SelectedItem as CategoryLinkModel;


                if (categoryelectedItem.Checked == true)
                    FamilyGrid.IsEnabled = true;
                else FamilyGrid.IsEnabled = false;

                lkpClient.FamilyCategory_UpdateOrDeleteTblStyleCategoryLinkAsync(CbBrand.SelectedValue.ToString(), SectionselectedItem.Iserial, directionselectedItem.Iserial, categoryelectedItem.Iserial,categoryelectedItem.Checked);

            }
            catch { }
        }

        private void FamilyCheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var SectionselectedItem = BrandSectionGrid.SelectedItem as SectionLinkModel;
                var directionselectedItem = DirectionGrid.SelectedItem as DirectionLinkModel;
                var categoryelectedItem = CategoryGrid.SelectedItem as CategoryLinkModel;
                var familyselectedItem = FamilyGrid.SelectedItem as FamilyLinkModel;

                if (familyselectedItem.Checked == true)
                    SubFamilyGrid.IsEnabled = true;
                else SubFamilyGrid.IsEnabled = false;

                lkpClient.FamilyCategory_UpdateOrDeleteTblFamilyCategoryLinkAsync(CbBrand.SelectedValue.ToString(), SectionselectedItem.Iserial,
                    directionselectedItem.Iserial, categoryelectedItem.Iserial, familyselectedItem.Iserial, familyselectedItem.Checked);

            }
            catch { }
        }

        private void SubFamilyCheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var SectionselectedItem = BrandSectionGrid.SelectedItem as SectionLinkModel;
                var directionselectedItem = DirectionGrid.SelectedItem as DirectionLinkModel;
                var categoryelectedItem = CategoryGrid.SelectedItem as CategoryLinkModel;
                var familyselectedItem = FamilyGrid.SelectedItem as FamilyLinkModel;
                var subfamilylselectedItem = SubFamilyGrid.SelectedItem as SubFamilyLinkModel;

               
                lkpClient.FamilyCategory_UpdateOrDeleteTblSubFamilyCategoryLinkAsync(CbBrand.SelectedValue.ToString(), SectionselectedItem.Iserial,
                    directionselectedItem.Iserial, categoryelectedItem.Iserial, familyselectedItem.Iserial, subfamilylselectedItem.Iserial, subfamilylselectedItem.Checked);

            }
            catch { }
        }

        #endregion


    }
}
