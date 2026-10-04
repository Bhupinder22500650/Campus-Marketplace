

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE CategoryName = N'Books')
BEGIN
    INSERT INTO dbo.Categories (CategoryName, CategoryDescription)
    VALUES (N'Books', N'Textbooks and other books');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE CategoryName = N'Electronics')
BEGIN
    INSERT INTO dbo.Categories (CategoryName, CategoryDescription)
    VALUES (N'Electronics', N'Phones, computers and other electronics');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE CategoryName = N'Clothing')
BEGIN
    INSERT INTO dbo.Categories (CategoryName, CategoryDescription)
    VALUES (N'Clothing', N'Clothes, shoes and accessories');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE CategoryName = N'Furniture')
BEGIN
    INSERT INTO dbo.Categories (CategoryName, CategoryDescription)
    VALUES (N'Furniture', N'Desks, chairs and other furniture');
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE CategoryName = N'Other')
BEGIN
    INSERT INTO dbo.Categories (CategoryName, CategoryDescription)
    VALUES (N'Other', N'Items that do not fit another category');
END;

-- Check the categories after adding them.
SELECT CategoryId, CategoryName, CategoryDescription
FROM dbo.Categories
ORDER BY CategoryName;
