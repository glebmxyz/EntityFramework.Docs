namespace NewInEfCore7;

public static class DocumentsContextStoredProcedures
{
    public static async Task CreateStoredProcedures(this DocumentsContext context)
    {
        switch (context.MappingStrategy)
        {
            case MappingStrategy.Tph:
                // Document
                await context.Database.ExecuteSqlRawAsync(
                    @"
CREATE PROCEDURE [dbo].[Document_Insert]
    @Discriminator [nvarchar](max),
    @Title [nvarchar](max),
    @NumberOfPages [int],
    @PublicationDate [datetime2],
    @CoverArt [varbinary](max),
    @Isbn [nvarchar](max),
    @CoverPrice [decimal](18,2),
    @IssueNumber [int],
    @EditorId [int]
AS
BEGIN
    INSERT INTO [Documents] ([Discriminator], [CoverArt], [NumberOfPages], [PublicationDate], [Title], [Isbn], [CoverPrice], [IssueNumber], [EditorId])
    OUTPUT INSERTED.[Id], INSERTED.[FirstRecordedOn], INSERTED.[RetrievedOn], INSERTED.[RowVersion]
    VALUES (@Discriminator, @CoverArt, @NumberOfPages, @PublicationDate, @Title, @Isbn, @CoverPrice, @IssueNumber, @EditorId);
END");

                await context.Database.ExecuteSqlRawAsync(
                    @"
CREATE PROCEDURE [dbo].[Document_Update]
    @Id [int],
    @Discriminator [nvarchar](max),
    @RowVersion_Original [rowversion],
    @Title [nvarchar](max),
    @NumberOfPages [int],
    @PublicationDate [datetime2],
    @CoverArt [varbinary](max),
    @Isbn [nvarchar](max),
    @CoverPrice [decimal](18,2),
    @IssueNumber [int],
    @EditorId [int],
    @FirstRecordedOn [datetime2],
    @RetrievedOn [datetime2] OUT,
    @RowVersion [rowversion] OUT
AS
BEGIN
    UPDATE [Documents] SET
        [Discriminator] = @Discriminator,
        [Title] = @Title,
        [NumberOfPages] = @NumberOfPages,
        [PublicationDate] = @PublicationDate,
        [CoverArt] = @CoverArt,
        [FirstRecordedOn] = @FirstRecordedOn,
        [Isbn] = @Isbn,
        [CoverPrice] = @CoverPrice,
        [IssueNumber] = @IssueNumber,
        [EditorId] = @EditorId,
        @RetrievedOn = [RetrievedOn],
        @RowVersion = [RowVersion]
    WHERE [Id] = @Id AND [RowVersion] = @RowVersion_Original
    SELECT @@ROWCOUNT;
END");

                await context.Database.ExecuteSqlRawAsync(
                    @"
CREATE PROCEDURE [dbo].[Document_Delete]
    @Id [int],
    @RowVersion_Original [rowversion]
AS
BEGIN
    DELETE FROM [Documents]
    OUTPUT 1
    WHERE [Id] = @Id AND [RowVersion] = @RowVersion_Original;
END");
                break;
            case MappingStrategy.Tpt:
                // Document
                await context.Database.ExecuteSqlRawAsync(
                    @"
CREATE PROCEDURE [dbo].[Document_Insert]
    @Title [nvarchar](max),
    @NumberOfPages [int],
    @PublicationDate [datetime2],
    @CoverArt [varbinary](max)
AS
BEGIN
    INSERT INTO [Documents] ([CoverArt], [NumberOfPages], [PublicationDate], [Title])
    OUTPUT INSERTED.[Id], INSERTED.[FirstRecordedOn], INSERTED.[RetrievedOn], INSERTED.[RowVersion]
    VALUES (@CoverArt, @NumberOfPages, @PublicationDate, @Title);
END");

                await context.Database.ExecuteSqlRawAsync(
                    @"
CREATE PROCEDURE [dbo].[Document_Update]
    @Id [int],
    @RowVersion_Original [rowversion],
    @Title [nvarchar](max),
    @NumberOfPages [int],
    @PublicationDate [datetime2],
    @CoverArt [varbinary](max),
    @FirstRecordedOn [datetime2],
    @RetrievedOn [datetime2] OUT,
    @RowVersion [rowversion] OUT
AS
BEGIN
    UPDATE [Documents] SET
        [Title] = @Title,
        [NumberOfPages] = @NumberOfPages,
        [PublicationDate] = @PublicationDate,
        [CoverArt] = @CoverArt,
        [FirstRecordedOn] = @FirstRecordedOn,
        @RetrievedOn = [RetrievedOn],
        @RowVersion = [RowVersion]
    WHERE [Id] = @Id AND [RowVersion] = @RowVersion_Original
    SELECT @@ROWCOUNT;
END");

                await context.Database.ExecuteSqlRawAsync(
                    @"
CREATE PROCEDURE [dbo].[Document_Delete]
    @Id [int],
    @RowVersion_Original [rowversion]
AS
BEGIN
    DELETE FROM [Documents]
    OUTPUT 1
    WHERE [Id] = @Id AND [RowVersion] = @RowVersion_Original;
END");

                // Book
                await context.Database.ExecuteSqlRawAsync(
                    @"
CREATE PROCEDURE [dbo].[Book_Insert]
    @Id [int],
    @Isbn [nvarchar](max)
AS
BEGIN
    INSERT INTO [Books] ([Id], [Isbn])
    VALUES (@Id, @Isbn);
END");

                await context.Database.ExecuteSqlRawAsync(
                    @"
CREATE PROCEDURE [dbo].[Book_Update]
    @Id [int],
    @Isbn [nvarchar](max)
AS
BEGIN
    UPDATE [Books] SET
        [Isbn] = @Isbn
    WHERE [Id] = @Id
    SELECT @@ROWCOUNT;
END");

                await context.Database.ExecuteSqlRawAsync(
                    @"
CREATE PROCEDURE [dbo].[Book_Delete]
    @Id [int]
AS
BEGIN
    DELETE FROM [Books]
    OUTPUT 1
    WHERE [Id] = @Id;
END");

                // Magazine
                await context.Database.ExecuteSqlRawAsync(
                    @"
CREATE PROCEDURE [dbo].[Magazine_Insert]
    @Id [int],
    @CoverPrice [decimal](18,2),
    @IssueNumber [int],
    @EditorId [int]
AS
BEGIN
    INSERT INTO [Magazines] ([Id], [CoverPrice], [IssueNumber], [EditorId])
    VALUES (@Id, @CoverPrice, @IssueNumber, @EditorId);
END");

                await context.Database.ExecuteSqlRawAsync(
                    @"
CREATE PROCEDURE [dbo].[Magazine_Update]
    @Id [int],
    @CoverPrice [decimal](18,2),
    @IssueNumber [int],
    @EditorId [int]
AS
BEGIN
    UPDATE [Magazines] SET
        [CoverPrice] = @CoverPrice,
        [IssueNumber] = @IssueNumber,
        [EditorId] = @EditorId
    WHERE [Id] = @Id
    SELECT @@ROWCOUNT;
END");

                await context.Database.ExecuteSqlRawAsync(
                    @"
CREATE PROCEDURE [dbo].[Magazine_Delete]
    @Id [int]
AS
BEGIN
    DELETE FROM [Magazines]
    OUTPUT 1
    WHERE [Id] = @Id;
END");
                break;
            case MappingStrategy.Tpc:
                // Book
                await context.Database.ExecuteSqlRawAsync(
                    @"
CREATE PROCEDURE [dbo].[Book_Insert]
    @Title [nvarchar](max),
    @NumberOfPages [int],
    @PublicationDate [datetime2],
    @CoverArt [varbinary](max),
    @Isbn [nvarchar](max)
AS
BEGIN
    INSERT INTO [Books] ([CoverArt], [Isbn], [NumberOfPages], [PublicationDate], [Title])
    OUTPUT INSERTED.[Id], INSERTED.[FirstRecordedOn], INSERTED.[RetrievedOn], INSERTED.[RowVersion]
    VALUES (@CoverArt, @Isbn, @NumberOfPages, @PublicationDate, @Title);
END");

                await context.Database.ExecuteSqlRawAsync(
                    @"
CREATE PROCEDURE [dbo].[Book_Update]
    @Id [int],
    @RowVersion_Original [rowversion],
    @Title [nvarchar](max),
    @NumberOfPages [int],
    @PublicationDate [datetime2],
    @CoverArt [varbinary](max),
    @FirstRecordedOn [datetime2],
    @Isbn [nvarchar](max),
    @RetrievedOn [datetime2] OUT,
    @RowVersion [rowversion] OUT
AS
BEGIN
    UPDATE [Books] SET
        [Title] = @Title,
        [NumberOfPages] = @NumberOfPages,
        [PublicationDate] = @PublicationDate,
        [CoverArt] = @CoverArt,
        [FirstRecordedOn] = @FirstRecordedOn,
        [Isbn] = @Isbn,
        @RetrievedOn = [RetrievedOn],
        @RowVersion = [RowVersion]
    WHERE [Id] = @Id AND [RowVersion] = @RowVersion_Original
    SELECT @@ROWCOUNT;
END");

                await context.Database.ExecuteSqlRawAsync(
                    @"
CREATE PROCEDURE [dbo].[Book_Delete]
    @Id [int],
    @RowVersion_Original [rowversion]
AS
BEGIN
    DELETE FROM [Books]
    OUTPUT 1
    WHERE [Id] = @Id AND [RowVersion] = @RowVersion_Original;
END");

                // Magazine
                await context.Database.ExecuteSqlRawAsync(
                    @"
CREATE PROCEDURE [dbo].[Magazine_Insert]
    @Title [nvarchar](max),
    @NumberOfPages [int],
    @PublicationDate [datetime2],
    @CoverArt [varbinary](max),
    @CoverPrice [decimal](18,2),
    @IssueNumber [int],
    @EditorId [int]
AS
BEGIN
    INSERT INTO [Magazines] ([CoverArt], [NumberOfPages], [PublicationDate], [Title], [CoverPrice], [IssueNumber], [EditorId])
    OUTPUT INSERTED.[Id], INSERTED.[FirstRecordedOn], INSERTED.[RetrievedOn], INSERTED.[RowVersion]
    VALUES (@CoverArt, @NumberOfPages, @PublicationDate, @Title, @CoverPrice, @IssueNumber, @EditorId);
END");

                await context.Database.ExecuteSqlRawAsync(
                    @"
CREATE PROCEDURE [dbo].[Magazine_Update]
    @Id [int],
    @RowVersion_Original [rowversion],
    @Title [nvarchar](max),
    @NumberOfPages [int],
    @PublicationDate [datetime2],
    @CoverArt [varbinary](max),
    @CoverPrice [decimal](18,2),
    @IssueNumber [int],
    @EditorId [int],
    @FirstRecordedOn [datetime2],
    @RetrievedOn [datetime2] OUT,
    @RowVersion [rowversion] OUT
AS
BEGIN
    UPDATE [Magazines] SET
        [Title] = @Title,
        [NumberOfPages] = @NumberOfPages,
        [PublicationDate] = @PublicationDate,
        [CoverArt] = @CoverArt,
        [FirstRecordedOn] = @FirstRecordedOn,
        [CoverPrice] = @CoverPrice,
        [IssueNumber] = @IssueNumber,
        [EditorId] = @EditorId,
        @RetrievedOn = [RetrievedOn],
        @RowVersion = [RowVersion]
    WHERE [Id] = @Id AND [RowVersion] = @RowVersion_Original
    SELECT @@ROWCOUNT;
END");

                await context.Database.ExecuteSqlRawAsync(
                    @"
CREATE PROCEDURE [dbo].[Magazine_Delete]
    @Id [int],
    @RowVersion_Original [rowversion]
AS
BEGIN
    DELETE FROM [Magazines]
    OUTPUT 1
    WHERE [Id] = @Id AND [RowVersion] = @RowVersion_Original;
END");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // Person
        await context.Database.ExecuteSqlRawAsync(
            @"
CREATE PROCEDURE [dbo].[Person_Insert]
    @Name [nvarchar](max)
AS
BEGIN
      INSERT INTO [People] ([Name])
      OUTPUT INSERTED.[Id]
      VALUES (@Name);
END");

        await context.Database.ExecuteSqlRawAsync(
            @"
CREATE PROCEDURE [dbo].[Person_Update]
    @Id [int],
    @Name_Original [nvarchar](max),
    @Name [nvarchar](max)
AS
BEGIN
    UPDATE [People] SET
        [Name] = @Name
    WHERE [Id] = @Id AND [Name] = @Name_Original
    SELECT @@ROWCOUNT
END");

        await context.Database.ExecuteSqlRawAsync(
            @"
CREATE PROCEDURE [dbo].[Person_Delete]
    @Id [int],
    @Name_Original [nvarchar](max)
AS
BEGIN
    DELETE FROM [People]
    OUTPUT 1
    WHERE [Id] = @Id AND [Name] = @Name_Original;
END");

        // Contacts
        await context.Database.ExecuteSqlRawAsync(
            @"
CREATE PROCEDURE [dbo].[Contacts_Insert]
    @PersonId [int],
    @Phone [nvarchar](max)
AS
BEGIN
      INSERT INTO [Contacts] ([PersonId], [Phone])
      VALUES (@PersonId, @Phone);
END");

        await context.Database.ExecuteSqlRawAsync(
            @"
CREATE PROCEDURE [dbo].[Contacts_Update]
    @PersonId [int],
    @Phone [nvarchar](max)
AS
BEGIN
    UPDATE [Contacts] SET
        [Phone] = @Phone
    WHERE [PersonId] = @PersonId
    SELECT @@ROWCOUNT;
END");

        await context.Database.ExecuteSqlRawAsync(
            @"
CREATE PROCEDURE [dbo].[Contacts_Delete]
    @PersonId [int]
AS
BEGIN
    DELETE FROM [Contacts]
    OUTPUT 1
    WHERE [PersonId] = @PersonId;
END");

        // Addresses
        await context.Database.ExecuteSqlRawAsync(
            @"
CREATE PROCEDURE [dbo].[Addresses_Insert]
    @ContactDetailsPersonId [int],
    @Street [nvarchar](max),
    @City [nvarchar](max),
    @Postcode [nvarchar](max),
    @Country [nvarchar](max)
AS
BEGIN
      INSERT INTO [Addresses] ([ContactDetailsPersonId], [City], [Country], [Postcode], [Street])
      VALUES (@ContactDetailsPersonId, @City, @Country, @Postcode, @Street);
END");

        await context.Database.ExecuteSqlRawAsync(
            @"
CREATE PROCEDURE [dbo].[Addresses_Update]
    @ContactDetailsPersonId [int],
    @Street [nvarchar](max),
    @City [nvarchar](max),
    @Postcode [nvarchar](max),
    @Country [nvarchar](max)
AS
BEGIN
    UPDATE [Addresses] SET
        [Street] = @Street,
        [City] = @City,
        [Postcode] = @Postcode,
        [Country] = @Country
    WHERE [ContactDetailsPersonId] = @ContactDetailsPersonId
    SELECT @@ROWCOUNT;
END");

        await context.Database.ExecuteSqlRawAsync(
            @"
CREATE PROCEDURE [dbo].[Addresses_Delete]
    @ContactDetailsPersonId [int]
AS
BEGIN
    DELETE FROM [Addresses]
    OUTPUT 1
    WHERE [ContactDetailsPersonId] = @ContactDetailsPersonId;
END");

        // BookPerson join table
        await context.Database.ExecuteSqlRawAsync(
            @"
CREATE PROCEDURE [dbo].[BookPerson_Insert]
    @AuthorsId [int],
    @PublishedWorksId [int]
AS
BEGIN
      INSERT INTO [BookPerson] ([AuthorsId], [PublishedWorksId])
      VALUES (@AuthorsId, @PublishedWorksId);
END");

        await context.Database.ExecuteSqlRawAsync(
            @"
CREATE PROCEDURE [dbo].[BookPerson_Delete]
    @AuthorsId [int],
    @PublishedWorksId [int]
AS
BEGIN
    DELETE FROM [BookPerson]
    OUTPUT 1
    WHERE [AuthorsId] = @AuthorsId AND [PublishedWorksId] = @PublishedWorksId;
END");
    }
}
