create procedure sp_BajaCuenta
    @id int
as
if not exists (select * from Cuenta where Id = @id)
    raiserror('no existe la cuenta', 16, 1)
delete from Cuenta where Id = @id
delete from Movimiento where CuentaId = @id
