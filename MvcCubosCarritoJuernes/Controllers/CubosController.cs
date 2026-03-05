using Microsoft.AspNetCore.Mvc;
using MvcCoreUtilidades.Helpers;
using MvcCubosCarritoJuernes.Models;
using MvcCubosCarritoJuernes.Repositories;

namespace MvcCubosCarritoJuernes.Controllers
{
    public class CubosController : Controller
    {
        IRepositoryCubos repo;
        private HelperPathProvider helperPath;

        public CubosController(IRepositoryCubos repo, HelperPathProvider helperPath)
        {
            this.repo = repo;
            this.helperPath = helperPath;
        }

        public async Task<IActionResult> Index()
        {
            List<Cubo> cubos = await this.repo.GetCubosAsync();
            return View(cubos);
        }

        public async Task<IActionResult> Details(int idcubo)
        {
            Cubo cubo = await this.repo.FindCuboAsync(idcubo);
            return View(cubo);
        }
    
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Cubo cubo, IFormFile fichero)
        {
            string fileName = fichero.FileName;
            string path = this.helperPath.MapPath(fileName, Folders.cubos);
            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await fichero.CopyToAsync(stream);
            }
            cubo.Imagen = fileName;
            await this.repo.InsertCuboAsync(cubo.IdCubo, cubo.Nombre, cubo.Modelo, cubo.Marca, cubo.Imagen, cubo.Precio);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int idcubo)
        {
            await this.repo.DeleteCuboAsync(idcubo);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int idcubo)
        {
            Cubo cubo = await this.repo.FindCuboAsync(idcubo);
            return View(cubo);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Cubo cubo, IFormFile fichero)
        {
            string fileName = fichero.FileName;
            string path = this.helperPath.MapPath(fileName, Folders.cubos);
            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await fichero.CopyToAsync(stream);
            }
            cubo.Imagen = fileName;
            this.repo.UpdateCuboAsync(cubo.IdCubo, cubo.Nombre, cubo.Modelo, cubo.Marca, cubo.Imagen, cubo.Precio);
            return RedirectToAction("Details", cubo.IdCubo);
        }
    }
}
