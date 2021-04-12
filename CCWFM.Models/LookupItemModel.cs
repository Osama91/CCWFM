using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCWFM.Models
{
    public class LookupItemModel
    {
        string iserial = "Iserial", codePath= "Code", nameEnPath= "Ename", nameArPath= "Aname";
        /// <summary>
        /// Iserial
        /// </summary>
        public string IserialPath { get { return iserial; } set { iserial = value; } }
        /// <summary>
        /// Code
        /// </summary>
        public string CodePath { get { return codePath; } set { codePath = value; } }
        /// <summary>
        /// Ename
        /// </summary>
        public string NameEnPath { get { return nameEnPath; } set { nameEnPath = value; } }
        /// <summary>
        /// Aname
        /// </summary>
        public string NameArPath { get { return nameArPath; } set { nameArPath = value; } }

    }
}