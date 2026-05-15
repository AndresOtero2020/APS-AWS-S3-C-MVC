using APS_AWS_S3_C.Data; //
using APS_AWS_S3_C.Models; //
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;  //
using System.Diagnostics; //
//Hacemos la referencias a las librerias 
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
// para poder usar el IFormFile
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Components.Forms;


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
            List<S3FileDetais> files=new List<S3FileDetais>();
            // Lo que retornamos en la lista 
            files = _db.s3FileDetais.ToList();

            // Lo que mostraremos en la lista.
            return View(files);
        }

        // Codigo para el s3 ASW 
        [HttpPost]
        public async Task<IActionResult> UploadfileToS3(IFormFile file)
        {
			// 1. Validación de seguridad: Si el archivo es nulo, devolvemos un error
	    	//	if (file == null || file.Length == 0)
		    //	{
		    //		return BadRequest("No se ha seleccionado ningún archivo.");
		    //	}

			// Colocamos la claves que generamos en el amazon S3
			// Usamos el cliente de Amazon
			using ( var amazonS3client= new AmazonS3Client(
                // Se coloca la claves de S3
                "AKIAXIRJEVYXVJRCZ74Y",
                "IMRwyRzo4b0cVudWjW6X+eyx12KtQkiyQFvAFt87",
                /// Colocamos la region del AWS 
				Amazon.RegionEndpoint.USEast2
				))
			{
                using (var memorystream =  new MemoryStream())
				{
                    file.CopyTo(memorystream);
                    var request = new TransferUtilityUploadRequest
					{
                        // Hacemos el llamado al buken
                        InputStream = memorystream,
                        Key = file.FileName,
                        // Nombre del buken 
                        BucketName = "s3-prenube26",
                        ContentType = file.ContentType,
                    };
                    var TranferUtility = new TransferUtility(amazonS3client);
                    await TranferUtility.UploadAsync(request);

				}

			}
            // hacemos el llamado al model de s3
            S3FileDetais fileDetais =new S3FileDetais();
            fileDetais.FileName=file.FileName;
            /// almacenamos la fecha 
            fileDetais.FileDate = DateTime.Today;


            _db.s3FileDetais.Add(fileDetais);
            //Los guardamos 
            _db.SaveChanges();

            //Mostramos el mensaje
            ViewBag.Success = "Archivo subido con éxito on S3 Bukent";

            //hacemos el return
            return RedirectToAction(nameof(Index));

			//return "Archivo subido";
			//return Ok(new { mensaje = "Archivo subido con éxito", nombre = file.FileName });
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
