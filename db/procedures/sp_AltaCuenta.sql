create procedure sp_AltaCuenta
    @titular varchar(max),
    @saldo money
as
if @saldo < 0
begin
    print 'saldo invalido'
    return
end
insert into Cuenta values (@titular, @saldo, getdate())
select * from Cuenta where Id = scope_identity()
