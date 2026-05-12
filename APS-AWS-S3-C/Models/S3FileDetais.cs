using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace APS_AWS_S3_C.Models
{
	// Creamos la libreria de talle de archivos 
	public class S3FileDetais
	{
		//Colocamos la llave 
		[Key]
		//Creamos los metodo  
		public int id { get; set; }
		public DateTime FileDate {  get; set; }
		public string FileName { get; set; }

		//public string MyProperty { get; set; }


	}
}
