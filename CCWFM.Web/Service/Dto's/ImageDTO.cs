using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CCWFM.Web.Service
{
    [DataContract]
    public class ImageDto
    {
        [DataMember]
        public byte[] Image { get; set; }
    }
}