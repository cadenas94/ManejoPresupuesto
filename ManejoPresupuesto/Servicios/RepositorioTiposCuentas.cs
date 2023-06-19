using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTiposCuentas
    {
        void CrearTipoCuenta(TipoCuenta tipoCuenta);
    }
    public class RepositorioTiposCuentas: IRepositorioTiposCuentas
    {
        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void CrearTipoCuenta(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);

                var id = connection.QuerySingle<int>($@"INSERT INTO TiposCuentas (Nombre, UsuarioId, Orden) Values(@Nombre,@UsuarioId,0);
                                                        SELECT SCOPE_IDENTITY(); ",tipoCuenta);
                tipoCuenta.Id = id;
            
        }
    }
}
