
SET NOCOUNT ON 
DECLARE @i INT = 2

WHILE @i < 1000000 
    BEGIN
        INSERT  [user]
        VALUES  ( concat(@i, '@', @i, '.com'),
                  '96e79218965eb72c92a549dd5a330112', @i, 1, GETDATE() ) 
        SET @i = @i + 1
    END


SELECT  MAX(username)
FROM    [user]

SELECT TOP 5
        *
FROM    [user]

SET NOCOUNT OFF