using APS_AWS_S3_C.Data;
using APS_AWS_S3_C.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace APS_AWS_S3_C.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        // Hacemos privada la la conexion con el data 
        private AplicationDbContext _db { get; set; }

        // Se realiza la referencia al documento 
        public HomeController(ILogger<HomeController> logger, AplicationDbContext db) 
        {
            // Iniciamos 
            _db = db; 
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Colocamos el listado de los archivos
            List<S3FileDetais> file=new List<S3FileDetais>();
            // Lo que retornamos en la lista 
            file = _db.s3FileDetais.ToList();

            // Lo que mostraremos en la lista.
            return View(file);
        }

        // Creamos la ruta 
        [HttpPost]
        public async Task<AcctionsResult> UploadFileToS3(IformFile file)
        {

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
