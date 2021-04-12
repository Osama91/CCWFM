

using System;
using System.Windows.Browser;

namespace CCWFM.ViewModel
{
    public class Cookies
    {

        public static void SetCookie(string key, Object value)
        {
            string oldCookie = HtmlPage.Document.GetProperty("cookie") as String;
            DateTime expiration = DateTime.UtcNow + TimeSpan.FromMinutes(5);
            string cookie = String.Format("{0}={1};expires={2}", key, value, expiration.ToString("R"));
            HtmlPage.Document.SetProperty("cookie", cookie);
        }

        public static string GetCookie(string key)
        {
            string[] cookies = HtmlPage.Document.Cookies.Split(';');
            key += '=';
            foreach (string cookie in cookies)
            {
                string cookieStr = cookie.Trim();
                if (cookieStr.StartsWith(key, StringComparison.OrdinalIgnoreCase))
                {
                    string[] vals = cookieStr.Split('=');

                    if (vals.Length >= 2)
                    {
                        return vals[1];
                    }

                    return string.Empty;
                }
            }

            return null;
        }

        /// <summary>
        /// Deletes a specified cookie by setting its value to empty and expiration to -1 days
        /// </summary>
        /// <param name="key">the cookie key to delete</param>
        public static void DeleteCookie(string key)
        {
            string oldCookie = HtmlPage.Document.GetProperty("cookie") as String;
            DateTime expiration = DateTime.UtcNow - TimeSpan.FromDays(1);
            string cookie = String.Format("{0}=;expires={1}", key, expiration.ToString("R"));
            HtmlPage.Document.SetProperty("cookie", cookie);
        }
    }
}