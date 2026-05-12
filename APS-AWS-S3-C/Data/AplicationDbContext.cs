using APS_AWS_S3_C.Models;
using Microsoft.EntityFrameworkCore;

namespace APS_AWS_S3_C.Data
{
	// Creamos un carpeta, luego dentro de la carpeta creamos una clase llamada 
	//AplicationDbContext para crear la conexion +
	// Hacemos referencia a la DbContext
	public class AplicationDbContext : DbContext
	//Luego generamos el contructor 
	{

		// INCORRECTO: 
		// public AplicationDbContext(DbContextOptions options) : base(options)

		// CORRECTO:
		public AplicationDbContext(DbContextOptions<AplicationDbContext> options)
			: base(options)
		{

		}
		

		//Hacemos referencia al modelo del constructor 
		public DbSet<S3FileDetais> s3FileDetais { get; set; }


		protected AplicationDbContext()
		{
		}

		
	}
}
