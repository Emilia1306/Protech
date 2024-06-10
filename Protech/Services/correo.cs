using Microsoft.Data.SqlClient;

namespace Protech.Services
{
    public class correo
    {

        private IConfiguration _configuration;

        public correo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void enviar(string destinatario, string asunto, string cuerpo)
        {
            try
            {
                string connectionString = _configuration.GetSection("ConnectionStrings").GetSection("ProtechDbConnection").Value;
                string sqlQuery = "exec msdb.dbo.sp_send_dbmail " +
                    "                    @profile_name = 'SQLMail_CATOLICA', " +
                    "                    @recipients = @par_destinatarios, " +
                    "                    @subject = @par_asunto, " +
                    "                    @body = @par_mensaje, " +
                    "                    @body_format = 'HTML';";

                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@par_destinatarios", destinatario);
                        command.Parameters.AddWithValue("@par_asunto", asunto);
                        command.Parameters.AddWithValue("@par_mensaje", cuerpo);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ");
                Console.WriteLine(ex.Message);

            }
        }


        public void ClientCreation(string destinatario, string username, string password)
        {
            string asunto = "¡Bienvenido/a a Protech! Accede a tus credenciales";

            string cuerpo = $@"
                <html>
                <head>
                    <style>

                        @import url('https://fonts.googleapis.com/css2?family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap');
                        body {{
                            font-family: 'Poppins';
                            color: #333333;
                            line-height: 1.6;
                        }}
                        .container {{
                            width: 100%;
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            border: 1px solid #e0e0e0;
                            border-radius: 10px;
                            box-shadow: 0 0 10px rgba(0,0,0,0.1);
                        }}
                        .header {{
                            text-align: center;
                        }}
                        .header img {{
                            width: 50px;
                        }}
                        .header p {{
                            font-size: 28px;
                            color: black;
                        }}
                        .ticket-number {{
                            font-size: 45px;
                            color: #4CAF50;
                            font-weight: bold;
                        }}
                        .content {{
                            margin-top: 20px;
                        }}
                        .content p {{
                            font-size: 16px;
                        }}
                        .details {{
                            margin-top: 20px;
                        }}
                        .details p {{
                            font-size: 16px;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 16px;
                            color: black;
                        }}
                        .bold {{
                            font-weight: bold;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <p>Bienvenido/a</p>
                            <div class='ticket-number'>{username}</div>
                        </div>
                        <div class='content'>
                            <p>Estimado/a {username}</p>
                            <p>Nos complace informarle que su cuenta ha sido creada con éxito. A continuación, encontrará las credenciales necesarias para acceder a su cuenta:</p>
                            <div class='details'>
                                <p><span class='bold'>Correo Electrónico:</span> {destinatario}</p>
                                <p><span class='bold'>Contraseña temporal:</span> {password}</p>

                                <p>Le recomendamos que, por motivos de seguridad, cambie su contraseña temporal la primera vez que acceda a su cuenta. Para cambiar su contraseña, siga estos pasos:</p>
                                <p>1. Inicie sesión en Protech con sus credenciales temporales.<p>
                                <p>2. Cambie su contraseña en la ventana emergente que saldrá cuando ingrese por primera vez.<p>
                            </div>
                        </div>
                        <div class='footer'>
                            <p>Gracias por ser parte de Protech<p>
                        </div>
                    </div>
                </body>
                </html>";

            enviar(destinatario, asunto, cuerpo);



        }


        public void EmployeeCreation(string destinatario, string username, string password)
        {
            string asunto = "¡Bienvenido/a a Protech! Accede a tus credenciales";

            string cuerpo = $@"
                <html>
                <head>
                    <style>

                        @import url('https://fonts.googleapis.com/css2?family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap');
                        body {{
                            font-family: 'Poppins';
                            color: #333333;
                            line-height: 1.6;
                        }}
                        .container {{
                            width: 100%;
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            border: 1px solid #e0e0e0;
                            border-radius: 10px;
                            box-shadow: 0 0 10px rgba(0,0,0,0.1);
                        }}
                        .header {{
                            text-align: center;
                        }}
                        .header img {{
                            width: 50px;
                        }}
                        .header p {{
                            font-size: 28px;
                            color: black;
                        }}
                        .ticket-number {{
                            font-size: 45px;
                            color: #4CAF50;
                            font-weight: bold;
                        }}
                        .content {{
                            margin-top: 20px;
                        }}
                        .content p {{
                            font-size: 16px;
                        }}
                        .details {{
                            margin-top: 20px;
                        }}
                        .details p {{
                            font-size: 16px;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 16px;
                            color: black;
                        }}
                        .bold {{
                            font-weight: bold;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <p>Bienvenido/a</p>
                            <div class='ticket-number'>{username}</div>
                        </div>
                        <div class='content'>
                            <p>Estimado/a {username}</p>
                            <p>Nos complace informarle que su cuenta ha sido creada con éxito. A continuación, encontrará las credenciales necesarias para acceder a su cuenta:</p>
                            <div class='details'>
                                <p><span class='bold'>Correo Electrónico:</span> {destinatario}</p>
                                <p><span class='bold'>Contraseña temporal:</span> {password}</p>

                                <p>Le recomendamos que, por motivos de seguridad, cambie su contraseña temporal la primera vez que acceda a su cuenta. Para cambiar su contraseña, siga estos pasos:</p>
                                <p>1. Inicie sesión en Protech con sus credenciales temporales.<p>
                                <p>2. Cambie su contraseña en la ventana emergente que saldrá cuando ingrese por primera vez.<p>
                            </div>
                        </div>
                        <div class='footer'>
                            <p>Gracias por trabajar con Protech<p>
                        </div>
                    </div>
                </body>
                </html>";

            enviar(destinatario, asunto, cuerpo);



        }


        public void TicketCreationConfirmation(string destinatario, string username, int numTicket, string ticketname, DateTime fechacreacion, string descripcion)
        {
            string asunto = "¡Ticket #" + numTicket + " creado con éxito! Accede a sus detalles";

            string cuerpo = $@"
                <html>
                <head>
                    <style>

                        @import url('https://fonts.googleapis.com/css2?family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap');
                        body {{
                            font-family: 'Poppins';
                            color: #333333;
                            line-height: 1.6;
                        }}
                        .container {{
                            width: 100%;
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            border: 1px solid #e0e0e0;
                            border-radius: 10px;
                            box-shadow: 0 0 10px rgba(0,0,0,0.1);
                        }}
                        .header {{
                            text-align: center;
                        }}
                        .header img {{
                            width: 50px;
                        }}
                        .header p {{
                            font-size: 28px;
                            color: black;
                        }}
                        .ticket-number {{
                            font-size: 45px;
                            color: #4CAF50;
                            font-weight: bold;
                        }}
                        .content {{
                            margin-top: 20px;
                        }}
                        .content p {{
                            font-size: 16px;
                        }}
                        .details {{
                            margin-top: 20px;
                        }}
                        .details p {{
                            font-size: 16px;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 16px;
                            color: black;
                        }}
                        .bold {{
                            font-weight: bold;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <p>¡Tu Ticket</p>
                            <div class='ticket-number'>#{numTicket}</div>
                            <p>ha sido creado con éxito!</p>
                        </div>
                        <div class='content'>
                            <p>Estimado/a {username}</p>
                            <p>Confirmamos la creación de su ticket en nuestro sistema, a continuación le brindamos los detalles.</p>
                            <div class='details'>
                                <p><span class='bold'>Número de Ticket:</span> {numTicket}</p>
                                <p><span class='bold'>Nombre del Ticket:</span> {ticketname}</p>
                                <p><span class='bold'>Descripción:</span> {descripcion}</p>
                                <p><span class='bold'>Fecha de creación:</span> {fechacreacion}</p>
                                <p><span class='bold'>Estado Inicial:</span> En proceso de asignación</p>
                            </div>
                        </div>
                        <div class='footer'>
                            <p>Nuestro equipo está trabajando en su solicitud, le mantendremos informado sobre el proceso.</p>
                        </div>
                    </div>
                </body>
                </html>";

            enviar(destinatario, asunto, cuerpo);



        }



        public void CustomerTicketAssignment(string destinatario, string username, int numTicket, DateTime fechacreacion, string descripcion, string employeename, string ticketname)
        {
            string asunto = "¡Tu Ticket #" + numTicket + " ha sido asignado a uno de nuestros técnicos! Accede a sus detalles";

            string cuerpo = $@"
                <html>
                <head>
                    <style>

                        @import url('https://fonts.googleapis.com/css2?family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap');
                        body {{
                            font-family: 'Poppins';
                            color: #333333;
                            line-height: 1.6;
                        }}
                        .container {{
                            width: 100%;
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            border: 1px solid #e0e0e0;
                            border-radius: 10px;
                            box-shadow: 0 0 10px rgba(0,0,0,0.1);
                        }}
                        .header {{
                            text-align: center;
                        }}
                        .header img {{
                            width: 50px;
                        }}
                        .header p {{
                            font-size: 28px;
                            color: black;
                        }}
                        .ticket-number {{
                            font-size: 45px;
                            color: #4CAF50;
                            font-weight: bold;
                        }}
                        .content {{
                            margin-top: 20px;
                        }}
                        .content p {{
                            font-size: 16px;
                        }}
                        .details {{
                            margin-top: 20px;
                        }}
                        .details p {{
                            font-size: 16px;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 16px;
                            color: black;
                        }}
                        .bold {{
                            font-weight: bold;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <p>¡Tu Ticket</p>
                            <div class='ticket-number'>#{numTicket}</div>
                            <p>ha sido asignado!</p>
                        </div>
                        <div class='content'>
                            <p>Estimado/a {username}</p>
                            <p>Confirmamos su ticket ha sido asignado a uno de nuestros técnicos, a continuación le brindamos los detalles.</p>
                            <div class='details'>
                                <p><span class='bold'>Número de Ticket:</span> {numTicket}</p>
                                <p><span class='bold'>Nombre del Ticket:</span> {ticketname}</p>
                                <p><span class='bold'>Descripción:</span> {descripcion}</p>
                                <p><span class='bold'>Empleado Asignado:</span> {employeename}</p>
                                <p><span class='bold'>Fecha de asignación:</span> {fechacreacion}</p>
                                
                            </div>
                        </div>
                        <div class='footer'>
                            <p>Nuestro equipo se encuentra trabajando en su ticket, agradecemos su espera.</p>
                        </div>
                    </div>
                </body>
                </html>";

            enviar(destinatario, asunto, cuerpo);



        }

        public void EmployeeTicketAssignment(string destinatario, string username, int numTicket, DateTime fechacreacion, string descripcion, string ticketname)
        {
            string asunto = "¡Te ha sido asignado el ticket #" + numTicket + "! Accede a sus detalles";

            string cuerpo = $@"
                <html>
                <head>
                    <style>

                        @import url('https://fonts.googleapis.com/css2?family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap');
                        body {{
                            font-family: 'Poppins';
                            color: #333333;
                            line-height: 1.6;
                        }}
                        .container {{
                            width: 100%;
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            border: 1px solid #e0e0e0;
                            border-radius: 10px;
                            box-shadow: 0 0 10px rgba(0,0,0,0.1);
                        }}
                        .header {{
                            text-align: center;
                        }}
                        .header img {{
                            width: 50px;
                        }}
                        .header p {{
                            font-size: 28px;
                            color: black;
                        }}
                        .ticket-number {{
                            font-size: 45px;
                            color: #4CAF50;
                            font-weight: bold;
                        }}
                        .content {{
                            margin-top: 20px;
                        }}
                        .content p {{
                            font-size: 16px;
                        }}
                        .details {{
                            margin-top: 20px;
                        }}
                        .details p {{
                            font-size: 16px;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 16px;
                            color: black;
                        }}
                        .bold {{
                            font-weight: bold;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <p>Ticket Asignado </p>
                            <div class='ticket-number'>#{numTicket}</div>
                            
                        </div>
                        <div class='content'>
                            <p>Estimado/a {username}</p>
                            <p>Le informamos que se te ha asignado un ticket para su resolución, a continuación le brindamos los detalles.</p>
                            <div class='details'>
                                <p><span class='bold'>Número de Ticket:</span> {numTicket}</p>
                                <p><span class='bold'>Nombre del Ticket:</span> {ticketname}</p>
                                <p><span class='bold'>Descripción:</span> {descripcion}</p>
                                <p><span class='bold'>Fecha de asignación:</span> {fechacreacion}</p>
                                
                            </div>
                        </div>
                        <div class='footer'>
                            <p>Esperamos la pronta resolución de este, muchas gracias por tu colaboración.</p>
                        </div>
                    </div>
                </body>
                </html>";

            enviar(destinatario, asunto, cuerpo);



        }


        public void NewTicketComment(string destinatario, string username, int numTicket, DateTime fechacreacion, string comentario, string ticketname)
        {
            string asunto = "¡Nuevo comentario sobre tu ticket #" + numTicket + "! Accede a sus detalles";

            string cuerpo = $@"
                <html>
                <head>
                    <style>

                        @import url('https://fonts.googleapis.com/css2?family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap');
                        body {{
                            font-family: 'Poppins';
                            color: #333333;
                            line-height: 1.6;
                        }}
                        .container {{
                            width: 100%;
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            border: 1px solid #e0e0e0;
                            border-radius: 10px;
                            box-shadow: 0 0 10px rgba(0,0,0,0.1);
                        }}
                        .header {{
                            text-align: center;
                        }}
                        .header img {{
                            width: 50px;
                        }}
                        .header p {{
                            font-size: 28px;
                            color: black;
                        }}
                        .ticket-number {{
                            font-size: 45px;
                            color: #4CAF50;
                            font-weight: bold;
                        }}
                        .content {{
                            margin-top: 20px;
                        }}
                        .content p {{
                            font-size: 16px;
                        }}
                        .details {{
                            margin-top: 20px;
                        }}
                        .details p {{
                            font-size: 16px;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 16px;
                            color: black;
                        }}
                        .bold {{
                            font-weight: bold;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <p>Ticket</p>
                            <div class='ticket-number'>#{numTicket}</div>
                            
                        </div>
                        <div class='content'>
                            <p>Estimado/a {username}</p>
                            <p>Le informamos que se ha publicado un nuevo comentario acerca del ticket {numTicket}, a continuación le brindamos los detalles.</p>
                            <div class='details'>
                                <p><span class='bold'>Número de Ticket:</span> {numTicket}</p>
                                <p><span class='bold'>Nombre del Ticket:</span> {ticketname}</p>
                                <p><span class='bold'>Comentario:</span> {comentario}</p>
                                <p><span class='bold'>Fecha de publicación:</span> {fechacreacion}</p>
                                
                            </div>
                        </div>
                        <div class='footer'>
                            <p>Nuestro equipo se encuentra trabajando en su ticket, agradecemos su espera.</p>
                        </div>
                    </div>
                </body>
                </html>";

            enviar(destinatario, asunto, cuerpo);



        }


        public void UpdateTicketStatus(string destinatario, string username, int numTicket, DateTime fechacreacion, string ticketname, string estadoTicket)
        {
            string asunto = "¡Su ticket #" + numTicket + " está en " + estadoTicket +"! Accede a sus detalles";

            string cuerpo = $@"
                <html>
                <head>
                    <style>

                        @import url('https://fonts.googleapis.com/css2?family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap');
                        body {{
                            font-family: 'Poppins';
                            color: #333333;
                            line-height: 1.6;
                        }}
                        .container {{
                            width: 100%;
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            border: 1px solid #e0e0e0;
                            border-radius: 10px;
                            box-shadow: 0 0 10px rgba(0,0,0,0.1);
                        }}
                        .header {{
                            text-align: center;
                        }}
                        .header img {{
                            width: 50px;
                        }}
                        .header p {{
                            font-size: 28px;
                            color: black;
                        }}
                        .ticket-number {{
                            font-size: 45px;
                            color: #4CAF50;
                            font-weight: bold;
                        }}
                        .content {{
                            margin-top: 20px;
                        }}
                        .content p {{
                            font-size: 16px;
                        }}
                        .details {{
                            margin-top: 20px;
                        }}
                        .details p {{
                            font-size: 16px;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 16px;
                            color: black;
                        }}
                        .bold {{
                            font-weight: bold;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <p>Ticket #{numTicket}</p>
                            <div class='ticket-number'>#{estadoTicket}</div>
                            
                        </div>
                        <div class='content'>
                            <p>Estimado/a {username}</p>
                            <p>Le informamos que el ticket #{numTicket} ha avanzado en su proceso de resolución, a continuación le brindamos los detalles.</p>
                            <div class='details'>

                                <p><span class='bold'>Número de Ticket:</span> {numTicket}</p>
                                <p><span class='bold'>Nombre de Ticket:</span> {ticketname}</p>
                                <br/>
                                <p><span class='bold'>Estado actual del ticket: {estadoTicket}</span></p>
                                <p><span class='bold'>Fecha de actualización:</span> {fechacreacion}</p>

                                
                            </div>
                        </div>
                        <div class='footer'>
                            <p>Esperamos la pronta resolución de este, muchas gracias por tu colaboración.</p>
                        </div>
                    </div>
                </body>
                </html>";

            enviar(destinatario, asunto, cuerpo);



        }



        public void TaskAssignment(string destinatario, string username, int numTask, int? idTicket, DateTime fechacreacion, string descripcion)
        {
            string asunto = "¡Te ha sido asignada una tarea! Accede a sus detalles";

            string cuerpo = $@"
                <html>
                <head>
                    <style>

                        @import url('https://fonts.googleapis.com/css2?family=Poppins:ital,wght@0,100;0,200;0,300;0,400;0,500;0,600;0,700;0,800;0,900;1,100;1,200;1,300;1,400;1,500;1,600;1,700;1,800;1,900&display=swap');
                        body {{
                            font-family: 'Poppins';
                            color: #333333;
                            line-height: 1.6;
                        }}
                        .container {{
                            width: 100%;
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            border: 1px solid #e0e0e0;
                            border-radius: 10px;
                            box-shadow: 0 0 10px rgba(0,0,0,0.1);
                        }}
                        .header {{
                            text-align: center;
                        }}
                        .header img {{
                            width: 50px;
                        }}
                        .header p {{
                            font-size: 28px;
                            color: black;
                        }}
                        .ticket-number {{
                            font-size: 45px;
                            color: #4CAF50;
                            font-weight: bold;
                        }}
                        .content {{
                            margin-top: 20px;
                        }}
                        .content p {{
                            font-size: 16px;
                        }}
                        .details {{
                            margin-top: 20px;
                        }}
                        .details p {{
                            font-size: 16px;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 16px;
                            color: black;
                        }}
                        .bold {{
                            font-weight: bold;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <p>Tarea</p>
                            <div class='ticket-number'>#{numTask}</div>
                        </div>
                        <div class='content'>
                            <p>Estimado/a {username}</p>
                            <p>Te ha sido asignada una nueva tarea. A continuación, encontrará los detalles necesarios para resolver la asignación:</p>
                            <div class='details'>
                                <p><span class='bold'>Ticket asociado a la tarea: </span> {idTicket}</p>
                                <p><span class='bold'>Descripción de la tarea:</span> {descripcion}</p>
                                <p><span class='bold'>Fecha de creación:</span> {fechacreacion}</p>
                            </div>
                        </div>
                        <div class='footer'>
                            <p>Protech te desea un buen día.</p>
                        </div>
                    </div>
                </body>
                </html>";

            enviar(destinatario, asunto, cuerpo);

        }


    }
}
