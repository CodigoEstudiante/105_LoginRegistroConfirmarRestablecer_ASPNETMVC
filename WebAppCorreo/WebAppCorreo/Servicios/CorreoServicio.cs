using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using WebAppCorreo.Models;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;


namespace WebAppCorreo.Servicios
{
    public static class CorreoServicio
    {
        private static string _Host = "smtp.gmail.com";
        private static int _Puerto = 587;

        private static string _NombreEnvia = "";
        private static string _Correo = "";
        private static string _Clave = "";

        public static bool Enviar(CorreoDTO correodto)
        {
            try
            {
                var email = new MimeMessage();

                email.From.Add(new MailboxAddress(_NombreEnvia, _Correo));
                email.To.Add(MailboxAddress.Parse(correodto.Para));
                email.Subject = correodto.Asunto;
                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = correodto.Contenido
                };

                var smtp = new SmtpClient();
                smtp.Connect(_Host, _Puerto, SecureSocketOptions.StartTls);

                smtp.Authenticate(_Correo, _Clave);
                smtp.Send(email);
                smtp.Disconnect(true);
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}