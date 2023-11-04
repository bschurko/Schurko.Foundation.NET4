IF (DB_ID('InMem') IS NULL )
BEGIN
	CREATE DATABASE [InMem]
	 ON PRIMARY   (Name='InMeme_Primary', FILENAME='C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\InMem_Primary.mdf', size = 8MB, FILEGROWTH = 8MB),
		FILEGROUP [FGMem] CONTAINS MEMORY_OPTIMIZED_DATA (NAME='InMem',FILENAME='C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\InMem')
		LOG ON (Name='InMeme_log', FILENAME='C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\Log\InMem_log.ldf', size = 16MB, FILEGROWTH = 8MB);
END
GO
USE [InMem]
GO
IF(object_id(N'MsgQueue') IS NOT NULL)
	DROP TABLE [dbo].[MsgQueue];
GO
CREATE TABLE [dbo].[MsgQueue]
( MessageId UniqueIdentifier NOT NULL,
  SegmentSeq TINYINT NOT NULL,
  Identifier CHAR(200) COLLATE Latin1_General_100_BIN2 NOT NULL
	INDEX IX_MsgQueue_Identifier NONCLUSTERED HASH WITH (BUCKET_COUNT=1000),
  SegmentMsg  VARCHAR(7500),
  InQueueTime DATETIME2 NOT NULL,
  ExpiryTime DATETIME2 NOT NULL,
  DeQueueTime DATETIME2  NULL,
  [State] TINYINT NOT NULL DEFAULT 0,
  CONSTRAINT PK_MsgQueue PRIMARY KEY NONCLUSTERED HASH ( MessageId, SegmentSeq) WITH (BUCKET_COUNT=10000)
) WITH (MEMORY_OPTIMIZED=ON, DURABILITY=SCHEMA_AND_DATA);

GO


IF(object_id(N'InMessageQueue') IS NOT NULL)
	DROP PROC [dbo].[InMessageQueue];
GO
CREATE PROC [dbo].[InMessageQueue] 
	@identifier CHAR(200),
	@message varchar(max) ,
	@lifeInQueueSeconds INT = 60,
	@maxLength INT = 7500
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON

	DECLARE @now DATETIME2, @expiryTime DATETIME2, @segmentSeq TINYINT, @startPos INT, @segmentMsg VARCHAR(7500),@length INT, @messageId UniqueIdentifier

	SELECT @now =SYSDATETIME(), @expiryTime = DATEADD(second, @lifeInQueueSeconds, SYSDATETIME()), 
	       @segmentSeq = 1, @startPos = 1, @length = LEN(@message), @messageId = UPPER(NEWID())
	

	BEGIN TRAN
		WHILE(@startPos <= @length)
		BEGIN
			SET @segmentMsg = SUBSTRING(@message, @startPos, @maxLength)
			INSERT INTO [dbo].[MsgQueue] (MessageId, SegmentSeq, Identifier, SegmentMsg, InQueueTime, ExpiryTime)
			VALUES (UPPER(@messageId), @segmentSeq, @identifier, @segmentMsg, @now, @expiryTime)

			SELECT @segmentSeq += 1,  @startPos += @maxLength
		END	    
	COMMIT TRAN

	SELECT UPPER(@messageId) AS MessageId

END
 


IF(object_id(N'DeMessageQueue') IS NOT NULL)
	DROP PROC [dbo].[DeMessageQueue];
GO
CREATE PROC [dbo].[DeMessageQueue] 
	@identifier CHAR(200),
	@numberOfMessage INT = 1,
	@output int OUTPUT
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON
	SET @output = 0

	DECLARE @MsgIds TABLE (MessageId UniqueIdentifier)
	DECLARE @now DATETIME2

	SET @now =SYSDATETIME()

	UPDATE TOP (@numberOfMessage) msg
	SET [State] = 1,
	    DeQueueTime =@now 
	OUTPUT INSERTED.MessageId INTO @MsgIds
	FROM [dbo].[MsgQueue] msg
	WHERE Identifier = @identifier
	AND [State] = 0
	AND SegmentSeq = 1
	AND ExpiryTime > @now 


	SELECT msg.MessageId, Identifier, 
		   (SELECT CAST(SegmentMsg as VARCHAR(max)) FROM [dbo].[MsgQueue] m WHERE m.MessageId = msg.MessageId ORDER BY SegmentSeq
		    FOR XML PATH(''),TYPE).value('(./text())[1]','VARCHAR(MAX)') AS [SegmentMsg]
	FROM [dbo].[MsgQueue] msg
	WHERE EXISTS (SELECT 1 FROM @MsgIds ids WHERE ids.MessageId = msg.MessageId)
	AND SegmentSeq = 1

	IF @@ERROR != 0
    BEGIN
		SET @output = 1
    END

END

IF(object_id(N'DeleteMessage') IS NOT NULL)
	DROP PROC [dbo].[DeleteMessage];
GO
CREATE PROC [dbo].[DeleteMessage] 
	@messageId Uniqueidentifier,
	@output int OUTPUT
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON
	SET @output = 0

	DELETE FROM [dbo].[MsgQueue]
	WHERE MessageId  = @messageId

	IF @@ERROR != 0
    BEGIN
		SET @output = 1
    END
END


IF(object_id(N'DeleteMessageByIdentifer') IS NOT NULL)
	DROP PROC [dbo].[DeleteMessageByIdentifer];
GO
CREATE PROC [dbo].[DeleteMessageByIdentifer] 
	@identifier CHAR(200),
	@output int OUTPUT
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON
	SET @output = 0

	DELETE FROM [dbo].[MsgQueue]
	WHERE Identifier  = @identifier

	IF @@ERROR != 0
    BEGIN
		SET @output = 1
    END
END

 
IF(object_id(N'PurgeMessageQueue') IS NOT NULL)
	DROP PROC [dbo].[PurgeDeleteMessageQueue];
GO
CREATE PROC [dbo].[PurgeMessageQueue] 
	@deQueueTimeoutSeconds INT = 60,
	@output int OUTPUT
AS
BEGIN
	SET NOCOUNT ON
	SET XACT_ABORT ON
	SET @output = 0

	DELETE FROM [dbo].[MsgQueue]
	WHERE ExpiryTime <= SYSDATETIME()

	UPDATE [dbo].[MsgQueue]
	SET [State] = 0,
	    DeQueueTime = NULL
    WHERE [State] = 1
	AND [DeQueueTime] <= DATEADD(second, -1 * @deQueueTimeoutSeconds, SYSDATETIME())

	IF @@ERROR != 0
    BEGIN
		SET @output = 1
    END
END

GO
EXEC  [dbo].[PurgeMessageQueue] 
GO

DECLARE @identifier CHAR(200), @message VARCHAR(max)

SET @identifier = 'IDENTIFIER 1234567890 -- USER:Tester'
SET @message ='1234567890ABCDEFGHIGKLMN1234567890ABCDEFGHIGKLMN1234567890ABCDEFGHIGKLMN1234567890ABCDEFGHIGKLMN1234567890ABCDEFGHIGKLMN1234567890ABCDEFGHIGKLMN1234567890ABCDEFGHIGKLMN1234567890ABCDEFGHIGKLMN1234567890ABCDEFGHIGKLMN'

EXEC dbo.InMessageQueue @identifier = @identifier ,@message = @message , @maxLength=10 ,@lifeInQueueSeconds = 300
EXEC dbo.DeMessageQueue @identifier = @identifier , @numberOfMessage = 5

GO
