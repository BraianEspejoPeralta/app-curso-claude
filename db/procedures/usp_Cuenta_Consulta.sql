/*
    Descripción: Consulta los datos de una cuenta por su identificador.
    Parámetros:
        @p_Id  INT  Identificador de la cuenta.
    Retorno: Un registro con Id, Titular, Saldo y FechaAlta; vacío si la cuenta no existe.
*/
CREATE OR ALTER PROCEDURE dbo.usp_Cuenta_Consulta
    @p_Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Titular, Saldo, FechaAlta
    FROM dbo.Cuenta
    WHERE Id = @p_Id;
END;
GO
