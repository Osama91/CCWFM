namespace CCWFM.Helpers.Enums
{
    public enum ImageCondition
    {
        LoadedFromDb,
        NewAdded,
        LoadedAndChanged
    }

    public enum ApprovalStatus
    {
        PendingRequest = 0,
        Approved = 1,
        Rejected = 2,
        ProceedNext = 3,
        Canceled = 4,
    }

    public enum StyleTabes
    {
        Style = 0,
        StyleDetails = 1,
        RetailSpecDetails = 2,
        Rfq = 3,
        TeckPack = 4,
        SalesOrder = 5,
        Operation = 6,
        Bom = 7,
       // TnaRoute = 8,
        Tna=8,
        StyleSpec=9
    }

    public enum ApprovalOptions
    {
        Pending = 0, Approved = 1, Rejected = 2, RequestingChanges = 3
    }

    public enum ObjectMode
    {
        NewObject,
        LoadedFromDb,
        MarkedForDeletion,
        StandBy
    }

    public enum SalesOrderType
    {
        RetailPo = 1,
        SalesOrderPo = 2,
        Rfq = 3,
        AdvancedSampleRequest =4
    }

    public enum PermissionItemName
    {
        BrandSectionMail,
        StyleFabricComposition,

        CheckListGroup,
        CheckListItem,
        CheckListTransaction,
        CheckListLink,

        AttendanceFileReason,
        StyleCategory,

        StyleOrderScreen
        ,
        MissionTracker,
        DyeingPlanAccForm,
        JobPermissions
            ,

        MeasurementChart
            ,

        Product
            ,

        Operations
            ,

        DataSettings
            ,

        TrnsMngmnt
            ,

        RouteCardForm
            ,

        FabricPreparations
            ,

        FabricInspectionForm
            ,

        MarkerDetailsForm
            ,

        FabSetupForm
            ,
        FabSetupWFForm
            ,
        FabricAttributes
            ,

        YarnAttributes
            ,

        SizesForm
            ,

        ColorsForm
            ,

        FabricFinishesForm
            ,

        FabricMaterialsForm
            ,

        FabricStructuresForm
            ,

        FabricTypesForm
            ,

        YarnCountsForm
            ,

        YarnFinishesForm
            ,

        ContentsForm
            ,

        GaugesForm
            ,

        StatusesForm
            ,

        UoMsForm
            ,

        ThreadNumbersForm
            ,

        fabImgs
            ,

        AccGroupPerm
            ,

        DefectsMenu
            ,

        BarcodeSettingsMenu
        ,

        BarCodeSettingsForm
            ,

        DefectsForm
            ,

        PartsOfMsrmntsForm
            ,

        YarnSourcesForm,

        Currencies,
        CostTypes,
        BrandSection,
        ColorIdentifierForm,
        ColorGroupForm,
        DirectionForm,
        FactoryGroupForm,
        EmployeeBehalfForm,
        EmployeeShiftForm,
        UserJobs,
        EmployeeShiftTabForm,
        MissionTabForm,
        ExcuseRulesForm,
        VacationTabForm,
        AttFileTabForm,
        UserJobsForm,
        StyleCodingForm,
        BrandSectionPermissionForm,
        UserBrandsForm,
        PermissionsForm,
        SupplierFabric,
        StyleStatus,
        FamilyForm,
        SeasonForm,
        BrandSectionForm,
        ColorCodeForm,
        ColorLinkForm,
        DirectionLinkForm,
        DesignForm,
        ColorThemesForm,
        FactoryGroup,
        ServiceCodingForm,
        AccessoriesGroupForm,
        AccessoriesCodingForm,
        AccessoriesSizeGroupForm,
        RetailPoForm,
        CCPoForm,
        TradeAgreementForm,
        PaymentScheduleSettingsForm,
        PaymentScheduleForm,
        BankDepositForm,
        PromotionForm,
        EmployeeInfoForm,
        FabricDesignsForm,
        FamilyLinkForm,
        BrandBudget,

        AssetsForm,
        Memory,
        HardDisk,
        Processor,
        AssetsType,
        AssetsStatus,
        AssetsTransaction,
        SeasonalMasterListForm,
        RequestForSampleForm,
        CurrencyDailyExchangeForm,
        RouteCoding,
        Reservation,
        GlobalRetailBusinessBudget,
        DyeingPlanForm,
        DyeingOrderForm,
        StandardBomForm,
        GlobalCCBusinessBudget,
        CCBrandBudget,
        FingerPrintTransaction,
        BrandSectionMailSample,
        CheckListDesignGroupHeader1,
        CheckListDesignGroupHeader2,
        TransferMsg,
        BrandStoreTarget,
        CheckListMail,
        VariableTermManual,
        GeneratePurchase,
        Sequence,
        PeriodsGl,
        GroupAccountLink,
        Bank,
        Account,
        PostingProfile,
        LedgerHeader,
        Journal,
        RecInv,
        Markup,
        BankGroup,
        InventPosting,
        CostCenter,
        CostDimSetup,
        CostCenterType,
        CostCenterOption,
        GlRule,
        CostAllocationMethod,
        GlRuleJob,
        MethodOfPayment,
        BankTransactionType,        
        MarkupGroup,
        GlPosting,
        TradeAgreementFabricView,
        AssetGroup,
        Asset,
        ExpensesGroup,
        Expenses,
        ClosingPosting,
        IncomeStatmentDesign,
        IncomeStatment,
        BankTransactionTypeGroup,
        EmpLeavingTransaction,
        EmpWeeklyDayOff,
        BankDepositApproval,
        EmpLeavingTransactionForm,
        ShopReqHeader,
        BrandStoreTargetForManagement,
        Gps,
        EmpLeavingTransactionForManagment,
        GlExpensis,
        GlGenEntity,
        GlChequeTransaction,
        Depreciation,
        AssetGroupForm,
        FactoryEmpLeavingTransaction,
        IssueJournalForm,
        Documentation,
        SeasonCurrenciesForm,
        GeneratePurchaseHeaderCurrenciesForm,
        RecInvProd, PurchaseBudget,
        VariableTermManualFactory,
        RouteCardInvoice,

        DyeingOrderInvoice,
        PurchaseOrderRequest,
        SalesOrderRequest,
        TransferForm,
        Adjustment,
        TransferTo,
        SalesOrderRequestInvoice,
        ShopArea,
        ProductionOrder,
        ProductionInvoice,
        CashDeposit,
        BankStatement,
        Contracts,
        FactoryDelivery,
        SubContractor,
        StoreVisaMachine,
        PeriodLock,
        FixAtt,
        OpeningBalance,
        ClosingAdvanceVendorPayment,
        StyleTNAForm,
        StoreCommission,
        GlCashTransaction,
        SalaryApproval,
        PositionRoute,
        CostCenterOrganizationUnit,
        CostCenterRouteGroup,
        JournalSetting,
        NewUserRequestForm,
        ConfirmNewUserRequestForm,
        StyleTheme,
        BrandSectionFamily,
        CashdepositeTypeForm,
        VisaMachineForm
    }

    public enum BarcodeFormatEnum
    {
        // Summary:
        //     Aztec 2D barcode format.
        AZTEC,

        //
        // Summary:
        //     CODABAR 1D format.
        CODABAR,

        //
        // Summary:
        //     Code 39 1D format.
        CODE_39,

        //
        // Summary:
        //     Code 93 1D format.
        CODE_93,

        //
        // Summary:
        //     Code 128 1D format.
        CODE_128,

        //
        // Summary:
        //     Data Matrix 2D barcode format.
        DATA_MATRIX,

        //
        // Summary:
        //     EAN-8 1D format.
        EAN_8,

        //
        // Summary:
        //     EAN-13 1D format.
        EAN_13,

        //
        // Summary:
        //     ITF (Interleaved Two of Five) 1D format.
        ITF,

        //
        // Summary:
        //     MaxiCode 2D barcode format.
        MAXICODE,

        //
        // Summary:
        //     PDF417 format.
        PDF_417,

        //
        // Summary:
        //     QR Code 2D barcode format.
        QR_CODE,

        //
        // Summary:
        //     RSS 14
        RSS_14,

        //
        // Summary:
        //     RSS EXPANDED
        RSS_EXPANDED,

        //
        // Summary:
        //     UPC-A 1D format.
        UPC_A,

        //
        // Summary:
        //     UPC-E 1D format.
        UPC_E,

        //
        // Summary:
        //     UPC/EAN extension format. Not a stand-alone format.
        UPC_EAN_EXTENSION,

        //
        // Summary:
        //     MSI
        MSI,

        //
        // Summary:
        //     UPC_A | UPC_E | EAN_13 | EAN_8 | CODABAR | CODE_39 | CODE_93 | CODE_128 |
        //     ITF | RSS_14 | RSS_EXPANDED
        All_1D,

        //
        // Summary:
        //     Plessey
        PLESSEY,
        CashdepositeTypeForm,
        VisaMachineForm
    }

    public enum FormMode
    {
        Standby,
        Search,
        Add,
        Update,
        Read
    }
}