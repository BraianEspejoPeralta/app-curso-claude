create procedure sp_ConsultaCuenta
    @id int
as
select * from Cuenta where Id = @id
