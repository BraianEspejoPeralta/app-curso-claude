/*
    Descripción: Da de baja una cuenta y todos sus movimientos, en una única transacción.
    Parámetros:
        @p_Id  INT  Identificador de la cuenta a eliminar.
    Retorno: Sin conjunto de resultados.
             Error 50002 si la cuenta no existe.
*/
CREATE OR ALTER PROCEDURE dbo.usp_Cuenta_Baja
    @p_Id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Cuenta WHERE Id = @p_Id)
            THROW 50002, 'No existe la cuenta.', 1;

        BEGIN TRANSACTION;

        -- Primero los movimientos (hijos) para no violar la clave foránea hacia dbo.Cuenta.
        DELETE FROM dbo.Movimiento
        WHERE CuentaId = @p_Id;

        DELETE FROM dbo.Cuenta
        WHERE Id = @p_Id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH;
END;
GO
