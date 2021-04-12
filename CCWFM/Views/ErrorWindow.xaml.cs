using System;
using System.Windows;
using System.Windows.Controls;

namespace CCWFM
{
    public partial class ErrorWindow : ChildWindow
    {
        public ErrorWindow(Exception e)
        {
            InitializeComponent();
            if (e != null)
            {
                try
                {
                    ErrorTextBox.Text = e.Message
                    + Environment.NewLine
                    + Environment.NewLine
                    + (e.StackTrace ?? string.Empty)
                    + Environment.NewLine
                    + "------------------------------------------------------------------------------"
                    + Environment.NewLine
                    + e.InnerException == null ? "" : e.InnerException.StackTrace
                    + Environment.NewLine
                    + "------------------------------------------------------------------------------"
                    + Environment.NewLine
                    + e.InnerException == null ? "" : e.InnerException.Message
                    + Environment.NewLine
                    + "------------------------------------------------------------------------------"
                    + Environment.NewLine
                    + e.InnerException.InnerException == null ? "" : e.InnerException.InnerException.StackTrace
                    + Environment.NewLine
                    + "------------------------------------------------------------------------------"
                    + Environment.NewLine
                    + e.InnerException.InnerException == null ? "" : e.InnerException.InnerException.Message;
                }
                catch (Exception)
                {
                    try
                    {
                        ErrorTextBox.Text = e.Message
                        + Environment.NewLine
                        + Environment.NewLine
                        + (e.StackTrace ?? string.Empty)
                        + Environment.NewLine
                        + "------------------------------------------------------------------------------"
                        + Environment.NewLine
                        + e.InnerException == null ? "" : (e.InnerException.StackTrace ?? string.Empty)
                        + Environment.NewLine
                        + "------------------------------------------------------------------------------"
                        + Environment.NewLine
                        + e.InnerException == null ? "" : e.InnerException.Message
                        + Environment.NewLine
                        + "------------------------------------------------------------------------------";
                    }
                    catch (Exception)
                    {
                        ErrorTextBox.Text = e.Message
                        + Environment.NewLine
                        + Environment.NewLine
                        + (e.StackTrace ?? string.Empty)
                        + Environment.NewLine
                        + "------------------------------------------------------------------------------"
                        + Environment.NewLine
                        + e.Message;
                    }
                }
            }
        }

        public ErrorWindow(Uri uri)
        {
            InitializeComponent();
            if (uri != null)
            {
                ErrorTextBox.Text = "Page not found: \"" + uri.ToString() + "\"";
            }
        }

        public ErrorWindow(string message, string details)
        {
            InitializeComponent();
            ErrorTextBox.Text = message + Environment.NewLine + Environment.NewLine + details;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}