using System.Runtime.Serialization;

namespace CCWFM.Web.Service
{
    [DataContract]
    public class ImageDto
    {
        [DataMember]
        public byte[] Image { get; set; }
    }
}