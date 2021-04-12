using System;
using System.Windows.Controls;

namespace CCWFM.Helpers.InputValidators
{
    public class NumValidations
    {
        public static bool validateTextDouble(object textToValidate)
        {
            double x;
            return double.TryParse(textToValidate.ToString(), out x);
        }

        public static void validateTextDouble(object sender, EventArgs e)
        {
            var X = new Exception();

            var T = (TextBox)sender;

            try
            {
                var x = double.Parse(T.Text);

                //Customizing Condition (Only numbers larger than
                //zero are permitted)
                if (x < 0 || T.Text.Contains(","))
                    throw X;
            }
            catch (Exception)
            {
                try
                {
                    var cursorIndex = T.SelectionStart - 1;
                    T.Text = T.Text.Remove(cursorIndex, 1);

                    //Align Cursor to same index
                    T.SelectionStart = cursorIndex;
                    T.SelectionLength = 0;
                }
                catch (Exception) { }
            }
        }

        public static void validateTextInt(object sender, EventArgs e)
        {
            var X = new Exception();

            var T = (TextBox)sender;

            try
            {
                var x = int.Parse(T.Text);

                //Customizing Condition (Only numbers larger than
                //zero are permitted)
                if (x < 0)
                    throw X;
            }
            catch (Exception)
            {
                try
                {
                    var cursorIndex = T.SelectionStart - 1;
                    T.Text = T.Text.Remove(cursorIndex, 1);

                    //Align Cursor to same index
                    T.SelectionStart = cursorIndex;
                    T.SelectionLength = 0;
                }
                catch (Exception) { }
            }
        }
    }
}