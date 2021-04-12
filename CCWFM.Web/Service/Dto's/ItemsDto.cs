using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace CCWFM.Web.Service
{
    [DataContract]
    public class ItemsDto
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Config { get; set; }
        [DataMember]
        public string Size { get; set; }
        [DataMember]
        public string Batch { get; set; }
        [DataMember]
        public string Desc { get; set; }
        [DataMember]
        public byte[] Image { get; set; }
        [DataMember]
        public List<ImageDto> Images { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ItemGroup { get; set; }
    }
}