namespace MOSTComputers.Services.DAL.Utils;

internal static class TableAndColumnNameUtils
{
    internal const string ProductsTableName = "dbo.MOSTPrices";

    internal const string CategoriesTableName = "dbo.Categories";
    internal const string ManifacturersTableName = "dbo.Manufacturer";

    internal const string FirstImagesTableName = "dbo.Images";
    internal const string AllImagesTableName = "dbo.ImagesAll";
    internal const string ImageFileNamesTableName = "dbo.ImageFileName";

    internal const string PropertiesTableName = "dbo.ProductXML";
    internal const string ProductCharacteristicsTableName = "dbo.ProductKeyword";

    internal const string PromotionsTableName = "dbo.Promotions";

    internal const string ProductStatusesTableName = "dbo.ProductStatuses";
    internal const string ProductWorkStatusesTableName = "dbo.TodoProductWorkStatuses";

    internal const string ExternalChangesTableName = "dbo.Changes";
    internal const string LocalChangesTableName = "dbo.Changes4Web";
    internal const string ToDoLocalChangesTableName = "dbo.TodoChanges4Web";

    internal const string FailedPropertyNamesOfProductsTableName = "dbo.FailedPropertyNamesOfProducts";

    internal const string ProductCharacteristicAndExternalXmlDataRelationsTableName = "[dbo].[ProductKeywordAndExternalXmlDataRelations]";
    internal const string FirstImagesTestingTableName = "dbo.TEST_Images";
    internal const string AllImagesTestingTableName = "dbo.TEST_ImagesAll";

    internal static class ProductsTable
    {
        internal const string IdColumnName = "CSTID";
        internal const string CategoryIdColumnName = "TID";
        internal const string NameColumnName = "CFGSUBTYPE";
        internal const string AdditionalWarrantyPriceColumnName = "ADDWRR";
        internal const string AdditionalWarrantyTermMonthsColumnName = "ADDWRRTERM";
        internal const string StandardWarrantyPriceColumnName = "ADDWRRDEF";
        internal const string StandardWarrantyTermMonthsColumnName = "DEFWRRTERM"; 
        internal const string DisplayOrderColumnName = "S";
        internal const string StatusColumnName = "OLD";
        internal const string PlShowColumnName = "PLSHOW";
        internal const string Price1ColumnName = "PRICE1";
        internal const string Price2Name = "PRICE2";
        internal const string Price3ColumnName = "PRICE3";
        internal const string CurrencyIdColumnName = "CurrencyId";
        internal const string RowGuidColumnName = "rowguid";
        internal const string PromotionIdColumnName = "PromPID";
        internal const string PromRidColumnName = "PromRID";
        internal const string PromotionPictureIdColumnName = "PromPictureID";
        internal const string PromotionExpireDateColumnName = "PromExpDate";
        internal const string AlertPictureIdColumnName = "AlertPictureID";
        internal const string AlertExpireDateColumnName = "AlertExpDate";
        internal const string PriceListDescriptionColumnName = "PriceListDescription";
        internal const string ManifacturerIdColumnName = "MfrID";
        internal const string SubcategoryIdColumnName = "SubcategoryID";
        internal const string PartNumber1ColumnName = "SPLMODEL";
        internal const string PartNumber2ColumnName = "SPLMODEL1";
        internal const string SearchStringColumnName = "SPLMODEL2";
    }

    internal static class CategoriesTable
    {
        internal const string IdColumnName = "CategoryID";
        internal const string DescriptionColumnName = "Description";
        internal const string IsLeafColumnName = "IsLeaf";
        internal const string DisplayOrderColumnName = "S";
        internal const string RowGuidColumnName = "rowguid";
        internal const string ProductsUpdateCounterColumnName = "ProductsUpdateCounter";
        internal const string ParentCategoryIdColumnName = "ParentId";
    }

    internal static class ManifacturersTable
    {
        internal const string IdColumnName = "MfrID";
        internal const string IdColumnAlias = "PersonalManifacturerId";

        internal const string RealCompanyNameColumnName = "Name";
        internal const string BGNameColumnName = "BGName";
        internal const string DisplayOrderColumnName = "ManifacturerDisplayOrder";
        internal const string ActiveColumnName = "Active";
    }

    internal static class FirstImagesTable
    {
        internal const string IdColumnName = "ID";
        internal const string IdColumnAlias = "ImageProductId";

        internal const string HtmlDataColumnName = "HtmlData";
        internal const string ImageDataColumnName = "Image";
        internal const string ImageContentTypeColumnName = "ImageFileExt";
        internal const string DateModifiedColumnName = "DateModified";
    }

    internal static class AllImagesTable
    {
        internal const string IdColumnName = "ID";
        internal const string IdColumnAlias = "ImagePrime";

        internal const string ProductIdColumnName = "CSTID";
        internal const string ProductIdColumnAlias = "ImageProductId";

        internal const string HtmlDataColumnName = "HtmlData";
        internal const string ImageDataColumnName = "Image";
        internal const string ImageContentTypeColumnName = "ImageFileExt";
        internal const string DateModifiedColumnName = "DateModified";
    }

    internal static class ImageFileNamesTable
    {
        internal const string ProductIdColumnName = "CSTID";
        internal const string DisplayOrderColumnName = "S";
        internal const string ImageNumberColumnName = "ImageNumber";
        internal const string FileNameColumnName = "ImgFileName";
        internal const string ActiveColumnName = "Active";
    }

    internal static class PropertiesTable
    {
        internal const string ProductIdColumnName = "CSTID";
        internal const string ProductIdColumnAlias = "PropertyProductId";

        internal const string ProductCharacteristicIdColumnName = "ProductKeywordID";

        internal const string DisplayOrderColumnName = "S";
        internal const string DisplayOrderColumnAlias = "PropertyDisplayOrder";

        internal const string CharacteristicColumnName = "Keyword";
        internal const string ValueColumnName = "KeywordValue";
        internal const string XmlPlacementColumnName = "Discr";
    }

    internal static class ProductCharacteristicsTable
    {
        internal const string IdColumnName = "ProductKeywordID";
        internal const string CategoryIdColumnName = "TID";
        internal const string NameColumnName = "Name";
        internal const string MeaningColumnName = "KeywordMeaning";
        internal const string DisplayOrderColumnName = "S";
        internal const string ActiveColumnName = "Active";
        internal const string PKUserIdColumnName = "PKUserID";
        internal const string LastUpdateColumnName = "LastUpdate";
        internal const string KWPrChColumnName = "KWPrCh";
    }

    internal static class PromotionsTable
    {
        internal const string IdColumnName = "PromotionID";
        internal const string NameColumnName = "PromotionName";
        internal const string PromotionAddedDateColumnName = "ChgDate";
        internal const string SourceColumnName = "PromSource";
        internal const string TypeColumnName = "PromType";
        internal const string StatusColumnName = "Status";
        internal const string SPOIDColumnName = "SPOID";
        internal const string DiscountEURColumnName = "PromotionEUR";
        internal const string DiscountUSDColumnName = "PromotionUSD";
        internal const string ActiveColumnName = "Active";
        internal const string StartDateColumnName = "StartDate";
        internal const string ExpirationDateColumnName = "ExpDate";
        internal const string MinimumQuantityColumnName = "MinQty";
        internal const string MaximumQuantityColumnName = "MaxQty";
        internal const string RequiredProductIdsColumnName = "RequiredCSTIDs";
        internal const string QuantityIncrementColumnName = "QtyIncrement";
        internal const string ExpQuantityColumnName = "ExpQty";
        internal const string SoldQuantityColumnName = "SoldQty";
        internal const string ConsignationColumnName = "Consignation";
        internal const string PointsColumnName = "Points";
        internal const string TimeStampColumnName = "Timestamp";
        internal const string ProductIdColumnName = "CSTID";
        internal const string CampaignIdColumnName = "CampaignID";
        internal const string RegistrationIdColumnName = "RegistrationID";
        internal const string PromotionVisualizationIdColumnName = "PromotionVisualizationId";
    }

    internal static class ProductStatusesTable
    {
        internal const string ProductIdColumnName = "CSTID";
        internal const string IsProcessedColumnName = "IsProcessed";
        internal const string NeedsToBeUpdatedColumnName = "NeedsToBeUpdated";
    }

    internal static class ProductWorkStatusesTable
    {
        internal const string IdColumnName = "ProductWorkStatusId";
        internal const string ProductIdColumnName = "ProductWorkStatusProductId";
        internal const string ProductNewStatusColumnName = "ProductNewStatus";
        internal const string ProductXmlStatusColumnName = "ProductXmlReadyStatus";
        internal const string ReadyForImageInsertColumnName = "ReadyForImageInsertStatus";
    }

    internal static class ExternalChangesTable
    {
        internal const string IdColumnName = "PK";
        internal const string IdColumnAlias = "ExternalChangePK";

        internal const string TableElementIdColumnName = "ID";
        internal const string TableElementIdColumnAlias = "ExternalChangeID";

        internal const string OperationTypeColumnName = "Operation";
        internal const string OperationTypeColumnAlias = "ExternalChangeOperation";

        internal const string TableNameColumnName = "TableName";
        internal const string TableNameColumnAlias = "ExternalChangeTableName";
    }

    internal static class LocalChangesTable
    {
        internal const string IdColumnName = "PK";
        internal const string IdColumnAlias = "LocalChangePK";

        internal const string TableElementIdColumnName = "ID";
        internal const string TableElementIdColumnAlias = "LocalChangeID";

        internal const string OperationTypeColumnName = "Operation";
        internal const string OperationTypeColumnAlias = "LocalChangeOperation";

        internal const string TableNameColumnName = "TableName";
        internal const string TableNameColumnAlias = "LocalChangeTableName";

        internal const string TimeStampColumnName = "TimeStamp";
        internal const string TimeStampColumnAlias = "LocalChangeTimeStamp";
    }

    internal static class ToDoLocalChangesTable
    {
        internal const string IdColumnName = "PK";
        internal const string IdColumnAlias = "LocalChangePK";

        internal const string TableElementIdColumnName = "ID";
        internal const string TableElementIdColumnAlias = "LocalChangeID";

        internal const string OperationTypeColumnName = "Operation";
        internal const string OperationTypeColumnAlias = "LocalChangeOperation";

        internal const string TableNameColumnName = "TableName";
        internal const string TableNameColumnAlias = "LocalChangeTableName";

        internal const string TimeStampColumnName = "TimeStamp";
        internal const string TimeStampColumnAlias = "LocalChangeTimeStamp";
    }

    internal static class FailedPropertyNamesOfProductsTable
    {
        internal const string ProductIdColumnName = "CSTID";
        internal const string PropertyNameColumnName = "PropertyName";
    }

    internal static class ProductCharacteristicAndExternalXmlDataRelationsTable
    {
        internal const string IdColumnName = "Id";
        internal const string CategoryIdColumnName = "TID";
        internal const string CategoryNameColumnName = "TIDName";
        internal const string ProductCharacteristicIdColumnName = "ProducKeywordID";
        internal const string ProductCharacteristicNameColumnName = "ProducKeywordName";
        internal const string ProductCharacteristicMeaningColumnName = "ProducKeywordMeaning";
        internal const string XmlNameColumnName = "XmlName";
        internal const string XmlDisplayOrderColumnName = "XmlDisplayOrder";
    }

    internal static class FirstImagesTestingTable
    {
        internal const string IdColumnName = "ID";
        internal const string IdColumnAlias = "ImageProductId";

        internal const string HtmlDataColumnName = "HtmlData";
        internal const string ImageDataColumnName = "Image";
        internal const string ImageContentTypeColumnName = "ImageFileExt";
        internal const string DateModifiedColumnName = "DateModified";
    }

    internal static class AllImagesTestingTable
    {
        internal const string IdColumnName = "ID";
        internal const string IdColumnAlias = "ImagePrime";

        internal const string ProductIdColumnName = "CSTID";
        internal const string ProductIdColumnAlias = "ImageProductId";

        internal const string HtmlDataColumnName = "HtmlData";
        internal const string ImageDataColumnName = "Image";
        internal const string ImageContentTypeColumnName = "ImageFileExt";
        internal const string DateModifiedColumnName = "DateModified";
    }
}