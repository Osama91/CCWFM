using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using _Model = CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<_Model.TblSize> GetSizesBySizeGroup(int? _Skip, int? _Take, int _SizeGroup)
        {
            using (var context = new _Model.WorkFlowManagerDBEntities())
            {
                if (_Skip != null)
                    return context.TblSizes.Where(x => x.TblSizeGroup == _SizeGroup).OrderBy(x => x.Id).Skip((int)_Skip).Take((int)_Take).ToList();
                else
                    return context.TblSizes.Where(x => x.TblSizeGroup == _SizeGroup).OrderBy(x => x.Id).ToList();
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<_Model.TblSize> GetSizes(int? _Skip, int? _Take)
        {
            List<_Model.TblSize> temp;
            using (var context = new _Model.WorkFlowManagerDBEntities())
            {
                if (_Skip != null)
                {
                    temp = context.TblSizes.OrderBy(x => x.Id).Skip((int)_Skip).Take((int)_Take).ToList();
                    return temp;
                }
                else
                {
                    temp = context.TblSizes.OrderBy(x => x.Id).ToList();
                    return temp;
                }
            }
        }
    }
}