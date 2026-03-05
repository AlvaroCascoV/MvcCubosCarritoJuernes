using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace MvcCoreUtilidades.Helpers
{
    public enum Folders { cubos }
    public class HelperPathProvider
    {
        private IWebHostEnvironment hostEnvironment;
        private IServer server;
        public HelperPathProvider(IWebHostEnvironment hostEnvironment, IServer server)
        {
            this.hostEnvironment = hostEnvironment;
            this.server = server;
        }
        public string MapPath(string fileName, Folders folder)
        {
            string carpeta = "";
            if (folder == Folders.cubos)
            {
                carpeta = "images/cubos";
            }
            string rootPath = hostEnvironment.WebRootPath;

            return Path.Combine(rootPath, carpeta, fileName);
        }
    }
}