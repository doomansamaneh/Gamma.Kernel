
------------------- create schema --------------------
CREATE SCHEMA AST;
go
------------------- create test table ----------------
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Ast].[ProductGroup]') AND type in (N'U'))
BEGIN
CREATE TABLE [Ast].[ProductGroup](
	[Id] [uniqueidentifier] NOT NULL,
	[ParentId] [uniqueidentifier] NULL,
	[Code] [nvarchar](50) NOT NULL,
	[Title] [nvarchar](240) NOT NULL,
	[Comment] [nvarchar](2000) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [nvarchar](520) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](520) NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_ProductGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

SET ANSI_PADDING ON
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Ast].[ProductGroup]') AND name = N'IX_ProductGroup_Code')
CREATE UNIQUE NONCLUSTERED INDEX [IX_ProductGroup_Code] ON [Ast].[ProductGroup]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER INDEX [IX_ProductGroup_Code] ON [Ast].[ProductGroup] DISABLE
GO

SET ANSI_PADDING ON
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Ast].[ProductGroup]') AND name = N'IX_ProductGroup_Title')
CREATE UNIQUE NONCLUSTERED INDEX [IX_ProductGroup_Title] ON [Ast].[ProductGroup]
(
	[Title] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER INDEX [IX_ProductGroup_Title] ON [Ast].[ProductGroup] DISABLE
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Ast].[FK_ProductGroup_ProductGroup_Parent]') AND parent_object_id = OBJECT_ID(N'[Ast].[ProductGroup]'))
ALTER TABLE [Ast].[ProductGroup]  WITH CHECK ADD  CONSTRAINT [FK_ProductGroup_ProductGroup_Parent] FOREIGN KEY([ParentId])
REFERENCES [Ast].[ProductGroup] ([Id])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Ast].[FK_ProductGroup_ProductGroup_Parent]') AND parent_object_id = OBJECT_ID(N'[Ast].[ProductGroup]'))
ALTER TABLE [Ast].[ProductGroup] CHECK CONSTRAINT [FK_ProductGroup_ProductGroup_Parent]
GO


------------------- create test data -----------------
DECLARE @i INT = 1;

WHILE @i <= 100
BEGIN
    INSERT INTO ast.ProductGroup (Id, Code, Title, IsActive, CreatedBy, DateCreated, ModifiedBy, DateModified)
    VALUES
    (
        NEWID(),
        CONCAT('PG', RIGHT('000' + CAST(@i AS VARCHAR(3)), 3)),
        CONCAT('Product Group ', @i),
        1,
		N'{"id":"11111111-0000-0000-0000-111111111111","name":"System Admin","ip":"::1","userAgent":"SQL"}',
		GETDATE(),
		N'{"id":"11111111-0000-0000-0000-111111111111","name":"System Admin","ip":"::1","userAgent":"SQL"}',
		GETDATE()
    );

    SET @i += 1;
END