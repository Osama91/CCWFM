using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CCWFM.ViewModel
{
    class Transaction
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public string SalaryTerm { get; set; }
    }
}
