using System.Linq;
using _Proxy = CCWFM.CRUDManagerService;

namespace CCWFM.ViewModel.FabricToolsViewModels
{
    internal static class FabricSetupsViewModelMapper
    {
        #region [ Methods ]

        public static _Proxy.tbl_FabricAttriputes MapToModelObject(FabricSetupsViewModel objectToBeMapped)
        {
            var temp = new _Proxy.tbl_FabricAttriputes
            {
                Iserial = objectToBeMapped.Iserial,
                FabricDesignsID = objectToBeMapped.FabricDesignsID,
                FabricFinishesID = objectToBeMapped.FabricFinishesID,
                FabricID = objectToBeMapped.FabricID,
                FabricMaterialsID = objectToBeMapped.FabricMaterialsID,
                FabricStructuresID = objectToBeMapped.FabricStructuresID,
                FabricTypesID = objectToBeMapped.FabricTypesID,
                DyingClassificationID = objectToBeMapped.DyingClassificationID,
                ExpectedDyingLossMargin = objectToBeMapped.ExpectedDyingLossMargin,
                FabricCategoryID = objectToBeMapped.FabricCategoryID,
                FabricDescription = objectToBeMapped.FabricDescription,
                FabricDescriptionAR = objectToBeMapped.FabricDescriptionAR,
                HorizontalShrinkage = objectToBeMapped.HorizontalShrinkage,
                Notes = objectToBeMapped.Notes,
                Twist = objectToBeMapped.Twist,
                VerticalShrinkage = objectToBeMapped.VerticalShrinkage,
                HorizontalShrinkageMax = objectToBeMapped.HorizontalShrinkageMax,
                VerticalShrinkageMax = objectToBeMapped.VerticalShrinkageMax,
                WeightPerSquarMeterAfterWashMin = objectToBeMapped.WeightPerSquarMeterAfterWash,
                WeightPerSquarMeterAsRawMin = objectToBeMapped.WeightPerSquarMeterAsRaw,
                WeightPerSquarMeterBeforWashMin = objectToBeMapped.WeightPerSquarMeterBeforWash,
                DyedFabricWidthMin = objectToBeMapped.DyedFabricWidth,
                WidthAsRawMin = objectToBeMapped.WidthAsRaw,
                WeightPerSquarMeterAfterWashMax = objectToBeMapped.WeightPerSquarMeterAfterWashMax,
                WeightPerSquarMeterAsRawMax = objectToBeMapped.WeightPerSquarMeterAsRawMax,
                WeightPerSquarMeterBeforWashMax = objectToBeMapped.WeightPerSquarMeterBeforWashMax,
                DyedFabricWidthMax = objectToBeMapped.DyedFabricWidthMax,
                WidthAsRawMax = objectToBeMapped.WidthAsRawMax,
                UoMID = objectToBeMapped.UoMID,
                YarnCountID = objectToBeMapped.YarnCountID,
                YarnFinishesID = objectToBeMapped.YarnFinishesID,
                YarnSource = objectToBeMapped.YarnSourceID,
                GaugesID = objectToBeMapped.GaugesID,
                ThreadNumbersID = objectToBeMapped.ThreadNumbersID,
                SupplierRef = objectToBeMapped.SupplierRef,
                InchesID =
                    objectToBeMapped.InshesProperty != null ? (int?)objectToBeMapped.InshesProperty.Iserial : null,
                TubularWidth = objectToBeMapped.TubularWidth,
                NoteUpdatedDate = objectToBeMapped.NoteUpdatedDate,
                IsPartialDetails = objectToBeMapped.IsPartialDetails,
                Colored = objectToBeMapped.Colored,
                YarnStatusID = objectToBeMapped.YarnStatusID
            };

            return temp;
        }

        public static void MapToViewModelObject(FabricSetupsViewModel temp, _Proxy.tbl_FabricAttriputes objectToBeMapped)
        {
            temp.Iserial = objectToBeMapped.Iserial;
            temp.FabricCategoryID = objectToBeMapped.FabricCategoryID;
            temp.FabricDesignsID = objectToBeMapped.FabricDesignsID;
            temp.FabricFinishesID = objectToBeMapped.FabricFinishesID;
            temp.FabricID = objectToBeMapped.FabricID;
            temp.FabricMaterialsID = objectToBeMapped.FabricMaterialsID;
            temp.FabricStructuresID = objectToBeMapped.FabricStructuresID;
            temp.FabricTypesID = objectToBeMapped.FabricTypesID;
            temp.DyingClassificationID = objectToBeMapped.DyingClassificationID;
            temp.ExpectedDyingLossMargin = objectToBeMapped.ExpectedDyingLossMargin;
            temp.FabricDescription = objectToBeMapped.FabricDescription;
            temp.FabricDescriptionAR = objectToBeMapped.FabricDescriptionAR;
            temp.Notes = objectToBeMapped.Notes;
            temp.Twist = objectToBeMapped.Twist;
            temp.VerticalShrinkage = objectToBeMapped.VerticalShrinkage;
            temp.HorizontalShrinkage = objectToBeMapped.HorizontalShrinkage;
            temp.HorizontalShrinkageMax = objectToBeMapped.HorizontalShrinkageMax;
            temp.VerticalShrinkageMax = objectToBeMapped.VerticalShrinkageMax;
            temp.WeightPerSquarMeterAfterWash = objectToBeMapped.WeightPerSquarMeterAfterWashMin;
            temp.WeightPerSquarMeterAsRaw = objectToBeMapped.WeightPerSquarMeterAsRawMin;
            temp.WeightPerSquarMeterBeforWash = objectToBeMapped.WeightPerSquarMeterBeforWashMin;
            temp.WidthAsRaw = objectToBeMapped.WidthAsRawMin;
            temp.DyedFabricWidth = objectToBeMapped.DyedFabricWidthMin;

            temp.WeightPerSquarMeterAfterWashMax = objectToBeMapped.WeightPerSquarMeterAfterWashMax;
            temp.WeightPerSquarMeterAsRawMax = objectToBeMapped.WeightPerSquarMeterAsRawMax;
            temp.WeightPerSquarMeterBeforWashMax = objectToBeMapped.WeightPerSquarMeterBeforWashMax;
            temp.WidthAsRawMax = objectToBeMapped.WidthAsRawMax;
            temp.DyedFabricWidthMax = objectToBeMapped.DyedFabricWidthMax;

            temp.UoMID = objectToBeMapped.UoMID;
            temp.UoMProperty = temp.UoMList.FirstOrDefault(x => x.Iserial == objectToBeMapped.UoMID);
            temp.YarnCountID = objectToBeMapped.YarnCountID;
            temp.YarnFinishesID = objectToBeMapped.YarnFinishesID;
            temp.GaugesID = objectToBeMapped.GaugesID;
            temp.ThreadNumbersID = objectToBeMapped.ThreadNumbersID;

            temp.StatusID = objectToBeMapped.Status;
            temp.SupplierRef = objectToBeMapped.SupplierRef;
            temp.YarnSourceID = objectToBeMapped.YarnSource;
            temp.InshesProperty
                = temp.InshesList.FirstOrDefault(x => x.Iserial == objectToBeMapped.InchesID);
            temp.TubularWidth = objectToBeMapped.TubularWidth;
            temp.NoteUpdatedDate = objectToBeMapped.NoteUpdatedDate;
            temp.IsPartialDetails = objectToBeMapped.IsPartialDetails;
            temp.Colored = objectToBeMapped.Colored;
            temp.YarnStatusID = objectToBeMapped.YarnStatusID;
        }

        public static _Proxy.FabricContentsComposition MapToModelObject(ContentCompositionViewModel objectToBeMapped, int fabCategoryId, string fabCode)
        {
            var temp = new _Proxy.FabricContentsComposition
            {
                ContentID = objectToBeMapped.FabContentID,
                FabricCategoryID = fabCategoryId,
                FabricCode = fabCode,
                Percentage = float.Parse(objectToBeMapped.ContentPercentage.ToString()),
            };
            return temp;
        }

        public static ContentCompositionViewModel MapToViewModelObject(_Proxy.FabricContentsComposition objectToBeMapped)
        {
            var x = new ContentCompositionViewModel
            {
                FabContent =
                {
                    Iserial = objectToBeMapped.tbl_lkp_Contents.Iserial,
                    Aname = objectToBeMapped.tbl_lkp_Contents.Aname,
                    Ename = objectToBeMapped.tbl_lkp_Contents.Ename,
                    Code = objectToBeMapped.tbl_lkp_Contents.Code
                },
                FabContentID = objectToBeMapped.ContentID,
                ContentCompositionSerial = objectToBeMapped.Iserial,
                ContentPercentage = objectToBeMapped.Percentage,
            };
            return x;
        }

        public static GImageViewModel MapToViewModelObject(_Proxy.tbl_FabricImage objectToBeMapped, string fabricCode)
        {
            var temp = new GImageViewModel(fabricCode)
            {
                _ImageState = Helpers.Enums.ImageCondition.LoadedFromDb,
                G_FabricId = objectToBeMapped.FabricCode,
                G_Image = objectToBeMapped.FabImage,
                G_ImageDescreption = objectToBeMapped.ImageDesc,
                _GIserial = objectToBeMapped.Iserial
            };
            return temp;
        }

        public static _Proxy.tbl_FabricImage MapToModelObject(GImageViewModel objectToBeMapped)
        {
            var temp = new _Proxy.tbl_FabricImage
            {
                FabricCode = objectToBeMapped.G_FabricId,
                FabImage = objectToBeMapped.G_Image,
                ImageDesc = objectToBeMapped.G_ImageDescreption
            };
            if (objectToBeMapped._GIserial != null)
            {
                temp.Iserial = (int)objectToBeMapped._GIserial;
            }
            return temp;
        }

        #endregion [ Methods ]
    }
}