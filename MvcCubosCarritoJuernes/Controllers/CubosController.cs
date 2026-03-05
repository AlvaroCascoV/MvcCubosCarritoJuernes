using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MvcCoreUtilidades.Helpers;
using MvcCubosCarritoJuernes.Models;
using MvcCubosCarritoJuernes.Repositories;

namespace MvcCubosCarritoJuernes.Controllers
{
    public class CubosController : Controller
    {
        IRepositoryCubos repo;
        private HelperPathProvider helperPath; 
        IMemoryCache memoryCache;

        public CubosController(IRepositoryCubos repo, HelperPathProvider helperPath, IMemoryCache memoryCache)
        {
            this.repo = repo;
            this.helperPath = helperPath;
            this.memoryCache = memoryCache;
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
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Almacenamiento(int? idfav, int? idcubo)
        {
            if(idfav != null)
            {
                List<Cubo> cubosFavs;
                if (this.memoryCache.Get("FAVORITOS") == null)
                {
                    cubosFavs = new List<Cubo>();
                }
                else
                {
                    cubosFavs = this.memoryCache.Get<List<Cubo>>("FAVORITOS");
                }
                Cubo cubo = await this.repo.FindCuboAsync(idfav.Value);
                cubosFavs.Add(cubo);
                this.memoryCache.Set("FAVORITOS", cubosFavs);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Favoritos(int? ideliminar)
        {
            if (ideliminar != null)
            {
                List<Cubo> cubosFavs = this.memoryCache.Get<List<Cubo>>("FAVORITOS");
                
                Cubo cuboEliminar = cubosFavs.Find(x => x.IdCubo == ideliminar.Value);
                cubosFavs.Remove(cuboEliminar);

                if(cubosFavs.Count == 0)
                {
                    this.memoryCache.Remove("FAVORITOS");
                }
                else
                {
                    this.memoryCache.Set("FAVORITOS", cubosFavs);
                }
            }
            return View();
        }
    }
}
