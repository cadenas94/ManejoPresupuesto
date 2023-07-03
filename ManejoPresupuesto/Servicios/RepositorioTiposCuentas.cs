using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task BorrarCuenta(int id);
        Task CrearTipoCuenta(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados);
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

                var id = await connection.QuerySingleAsync<int>("TiposCuentas_Insertar ",new {UsuarioId = tipoCuenta.UsuarioId, 
                                                                                              nombre = tipoCuenta.Nombre},
                                                                                              commandType: System.Data.CommandType.StoredProcedure);
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
            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden FROM TIPOSCUENTAS WHERE USUARIOID= @usuarioId Order by ORDEN ", new { usuarioId });

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

        public async Task BorrarCuenta(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE TIPOSCUENTAS WHERE ID = @Id ", new {id});

        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados)
        {
            //Dapper al recibir como parametro un IEnumerable de forma q ejecutara la query por cada tipocuenta que le pasemos.
            var query = "UPDATE TIPOSCUENTAS SET ORDEN=@Orden WHERE ID=@Id;";
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(query, tipoCuentasOrdenados);
        }
    }
}
