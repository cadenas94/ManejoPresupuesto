using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task CrearCuenta(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    }
    public class RepositorioCuentas: IRepositorioCuentas
    {
        private readonly string connectionString;

        public RepositorioCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task CrearCuenta(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);

            var id = await connection.QuerySingleAsync<int>(@"Cuentas_Insertar ", new {Nombre = cuenta.Nombre,
                                                                                     TipoCuentaId = cuenta.TipoCuentaId,
                                                                                     Balance = cuenta.Balance,
                                                                                     Descripcion = cuenta.Descripcion},
                                                                                     commandType: System.Data.CommandType.StoredProcedure);
            cuenta.Id = id;

        }

        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Cuenta>(@" SELECT CUENTAS.ID, CUENTAS.Nombre, BALANCE, TC.Nombre AS TIPOCUENTA
                                                         FROM CUENTAS INNER JOIN TiposCuentas TC
                                                         ON TC.Id = CUENTAS.TipoCuentaId
                                                         WHERE TC.UsuarioId = @UsuarioId
                                                         ORDER BY TC.Orden", new {usuarioId});
        }

        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(@"SELECT CUENTAS.ID, CUENTAS.Nombre, BALANCE, Descripcion, TipoCuentaId
                                                                 FROM CUENTAS 
                                                                 INNER JOIN TiposCuentas TC
                                                                 ON TC.Id = CUENTAS.TipoCuentaId
                                                                 WHERE TC.UsuarioId = @UsuarioId AND Cuentas.Id = @Id", 
                                                                 new {id, usuarioId});
        }

        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(@"UPDATE Cuentas Set Nombre = @Nombre, 
                                                                 Balance =@Balance,Descripcion=@Descripcion,
                                                                 TipoCuentaId=@TipoCuentaId WHERE Id=@Id", cuenta);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE CUENTAS WHERE ID = @Id ", new { id });

        }
    }
}
