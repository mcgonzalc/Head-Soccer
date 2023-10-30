﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.Remoting.Channels;

namespace Cliente
{
    public partial class PantallaSesionUsuario : Form
    {

        Socket server;
        Thread threadlogueo, threadsalaespera;
        delegate void DelegadoParaPonerTexto(string texto);

        List<SalaDeEspera> formularios = new List<SalaDeEspera>();
        public PantallaSesionUsuario()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        public void AtenderServidor()
        {
            while (true)
            {
                //Recibimos mensaje del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);

                //Creamos un vector con cada trozo del mensaje recibido (cada cosa que va por cada / es un "trozo")
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');

                //El primer trozo es el código de la operación realizada
                int codigo = Convert.ToInt32(trozos[0]);
                string respuestaservidor;

                switch (codigo)
                {
                    case 1:  //Queremos iniciar sesión en nuestra cuenta
                          respuestaservidor = trozos[2].Split('\0')[0];
                          if (respuestaservidor == "SI")
                            {
                                MessageBox.Show("Sesión iniciada correctamente, saludos " + trozos[1]);
                                //Arrancamos el thread que atenderá los mensajes del servidor
                                ThreadStart ts = delegate
                                {
                                    AbrirSaladeEspera();
                                };
                                threadsalaespera = new Thread(ts);
                                threadsalaespera.Start();
                            }
                          else if (respuestaservidor == "NO")
                            {
                                MessageBox.Show("Combinación de usuario y contraseña incorrecta");
                            }
                        break;

                    case 2:  //Queremos crearnos una nueva cuenta
                            respuestaservidor = trozos[2].Split('\0')[0];
                            if (respuestaservidor == "SI")
                            {
                                MessageBox.Show("Cuenta creada satisfactoriamente, saludos " + trozos[1]);
                            }
                            else if (respuestaservidor == "NO")
                            {
                                MessageBox.Show("El nombre de usuario facilitado ya existe, prueba con otro que esté disponible");
                            }
                            else if (respuestaservidor == "ERROR")
                        {
                            MessageBox.Show("Ha ocurrido un error inesperado, prueba de intentarlo hacer más tarde");
                        }
                        break;

                    case 3:
                          respuestaservidor = trozos[1].Split('\0')[0];
                          formularios[0].ModificarResultadoConsulta(respuestaservidor);
                        break;

                    case 4:
                          respuestaservidor = trozos[1].Split('\0')[0];
                          formularios[0].ModificarResultadoConsulta(respuestaservidor);
                        break;

                    case 5:
                           respuestaservidor = trozos[1].Split('\0')[0];
                           formularios[0].ModificarResultadoConsulta(respuestaservidor);
                        break;

                    case 6:
                          respuestaservidor = trozos[1].Split('\0')[0];
                          formularios[0].ActualizarListaConectados(respuestaservidor);
                        break;
                }
            }
        }

        public void AbrirSaladeEspera()
        {
            SalaDeEspera SaladeEspera = new SalaDeEspera(server);
            formularios.Add(SaladeEspera);
            SaladeEspera.ShowDialog();
        }
        private void OpcionInicioSesion_CheckedChanged(object sender, EventArgs e)
        {
            if (OpcionInicioSesion.Checked == true)
            {
                BotonInicioSesion.Enabled = true;
                BotonRegistroCuenta.Enabled = false;
            }
        }

        private void OpcionCuentaNueva_CheckedChanged(object sender, EventArgs e)
        {
            if (OpcionCuentaNueva.Checked == true)
            {
                BotonInicioSesion.Enabled = false;
                BotonRegistroCuenta.Enabled = true;
            }
        }

        private void BotonRegistroCuenta_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con la IP del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.102");
            IPEndPoint ipep = new IPEndPoint(direc, 9051);

            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                if ((Contrasena.Text != "") && (Usuario.Text != ""))
                {
                    server.Connect(ipep); //Intentamos conectar el socket
                    this.BackColor = Color.Green;
                    MessageBox.Show("Conectado al servidor correctamente");
                    string mensaje = "2/" + Usuario.Text + '/' + Contrasena.Text;
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    //Arrancamos el thread que atenderá los mensajes del servidor
                    ThreadStart ts = delegate
                    {
                        AtenderServidor();
                    };
                    threadlogueo = new Thread(ts);
                    threadlogueo.Start();
                }

                else
                {
                    MessageBox.Show("No has introducido todos los datos necesarios para loguearte o registrarte");
                }
            }
            catch (SocketException)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No se ha podido conectar con el servidor");
                return;
            }
        }

        private void BotonInicioSesion_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con la IP del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.102");
            IPEndPoint ipep = new IPEndPoint(direc, 9051);

            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                if ((Contrasena.Text != "") || (Usuario.Text != ""))
                {
                    server.Connect(ipep); //Intentamos conectar el socket
                    this.BackColor = Color.Green;
                    MessageBox.Show("Conectado al servidor correctamente");
                    string mensaje = "1/" + Usuario.Text + '/' + Contrasena.Text;
                    // Enviamos al servidor el nombre tecleado
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                    server.Send(msg);
                    //Arrancamos el thread que atenderá los mensajes del servidor
                    ThreadStart ts = delegate
                    {
                        AtenderServidor();
                    };
                    threadlogueo = new Thread(ts);
                    threadlogueo.Start();
                }
                else
                {
                    MessageBox.Show("No has introducido todos los datos necesarios para loguearte o registrarte");
                }         
            }
            catch (SocketException)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No se ha podido conectar con el servidor");
                return;
            }
        }

        private void PantallaSesionUsuario_Load(object sender, EventArgs e)
        {

        }

        private void PantallaSesionUsuario_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.BackColor == Color.Green)
            {
                //Mensaje de desconexión
                string mensaje = "0/" + Usuario.Text;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                // Nos desconectamos
                threadlogueo.Abort();
                threadsalaespera.Abort();
                this.BackColor = Color.Gray;
                server.Shutdown(SocketShutdown.Both);
                server.Close();
            } 
        }
    }
}
