-- ================================================
-- Stored Procedure for creating dbo.Logs records.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Brett Schurko
-- Create date: 2023-11-06
-- =============================================
CREATE PROCEDURE spCreateLog
	@Message NVARCHAR(MAX),
	@MessageTemplate NVARCHAR(MAX),
	@Level NVARCHAR(128),
	@TimeStamp DATETIME,
	@Exception NVARCHAR(MAX),
	@Properties NVARCHAR(MAX)
 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @TimeStamp = NULL
	BEGIN
		SET @TimeStamp = GETDATE()
	END

	BEGIN TRAN
		BEGIN
			 
			INSERT INTO [dbo].[Logs] ([Message], MessageTemplate, [Level], [TimeStamp], Exception, Properties)
			VALUES(@Message, @MessageTemplate, @Level, @TimeStamp, @Exception, @Properties)

		END	    
	COMMIT TRAN
END
 