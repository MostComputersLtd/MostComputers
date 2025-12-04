namespace MOSTComputers.Services.DataAccess.Products.Utils;

internal static class TableAndColumnNameUtils
{
    internal const string ProductsTableName = "dbo.[MOSTPrices]";

    internal const string CategoriesTableName = "[dbo].[Categories]";
    internal const string SubCategoriesTableName = "[dbo].[ProductSubcategories]";
    internal const string ManufacturersTableName = "[dbo].[Manufacturer]";

    internal const string FirstImagesTableName = "[dbo].[Images]";
    internal const string AllImagesTableName = "[dbo].[ImagesAll]";
    internal const string ImageFileNamesTableName = "[dbo].[ImageFileName]";

    internal const string PropertiesTableName = "[dbo].[ProductXML]";
    internal const string ProductCharacteristicsTableName = "[dbo].[ProductKeyword]";

    internal const string PromotionsTableName = "[dbo].[Promotions]";

    internal const string PromotionGroupsTableName = "[dbo].[PromotionGroups]";
    internal const string ManufacturerToPromotionGroupRelationsTableName = "[dbo].[ManufacturerToPromotionGroupRelations]";
    internal const string GroupPromotionContentsTableName = "[dbo].[PromotionContents]";
    internal const string GroupPromotionImagesTableName = "[dbo].[PromotionImages]";
    internal const string GroupPromotionImageFileDatasTableName = "[dbo].[GroupPromotionImageFileData]";

    internal const string PromotionFilesTableName = "[dbo].[PromFiles]";
    internal const string PromotionProductFilesTableName = "[dbo].[PromProductRFiles]";

    internal const string ProductStatusesTableName = "[dbo].[ProductStatuses]";
    internal const string ProductWorkStatusesTableName = "[dbo].[TodoProductWorkStatuses]";

    internal const string ProductGTINCodesTableName = "[dbo].[ProductGTINCodes]";
    internal const string ProductSerialNumbersTableName = "[dbo].[ProductSerialNumbers]";

    internal const string ExternalChangesTableName = "[dbo].[Changes]";
    internal const string LocalChangesTableName = "[dbo].[Changes4Web]";
    internal const string ToDoLocalChangesTableName = "[dbo].[TodoChanges4Web]";

    internal const string ProductCharacteristicAndExternalXmlDataRelationsTableName = "[dbo].[ProductKeywordAndExternalXmlDataRelations]";
    internal const string ProductCharacteristicAndImageHtmlRelationsTableName = "[dbo].[ProductKeywordImageHtmlRelations]";

    internal const string ExchangeRatesTableName = "[dbo].[ExchangeRates]";
    internal const string SystemCountersTableName = "[dbo].[Counters]";

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
        internal const string Price2ColumnName = "PRICE2";
        internal const string Price3ColumnName = "PRICE3";
        internal const string CurrencyIdColumnName = "CurrencyId";
        internal const string RowGuidColumnName = "rowguid";
        internal const string PromotionPidColumnName = "PromPID";
        internal const string PromotionRidColumnName = "PromRID";
        internal const string PromotionPictureIdColumnName = "PromPictureID";
        internal const string PromotionExpireDateColumnName = "PromExpDate";
        internal const string AlertPictureIdColumnName = "AlertPictureID";
        internal const string AlertExpireDateColumnName = "AlertExpDate";
        internal const string PriceListDescriptionColumnName = "PriceListDescription";
        internal const string ManufacturerIdColumnName = "MfrID";
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

    internal static class SubCategoriesTable
    {
        internal const string IdColumnName = "SubcategoryID";

        internal const string CategoryIdColumnName = "TID";
        internal const string CategoryIdAlias = "SubCategoryTID";

        internal const string NameColumnName = "Name";
        internal const string NameAlias = "SubCategoryName";

        internal const string DisplayOrderColumnName = "S";
        internal const string DisplayOrderAlias = "SubCategoryS";

        internal const string ActiveColumnName = "Active";
        internal const string ActiveAlias = "SubCategoryActive";
    }

    internal static class ManufacturersTable
    {
        internal const string IdColumnName = "MfrID";
        internal const string IdColumnAlias = "PersonalManufacturerId";

        internal const string RealCompanyNameColumnName = "Name";
        internal const string BGNameColumnName = "BGName";
        internal const string DisplayOrderColumnName = "S";
        internal const string DisplayOrderColumnAlias = "ManufacturerDisplayOrder";
        internal const string ActiveColumnName = "Active";
    }

    internal static class FirstImagesTable
    {
        internal const string IdColumnName = "ID";
        internal const string IdColumnAlias = "ImageProductId";

        internal const string DescriptionColumnName = "Description";
        internal const string DescriptionColumnAlias = "HtmlData";

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

        internal const string DescriptionColumnName = "Description";
        internal const string DescriptionColumnAlias = "HtmlData";

        internal const string ImageDataColumnName = "Image";
        internal const string ImageContentTypeColumnName = "ImageFileExt";
        internal const string DateModifiedColumnName = "DateModified";

        internal const string CountColumnName = "ImagesAllCount";
    }

    internal static class ImageFileNamesTable
    {
        internal const string IdColumnName = "ID";
        internal const string ProductIdColumnName = "CSTID";
        internal const string DisplayOrderColumnName = "S";
        internal const string ImageIdColumnName = "ImageId";
        internal const string FileNameColumnName = "ImgFileName";
        internal const string ActiveColumnName = "Active";

        internal const string CreateUserNameColumnName = "CreateUserName";
        internal const string CreateDateColumnName = "CreateDate";
        internal const string LastUpdateUserNameColumnName = "LastUpdateUserName";
        internal const string LastUpdateDateColumnName = "LastUpdateDate";
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

        internal const string CountColumnName = "PropertiesCount";
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

    internal static class PromotionGroupsTable
    {
        internal const string IdColumnName = "Id";
        internal const string NameColumnName = "Name";
        internal const string HeaderColumnName = "Header";
        internal const string LogoColumnName = "Logo";
        internal const string DisplayOrderColumnName = "S";
        internal const string LogoContentTypeColumnName = "LogoContentType";
        internal const string IsDefaultColumnName = "IsDefault";
        internal const string ShowEmptyForLoggedColumnName = "ShowEmptyForLogged";
        internal const string ShowEmptyForNonLoggedColumnName = "ShowEmptyForNonLogged";
    }

    internal static class ManufacturerToPromotionGroupRelationsTable
    {
        internal const string ManufacturerIdColumnName = "ManufacturerId";
        internal const string PromotionGroupIdColumnName = "PromotionGroupId";
    }

    internal static class GroupPromotionContentsTable
    {
        internal const string IdColumnName = "Id";
        internal const string NameColumnName = "Name";
        internal const string GroupIdColumnName = "GroupId";
        internal const string HtmlContentColumnName = "Content";
        internal const string StartDateColumnName = "StartDate";
        internal const string ExpireDateColumnName = "ExpireDate";
        internal const string DisplayOrderColumnName = "S";
        internal const string DateModifiedColumnName = "DateModified";
        internal const string DisabledColumnName = "Disabled";
        internal const string RestrictedColumnName = "Restricted";
        internal const string MemberOfDefaultGroupColumnName = "MemberOfDefaultGroup";
        internal const string DefaultGroupPriorityColumnName = "DefaultGroupPriority";
    }

    internal static class GroupPromotionImagesTable
    {
        internal const string IdColumnName = "Id";
        internal const string PromotionIdColumnName = "PromotionId";
        internal const string ImageColumnName = "Image";
        internal const string ContentTypeColumnName = "ContentType";
    }

    internal static class GroupPromotionImageFileDatasTable
    {
        internal const string IdColumnName = "Id";
        internal const string ImageIdColumnName = "ImageId";
        internal const string PromotionIdColumnName = "PromotionId";
        internal const string FileNameColumnName = "FileName";
    }

    internal static class PromotionFilesTable
    {
        internal const string IdColumnName = "PrID";
        internal const string IdColumnAlias = "PromotionFileID";

        internal const string NameColumnName = "PromoName";

        internal const string ActiveColumnName = "Active";
        internal const string ActiveColumnAlias = "PromotionFileActive";

        internal const string ValidFromDateColumnName = "ValidFrom";
        internal const string ValidFromDateColumnAlias = "PromotionFileValidFrom";

        internal const string ValidToDateColumnName = "ValidTo";
        internal const string ValidToDateColumnAlias = "PromotionFileValidTo";

        internal const string FileNameColumnName = "FileName";

        internal const string DescriptionColumnName = "Description";
        internal const string RelatedProductsColumnName = "RelatedProducts";

        internal const string CreateUserNameColumnName = "CreateUserName";
        internal const string CreateUserNameColumnAlias = "PromotionFileCreateUserName";

        internal const string CreateDateColumnName = "CreateDate";
        internal const string CreateDateColumnAlias = "PromotionFileCreateDate";

        internal const string LastUpdateUserNameColumnName = "LastUpdateUserName";
        internal const string LastUpdateUserNameColumnAlias = "PromotionFileLastUpdateUserName";

        internal const string LastUpdateDateColumnName = "LastUpdateDate";
        internal const string LastUpdateDateColumnAlias = "PromotionFileLastUpdateDate";
    }

    internal static class PromotionProductFilesTable
    {
        internal const string IdColumnName = "PrCID";
        internal const string ProductIdColumnName = "CSTID";

        internal const string PromotionFileIdColumnName = "PrID";
        internal const string PromotionFileIdColumnAlias = "PromotionProductRelationFileId";

        internal const string ValidFromDateColumnName = "ValidFrom";
        internal const string ValidFromDateColumnAlias = "PromotionProductRelationValidFrom";

        internal const string ValidToDateColumnName = "ValidTo";
        internal const string ValidToDateColumnAlias = "PromotionProductRelationValidTo";

        internal const string ActiveColumnName = "Active";
        internal const string ActiveColumnAlias = "PromotionProductRelationActive";

        internal const string ImagesAllIdColumnName = "ImagesAllId";

        internal const string CreateUserNameColumnName = "CreateUserName";
        internal const string CreateUserNameColumnAlias = "PromotionProductRelationCreateUserName";

        internal const string CreateDateColumnName = "CreateDate";
        internal const string CreateDateColumnAlias = "PromotionProductRelationCreateDate";

        internal const string LastUpdateUserNameColumnName = "LastUpdateUserName";
        internal const string LastUpdateUserNameColumnAlias = "PromotionProductRelationLastUpdateUserName";

        internal const string LastUpdateDateColumnName = "LastUpdateDate";
        internal const string LastUpdateDateColumnAlias = "PromotionProductRelationLastUpdateDate";

        internal const string CountColumnName = "PromotionProductRFilesCount";
    }

    internal static class ProductStatusesTable
    {
        internal const string ProductIdColumnName = "CSTID";
        internal const string IsProcessedColumnName = "IsProcessed";
        internal const string NeedsToBeUpdatedColumnName = "NeedsToBeUpdated";
    }

    internal static class ProductWorkStatusesTable
    {
        internal const string IdColumnName = "Id";
        internal const string IdColumnAlias = "ProductWorkStatusId";
        
        internal const string ProductIdColumnName = "CSTID";
        internal const string ProductIdColumnAlias = "ProductWorkStatusProductId";

        internal const string ProductNewStatusColumnName = "ProductNewStatus";
        internal const string ProductXmlStatusColumnName = "ProductXmlReadyStatus";
        internal const string ReadyForImageInsertColumnName = "ReadyForImageInsertStatus";

        internal const string CreateUserNameColumnName = "CreateUserName";
        internal const string CreateDateColumnName = "CreateDate";
        internal const string LastUpdateUserNameColumnName = "LastUpdateUserName";
        internal const string LastUpdateDateColumnName = "LastUpdateDate";
    }

    internal static class ProductGTINCodesTable
    {
        internal const string ProductIdColumnName = "CSTID";
        internal const string CodeTypeColumnName = "CodeType";
        internal const string CodeTypeAsTextColumnName = "CodeTypeAsText";
        internal const string ValueColumnName = "Value";

        internal const string CreateUserNameColumnName = "CreateUserName";
        internal const string CreateDateColumnName = "CreateDate";
        internal const string LastUpdateUserNameColumnName = "LastUpdateUserName";
        internal const string LastUpdateDateColumnName = "LastUpdateDate";
    }

    internal static class ProductSerialNumbersTable
    {
        internal const string ProductIdColumnName = "CSTID";
        internal const string SerialNumberColumnName = "SerialNumber";
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

    internal static class ProductCharacteristicAndImageHtmlRelationsTable
    {
        internal const string IdColumnName = "Id";
        internal const string CategoryIdColumnName = "TID";
        internal const string ProductCharacteristicIdColumnName = "ProducKeywordID";
        internal const string HtmlNameColumnName = "HtmlName";
    }

    internal static class ExchangeRatesTable
    {
        internal const string CurrencyFromIdColumnName = "CurrencyFromId";
        internal const string CurrencyToIdColumnName = "CurrencyToId";
        internal const string RateColumnName = "Rate";
    }

    internal static class SystemCountersTable
    {
        internal const string OriginalChangesLastSearchedPKColumnName = "Changes4WebLastPK";
    }
}