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
        public List<_Model.SizeCode> GetSizesBySizeGroup(int? _Skip, int? _Take, string _SizeGroup)
        {
            using (_Model.WorkFlowManagerDBEntities context = new _Model.WorkFlowManagerDBEntities())
            {
                if (!string.IsNullOrEmpty(_SizeGroup.Trim()))
                {
                    if (_Skip != null)
                        return context.SizeCodes.Where(x => x.SizeCode_SizeGroup == _SizeGroup).OrderBy(x => x.SizeCode_Id).Skip((int)_Skip).Take((int)_Take).ToList();
                    else
                        return context.SizeCodes.Where(x => x.SizeCode_SizeGroup == _SizeGroup).OrderBy(x => x.SizeCode_Id).ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<_Model.SizeCode> GetSizes(int? _Skip, int? _Take)
        {
            List<_Model.SizeCode> temp;
            using (_Model.WorkFlowManagerDBEntities context = new _Model.WorkFlowManagerDBEntities())
            {
                if (_Skip != null)
                {
                    temp = context.SizeCodes.OrderBy(x => x.SizeCode_Id).Skip((int)_Skip).Take((int)_Take).ToList();
                    return temp;
                }
                else
                {
                    temp = context.SizeCodes.OrderBy(x => x.SizeCode_Id).ToList();
                    return temp;
                }
            }
        }
    }
}