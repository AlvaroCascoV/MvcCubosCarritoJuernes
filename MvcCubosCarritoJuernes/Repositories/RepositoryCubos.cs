using Humanizer;
using Microsoft.EntityFrameworkCore;
using MvcCubosCarritoJuernes.Data;
using MvcCubosCarritoJuernes.Models;
using MySql.Data.MySqlClient;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MvcCubosCarritoJuernes.Repositories
{

    #region PROCEDURES

//    DELIMITER //

//CREATE PROCEDURE SP_INSERT_CUBO(
//    IN p_idcubo      INT,
//    IN p_nombre      VARCHAR(50),
//    IN p_modelo      VARCHAR(50),
//    IN p_marca       VARCHAR(50),
//    IN p_imagen     VARCHAR(50),
//    IN p_precio     INT
//)
//BEGIN

//INSERT INTO CUBOS(id_cubo, nombre, modelo, marca, imagen, precio)
//VALUES(p_idcubo, p_nombre, p_modelo, p_marca, p_imagen, p_precio);

//    END //

//    DELIMITER;
    #endregion


    public class RepositoryCubos : IRepositoryCubos
    {
        private CubosContext context;
        public RepositoryCubos(CubosContext context) 
        {
            this.context = context;
        }
        public async Task<List<Cubo>> GetCubosAsync()
        {
            return await this.context.Cubos.ToListAsync();
        }

        public async Task<Cubo> FindCuboAsync(int idCubo)
        {
            var consulta = from datos in this.context.Cubos where datos.IdCubo == idCubo select datos;
            return await consulta.FirstOrDefaultAsync();
        }
        public async Task InsertCuboAsync(int idCubo, string nombre, string modelo, string marca, string imagen, int precio)
        {
            string sql = "CALL SP_INSERT_CUBO(@id_cubo, @nombre, @modelo, @marca, @imagen, @precio);";
            MySqlParameter pamIdCubo = new MySqlParameter("@id_cubo", idCubo);
            MySqlParameter pamNombre = new MySqlParameter("@nombre", nombre);
            MySqlParameter pamModelo = new MySqlParameter("@modelo", modelo);
            MySqlParameter pamMarca = new MySqlParameter("@marca", marca);
            MySqlParameter pamImagen = new MySqlParameter("@imagen", imagen);
            MySqlParameter pamPrecio = new MySqlParameter("@precio", precio);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamIdCubo, pamNombre, pamModelo, pamMarca, pamImagen, pamPrecio);
        }

        public async Task DeleteCuboAsync(int idCubo)
        {
            Cubo cubo = this.context.Cubos.Find(idCubo);
            if (cubo != null)
            {
                this.context.Cubos.Remove(cubo);
                this.context.SaveChanges();
            }
        }

        public async Task UpdateCuboAsync(int idCubo, string nombre, string modelo, string marca, string imagen, int precio)
        {
            Cubo cubo = this.context.Cubos.Find(idCubo);
            if (cubo != null)
            {
                cubo.Nombre = nombre;
                cubo.Modelo = modelo;
                cubo.Marca = marca;
                cubo.Imagen = imagen;
                cubo.Precio = precio;
                this.context.SaveChanges();
            }
        }
    }
}
