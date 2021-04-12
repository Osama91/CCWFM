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
            using (var context = new WorkFlowManagerDBEntities())
            {
                var fabAttr = context.tbl_FabricAttriputes.ToList();
                return fabAttr;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<V_FabAttr> GetFabAttributesByCategory(int _FabricCategoryID)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var fabAttr = context.V_FabAttr
                    .Where(x => x.FabricCategoryID == _FabricCategoryID).ToList();

                return fabAttr;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public tbl_FabricAttriputes GetFabAttributes(string _FabID, int _FabCategory)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var fabAttr =
                    context.tbl_FabricAttriputes.Include("tbl_lkp_UoM").SingleOrDefault(x => x.FabricID == _FabID && x.FabricCategoryID == _FabCategory);
                return fabAttr;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public void AddFabCompositions(List<FabricContentsComposition> FabricAttrCompositionObj)
        {
            using (var context = new WorkFlowManagerDBEntities())
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
        public void AddFabWFCompositions(List<WF_FabricContentsComposition> FabricAttrCompositionObj)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    foreach (var item in FabricAttrCompositionObj)
                    {
                        context.WF_FabricContentsComposition.AddObject(item);
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
            using (var context = new WorkFlowManagerDBEntities())
            {
                var fabAttr = context.FabricContentsCompositions.Include("tbl_lkp_Contents")
                    .Where(x => x.FabricCode == _FabID && x.FabricCategoryID == _FabCategory).ToList();
                return fabAttr;
            }
        }
        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public List<WF_FabricContentsComposition> GetWFFabContentCompositions(string _FabID, int _FabCategory)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                var fabAttr = context.WF_FabricContentsComposition.Include("tbl_lkp_Contents")
                    .Where(x => x.FabricCode == _FabID ).ToList();
                return fabAttr;
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public string AddFabAttributes(tbl_FabricAttriputes FabricAttrObj, int _FabCategoryID, bool OverrideCheck)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    var fabConfigs =
                        context.tbl_FabricCodingConfigurations.SingleOrDefault(x => x.FabricCategoryID == _FabCategoryID);
                    if (_FabCategoryID == 4 || _FabCategoryID == 5)
                    {
                        FabricAttrObj.FabricID = FabricAttrObj.FabricID + "D";
                        var temp = (from x in context.tbl_FabricAttriputes
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
                        FabricAttrObj.FabricID = fabConfigs.NextFabID.ToString();
                        fabConfigs.NextFabID += 1;
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
                                           && x.HorizontalShrinkageMax == FabricAttrObj.HorizontalShrinkageMax
                                           && x.VerticalShrinkageMax == FabricAttrObj.VerticalShrinkageMax
                                           && x.WidthAsRawMax == FabricAttrObj.WidthAsRawMax
                                           && x.YarnCountID == FabricAttrObj.YarnCountID
                                           && x.YarnFinishesID == FabricAttrObj.YarnFinishesID
                                           && x.YarnSource == FabricAttrObj.YarnSource
                                           && x.YarnStatusID == FabricAttrObj.YarnStatusID
                                           && x.SupplierRef == FabricAttrObj.SupplierRef
                                           && x.InchesID == FabricAttrObj.InchesID
                                           && x.TubularWidth == FabricAttrObj.TubularWidth
                                           && x.NoteUpdatedDate == FabricAttrObj.NoteUpdatedDate
                                             && x.IsPartialDetails == FabricAttrObj.IsPartialDetails
                                            && x.Colored == FabricAttrObj.Colored
                                     select x).ToList();
                    }
                    if (tempQuery.Count < 1)
                    {
                        FabricAttrObj.Status = 1;
                        if (FabricAttrObj.FabricMaterialsID == null)
                        {
                            FabricAttrObj.FabricMaterialsID = 5;
                        }
                        context.tbl_FabricAttriputes.AddObject(FabricAttrObj);
                        context.SaveChanges();
                        return FabricAttrObj.FabricID;
                    }
                    throw new
                        Exception
                        ("Warning Code: FS-SIEX-2\nThe combination you are trying to save for this category already exists for item \""
                         + tempQuery[0].FabricID + "\"");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public bool UpdateFabAttributes(tbl_FabricAttriputes fabricAttrObj, int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    var temp = (from x in context.tbl_FabricAttriputes.Include("FabricContentsCompositions")
                                where x.FabricID == fabricAttrObj.FabricID
                                && x.FabricCategoryID == fabricAttrObj.FabricCategoryID
                                select x).SingleOrDefault();

                    //var inventTransExists = DeleteAxFabItem(temp, true, userIserial);
                    var inventTransExists = false;
                    if (!inventTransExists)
                    {
                        foreach (var item in temp.FabricContentsCompositions.ToList())
                        {
                            context.DeleteObject(item);
                        }
                        var fabCategoryTemp =
                        fabricAttrObj.FabricCategoryID == 3 ? "FINISHED" :
                     fabricAttrObj.FabricCategoryID == 2 ? "Knited" :
                     fabricAttrObj.FabricCategoryID == 4 ? "DyingKnited" :
                  fabricAttrObj.FabricCategoryID == 5 ? "DyingWoven" :
                      "YARN";
                        var fabricMaterialsProp =
                            context.tbl_lkp_FabricMaterials.FirstOrDefault(
                                x => x.Iserial == fabricAttrObj.FabricMaterialsID);
                        temp.Status = 3;
                        if (temp.Notes != fabricAttrObj.Notes)
                        {
                            temp.Notes = fabricAttrObj.Notes;
                            fabricAttrObj.NoteUpdatedDate = DateTime.Now;

                            context.tbl_FabricAttriputesNotesLog.AddObject(new tbl_FabricAttriputesNotesLog
                            {
                                FabricID = fabricAttrObj.FabricID,
                                NoteUpdatedDate = fabricAttrObj.NoteUpdatedDate,
                                Notes = fabricAttrObj.Notes,
                            });
                        }

                        GenericUpdate(temp, fabricAttrObj, context);

                        context.SaveChanges();
                        //InsertFabItem(fabricAttrObj, fabCategoryTemp, fabricMaterialsProp != null ? fabricMaterialsProp.Ename : null, userIserial);
                    }
                    return inventTransExists;

                    //temp.FabricDesignsID = FabricAttrObj.FabricDesignsID;
                    //temp.FabricFinishesID = FabricAttrObj.FabricFinishesID;
                    //temp.FabricID = FabricAttrObj.FabricID;
                    //temp.FabricMaterialsID = FabricAttrObj.FabricMaterialsID;
                    //temp.FabricStructuresID = FabricAttrObj.FabricStructuresID;
                    //temp.FabricTypesID = FabricAttrObj.FabricTypesID;
                    //temp.DyingClassificationID = FabricAttrObj.DyingClassificationID;
                    //temp.ExpectedDyingLossMargin = FabricAttrObj.ExpectedDyingLossMargin;
                    //temp.FabricCategoryID = FabricAttrObj.FabricCategoryID;
                    //temp.FabricDescription = FabricAttrObj.FabricDescription;
                    //temp.HorizontalShrinkage = FabricAttrObj.HorizontalShrinkage;
                    //temp.Notes = FabricAttrObj.Notes;
                    //temp.Twist = FabricAttrObj.Twist;
                    //temp.VerticalShrinkage = FabricAttrObj.VerticalShrinkage;
                    //temp.WeightPerSquarMeterAfterWashMin = FabricAttrObj.WeightPerSquarMeterAfterWashMin;
                    //temp.WeightPerSquarMeterAsRawMin = FabricAttrObj.WeightPerSquarMeterAsRawMin;
                    //temp.WeightPerSquarMeterBeforWashMin = FabricAttrObj.WeightPerSquarMeterBeforWashMin;
                    //temp.WidthAsRawMin = FabricAttrObj.WidthAsRawMin;
                    //temp.DyedFabricWidthMin = FabricAttrObj.DyedFabricWidthMin;
                    //temp.WeightPerSquarMeterAfterWashMax = FabricAttrObj.WeightPerSquarMeterAfterWashMax;
                    //temp.WeightPerSquarMeterAsRawMax = FabricAttrObj.WeightPerSquarMeterAsRawMax;
                    //temp.WeightPerSquarMeterBeforWashMax = FabricAttrObj.WeightPerSquarMeterBeforWashMax;
                    //temp.WidthAsRawMax = FabricAttrObj.WidthAsRawMax;
                    //temp.DyedFabricWidthMax = FabricAttrObj.DyedFabricWidthMax;
                    //temp.UoMID = FabricAttrObj.UoMID;
                    //temp.YarnCountID = FabricAttrObj.YarnCountID;
                    //temp.YarnFinishesID = FabricAttrObj.YarnFinishesID;
                    //temp.GaugesID = FabricAttrObj.GaugesID;
                    //temp.ThreadNumbersID = FabricAttrObj.ThreadNumbersID;
                    //temp.SupplierRef = FabricAttrObj.SupplierRef;
                    //temp.InchesID = FabricAttrObj.InchesID;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public bool UpdateWFFabAttributes(tbl_FabricAttriputes fabricAttrObj, int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    var temp = (from x in context.tbl_FabricAttriputes.Include("FabricContentsCompositions")
                                where x.FabricID == fabricAttrObj.FabricID
                                && x.FabricCategoryID == fabricAttrObj.FabricCategoryID
                                select x).SingleOrDefault();

                    //var inventTransExists = DeleteAxFabItem(temp, true, userIserial);
                    var inventTransExists = false;
                    if (!inventTransExists)
                    {
                        foreach (var item in temp.FabricContentsCompositions.ToList())
                        {
                            context.DeleteObject(item);
                        }
                        var fabCategoryTemp =
                        fabricAttrObj.FabricCategoryID == 3 ? "FINISHED" :
                     fabricAttrObj.FabricCategoryID == 2 ? "Knited" :
                     fabricAttrObj.FabricCategoryID == 4 ? "DyingKnited" :
                  fabricAttrObj.FabricCategoryID == 5 ? "DyingWoven" :
                      "YARN";
                        var fabricMaterialsProp =
                            context.tbl_lkp_FabricMaterials.FirstOrDefault(
                                x => x.Iserial == fabricAttrObj.FabricMaterialsID);
                        temp.Status = 3;
                        if (temp.Notes != fabricAttrObj.Notes)
                        {
                            temp.Notes = fabricAttrObj.Notes;
                            fabricAttrObj.NoteUpdatedDate = DateTime.Now;

                            context.tbl_FabricAttriputesNotesLog.AddObject(new tbl_FabricAttriputesNotesLog
                            {
                                FabricID = fabricAttrObj.FabricID,
                                NoteUpdatedDate = fabricAttrObj.NoteUpdatedDate,
                                Notes = fabricAttrObj.Notes,
                            });
                        }

                        GenericUpdate(temp, fabricAttrObj, context);

                        context.SaveChanges();
                        //InsertFabItem(fabricAttrObj, fabCategoryTemp, fabricMaterialsProp != null ? fabricMaterialsProp.Ename : null, userIserial);
                    }
                    return inventTransExists;

                    //temp.FabricDesignsID = FabricAttrObj.FabricDesignsID;
                    //temp.FabricFinishesID = FabricAttrObj.FabricFinishesID;
                    //temp.FabricID = FabricAttrObj.FabricID;
                    //temp.FabricMaterialsID = FabricAttrObj.FabricMaterialsID;
                    //temp.FabricStructuresID = FabricAttrObj.FabricStructuresID;
                    //temp.FabricTypesID = FabricAttrObj.FabricTypesID;
                    //temp.DyingClassificationID = FabricAttrObj.DyingClassificationID;
                    //temp.ExpectedDyingLossMargin = FabricAttrObj.ExpectedDyingLossMargin;
                    //temp.FabricCategoryID = FabricAttrObj.FabricCategoryID;
                    //temp.FabricDescription = FabricAttrObj.FabricDescription;
                    //temp.HorizontalShrinkage = FabricAttrObj.HorizontalShrinkage;
                    //temp.Notes = FabricAttrObj.Notes;
                    //temp.Twist = FabricAttrObj.Twist;
                    //temp.VerticalShrinkage = FabricAttrObj.VerticalShrinkage;
                    //temp.WeightPerSquarMeterAfterWashMin = FabricAttrObj.WeightPerSquarMeterAfterWashMin;
                    //temp.WeightPerSquarMeterAsRawMin = FabricAttrObj.WeightPerSquarMeterAsRawMin;
                    //temp.WeightPerSquarMeterBeforWashMin = FabricAttrObj.WeightPerSquarMeterBeforWashMin;
                    //temp.WidthAsRawMin = FabricAttrObj.WidthAsRawMin;
                    //temp.DyedFabricWidthMin = FabricAttrObj.DyedFabricWidthMin;
                    //temp.WeightPerSquarMeterAfterWashMax = FabricAttrObj.WeightPerSquarMeterAfterWashMax;
                    //temp.WeightPerSquarMeterAsRawMax = FabricAttrObj.WeightPerSquarMeterAsRawMax;
                    //temp.WeightPerSquarMeterBeforWashMax = FabricAttrObj.WeightPerSquarMeterBeforWashMax;
                    //temp.WidthAsRawMax = FabricAttrObj.WidthAsRawMax;
                    //temp.DyedFabricWidthMax = FabricAttrObj.DyedFabricWidthMax;
                    //temp.UoMID = FabricAttrObj.UoMID;
                    //temp.YarnCountID = FabricAttrObj.YarnCountID;
                    //temp.YarnFinishesID = FabricAttrObj.YarnFinishesID;
                    //temp.GaugesID = FabricAttrObj.GaugesID;
                    //temp.ThreadNumbersID = FabricAttrObj.ThreadNumbersID;
                    //temp.SupplierRef = FabricAttrObj.SupplierRef;
                    //temp.InchesID = FabricAttrObj.InchesID;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        public bool DeleteFabAttributes(tbl_FabricAttriputes fabricAttrObj, int userIserial)
        {
            using (var context = new WorkFlowManagerDBEntities())
            {
                try
                {
                    var temp = (from x in context.tbl_FabricAttriputes
                                where x.FabricID == fabricAttrObj.FabricID
                                && x.FabricCategoryID == fabricAttrObj.FabricCategoryID
                                select x).SingleOrDefault();
                    if (temp != null && !DeleteAxFabItem(temp, true, userIserial))
                    {
                        context.tbl_FabricAttriputes.DeleteObject(temp);
                        context.SaveChanges();
                    }
                    return DeleteAxFabItem(temp, true, userIserial);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}