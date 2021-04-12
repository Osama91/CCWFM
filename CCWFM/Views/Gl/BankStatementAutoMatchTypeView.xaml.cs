﻿using CCWFM.ViewModel.OGViewModels.ControlsOverride;
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

namespace CCWFM.Views.Gl
{
    public partial class BankStatementAutoMatchTypeView : ChildWindowsOverride
    {
        public BankStatementAutoMatchTypeView()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            //this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            //this.Close();
        }        
    }
}