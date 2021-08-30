/*
    Colors
*/

----Load Colors

IF OBJECT_ID('TempDb..#backfillColor', 'U') IS NOT NULL
  DROP TABLE #backfillColor
;
SELECT
   [UId],
   [Name]
INTO
 #backfillColor
FROM
 dbo.[Color]
WHERE
 1 = 2
;
INSERT INTO #backfillColor
VALUES
    {colorValues}

MERGE[dbo].[Color] AS TARGET
USING #backfillColor AS SOURCE
ON TARGET.[Name] = SOURCE.[Name]
WHEN MATCHED THEN
   UPDATE SET
       TARGET.[UId] = SOURCE.[UId]

WHEN NOT MATCHED BY TARGET THEN
  INSERT (
      [UId],
      [Name]
  ) VALUES (
      SOURCE.[UId],
      SOURCE.[Name]
  )
;

GO