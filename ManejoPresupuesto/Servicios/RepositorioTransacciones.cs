using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using System.Transactions;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
    }
    public class RepositorioTransacciones: IRepositorioTransacciones
    {
        private readonly string connectionString;
        public RepositorioTransacciones(IConfiguration configuration) 
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("Transacciones_Insertar", new { transaccion.UsuarioId, transaccion.FechaTransaccion, transaccion.Monto,
                transaccion.CategoriaId, transaccion.CuentaId, transaccion.Nota},
                commandType: System.Data.CommandType.StoredProcedure);

            transaccion.Id = id;
        }
        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"SELECT t.Id, t.monto, t.FechaTransaccion, c.Nombre as Categoria,
                                                              cu.Nombre as Cuenta, c.TipoOperacionId
                                                              FROM Transacciones t INNER JOIN Categorias c on c.Id=t.CategoriaId 
                                                              INNER JOIN Cuentas cu on cu.Id = t.CuentaId 
                                                              WHERE t.cuentaId = @CuentaId and t.UsuarioId = @UsuarioId
                                                              AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin", modelo);
        }


        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.ExecuteAsync("Transacciones_Actualizar", new
            {
                transaccion.Id,
                transaccion.FechaTransaccion,
                transaccion.Monto,
                transaccion.CategoriaId,
                transaccion.CuentaId,
                transaccion.Nota,
                montoAnterior,
                cuentaAnteriorId
            },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(@"SELECT T.*, cat.TipoOperacionId FROM Transacciones T 
                                                                          INNER JOIN Categorias Cat 
                                                                          ON Cat.Id= T.CategoriaId 
                                                                          WHERE T.ID= @Id AND t.UsuarioId= @UsuarioId "
                                                                            , new { id, usuarioId });

        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Borrar", new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
