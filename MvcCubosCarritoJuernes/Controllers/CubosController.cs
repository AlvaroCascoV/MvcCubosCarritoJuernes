using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MvcCoreUtilidades.Helpers;
using MvcCubosCarritoJuernes.Extensions;
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
            if (idfav != null)
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

            if (idcubo != null)
            {
                List<int> idsCubosList;
                if (HttpContext.Session.GetObject<List<int>>("IDSCUBOS") == null)
                {
                    idsCubosList = new List<int>();
                }
                else
                {
                    idsCubosList = HttpContext.Session.GetObject<List<int>>("IDSCUBOS");
                }
                idsCubosList.Add(idcubo.Value);
                HttpContext.Session.SetObject("IDSCUBOS", idsCubosList);
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

                if (cubosFavs.Count == 0)
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

        public async Task<IActionResult> Carrito(int? ideliminar, string? accion)
        {
            List<int> idsCubosList = HttpContext.Session.GetObject<List<int>>("IDSCUBOS");
            if (idsCubosList == null)
            {
                return View();
            }
            else
            {
                if (ideliminar != null)
                {
                    idsCubosList.Remove(ideliminar.Value);
                    if (idsCubosList.Count == 0)
                    {
                        HttpContext.Session.Remove("IDSCUBOS");
                        return View();
                    }
                    else
                    {
                        HttpContext.Session.SetObject("IDSCUBOS", idsCubosList);
                    }
                }

                if(accion != null)
                {
                    if(accion.ToLower() == "comprar")
                    {
                        await this.repo.InsertCompraAsync(idsCubosList);
                        HttpContext.Session.Remove("IDSCUBOS");
                        return View();
                    }
                }
                List<Cubo> cubosCarro = await this.repo.GetCubosCarritoAsync(idsCubosList);
                int total = cubosCarro.Sum(x => x.Precio);
                ViewData["PRECIOTOTAL"] = total;
                return View(cubosCarro);
            }
        }
    }
}
