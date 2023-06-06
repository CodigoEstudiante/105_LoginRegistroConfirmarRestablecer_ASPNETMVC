using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WebAppCorreo.Models;
using WebAppCorreo.Datos;
using WebAppCorreo.Servicios;


namespace WebAppCorreo.Controllers
{
    public class InicioController : Controller
    {
        // GET: Inicio
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string correo,string clave)
        {
            UsuarioDTO usuario = DBUsuario.Validar(correo, UtilidadServicio.ConvertirSHA256(clave));

            if(usuario != null)
            {
                if (!usuario.Confirmado)
                {
                    ViewBag.Mensaje = $"Falta confirmar su cuenta. Se le envio un correo a {correo}";
                }else if (usuario.Restablecer)
                {
                    ViewBag.Mensaje = $"Se ha solicitado restablecer su cuenta, favor revise su bandeja del correo {correo}";
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                ViewBag.Mensaje = "No se encontraron coincidencias";
            }


            return View();
        }

        public ActionResult Registrar()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Registrar(UsuarioDTO usuario)
        {
            if(usuario.Clave != usuario.ConfirmarClave)
            {
                ViewBag.Nombre = usuario.Nombre;
                ViewBag.Correo = usuario.Correo;
                ViewBag.Mensaje = "Las contraseñas no coinciden";
                return View();
            }

            if(DBUsuario.Obtener(usuario.Correo) == null)
            {
                usuario.Clave = UtilidadServicio.ConvertirSHA256(usuario.Clave);
                usuario.Token = UtilidadServicio.GenerarToken();
                usuario.Restablecer = false;
                usuario.Confirmado = false;
                bool respuesta = DBUsuario.Registrar(usuario);

                if (respuesta)
                {
                    string path = HttpContext.Server.MapPath("~/Plantilla/Confirmar.html");
                    string content = System.IO.File.ReadAllText(path);
                    string url = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Headers["host"], "/Inicio/Confirmar?token=" + usuario.Token);

                    string htmlBody = string.Format(content, usuario.Nombre, url);

                    CorreoDTO correoDTO = new CorreoDTO()
                    {
                        Para = usuario.Correo,
                        Asunto = "Correo confirmacion",
                        Contenido = htmlBody
                    };

                    bool enviado = CorreoServicio.Enviar(correoDTO);
                    ViewBag.Creado = true;
                    ViewBag.Mensaje = $"Su cuenta ha sido creada. Hemos enviado un mensaje al correo {usuario.Correo} para confirmar su cuenta";
                }
                else
                {
                    ViewBag.Mensaje = "No se pudo crear su cuenta";
                }



            }
            else
            {
                ViewBag.Mensaje = "El correo ya se encuentra registrado";
            }


            return View();
        }

        public ActionResult Confirmar(string token)
        {
            ViewBag.Respuesta = DBUsuario.Confirmar(token);
            return View();
        }

        public ActionResult Restablecer()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Restablecer(string correo)
        {
            UsuarioDTO usuario = DBUsuario.Obtener(correo);
            ViewBag.Correo = correo;
            if(usuario != null)
            {
                bool respuesta = DBUsuario.RestablecerActualizar(1, usuario.Clave, usuario.Token);

                if (respuesta)
                {
                    string path = HttpContext.Server.MapPath("~/Plantilla/Restablecer.html");
                    string content = System.IO.File.ReadAllText(path);
                    string url = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Headers["host"], "/Inicio/Actualizar?token=" + usuario.Token);

                    string htmlBody = string.Format(content, usuario.Nombre, url);

                    CorreoDTO correoDTO = new CorreoDTO()
                    {
                        Para = correo,
                        Asunto = "Restablecer cuenta",
                        Contenido = htmlBody
                    };

                    bool enviado = CorreoServicio.Enviar(correoDTO);
                    ViewBag.Restablecido = true;
                }
                else
                {
                    ViewBag.Mensaje = "No se pudo restablecer la cuenta";
                }

            }
            else
            {
                ViewBag.Mensaje = "No se encontraron coincidencias con el correo";
            }

            return View();
        }

        public ActionResult Actualizar(string token)
        {
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public ActionResult Actualizar(string token,string clave, string confirmarClave)
        {
            ViewBag.Token = token;
            if (clave != confirmarClave)
            {
                ViewBag.Mensaje = "Las contraseñas no coinciden";
                return View();
            }

            bool respuesta = DBUsuario.RestablecerActualizar(0, UtilidadServicio.ConvertirSHA256(clave), token);

            if (respuesta)
                ViewBag.Restablecido = true;
            else
                ViewBag.Mensaje = "No se pudo actualizar";

            return View();
        }
    }
}