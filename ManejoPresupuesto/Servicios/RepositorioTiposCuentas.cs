using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task CrearTipoCuenta(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
    }
    public class RepositorioTiposCuentas: IRepositorioTiposCuentas
    {
        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task CrearTipoCuenta(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);

                var id = await connection.QuerySingleAsync<int>($@"INSERT INTO TiposCuentas (Nombre, UsuarioId, Orden) Values(@Nombre,@UsuarioId,0);
                                                        SELECT SCOPE_IDENTITY(); ",tipoCuenta);
                tipoCuenta.Id = id;
            
        }

        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            //Q traiga lo primero que encuentre o un valoir por defecto 0
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@$"SELECT 1 FROM TIPOSCUENTAS WHERE NOMBRE = @Nombre AND USUARIOID = @UsuarioId;",
                                                                           new {nombre, usuarioId});

            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden FROM TIPOSCUENTAS WHERE USUARIOID= @usuarioId ", new { usuarioId });

        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TIPOSCUENTAS SET NOMBRE = @Nombre WHERE ID = @Id ", tipoCuenta);


        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT ID, NOMBRE, ORDEN FROM TIPOSCUENTAS WHERE ID= @Id AND USUARIOID = @UsuarioId "
                                                                            ,new {id, usuarioId});

        }
    }
}
