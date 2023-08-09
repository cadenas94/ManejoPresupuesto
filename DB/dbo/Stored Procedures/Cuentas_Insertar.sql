
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Cuentas_Insertar]
	@Nombre nvarchar(50),
	@TipoCuentaId int,
	@Balance Decimal(18,2),
	@Descripcion nvarchar(250)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @Orden int;
	
	--SELECT @Orden = COALESCE(MAX(Orden), 0) +1
	--FROM TiposCuentas
	--WHERE UsuarioId = @UsuarioId

	INSERT INTO Cuentas(Nombre, TipoCuentaId, Balance, Descripcion)
	VALUES(@Nombre, @TipoCuentaId, @Balance, @Descripcion);

	SELECT SCOPE_IDENTITY();


END
