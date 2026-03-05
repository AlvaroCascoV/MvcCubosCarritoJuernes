using MvcCubosCarritoJuernes.Models;

namespace MvcCubosCarritoJuernes.Repositories
{
    public interface IRepositoryCubos
    {
        Task<List<Cubo>> GetCubosAsync();
        Task<Cubo> FindCuboAsync(int idCubo);
        Task InsertCuboAsync(int idCubo, string nombre, string modelo, string marca, string imagen, int precio);
        Task UpdateCuboAsync(int idCubo, string nombre, string modelo, string marca, string imagen, int precio);
        Task DeleteCuboAsync(int idCubo);
    }
}
