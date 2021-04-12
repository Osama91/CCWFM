using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service.Operations.GlOperations
{
    public partial class GlService
    {
        [OperationContract]
        private List<TblColorTest> GetTblItemPricebyCode(string code, string company, out List<TblSizeRetail> sizeIserials)
        {
            using (var entity = new ccnewEntities(GetSqlConnectionString(company)))
            {
                var colorsIserials = entity.TBLITEMprices.Where(x => x.Style == code).Select(x => x.TblColor1).Distinct();
                sizeIserials = entity.TBLITEMprices.Where(x => x.Style == code).Select(x => x.TblSize1).Distinct().ToList();

                return colorsIserials.ToList();
            }
        }
    }
}