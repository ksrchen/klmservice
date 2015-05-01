using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loader
{

    public partial class attachment
    {
        public int ID { get; set; }
        public string Board { get; set; }
        public string ClassID { get; set; }
        public string ClassKey { get; set; }
        public string ClassSourceKey { get; set; }
        public string FileExtension { get; set; }
        public string IsDeleted { get; set; }
        public string MediaDescription { get; set; }
        public string MediaKey { get; set; }
        public string MediaOrder { get; set; }
        public string MediaType { get; set; }
        public string MediaURL { get; set; }
        public string MLS_ID { get; set; }
        public string OfficeCode { get; set; }
        public string SourceKey { get; set; }
        public string TimestampModified { get; set; }
        public string TimestampUploaded { get; set; }
    }

}
