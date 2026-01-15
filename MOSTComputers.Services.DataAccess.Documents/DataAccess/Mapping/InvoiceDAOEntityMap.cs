using Dapper.FluentMap.Mapping;
using MOSTComputers.Services.DataAccess.Documents.Models.DAO;

namespace MOSTComputers.Services.DataAccess.Documents.DataAccess.Mapping;

using static MOSTComputers.Services.DataAccess.Documents.Utils.TableAndColumnNameUtils.InvoicesTable;

internal class InvoiceDAOEntityMap : EntityMap<InvoiceDAO>
{
    public InvoiceDAOEntityMap()
    {
        Map(x => x.ExportId).ToColumn(ExportIdColumn);
        Map(x => x.ExportDate).ToColumn(ExportDateColumn);
        Map(x => x.ExportUserId).ToColumn(ExportUserIDColumn);
        Map(x => x.ExportUser).ToColumn(ExportUserColumn);
        Map(x => x.InvoiceId).ToColumn(InvoiceIdColumn);
        Map(x => x.FirmId).ToColumn(FirmIdColumn);
        Map(x => x.CustomerBID).ToColumn(CustomerBIDColumn);
        Map(x => x.InvoiceDirection).ToColumn(InvoiceDirectionColumn);
        Map(x => x.CustomerName).ToColumn(CustomerNameColumn);
        Map(x => x.MPerson).ToColumn(MPersonColumn);
        Map(x => x.CustomerAddress).ToColumn(CustomerAddressColumn);
        Map(x => x.InvoiceDate).ToColumn(InvoiceDateColumn);
        Map(x => x.PDDC).ToColumn(PDDCColumn);
        Map(x => x.UserName).ToColumn(UserNameColumn);
        Map(x => x.Status).ToColumn(StatusColumn);
        Map(x => x.InvoiceNumber).ToColumn(InvoiceNumberColumn);
        Map(x => x.PayType).ToColumn(PayTypeColumn);
        Map(x => x.RPerson).ToColumn(RPersonColumn);
        Map(x => x.RDATE).ToColumn(RDATEColumn);
        Map(x => x.Bulstat).ToColumn(BulstatColumn);
        Map(x => x.PratkaId).ToColumn(PratkaIdColumn);
        Map(x => x.InvType).ToColumn(InvTypeColumn);
        Map(x => x.InvBasis).ToColumn(InvBasisColumn);
        Map(x => x.RelatedInvoiceNumber).ToColumn(RelatedInvNoColumn);
        Map(x => x.IsVATRegistered).ToColumn(IsVATRegisteredColumn);
        Map(x => x.PrintedNETAmount).ToColumn(PrintedNETAmountColumn);
        Map(x => x.DueDate).ToColumn(DueDateColumn);
        Map(x => x.CustomerBankName).ToColumn(CustomerBankNameColumn);
        Map(x => x.CustomerBankIBAN).ToColumn(CustomerBankIBANColumn);
        Map(x => x.CustomerBankBIC).ToColumn(CustomerBankBICColumn);
        Map(x => x.PaymentStatus).ToColumn(PaymentStatusColumn);
        Map(x => x.PaymentStatusDate).ToColumn(PaymentStatusDateColumn);
        Map(x => x.PaymentStatusUserName).ToColumn(PaymentStatusUserNameColumn);
        Map(x => x.InvoiceCurrency).ToColumn(InvoiceCurrencyColumn);

        Map(x => x.InvoiceItems).Ignore();
    }
}