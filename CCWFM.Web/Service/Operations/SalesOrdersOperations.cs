using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using _Model = CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        /// <summary>
        /// Pass null to all parameters to get all sales orders(Filtered by style header
        /// </summary>
        /// <param name="_Skip"></param>
        /// <param name="_Take"></param>
        /// <returns></returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<_Model.StyleHeader_SalesOrder> GetSalesOrders(int? _Skip, int? _Take, string _StyleHeader)
        {
            using (var context = new _Model.WorkFlowManagerDBEntities())
            {
                if (_Skip != null && _Take != null)
                    return context.StyleHeader_SalesOrder
                        .Where(x => x.StyleHeader == _StyleHeader)
                        .OrderByDescending(x => x.SalesOrderCreationDate)
                        .Skip((int)_Skip)
                        .Take((int)_Take)
                        .ToList();
                else
                    return context.StyleHeader_SalesOrder
                        .Where(x => x.StyleHeader == _StyleHeader)
                        .OrderByDescending(x => x.SalesOrderCreationDate)
                        .ToList();
            }
        }

        /// <summary>
        /// Pass null to all parameters to get all Sales Order Color
        /// </summary>
        /// <param name="_Skip"></param>
        /// <param name="_Take"></param>
        /// <returns></returns>
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<_Model.StyleHeader_SalesOrder_Colors> GetSalesOrderColors(int? _Skip, int? _Take, string _SalesOrder)
        {
            using (var context = new _Model.WorkFlowManagerDBEntities())
            {
                if (_Skip != null && _Take != null)
                    return context.StyleHeader_SalesOrder_Colors
                        .Where(x => x.StyleHeader_Colors_SalesOrder == _SalesOrder)
                        .OrderBy(x => x.StyleHeader_Colors_Index)
                        .Skip((int)_Skip)
                        .Take((int)_Take)
                        .ToList();
                else
                    return context.StyleHeader_SalesOrder_Colors
                        .Where(x => x.StyleHeader_Colors_SalesOrder == _SalesOrder)
                        .OrderBy(x => x.StyleHeader_Colors_Index)
                        .ToList();
            }
        }
    }
}