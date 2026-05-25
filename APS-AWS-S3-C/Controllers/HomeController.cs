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
		//------------------------INSERTAMOS--------------------------------------------------------------------------
		public IActionResult Index()
        {
            // Colocamos el listado de los archivos
            List<S3FileDetais> files=new List<S3FileDetais>();
            // Lo que retornamos en la lista 
            files = _db.s3FileDetais.ToList();

            // Lo que mostraremos en la lista.
            return View(files);
        }
	//  (CARGAR ARCHIVOS)
        [HttpPost]
        public async Task<IActionResult> UploadfileToS3(IFormFile file)
        {
			// ¡AÑADE  AL INICIO DEL MÉTODO!
			if (file == null || file.Length == 0)
			{
				return BadRequest("Error: El archivo no fue recibido por el servidor. Revisa el formulario.");
			}

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

		//--------------------------ELIMINAMOS------------------------------------------------------------------------------

		// Creamos la acciones para poder eliminar los archivos del S3 AWS
		public IActionResult DeleteFile(Int32 id)
		{
			// 1. Buscamos el archivo en la base de datos por su ID
			S3FileDetais details = _db.s3FileDetais.FirstOrDefault(x => x.id == id);

			// 2. ¡VALIDACIÓN CRÍTICA! Si es null, detenemos el flujo aquí
			if (details == null)
			{
				return Content($"Error: No se encontró ningún archivo en la base de datos con el ID {id}. Revisa si el ID que viaja en la URL es correcto.");
			}

			// 3. Si sí existe, se lo pasamos a la vista de forma segura
			return View(details);
		}

		[HttpPost]
		public async Task<IActionResult> DeleteFileToS3(string FileName)
		{
			// 1. Validación de seguridad: Evita que el parámetro llegue vacío
			if (string.IsNullOrEmpty(FileName))
			{
				return BadRequest("El nombre del archivo no fue proporcionado.");
			}
			//Credenciales
			using (var amazonS3client = new AmazonS3Client(
				"AKIAXIRJEVYXVJRCZ74Y",
				"IMRwyRzo4b0cVudWjW6X+eyx12KtQkiyQFvAFt87",
				Amazon.RegionEndpoint.USEast2
			))
			{
				var transfertUtility = new TransferUtility(amazonS3client);

				await transfertUtility.S3Client.DeleteObjectAsync(new DeleteObjectRequest()
				{
					BucketName = "s3-prenube26",
					Key = FileName // <--- CORREGIDO: Ahora usa directamente el parámetro del método
				});

				// Buscamos en la base de datos para borrar el registro local
				S3FileDetais fileDetais = _db.s3FileDetais
				.FirstOrDefault(x => x.FileName == FileName);
				if (fileDetais != null)
				{
					_db.s3FileDetais.Remove(fileDetais);
					await _db.SaveChangesAsync(); // Es mejor usar el SaveChanges asíncrono
					ViewBag.Success = "Archivo Eliminado con éxito en S3 Bucket";
				}

				return RedirectToAction(nameof(Index));
			}
		}

		/*

				public IActionResult DeleteFile(Int32 id)
				{ 
					S3FileDetais details = new S3FileDetais();
					details = _db.s3FileDetais.FirstOrDefault(x => x.id == id);
					return View(details);
				}

				// Colocamos las funciones para eliminar el archivos 
				[HttpPost]
				public async Task<IActionResult> DeleteFileToS3(string File_name)
				{
					// 1. Validación de seguridad: Evita que el parámetro llegue vacío
					if (string.IsNullOrEmpty(File_name))
					{
						return BadRequest("El nombre del archivo no fue proporcionado.");
					} 

					// Colocamos la claves que generamos en el amazon S3
					// Usamos el cliente de Amazon
					using (var amazonS3client = new AmazonS3Client(
						// Se coloca la claves de S3
						"AKIAXIRJEVYXVJRCZ74Y",
						"IMRwyRzo4b0cVudWjW6X+eyx12KtQkiyQFvAFt87",
						/// Colocamos la region del AWS 
						Amazon.RegionEndpoint.USEast2
						))
					{
						// tranformamos, para poder usar el codigo de la imagen
						var transfertUtility = new TransferUtility(amazonS3client);
						await transfertUtility.S3Client.DeleteObjectAsync(new DeleteObjectRequest()
						{
							BucketName = "s3-prenube26",
							Key = File_name

							/*
								// Hacemos el llamado al buken
								InputStream = memorystream,
								Key = file.FileName,
								// Nombre del buken 
								BucketName = "s3-prenube26",
								ContentType = file.ContentType,

						});

						// hacemos el llamado al model de s3
						S3FileDetais fileDetais = new S3FileDetais();
						fileDetais = _db.s3FileDetais.FirstOrDefault(x => x.FileName.ToLower() == File_name.ToLower());
						_db.s3FileDetais.Remove(fileDetais);
						_db.SaveChanges();

						//Mostramos el mensaje
						ViewBag.Success = "Archivo Eliminado con éxito en S3 Bukent";

						//hacemos el return
						return RedirectToAction(nameof(Index));

						//return "Archivo subido";
						//return Ok(new { mensaje = "Archivo subido con éxito", nombre = file.FileName });
					}
				}
			 */

		//-------------------------------------------------------------------------------------------------------

		//--------------------------DESCARGAR------------------------------------------------------------------------------


		public IActionResult ViewFileForDow(Int32 id)
		{
			// 1. Buscamos el archivo en la base de datos por su ID
			S3FileDetais details = _db.s3FileDetais.FirstOrDefault(x => x.id == id);

			// 2. ¡VALIDACIÓN CRÍTICA! Si es null, detenemos el flujo aquí
			if (details == null)
			{
				return Content($"Error: No se encontró ningún archivo en la base de datos con el ID {id}. Revisa si el ID que viaja en la URL es correcto.");
			}

			// 3. Si sí existe, se lo pasamos a la vista de forma segura
			return View(details);
		}

		[HttpPost]
		public async Task<IActionResult> DownloadFile(string FileName)
		{
			// 1. Validación de seguridad: Evita que el parámetro llegue vacío
			if (string.IsNullOrEmpty(FileName))
			{
				return BadRequest("El nombre del archivo no fue proporcionado.");
			}
			//Credenciales
			using (var amazonS3client = new AmazonS3Client(
				"AKIAXIRJEVYXVJRCZ74Y",
				"IMRwyRzo4b0cVudWjW6X+eyx12KtQkiyQFvAFt87",
				Amazon.RegionEndpoint.USEast2
			))
			{

				var transfertUtility = new TransferUtility(amazonS3client);
				var response = await transfertUtility.S3Client.GetObjectAsync(new GetObjectRequest()
				{
					BucketName = "s3-prenube26",
					Key=FileName
				}	
				);
				// Validamos si no viene vacio o null el id del bukem
				if (response.ResponseStream==null) 
				{
					return NotFound();
				}
				ViewBag.Success = "Archivo descargado con éxito en S3 Bucket";
				return File(response.ResponseStream, response.Headers.ContentType, FileName);

			
			}
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
