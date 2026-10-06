/*
    Descripción: Da de alta una cuenta con su titular y saldo inicial, registrando la fecha de alta.
    Parámetros:
        @p_Titular  NVARCHAR(100)  Nombre del titular de la cuenta.
        @p_Saldo    MONEY          Saldo inicial; no puede ser negativo.
    Retorno: Un registro con la cuenta creada (Id, Titular, Saldo, FechaAlta).
             Error 50001 si el saldo inicial es negativo.
*/
CREATE OR ALTER PROCEDURE dbo.usp_Cuenta_Alta
    @p_Titular NVARCHAR(100),
    @p_Saldo MONEY
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @v_Id INT;

    BEGIN TRY
        IF @p_Saldo < 0
            THROW 50001, 'El saldo inicial no puede ser negativo.', 1;

        BEGIN TRANSACTION;

        INSERT INTO dbo.Cuenta (Titular, Saldo, FechaAlta)
        VALUES (@p_Titular, @p_Saldo, GETDATE());

        SET @v_Id = SCOPE_IDENTITY();

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;

    SELECT Id, Titular, Saldo, FechaAlta
    FROM dbo.Cuenta
    WHERE Id = @v_Id;
END;
GO
