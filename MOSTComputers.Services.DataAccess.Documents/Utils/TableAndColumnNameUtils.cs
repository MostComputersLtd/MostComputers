namespace MOSTComputers.Services.DataAccess.Documents.Utils;
internal static class TableAndColumnNameUtils
{
    internal const string InvoicesTableName = "[dbo].[ExportedInvoices]";
    internal const string InvoiceItemsTableName = "[dbo].[ExportedInvoiceItems]";

    internal const string WarrantyCardsTableName = "[dbo].[ExportedWarrantyCards]";
    internal const string WarrantyCardItemsTableName = "[dbo].[ExportedWarrantyCardItems]";

    internal const string CustomerDataViewName = "[dbo].[ViewCustomersID]";

    internal const string FirmsTableName = "[dbo].[Firms]";

    internal static class InvoicesTable
    {
        internal const string ExportIdColumn = "ExportID";
        internal const string ExportDateColumn = "ExportDate";
        internal const string ExportUserIDColumn = "ExportUserID";
        internal const string ExportUserColumn = "ExportUser";
        internal const string InvoiceIdColumn = "INVID";
        internal const string FirmIdColumn = "FIRMID";
        internal const string CustomerBIDColumn = "BID";
        internal const string InvoiceDirectionColumn = "INVDIR";
        internal const string CustomerNameColumn = "CUSTOMER";
        internal const string MPersonColumn = "MPERSON";
        internal const string CustomerAddressColumn = "ADDRESS";
        internal const string InvoiceDateColumn = "INVDATE";
        internal const string PDDCColumn = "PDDC";
        internal const string UserNameColumn = "UserName";
        internal const string StatusColumn = "STATUS";
        internal const string InvoiceNumberColumn = "INVNO";
        internal const string PayTypeColumn = "PAYTYPE";
        internal const string RPersonColumn = "RPERSON";
        internal const string RDATEColumn = "RDATE";
        internal const string BulstatColumn = "BULSTAT";
        internal const string PratkaIdColumn = "PratkaID";
        internal const string InvTypeColumn = "InvType";
        internal const string InvBasisColumn = "InvBasis";
        internal const string RelatedInvNoColumn = "RelatedInvNo";
        internal const string IsVATRegisteredColumn = "IsVATRegistered";
        internal const string PrintedNETAmountColumn = "PrintedNETAmount";
        internal const string DueDateColumn = "DueDate";
        internal const string CustomerBankNameColumn = "BankName";
        internal const string CustomerBankIBANColumn = "BankIBAN";
        internal const string CustomerBankBICColumn = "BankBIC";
        internal const string PaymentStatusColumn = "PStatus";
        internal const string PaymentStatusDateColumn = "PStatusDate";
        internal const string PaymentStatusUserNameColumn = "PStatusUserName";
        internal const string InvoiceCurrencyColumn = "InvCrr";
    }

    internal static class InvoiceItemsTable
    {
        internal const string ExportedItemIdColumn = "ExportedItemID";

        internal const string ExportIdColumn = "ExportID";
        internal const string ExportIdAlias = "InvoiceItemExportID";

        internal const string IEIDColumn = "IEID";

        internal const string InvoiceIdColumn = "INVID";
        internal const string InvoiceIdAlias = "InvoiceItemINVID";

        internal const string NameColumn = "NAME";
        internal const string PriceInLevaColumn = "PRICELV";
        internal const string QuantityColumn = "QTY";
        internal const string DisplayOrderColumn = "S";
    }

    internal static class WarrantyCardsTable
    {
        internal const string ExportIdColumn = "ExportID";
        internal const string ExportDateColumn = "ExportDate";
        internal const string ExportUserIdColumn = "ExportUserID";
        internal const string ExportUserColumn = "ExportUser";
        internal const string OrderIdColumn = "OrdID";
        internal const string CustomerBIDColumn = "BID";
        internal const string CustomerNameColumn = "CName";
        internal const string WarrantyCardDateColumn = "WCDate";
        internal const string WarrantyCardTermColumn = "WCTerm";
    }

    internal static class WarrantyCardItemsTable
    {
        internal const string ExportedItemIdColumn = "ExportedItemID";

        internal const string ExportIdColumn = "ExportID";
        internal const string ExportIdAlias = "WarrantyCardItemExportID";

        internal const string OrderIdColumn = "OrdID";
        internal const string OrderIdAlias = "WarrantyCardItemOrdID";

        internal const string ProductIdColumn = "CSTID";
        internal const string ProductNameColumn = "ProductName";
        internal const string PriceInLevaColumn = "PRICELV";
        internal const string QuantityColumn = "QTY";
        internal const string SerialNumberColumn = "SN";

        internal const string WarrantyCardItemTermInMonthsColumn = "WCTerm";
        internal const string WarrantyCardItemTermInMonthsAlias = "WarrantyCardItemWCTerm";

        internal const string DisplayOrderColumn = "S";
    }

    internal static class CustomerDataView
    {
        internal const string IdColumn = "BID";
        internal const string NameColumn = "Name";
        internal const string ContactPersonNameColumn = "ContactPerson";
        internal const string CountryColumn = "Country";
        internal const string AddressColumn = "Address";
        internal const string EmployeeIdColumn = "EmployeeID";
    }

    internal static class FirmsTable
    {
        internal const string IdColumn = "FIRMID";
        internal const string NameColumn = "NAME";
        internal const string OrderColumn = "S";
        internal const string InfoColumn = "INFO";
        internal const string InvoiceNumberColumn = "CINVNO";
        internal const string AddressColumn = "ADDRESS";
        internal const string MPersonColumn = "MPERSON";
        internal const string TaxNumberColumn = "DNO";
        internal const string BulstatColumn = "BULSTAT";
    }
}