SELECT l.ListingId, l.SellerId, l.ListingCategory
FROM dbo.Listings AS l
LEFT JOIN dbo.Users AS u ON l.SellerId = u.UserId
LEFT JOIN dbo.Categories AS c ON l.ListingCategory = c.CategoryId
WHERE u.UserId IS NULL OR c.CategoryId IS NULL;