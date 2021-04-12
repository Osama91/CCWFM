using System;

namespace CCWFM.Models
{
    public static class Helper
    {
        public static string GetInnerExceptionMessage(Exception ex)
        {
            if (ex.InnerException == null)
                return ex.Message;
            else
                return GetInnerExceptionMessage(ex.InnerException);
        }
        public static Exception GetInnerException(Exception ex)
        {
            if (ex.InnerException == null)
                return ex;
            else
                return GetInnerException(ex.InnerException);
        }
    }
}
