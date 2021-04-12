using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Os.Controls.DataGrid
{
    public static class DataGridColumnHelper
    {
        public static readonly DependencyProperty FilterTypeProperty = DependencyProperty.RegisterAttached("FilterType",
                                                                                 typeof(string), typeof(System.Windows.Controls.DataGrid), new PropertyMetadata(null));

        public static void SetFilterTypeBinding(this DataGridColumn element, string value)
        {
            element.SetValue(FilterTypeProperty, value);
        }

        public static string GetFilterTypeBinding(this DataGridColumn element)
        {
            return (string)element.GetValue(FilterTypeProperty);
        }

        // Using a DependencyProperty as the backing store for MyProperty.This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReadOnlyCustomProperty =
            DependencyProperty.RegisterAttached("IsReadOnlyCustom", typeof(object), typeof(System.Windows.Controls.DataGrid), new PropertyMetadata(null, Callback));

        private static void Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((System.Windows.Controls.DataGrid)d).IsReadOnly = (bool)e.NewValue;
        }

        public static void SetIsReadOnly(DependencyObject obj, object isReadOnly)
        {
            try
            {
                obj.SetValue(IsReadOnlyCustomProperty, isReadOnly);
            }
            catch (Exception)
            {
            }
        }

        public static bool GetIsReadOnly(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsReadOnlyCustomProperty);
        }

        public static readonly DependencyProperty HeaderBindingProperty = DependencyProperty.RegisterAttached(
            "HeaderBinding",
            typeof(object),
            typeof(DataGridColumnHelper),
            new PropertyMetadata(null, HeaderBinding_PropertyChanged));

        public static object GetHeaderBinding(DependencyObject source)
        {
            return source.GetValue(HeaderBindingProperty);
        }

        public static void SetHeaderBinding(DependencyObject target, object value)
        {
            target.SetValue(HeaderBindingProperty, value);
        }

        private static void HeaderBinding_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            var column = d as DataGridColumn;

            if (column == null) { return; }

            column.Header = e.NewValue;
        }


        
        public static readonly DependencyProperty FilterListProperty = DependencyProperty.RegisterAttached(
        "FilterList",
        typeof(IEnumerable),
        typeof(DataGridColumnHelper),
        new PropertyMetadata(null, FilterListBinding_PropertyChanged));

        public static object GetFilterList(DependencyObject source)
        {
            return source.GetValue(FilterListProperty);
        }

        public static void SetFilterList(DependencyObject target, object value)
        {
            target.SetValue(FilterListProperty, value);
        }

        private static void FilterListBinding_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = d as DataGridColumn;

            if (column == null) { return; }

          //  column.ColumnFilterControl = e.NewValue;
        }



        public static readonly DependencyProperty HideFilterProperty =
          DependencyProperty.RegisterAttached("HideFilter", typeof(object), typeof(DataGridColumnHelper),
              new PropertyMetadata(null, OnHideFilterChanged));

        public static object GetHideFilterBinding(DependencyObject source)
        {
            return source.GetValue(HideFilterProperty);
        }

        public static void SetHideFilterBinding(DependencyObject target, object value)
        {
            target.SetValue(HideFilterProperty, value);
        }

        private static void OnHideFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = d as DataGridColumn;

            if (column == null)
            {
                return;
            }
            if ((bool)e.NewValue)
            {
                //column.SortMemberPath
            }
        }
    }
}