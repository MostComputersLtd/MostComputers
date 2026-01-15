using MOSTComputers.Services.DataAccess.Documents.Models;
using MOSTComputers.Services.DataAccess.Documents.Models.DAO;

namespace MOSTComputers.Services.DataAccess.Documents.Mapping;
internal static class ResponseMapper
{
    internal static Invoice Map(InvoiceDAO invoiceDao)
    {
        return new()
        {
            ExportId = invoiceDao.ExportId,
            ExportDate = invoiceDao.ExportDate,
            ExportUserId = invoiceDao.ExportUserId,
            ExportUser = invoiceDao.ExportUser,
            InvoiceId = invoiceDao.InvoiceId,
            FirmId = invoiceDao.FirmId,
            CustomerBID = invoiceDao.CustomerBID,
            InvoiceDirection = (InvoiceDirection?)invoiceDao.InvoiceDirection,
            CustomerName = invoiceDao.CustomerName,
            MPerson = invoiceDao.MPerson,
            CustomerAddress = invoiceDao.CustomerAddress,
            InvoiceDate = invoiceDao.InvoiceDate,
            VatPercent = invoiceDao.PDDC,
            UserName = invoiceDao.UserName,
            Status = invoiceDao.Status,
            InvoiceNumber = invoiceDao.InvoiceNumber,
            PayType = invoiceDao.PayType,
            RPerson = invoiceDao.RPerson,
            RDATE = invoiceDao.RDATE,
            Bulstat = invoiceDao.Bulstat,
            PratkaId = invoiceDao.PratkaId,
            InvType = invoiceDao.InvType,
            InvBasis = invoiceDao.InvBasis,
            RelatedInvoiceNumber = invoiceDao.RelatedInvoiceNumber,
            IsVATRegistered = invoiceDao.IsVATRegistered,
            PrintedNETAmount = invoiceDao.PrintedNETAmount,
            DueDate = invoiceDao.DueDate,
            BankNameAndId = invoiceDao.CustomerBankName,
            BankIBAN = invoiceDao.CustomerBankIBAN,
            CustomerBankBIC = invoiceDao.CustomerBankBIC,
            PaymentStatus = invoiceDao.PaymentStatus,
            PaymentStatusDate = invoiceDao.PaymentStatusDate,
            PaymentStatusUserName = invoiceDao.PaymentStatusUserName,
            InvoiceCurrency = invoiceDao.InvoiceCurrency,

            InvoiceItems = MapRange(invoiceDao.InvoiceItems),
        };
    }

    internal static InvoiceItem Map(InvoiceItemDAO invoiceItemDao)
    {
        return new()
        {
            ExportedItemId = invoiceItemDao.ExportedItemId,
            ExportId = invoiceItemDao.ExportId,
            IEID = invoiceItemDao.IEID,
            InvoiceId = invoiceItemDao.InvoiceId,
            Name = invoiceItemDao.Name,
            PriceInLeva = invoiceItemDao.PriceInLeva,
            Quantity = invoiceItemDao.Quantity,
            DisplayOrder = invoiceItemDao.DisplayOrder,
        };
    }

    internal static WarrantyCard Map(WarrantyCardDAO warrantyCardDAO)
    {
        return new()
        {
            ExportId = warrantyCardDAO.ExportId,
            ExportDate = warrantyCardDAO.ExportDate,
            ExportUserId = warrantyCardDAO.ExportUserId,
            ExportUser = warrantyCardDAO.ExportUser,
            OrderId = warrantyCardDAO.OrderId,
            CustomerBID = warrantyCardDAO.CustomerBID,
            CustomerName = warrantyCardDAO.CustomerName,
            WarrantyCardDate = warrantyCardDAO.WarrantyCardDate,
            WarrantyCardTerm = warrantyCardDAO.WarrantyCardTerm,

            WarrantyCardItems = MapRange(warrantyCardDAO.WarrantyCardItems),
        };
    }

    internal static WarrantyCardItem Map(WarrantyCardItemDAO warrantyCardItemDAO)
    {
        return new()
        {
            ExportedItemId = warrantyCardItemDAO.ExportedItemId,
            ExportId = warrantyCardItemDAO.ExportId,
            OrderId = warrantyCardItemDAO.OrderId,
            ProductId = warrantyCardItemDAO.ProductId,
            ProductName = warrantyCardItemDAO.ProductName,
            PriceInLeva = warrantyCardItemDAO.PriceInLeva,
            Quantity = warrantyCardItemDAO.Quantity,
            SerialNumber = warrantyCardItemDAO.SerialNumber,
            WarrantyCardItemTermInMonths = warrantyCardItemDAO.WarrantyCardItemTermInMonths,
            DisplayOrder = warrantyCardItemDAO.DisplayOrder,
        };
    }

    internal static List<Invoice> MapRange(IEnumerable<InvoiceDAO>? invoiceDAOs)
    {
        List<Invoice> output = new();

        if (invoiceDAOs is null) return output;

        foreach (InvoiceDAO invoiceDAO in invoiceDAOs)
        {
            output.Add(Map(invoiceDAO));
        }

        return output;
    }

    internal static List<InvoiceItem> MapRange(IEnumerable<InvoiceItemDAO>? invoiceItemDAOs)
    {
        List<InvoiceItem> output = new();

        if (invoiceItemDAOs is null) return output;

        foreach (InvoiceItemDAO invoiceItemDAO in invoiceItemDAOs)
        {
            output.Add(Map(invoiceItemDAO));
        }

        return output;
    }

    internal static List<WarrantyCard> MapRange(IEnumerable<WarrantyCardDAO>? warrantyCardDAOs)
    {
        List<WarrantyCard> output = new();

        if (warrantyCardDAOs is null) return output;

        foreach (WarrantyCardDAO warrantyCardDAO in warrantyCardDAOs)
        {
            output.Add(Map(warrantyCardDAO));
        }

        return output;
    }

    internal static List<WarrantyCardItem> MapRange(IEnumerable<WarrantyCardItemDAO>? warrantyCardItemDAOs)
    {
        List<WarrantyCardItem> output = new();

        if (warrantyCardItemDAOs is null) return output;

        foreach (WarrantyCardItemDAO warrantyCardItemDAO in warrantyCardItemDAOs)
        {
            output.Add(Map(warrantyCardItemDAO));
        }

        return output;
    }
}