﻿using BancoDelSolModel.DAL;
using BancoDelSolModel.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BancoDelSolWeb.pages
{
    public partial class TransferenciaCliente : System.Web.UI.Page
    {
        String runCliente;
        int monto;
        int cuentaDestino;
        private ICuentaDAL cuentaDal = new CuentaDALObjetos();
        private IClienteDAL clienteDAL = new ClienteDALObjetos();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.divTransferenciaTercero.Visible = false;

                if (Request.QueryString["run"] == null) return;
                runCliente = Request.QueryString["run"].ToString();

                List<Cuenta> cuentaCliente = cuentaDal.Cuentasfiltradas(runCliente);

                this.gvCuentas.DataSource = cuentaCliente; //Cargar Grilla de Cuentas
                this.gvCuentas.DataBind(); //Actualizar Grilla
                
            }
        }

        protected void gvCuentas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "transferenciaTerceros")
            {
                int cuentaCliente = Convert.ToInt32(e.CommandArgument);
                this.divTransferenciaTercero.Visible = true;
                this.lblNumCunetaTercero.Text = Convert.ToString(cuentaCliente);
            }

        }

        protected void btnTransferirTercero_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                String runTercero = this.txtRunTercero.Text;
                cuentaDestino = Convert.ToInt32(this.txtCuentaTercero.Text);
                monto = Convert.ToInt32(this.txtMontoTerceros.Text);
                runCliente = Request.QueryString["run"].ToString();
                List<Cuenta> cuentaCliente = cuentaDal.Cuentasfiltradas(runCliente);
                int cuentaRemitente = cuentaCliente[0].Num_cuenta;

                List<Cuenta> cuentas = cuentaDal.Obtener();

                for (int i = 0; i < cuentas.Count(); i++)
                {
                    for (int j = 0; j < cuentas.Count(); j++)
                    {
                        if (cuentas[i].Num_cuenta == cuentaRemitente && cuentas[j].Num_cuenta == cuentaDestino && cuentaRemitente != cuentaDestino)
                        {
                            if (monto > cuentas[i].Saldo)
                            {
                                int diferencia = (monto - cuentas[i].Saldo);
                                if (diferencia > cuentas[i].Credito)
                                {
                                   //enviar mensaje que el monto no puede superar a la suma del saldo y el crédito
                                }
                                else
                                {
                                    cuentas[i].Credito = cuentas[i].Credito - diferencia;
                                    cuentas[i].Saldo = 0;
                                    cuentas[j].Saldo = cuentas[j].Saldo + monto;
                                    Movimiento movRemi = new Movimiento((cuentas[i].Movimientos.Count() + 100), cuentas[i], "Transferencia", monto);
                                    Movimiento movDest = new Movimiento((cuentas[j].Movimientos.Count() + 100), cuentas[j], "Transferencia", monto);
                                    cuentas[i].Movimientos.Add(movRemi);
                                    cuentas[j].Movimientos.Add(movDest);
                                    this.lblConfirmacionTransferencia.Text = "Se ha realizado la transferencia a " + cuentas[j].CuentaHabiente.Nombre + " " + cuentas[j].CuentaHabiente.Paterno + " exitosamente";
                                    this.lblConfirmacionTransferencia.Visible = true;
                                    this.gvCuentas.DataSource = cuentaCliente; //Cargar Grilla de Cuentas
                                    this.gvCuentas.DataBind(); //Actualizar Grilla
                                    this.divTransferenciaTercero.Visible = false;
                                }
                            }
                            else
                            {
                                cuentas[i].Saldo = cuentas[i].Saldo - monto;
                                cuentas[j].Saldo = cuentas[j].Saldo + monto;
                                Movimiento movRemi = new Movimiento((cuentas[i].Movimientos.Count() + 100), cuentas[i], "Transferencia", monto);
                                Movimiento movDest = new Movimiento((cuentas[j].Movimientos.Count() + 100), cuentas[j], "Transferencia", monto);
                                cuentas[i].Movimientos.Add(movRemi);
                                cuentas[j].Movimientos.Add(movDest);
                                this.lblConfirmacionTransferencia.Text = "Se ha realizado la transferencia a " + cuentas[j].CuentaHabiente.Nombre + " " + cuentas[j].CuentaHabiente.Paterno + " exitosamente";
                                this.lblConfirmacionTransferencia.Visible = true;
                                this.gvCuentas.DataSource = cuentaCliente; //Cargar Grilla de Cuentas
                                this.gvCuentas.DataBind(); //Actualizar Grilla
                                this.divTransferenciaTercero.Visible = false;
                            }
                        }
                        else
                        {
                            if (cuentaRemitente == cuentaDestino)
                            {
                                //enviar mensaje que no se puede transferir a la misma cuenta
                            }
                        }

                    }
                }
            }

        }

        protected void cvCuentaTercero_ServerValidate(object source, ServerValidateEventArgs args)
        {
            List<Cuenta> cuentas = cuentaDal.Obtener();
            cuentaDestino = Convert.ToInt32(this.txtCuentaTercero.Text);
            for(int i = 0; i < cuentas.Count(); i++)
            {
                if(cuentas[i].Num_cuenta == cuentaDestino)
                {
                        args.IsValid = true;
                }
            }

        }

        protected void cvRunTercero_ServerValidate(object source, ServerValidateEventArgs args)
        {
            List<Cliente> clientes = clienteDAL.Obtener();
            String run = this.txtRunTercero.Text;
            for (int i = 0; i < clientes.Count(); i++)
            {
                if (clientes[i].Run == run)
                {
                    args.IsValid = true;
                }
            }
        }

        protected void cvMontoTercero_ServerValidate(object source, ServerValidateEventArgs args)
        {
            List<Cuenta> cuentas = cuentaDal.Obtener();
            monto = Convert.ToInt32(this.txtMontoTerceros.Text);
            if (monto < 0)
            {
                cvMontoTercero.Text = "El monto no puede ser menor que 0";
                args.IsValid = false;
            }
            else
            {
                for (int i = 0; i < cuentas.Count(); i++)
                {
                    int diferencia = (monto - cuentas[i].Saldo);
                    if (diferencia > cuentas[i].Credito)
                    {

                        cvMontoTercero.Text = "El monto a transferir no puede superar su saldo y línea de crédito. Ingrese un monto menor";
                        args.IsValid = false;
                    }
                }
                
            }
        }

        protected void cvContrasena_ServerValidate(object source, ServerValidateEventArgs args)
        {
            List<Cuenta> cuentas = cuentaDal.Obtener();
            String clave = this.txtContrasena.Text;
            for (int i = 0; i < cuentas.Count(); i++)
            {
                if (clave.Equals(cuentas[i].Clave))
                {
                    args.IsValid = true;
                }
            }
       
        }
    }
}