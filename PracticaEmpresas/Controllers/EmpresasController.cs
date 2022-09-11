using Microsoft.AspNetCore.Mvc;
using PracticaEmpresas.Models;
using RestSharp;
using System.Dynamic;

namespace PracticaEmpresas.Controllers
{
    //Middleware entre Angular App (ClientApp) y Servicio de empresas proporcionado por Banco
    [ApiController]
    [Route("[controller]")]
    public class EmpresasController : ControllerBase
    {
        private readonly ILogger<EmpresasController> _logger;
        
        //Api expuesto por Banco
        private string urlBanco = "https://apitest-bt.herokuapp.com/api/v1/empresas";

        public EmpresasController(ILogger<EmpresasController> logger)
        {
            _logger = logger;
        }

        //Se obtiene todas las empresas desde el servicio del Banco
        [HttpGet]
        public IEnumerable<ItemEmpresaListar> Get()
        {
            var resultado = new List<ItemEmpresaListar>();

            using (var clienteRest = this.ObtenerRestCliente())
            {
                var solicitudGet = this.ObtenerRestRequest();
                solicitudGet.Method = Method.Get;

                try
                {
                    var respuestaBanco = clienteRest.Get<List<ItemEmpresa>>(solicitudGet);

                    if (respuestaBanco != null)
                    {
                        foreach (var empresa in respuestaBanco)
                        {
                            try
                            {
                                resultado.Add(new ItemEmpresaListar()
                                {
                                    Numero = empresa.id,
                                    Nombre = empresa.nombre_comercial,
                                    Estado = empresa.estado,
                                    Email = empresa.correo,
                                    Nit = empresa.nit
                                });
                            }
                            catch (Exception errorItemEmpresa)
                            {
                                _logger.LogError(errorItemEmpresa, "Error obteniendo item empresa desde Banco!");
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    _logger.LogError(error, "Error obteniendo empresas desde Banco!");
                }
            }

            return resultado.OrderBy(x => x.Numero);
        }

        //Se manda a crear una nueva empresa al servicio del Banco
        [HttpPost]
        public ActionResult Post(ItemEmpresaPost entrada)
        {
            if (entrada == null)
                return BadRequest("Informacion basica de empresa no recibida!");

            using (var clienteRest = this.ObtenerRestCliente())
            {
                try
                {
                    var solicitudPost = this.ObtenerRestRequest();
                    solicitudPost.Method = Method.Post;
                    solicitudPost.AddJsonBody(new ItemEmpresa
                    {
                        nombre_comercial = entrada.nombreComercial,
                        razon_social = entrada.razonSocial,
                        telefono = entrada.telefono,
                        correo = entrada.correoElectronico,
                        nit = entrada.nit,
                        direccion = entrada.direccion,
                        estado = entrada.estado
                    });

                    var respuestaBanco = clienteRest.Post(solicitudPost);
                    if (respuestaBanco == null || respuestaBanco.StatusCode != System.Net.HttpStatusCode.Created)
                        return BadRequest("No se pudo crear la empresa en Banco!");

                    return Ok();
                }
                catch (Exception error)
                {
                    _logger.LogError(error, "Error al intentar crear empresa en Banco!");
                    return BadRequest(error.Message);
                }
            }
        }

        //Obtiene empresa desde el Banco por Id o Numero
        [HttpGet("{id:int}")]
        public ActionResult<ItemEmpresa> GetById(int? id)
        {
            if (id == null || id == default(int))
                return BadRequest("El numero de empresa debe ser ingresado!");
            
            using (var clienteRest = this.ObtenerRestCliente())
            {
                try
                {
                    clienteRest.Options.BaseUrl = new Uri(string.Format("{0}/{1}", this.urlBanco, id));
                    var solicitudGet = this.ObtenerRestRequest();
                    solicitudGet.Method = Method.Get;

                    var infoEmpresaBanco = clienteRest.Get<ItemEmpresa>(solicitudGet);
                    if (infoEmpresaBanco == null || string.IsNullOrEmpty(infoEmpresaBanco.created_at))
                        return NotFound();

                    return infoEmpresaBanco;
                }
                catch (Exception error)
                {
                    _logger.LogError(error, "Error obteniendo empresa por Id");
                }
            }

            return NotFound();
        }

        //Se manda a modificar una nueva empresa al servicio del Banco
        [HttpPut("{id:int?}", Name = "modificacion")]
        public ActionResult Put(int? id, ItemEmpresaPost entrada)
        {
            if (entrada == null)
                return BadRequest("Informacion basica de empresa no recibida!");

            using (var clienteRest = this.ObtenerRestCliente())
            {
                try
                {
                    clienteRest.Options.BaseUrl = new Uri(string.Format("{0}/{1}", this.urlBanco, id));
                    var solicitudPost = this.ObtenerRestRequest();
                    solicitudPost.Method = Method.Put;
                    solicitudPost.AddJsonBody(new ItemEmpresa
                    {
                        nombre_comercial = entrada.nombreComercial,
                        razon_social = entrada.razonSocial,
                        telefono = entrada.telefono,
                        correo = entrada.correoElectronico,
                        nit = entrada.nit,
                        direccion = entrada.direccion,
                        estado = entrada.estado
                    });

                    var respuestaBanco = clienteRest.Put(solicitudPost);
                    if (respuestaBanco == null || respuestaBanco.StatusCode != System.Net.HttpStatusCode.OK)
                        return BadRequest("No se pudo crear la empresa en Banco!");

                    return Ok();
                }
                catch (Exception error)
                {
                    _logger.LogError(error, "Error al intentar crear empresa en Banco!");
                    return BadRequest(error.Message);
                }
            }
        }

        //Elimina la empresa en servicio de Banco
        [HttpDelete("{id:int?}")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return BadRequest("Debe ingresar el numero de empresa a eliminar!");

            using (var clienteRest = this.ObtenerRestCliente())
            {
                try
                {
                    clienteRest.Options.BaseUrl = new Uri(string.Format("{0}/{1}", this.urlBanco, id));
                    var solicitudDelete = this.ObtenerRestRequest();
                    solicitudDelete.Method = Method.Delete;

                    var respuestaBanco = clienteRest.Delete(solicitudDelete);
                    if (respuestaBanco == null || respuestaBanco.StatusCode != System.Net.HttpStatusCode.NoContent)
                        return BadRequest("No se pudo crear la empresa en Banco!");

                    return Ok();
                }
                catch (Exception error)
                {
                    _logger.LogError(error, "Error al intentar crear empresa en Banco!");
                    return BadRequest(error.Message);
                }
            }
        }

        private RestClient ObtenerRestCliente()
        {
            return new RestClient(this.urlBanco);
        }

        private RestRequest ObtenerRestRequest()
        {
            var resultado = new RestRequest();
            resultado.AddHeader("user", "User123");
            resultado.AddHeader("password", "Password123");

            return resultado;
        }
    }
}
