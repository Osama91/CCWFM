using System;
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
        public List<_Model.StyleHeader> GetStyleHeaders(int? _Skip, int? _Take)
        {
            try
            {
                using (_Model.WorkFlowManagerDBEntities context = new _Model.WorkFlowManagerDBEntities())
                {
                    if (_Skip != null)
                        return context.StyleHeaders.OrderByDescending(x => x.StyleHeader_CreatedDate).Skip((int)_Skip).Take((int)_Take).ToList();
                    else
                        return context.StyleHeaders.OrderByDescending(x => x.StyleHeader_CreatedDate).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}