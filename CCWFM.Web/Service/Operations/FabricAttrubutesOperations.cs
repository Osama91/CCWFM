using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using CCWFM.Web.Model;

namespace CCWFM.Web.Service
{
    public partial class CRUD_ManagerService
    {
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<tbl_FabricAttriputes> GetAllFabAttributes()
        {
            using (WorkFlowManagerDBEntities context = new WorkFlowManagerDBEntities())
            {
                var _FabAttr = context.tbl_FabricAttriputes.ToList();
                if (_FabAttr != null)
                {
                    return _FabAttr;
                }
                return null;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<AccConfiguration> GetAccessoriesConfigurationsByDataArea(string _DataAreaID)
        {
            using (WorkFlowManagerDBEntities context = new WorkFlowManagerDBEntities())
            {
                var _Res = context.AccConfigurations.Where(x => x.DATAAREAID == _DataAreaID).ToList();
                if (_Res != null)
                {
                    return _Res;
                }
                return null;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<AccSizeConfig> GetAccessoriesSizeConfigurationsByDataArea(string _DataAreaID)
        {
            using (WorkFlowManagerDBEntities context = new WorkFlowManagerDBEntities())
            {
                var _Res = context.AccSizeConfigs.Where(x => x.DATAAREAID == _DataAreaID).ToList();
                if (_Res != null)
                {
                    return _Res;
                }
                return null;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<V_FabAttr> GetFabAttributesByCategory(int _FabricCategoryID)
        {
            using (WorkFlowManagerDBEntities context = new WorkFlowManagerDBEntities())
            {
                var _FabAttr = context.V_FabAttr
                    .Where(x => x.FabricCategoryID == _FabricCategoryID).ToList();
                if (_FabAttr != null)
                {
                    return _FabAttr;
                }
                return null;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public tbl_FabricAttriputes GetFabAttributes(string _FabID, int _FabCategory)
        {
            using (WorkFlowManagerDBEntities context = new WorkFlowManagerDBEntities())
            {
                var _FabAttr =
                    context.tbl_FabricAttriputes
                    .Where(x => x.FabricID == _FabID && x.FabricCategoryID == _FabCategory)
                    .SingleOrDefault();

                if (_FabAttr != null)
                {
                    return _FabAttr;
                }
                return null;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void AddFabCompositions(List<FabricContentsComposition> FabricAttrCompositionObj)
        {
            using (WorkFlowManagerDBEntities context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    foreach (var item in FabricAttrCompositionObj)
                    {
                        context.FabricContentsCompositions.AddObject(item);
                    }
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<FabricContentsComposition> GetFabContentCompositions(string _FabID, int _FabCategory)
        {
            using (WorkFlowManagerDBEntities context = new WorkFlowManagerDBEntities())
            {
                var _FabAttr = context.FabricContentsCompositions
                    .Where(x => x.FabricCode == _FabID && x.FabricCategoryID == _FabCategory).ToList();
                if (_FabAttr != null)
                {
                    return _FabAttr;
                }
                return null;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public string AddFabAttributes(tbl_FabricAttriputes FabricAttrObj, int _FabCategoryID, bool OverrideCheck)
        {
            using (WorkFlowManagerDBEntities context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    tbl_FabricCodingConfigurations _FabConfigs =
                        context.tbl_FabricCodingConfigurations
                        .Where(x => x.FabricCategoryID == _FabCategoryID).SingleOrDefault();
                    if (_FabCategoryID == 4 || _FabCategoryID == 5)
                    {
                        FabricAttrObj.FabricID = FabricAttrObj.FabricID + "D";
                        tbl_FabricAttriputes temp = (from x in context.tbl_FabricAttriputes
                                                     where x.FabricCategoryID == _FabCategoryID
                                                     && x.FabricID == FabricAttrObj.FabricID
                                                     select x).SingleOrDefault();
                        if (temp != null)
                        {
                            throw new Exception("Warning Code: FS-SIEX-1\n The data you are trying to add already exists in the database!");
                        }
                    }
                    else
                    {
                        FabricAttrObj.FabricID = _FabConfigs.NextFabID.ToString();
                        _FabConfigs.NextFabID += 1;
                    }
                    var tempQuery = new List<tbl_FabricAttriputes>();
                    if (!OverrideCheck)
                    {
                        tempQuery = (from x in context.tbl_FabricAttriputes.ToList()
                                     where x.DyedFabricWidthMin == FabricAttrObj.DyedFabricWidthMin
                                     && x.DyedFabricWidthMax == FabricAttrObj.DyedFabricWidthMax
                                     && x.DyingClassificationID == FabricAttrObj.DyingClassificationID
                                     && x.ExpectedDyingLossMargin == FabricAttrObj.ExpectedDyingLossMargin
                                     && x.FabricCategoryID == _FabCategoryID
                                     && x.FabricDescription == FabricAttrObj.FabricDescription
                                     && x.FabricDesignsID == FabricAttrObj.FabricDesignsID
                                     && x.FabricFinishesID == FabricAttrObj.FabricFinishesID
                                     && x.FabricMaterialsID == FabricAttrObj.FabricMaterialsID
                                     && x.FabricStructuresID == FabricAttrObj.FabricStructuresID
                                     && x.FabricTypesID == FabricAttrObj.FabricTypesID
                                     && x.GaugesID == FabricAttrObj.GaugesID
                                     && x.HorizontalShrinkage == FabricAttrObj.HorizontalShrinkage
                                     && x.SupplierRef == FabricAttrObj.SupplierRef
                                     && x.ThreadNumbersID == FabricAttrObj.ThreadNumbersID
                                     && x.Twist == FabricAttrObj.Twist
                                     && x.UoMID == FabricAttrObj.UoMID
                                     && x.VerticalShrinkage == FabricAttrObj.VerticalShrinkage
                                     && x.WeightPerSquarMeterAfterWashMin == FabricAttrObj.WeightPerSquarMeterAfterWashMin
                                     && x.WeightPerSquarMeterAsRawMin == FabricAttrObj.WeightPerSquarMeterAsRawMin
                                     && x.WeightPerSquarMeterBeforWashMin == FabricAttrObj.WeightPerSquarMeterBeforWashMin
                                     && x.WidthAsRawMin == FabricAttrObj.WidthAsRawMin
                                     && x.WeightPerSquarMeterAfterWashMax == FabricAttrObj.WeightPerSquarMeterAfterWashMax
                                     && x.WeightPerSquarMeterAsRawMax == FabricAttrObj.WeightPerSquarMeterAsRawMax
                                     && x.WeightPerSquarMeterBeforWashMax == FabricAttrObj.WeightPerSquarMeterBeforWashMax
                                     && x.WidthAsRawMax == FabricAttrObj.WidthAsRawMax
                                     && x.YarnCountID == FabricAttrObj.YarnCountID
                                     && x.YarnFinishesID == FabricAttrObj.YarnFinishesID
                                     && x.YarnSource == FabricAttrObj.YarnSource
                                     && x.SupplierRef == FabricAttrObj.SupplierRef
                                     && x.InchesID == FabricAttrObj.InchesID
                                     select x).ToList();
                    }
                    if (tempQuery.Count < 1)
                    {
                        FabricAttrObj.Status = 1;
                        context.tbl_FabricAttriputes.AddObject(FabricAttrObj);
                        context.SaveChanges();
                        return FabricAttrObj.FabricID;
                    }
                    else
                    {
                        throw new
                            Exception
                            ("Warning Code: FS-SIEX-2\nThe combination you are trying to save for this category already exists for item \""
                            + tempQuery[0].FabricID + "\"");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void UpdateFabAttributes(tbl_FabricAttriputes FabricAttrObj)
        {
            using (WorkFlowManagerDBEntities context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    tbl_FabricAttriputes temp = (from x in context.tbl_FabricAttriputes
                                                 where x.FabricID == FabricAttrObj.FabricID
                                                 && x.FabricCategoryID == FabricAttrObj.FabricCategoryID
                                                 select x).SingleOrDefault();
                    temp.FabricDesignsID = FabricAttrObj.FabricDesignsID;
                    temp.FabricFinishesID = FabricAttrObj.FabricFinishesID;
                    temp.FabricID = FabricAttrObj.FabricID;
                    temp.FabricMaterialsID = FabricAttrObj.FabricMaterialsID;
                    temp.FabricStructuresID = FabricAttrObj.FabricStructuresID;
                    temp.FabricTypesID = FabricAttrObj.FabricTypesID;
                    temp.DyingClassificationID = FabricAttrObj.DyingClassificationID;
                    temp.ExpectedDyingLossMargin = FabricAttrObj.ExpectedDyingLossMargin;
                    temp.FabricCategoryID = FabricAttrObj.FabricCategoryID;
                    temp.FabricDescription = FabricAttrObj.FabricDescription;
                    temp.HorizontalShrinkage = FabricAttrObj.HorizontalShrinkage;
                    temp.Notes = FabricAttrObj.Notes;
                    temp.Twist = FabricAttrObj.Twist;
                    temp.VerticalShrinkage = FabricAttrObj.VerticalShrinkage;
                    temp.WeightPerSquarMeterAfterWashMin = FabricAttrObj.WeightPerSquarMeterAfterWashMin;
                    temp.WeightPerSquarMeterAsRawMin = FabricAttrObj.WeightPerSquarMeterAsRawMin;
                    temp.WeightPerSquarMeterBeforWashMin = FabricAttrObj.WeightPerSquarMeterBeforWashMin;
                    temp.WidthAsRawMin = FabricAttrObj.WidthAsRawMin;
                    temp.DyedFabricWidthMin = FabricAttrObj.DyedFabricWidthMin;
                    temp.WeightPerSquarMeterAfterWashMax = FabricAttrObj.WeightPerSquarMeterAfterWashMax;
                    temp.WeightPerSquarMeterAsRawMax = FabricAttrObj.WeightPerSquarMeterAsRawMax;
                    temp.WeightPerSquarMeterBeforWashMax = FabricAttrObj.WeightPerSquarMeterBeforWashMax;
                    temp.WidthAsRawMax = FabricAttrObj.WidthAsRawMax;
                    temp.DyedFabricWidthMax = FabricAttrObj.DyedFabricWidthMax;
                    temp.UoMID = FabricAttrObj.UoMID;
                    temp.YarnCountID = FabricAttrObj.YarnCountID;
                    temp.YarnFinishesID = FabricAttrObj.YarnFinishesID;
                    temp.GaugesID = FabricAttrObj.GaugesID;
                    temp.ThreadNumbersID = FabricAttrObj.ThreadNumbersID;
                    temp.SupplierRef = FabricAttrObj.SupplierRef;
                    temp.InchesID = FabricAttrObj.InchesID;
                    temp.Status = 3;
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void DeleteFabAttributes(tbl_FabricAttriputes FabricAttrObj)
        {
            using (WorkFlowManagerDBEntities context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    context.tbl_FabricAttriputes.DeleteObject(FabricAttrObj);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}