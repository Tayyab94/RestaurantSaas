using Microsoft.EntityFrameworkCore;
using RestaurantSaaS.Domain.Entities;

namespace RestaurantSaaS.Infrastructure.Persistence;

/// <summary>One DbSet per table of the schema (the identity sets are in AppDbContext.cs).</summary>
public sealed partial class AppDbContext
{
    // Platform
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<PlanFeature> PlanFeatures => Set<PlanFeature>();
    public DbSet<BusinessTypeTemplate> BusinessTypeTemplates => Set<BusinessTypeTemplate>();
    public DbSet<TemplateFeature> TemplateFeatures => Set<TemplateFeature>();
    public DbSet<TenantFeature> TenantFeatures => Set<TenantFeature>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<SubscriptionInvoice> SubscriptionInvoices => Set<SubscriptionInvoice>();
    public DbSet<SubscriptionPayment> SubscriptionPayments => Set<SubscriptionPayment>();
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Printer> Printers => Set<Printer>();
    public DbSet<PrinterRoute> PrinterRoutes => Set<PrinterRoute>();
    public DbSet<ReceiptTemplate> ReceiptTemplates => Set<ReceiptTemplate>();
    public DbSet<TaxRate> TaxRates => Set<TaxRate>();
    public DbSet<NumberSequence> NumberSequences => Set<NumberSequence>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<IntegrationSetting> IntegrationSettings => Set<IntegrationSetting>();
    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();
    public DbSet<WebhookSubscription> WebhookSubscriptions => Set<WebhookSubscription>();
    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<PlatformSetting> PlatformSettings => Set<PlatformSetting>();
    public DbSet<AppRelease> AppReleases => Set<AppRelease>();
    public DbSet<ImpersonationSession> ImpersonationSessions => Set<ImpersonationSession>();
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<OnboardingProgress> OnboardingProgresses => Set<OnboardingProgress>();
    public DbSet<DataExport> DataExports => Set<DataExport>();
    public DbSet<RestoreLog> RestoreLogs => Set<RestoreLog>();
    public DbSet<AppUpdateLog> AppUpdateLogs => Set<AppUpdateLog>();
    public DbSet<InstallationInfo> InstallationInfos => Set<InstallationInfo>();
    public DbSet<Translation> Translations => Set<Translation>();

    // Licensing
    public DbSet<LicenseSigningKey> LicenseSigningKeys => Set<LicenseSigningKey>();
    public DbSet<LicenseBatch> LicenseBatches => Set<LicenseBatch>();
    public DbSet<License> Licenses => Set<License>();
    public DbSet<LicenseActivation> LicenseActivations => Set<LicenseActivation>();
    public DbSet<LicenseTransfer> LicenseTransfers => Set<LicenseTransfer>();
    public DbSet<TrialRegistry> TrialRegistries => Set<TrialRegistry>();
    public DbSet<LicenseEvent> LicenseEvents => Set<LicenseEvent>();
    public DbSet<LicenseRecord> LicenseRecords => Set<LicenseRecord>();
    public DbSet<LicenseClockMark> LicenseClockMarks => Set<LicenseClockMark>();
    public DbSet<LicenseCheckLog> LicenseCheckLogs => Set<LicenseCheckLog>();
    public DbSet<LicenseRequest> LicenseRequests => Set<LicenseRequest>();
    public DbSet<LicenseReminder> LicenseReminders => Set<LicenseReminder>();
    public DbSet<LicenseAttempt> LicenseAttempts => Set<LicenseAttempt>();
    public DbSet<LicenseChange> LicenseChanges => Set<LicenseChange>();
    public DbSet<LegacyLicense> LegacyLicenses => Set<LegacyLicense>();

    // Identity
    public DbSet<UserBranch> UserBranches => Set<UserBranch>();
    public DbSet<UserPermissionOverride> UserPermissionOverrides => Set<UserPermissionOverride>();
    public DbSet<ApprovalLimit> ApprovalLimits => Set<ApprovalLimit>();
    public DbSet<VerificationCode> VerificationCodes => Set<VerificationCode>();
    public DbSet<UserTwoFactor> UserTwoFactors => Set<UserTwoFactor>();
    public DbSet<UserPreference> UserPreferences => Set<UserPreference>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<LoginHistory> LoginHistories => Set<LoginHistory>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<ErrorLog> ErrorLogs => Set<ErrorLog>();
    public DbSet<ApprovalLog> ApprovalLogs => Set<ApprovalLog>();

    // Menu
    public DbSet<MenuCategory> MenuCategories => Set<MenuCategory>();
    public DbSet<Station> Stations => Set<Station>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<ItemVariant> ItemVariants => Set<ItemVariant>();
    public DbSet<ModifierGroup> ModifierGroups => Set<ModifierGroup>();
    public DbSet<Modifier> Modifiers => Set<Modifier>();
    public DbSet<ItemModifierGroup> ItemModifierGroups => Set<ItemModifierGroup>();
    public DbSet<BranchMenuItem> BranchMenuItems => Set<BranchMenuItem>();
    public DbSet<Deal> Deals => Set<Deal>();
    public DbSet<DealItem> DealItems => Set<DealItem>();
    public DbSet<ItemBarcode> ItemBarcodes => Set<ItemBarcode>();
    public DbSet<MenuItemPriceHistory> MenuItemPriceHistories => Set<MenuItemPriceHistory>();
    public DbSet<Allergen> Allergens => Set<Allergen>();
    public DbSet<MenuItemAllergen> MenuItemAllergens => Set<MenuItemAllergen>();
    public DbSet<RawMaterialAllergen> RawMaterialAllergens => Set<RawMaterialAllergen>();

    // Floors
    public DbSet<Floor> Floors => Set<Floor>();
    public DbSet<RestaurantTable> RestaurantTables => Set<RestaurantTable>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<WaitingListEntry> WaitingListEntries => Set<WaitingListEntry>();
    public DbSet<QrOrderSession> QrOrderSessions => Set<QrOrderSession>();

    // Customers
    public DbSet<CustomerGroup> CustomerGroups => Set<CustomerGroup>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();
    public DbSet<CustomerContact> CustomerContacts => Set<CustomerContact>();
    public DbSet<CustomerLedgerEntry> CustomerLedgerEntries => Set<CustomerLedgerEntry>();
    public DbSet<CustomerAdvance> CustomerAdvances => Set<CustomerAdvance>();
    public DbSet<LoyaltyProgram> LoyaltyPrograms => Set<LoyaltyProgram>();
    public DbSet<LoyaltyTransaction> LoyaltyTransactions => Set<LoyaltyTransaction>();
    public DbSet<GiftCard> GiftCards => Set<GiftCard>();
    public DbSet<GiftCardTransaction> GiftCardTransactions => Set<GiftCardTransaction>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<CouponRedemption> CouponRedemptions => Set<CouponRedemption>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<PriceList> PriceLists => Set<PriceList>();
    public DbSet<PriceListItem> PriceListItems => Set<PriceListItem>();
    public DbSet<ConsolidatedInvoice> ConsolidatedInvoices => Set<ConsolidatedInvoice>();
    public DbSet<ConsolidatedInvoiceLine> ConsolidatedInvoiceLines => Set<ConsolidatedInvoiceLine>();
    public DbSet<CustomerMergeLog> CustomerMergeLogs => Set<CustomerMergeLog>();
    public DbSet<ImportJob> ImportJobs => Set<ImportJob>();

    // Orders
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderTable> OrderTables => Set<OrderTable>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderItemModifier> OrderItemModifiers => Set<OrderItemModifier>();
    public DbSet<OrderDiscount> OrderDiscounts => Set<OrderDiscount>();
    public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
    public DbSet<OrderNote> OrderNotes => Set<OrderNote>();
    public DbSet<OrderTransfer> OrderTransfers => Set<OrderTransfer>();
    public DbSet<DiscountPreset> DiscountPresets => Set<DiscountPreset>();
    public DbSet<ReasonCode> ReasonCodes => Set<ReasonCode>();
    public DbSet<RemakeRecord> RemakeRecords => Set<RemakeRecord>();
    public DbSet<ItemAvailabilityLog> ItemAvailabilityLogs => Set<ItemAvailabilityLog>();
    public DbSet<KdsScreen> KdsScreens => Set<KdsScreen>();
    public DbSet<KitchenTicket> KitchenTickets => Set<KitchenTicket>();
    public DbSet<KitchenTicketItem> KitchenTicketItems => Set<KitchenTicketItem>();
    public DbSet<KitchenPerformanceDaily> KitchenPerformanceDailies => Set<KitchenPerformanceDaily>();

    // Billing
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<InvoiceTax> InvoiceTaxes => Set<InvoiceTax>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Tip> Tips => Set<Tip>();
    public DbSet<PrintLog> PrintLogs => Set<PrintLog>();
    public DbSet<PrintJob> PrintJobs => Set<PrintJob>();
    public DbSet<FbrSubmission> FbrSubmissions => Set<FbrSubmission>();
    public DbSet<DailySalesSummary> DailySalesSummaries => Set<DailySalesSummary>();

    // Cash
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<CashMovement> CashMovements => Set<CashMovement>();
    public DbSet<CashDrawerLog> CashDrawerLogs => Set<CashDrawerLog>();
    public DbSet<DayClosing> DayClosings => Set<DayClosing>();

    // Delivery
    public DbSet<DeliveryArea> DeliveryAreas => Set<DeliveryArea>();
    public DbSet<Rider> Riders => Set<Rider>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<DeliveryStatusHistory> DeliveryStatusHistories => Set<DeliveryStatusHistory>();
    public DbSet<RiderSettlement> RiderSettlements => Set<RiderSettlement>();
    public DbSet<RiderEarning> RiderEarnings => Set<RiderEarning>();

    // Returns
    public DbSet<ReturnReason> ReturnReasons => Set<ReturnReason>();
    public DbSet<ReturnOrder> ReturnOrders => Set<ReturnOrder>();
    public DbSet<ReturnItem> ReturnItems => Set<ReturnItem>();

    // Inventory
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<UnitConversion> UnitConversions => Set<UnitConversion>();
    public DbSet<RawMaterialCategory> RawMaterialCategories => Set<RawMaterialCategory>();
    public DbSet<RawMaterial> RawMaterials => Set<RawMaterial>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeItem> RecipeItems => Set<RecipeItem>();
    public DbSet<RecipeStep> RecipeSteps => Set<RecipeStep>();
    public DbSet<StockLocation> StockLocations => Set<StockLocation>();
    public DbSet<BranchStock> BranchStocks => Set<BranchStock>();
    public DbSet<StockBatch> StockBatches => Set<StockBatch>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();
    public DbSet<StockAdjustmentItem> StockAdjustmentItems => Set<StockAdjustmentItem>();
    public DbSet<WastageRecord> WastageRecords => Set<WastageRecord>();
    public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();
    public DbSet<StockTransferItem> StockTransferItems => Set<StockTransferItem>();
    public DbSet<StockCount> StockCounts => Set<StockCount>();
    public DbSet<StockCountItem> StockCountItems => Set<StockCountItem>();
    public DbSet<ProductionRun> ProductionRuns => Set<ProductionRun>();
    public DbSet<PackagingRule> PackagingRules => Set<PackagingRule>();
    public DbSet<StockRequisition> StockRequisitions => Set<StockRequisition>();
    public DbSet<StockRequisitionItem> StockRequisitionItems => Set<StockRequisitionItem>();
    public DbSet<VarianceAnalysis> VarianceAnalysises => Set<VarianceAnalysis>();
    public DbSet<ReorderSuggestion> ReorderSuggestions => Set<ReorderSuggestion>();
    public DbSet<RecipeCostSnapshot> RecipeCostSnapshots => Set<RecipeCostSnapshot>();
    public DbSet<PrepList> PrepLists => Set<PrepList>();
    public DbSet<PrepListItem> PrepListItems => Set<PrepListItem>();
    public DbSet<ProductionPlan> ProductionPlans => Set<ProductionPlan>();
    public DbSet<ProductionPlanItem> ProductionPlanItems => Set<ProductionPlanItem>();
    public DbSet<ProductionPlanDemand> ProductionPlanDemands => Set<ProductionPlanDemand>();
    public DbSet<PrepLabel> PrepLabels => Set<PrepLabel>();

    // Purchasing
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<SupplierContact> SupplierContacts => Set<SupplierContact>();
    public DbSet<SupplierItem> SupplierItems => Set<SupplierItem>();
    public DbSet<SupplierItemPriceHistory> SupplierItemPriceHistories => Set<SupplierItemPriceHistory>();
    public DbSet<SupplierAdvance> SupplierAdvances => Set<SupplierAdvance>();
    public DbSet<SupplierEvaluation> SupplierEvaluations => Set<SupplierEvaluation>();
    public DbSet<SupplierLedgerEntry> SupplierLedgerEntries => Set<SupplierLedgerEntry>();
    public DbSet<SupplierPayment> SupplierPayments => Set<SupplierPayment>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public DbSet<GoodsReceipt> GoodsReceipts => Set<GoodsReceipt>();
    public DbSet<GoodsReceiptItem> GoodsReceiptItems => Set<GoodsReceiptItem>();
    public DbSet<PurchaseReturn> PurchaseReturns => Set<PurchaseReturn>();
    public DbSet<PurchaseReturnItem> PurchaseReturnItems => Set<PurchaseReturnItem>();

    // Staff
    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<WorkShift> WorkShifts => Set<WorkShift>();
    public DbSet<EmployeeSchedule> EmployeeSchedules => Set<EmployeeSchedule>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<Holiday> Holidays => Set<Holiday>();
    public DbSet<PayComponent> PayComponents => Set<PayComponent>();
    public DbSet<EmployeePayComponent> EmployeePayComponents => Set<EmployeePayComponent>();
    public DbSet<PayrollRun> PayrollRuns => Set<PayrollRun>();
    public DbSet<Payroll> Payrolls => Set<Payroll>();
    public DbSet<PayrollItem> PayrollItems => Set<PayrollItem>();
    public DbSet<PayAdjustment> PayAdjustments => Set<PayAdjustment>();
    public DbSet<TipPool> TipPools => Set<TipPool>();
    public DbSet<TipPoolShare> TipPoolShares => Set<TipPoolShare>();
    public DbSet<EmployeeLoan> EmployeeLoans => Set<EmployeeLoan>();
    public DbSet<LoanInstallment> LoanInstallments => Set<LoanInstallment>();
    public DbSet<LoanTransaction> LoanTransactions => Set<LoanTransaction>();

    // Accounting
    public DbSet<ChartOfAccount> ChartOfAccounts => Set<ChartOfAccount>();
    public DbSet<AccountMapping> AccountMappings => Set<AccountMapping>();
    public DbSet<AccountingPeriod> AccountingPeriods => Set<AccountingPeriod>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<JournalLine> JournalLines => Set<JournalLine>();
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<BankTransaction> BankTransactions => Set<BankTransaction>();
    public DbSet<Cheque> Cheques => Set<Cheque>();
    public DbSet<AccountingExport> AccountingExports => Set<AccountingExport>();

    // FoodSafety
    public DbSet<Equipment> EquipmentItems => Set<Equipment>();
    public DbSet<FoodSafetyChecklist> FoodSafetyChecklists => Set<FoodSafetyChecklist>();
    public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();
    public DbSet<ChecklistRun> ChecklistRuns => Set<ChecklistRun>();
    public DbSet<ChecklistRunItem> ChecklistRunItems => Set<ChecklistRunItem>();
    public DbSet<TemperatureLog> TemperatureLogs => Set<TemperatureLog>();
    public DbSet<FoodSafetyIncident> FoodSafetyIncidents => Set<FoodSafetyIncident>();

    // System
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationRecipient> NotificationRecipients => Set<NotificationRecipient>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<MessageLog> MessageLogs => Set<MessageLog>();
    public DbSet<MessageTemplate> MessageTemplates => Set<MessageTemplate>();
    public DbSet<BackupLog> BackupLogs => Set<BackupLog>();
    public DbSet<OwnerReportSchedule> OwnerReportSchedules => Set<OwnerReportSchedule>();
    public DbSet<PushToken> PushTokens => Set<PushToken>();
    public DbSet<IntegrationLog> IntegrationLogs => Set<IntegrationLog>();
    public DbSet<ExternalMenuMapping> ExternalMenuMappings => Set<ExternalMenuMapping>();
    public DbSet<WebhookDelivery> WebhookDeliveries => Set<WebhookDelivery>();
}
