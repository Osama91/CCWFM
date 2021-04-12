using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace CCWFM.Views.OGView
{

    #region Models
    public class SectionLinkModel
    {
        
        public string Code { get; set; }

        public string Aname { get; set; }

        public string Ename { get; set; }

        public int Iserial { get; set; }

        public string TblBrand { get; set; }

        public bool Checked { get; set; }

        public int TblLkpBrandSection { get; set; }
    }

    public class DirectionLinkModel
    {

        public string Code { get; set; }

        public string Aname { get; set; }

        public string Ename { get; set; }

        public int Iserial { get; set; }

        public string TblBrand { get; set; }

        public int TblLkpBrandSection { get; set; }

        public bool Checked { get; set; }

       
    }

    public class CategoryLinkModel
    {

        public string Code { get; set; }

        public string Aname { get; set; }

        public string Ename { get; set; }

        public int Iserial { get; set; }

        public string TblBrand { get; set; }

        public int TblLkpBrandSection { get; set; }

        public int TblLkpDirection { get; set; }

        public bool Checked { get; set; }

       
    }

    public class FamilyLinkModel
    {

        public string Code { get; set; }

        public string Aname { get; set; }

        public string Ename { get; set; }

        public int Iserial { get; set; }

        public string TblBrand { get; set; }

        public int TblLkpBrandSection { get; set; }

        public int TblLkpDirection { get; set; }

        public int TblStyleCategory { get; set; }

        public bool Checked { get; set; }
    }


    public class SubFamilyLinkModel
    {

        public string Code { get; set; }

        public string Aname { get; set; }

        public string Ename { get; set; }

        public int Iserial { get; set; }

        public string TblBrand { get; set; }

        public int TblLkpBrandSection { get; set; }

        public int TblLkpDirection { get; set; }

        public int TblStyleCategory { get; set; }

        public int TblFamilyCategory { get; set; }

        public bool Checked { get; set; }
    }

    #endregion

    public class FamilyCategoryLinkViewModel : Page
    {

    }
}