﻿using CajerosBTBot.Bean;
using CajerosBTBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;

namespace CajerosBTBot.implementaciones
{
    public class CajeroDaoImpl : IConsultorDB
    {
        public Boolean ObtenerEstatusCajero(string cajero)
        {
           Boolean est = false;
            try
            {
                //SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                //builder.DataSource = "serviciobt.database.windows.net";
                //builder.UserID = "adminservbt";
                //builder.Password = "serv.bt0916";
                //builder.InitialCatalog = "serviciobanorte-btdb";
                string myConnStr = ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;

                //using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                using (SqlConnection connection = new SqlConnection(myConnStr))
                {
                    connection.Open();

                    StringBuilder cn = new StringBuilder();
                    cn.Append(" select count(1) from falla_f_fallas_diaria ");
                    cn.Append(" where id_producto = '" + cajero + "'");
                    String res = cn.ToString();
                    SqlCommand comm = new SqlCommand(res, connection);
                    Int32 count = (Int32)comm.ExecuteScalar();
                    if (count > 0)
                    {
                        est = true;

                    }
                    else {
                        est = false;
                    }
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            
            return est;
        }

        public Boolean ObtenerCajero(string cajero)
        {
            Boolean est = false;
            try
            { 
                string myConnStr = ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(myConnStr))
                {
                    connection.Open();

                    StringBuilder cn = new StringBuilder();
                    cn.Append(" select count(1) from atm_d_cajero ");
                    cn.Append(" where id_cajero = '" + cajero + "'");
                    cn.Append(" and tipo_producto=2 ");
                    String res = cn.ToString();
                    SqlCommand comm = new SqlCommand(res, connection);
                    Int32 count = (Int32)comm.ExecuteScalar();
                    if (count > 0)
                    {
                        est = true;

                    }
                    else
                    {
                        est = false;
                    }
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return est;
        }

        public List<Cajero> ObtenerFallaCajero(string cajero)
        {
            List<Cajero> cajeros = new List<Cajero>();
            try
            {

                string myConnStr = ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(myConnStr))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    connection.Open();
                      
                        StringBuilder sb = new StringBuilder();
                        sb.Append("SELECT d.id_producto as cajero, d.fecha_inicio as fecha, f.tipo_falla as falla, d.folio, sum(d.conteo) as conteo, d.responsable, d.fecha_estimada_solucion as fechasolucion, ");
                        sb.Append(" e.empresa, e.grupo");
                        sb.Append(" FROM falla_f_fallas_diaria d");
                        sb.Append(" join falla_d_fallas f ");
                        sb.Append(" on f.id_falla = d.id_falla");
                        sb.Append(" left join atm_d_cajero a on a.id_cajero = d.id_producto");
                        sb.Append(" left join cat_d_empresa_grupo e on e.id_empresa = a.id_empresa ");
                        sb.Append(" where d.id_producto='" + cajero + "'");
                        sb.Append(" and d.id_tipo_producto = 2");
                        sb.Append(" group by d.id_producto, d.fecha_inicio, f.tipo_falla, d.folio, d.responsable, d.fecha_estimada_solucion,");
                        sb.Append(" e.empresa, e.grupo");
                    String sql = sb.ToString();


                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            SqlDataReader myReader = null;
                            myReader = command.ExecuteReader();

                            while (myReader.Read())
                            {
                                Cajero cajeroBean = new Cajero();

                                cajeroBean.cajero = myReader["cajero"].ToString();
                                cajeroBean.fecha = myReader["fecha"].ToString();
                                cajeroBean.conteo = myReader["conteo"].ToString();
                                cajeroBean.tipoFalla = myReader["falla"].ToString();
                                cajeroBean.folio = myReader["folio"].ToString();
                                cajeroBean.responsable= myReader["responsable"].ToString();
                                cajeroBean.fechasolucion= myReader["fechasolucion"].ToString();
                                cajeroBean.empresa= myReader["empresa"].ToString();
                                cajeroBean.grupo= myReader["grupo"].ToString();

                            cajeros.Add(cajeroBean);
                            }

                        }
                    
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return cajeros;
        }





        public List<Empresa> obtenerHistoricoCajeroEmpresa(string empresa, string periodo)
        {
            List<Empresa> cajeros = new List<Empresa>();
            try
            {

                string myConnStr = ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(myConnStr))
                {
                    connection.Open();
                    StringBuilder cn = new StringBuilder();
                    switch (periodo)
                    {
                        case "TOP 5":
                            cn.Append(" SELECT TOP 5a.id_cajero as cajero, s.tipo_falla as tipofalla, e.empresa, f.folio, f.fecha_inicio as fecha ");
                            cn.Append(" from falla_f_fallas_diaria f");
                            cn.Append(" left join falla_d_fallas s on s.id_falla =f.id_falla");
                            cn.Append(" left join atm_d_cajero a on a.id_cajero = f.id_producto");
                            cn.Append(" left join cat_d_empresa_grupo e on e.id_empresa=a.id_empresa");
                            cn.Append(" left join falla_d_fallas d on d.id_falla = f.id_falla");
                            cn.Append(" where f.id_tipo_producto = 2");
                            cn.Append(" and e.empresa like'%" + empresa + "%'");
                            cn.Append(" order by fecha_inicio desc ");
                            break;

                        case "MONTH":
                            cn.Append(" SELECT a.id_cajero as cajero, s.tipo_falla as tipofalla, e.empresa, f.folio, f.fecha as fecha ");
                            cn.Append(" from falla_f_fallas_diaria f");
                            cn.Append(" left join falla_d_fallas s on s.id_falla =f.id_falla");
                            cn.Append(" left join atm_d_cajero a on a.id_cajero = f.id_producto");
                            cn.Append(" left join cat_d_empresa_grupo e on e.id_empresa=a.id_empresa");
                            cn.Append(" left join falla_d_fallas d on d.id_falla = f.id_falla");
                            cn.Append(" where f.id_tipo_producto = 2");
                            cn.Append(" and e.empresa like'%" + empresa + "%'");
                            cn.Append(" and datepart(mm, f.fecha)= datepart(mm, getdate()) ");
                            cn.Append(" order by fecha desc ");
                            break;
                        default:
                            {
                                cn.Append(" SELECT a.id_cajero as cajero, s.tipo_falla as tipofalla, e.empresa, f.folio, f.fecha as fecha ");
                                cn.Append(" from falla_f_fallas_diaria2 f");
                                cn.Append(" left join falla_d_fallas s on s.id_falla =f.id_falla");
                                cn.Append(" left join atm_d_cajero a on a.id_cajero = f.id_producto");
                                cn.Append(" left join cat_d_empresa_grupo e on e.id_empresa=a.id_empresa");
                                cn.Append(" left join falla_d_fallas d on d.id_falla = f.id_falla");
                                cn.Append(" where f.id_tipo_producto = 2");
                                cn.Append(" and e.empresa like'%" + empresa + "%'");
                                cn.Append(" and datepart(mm, f.fecha)= datepart(mm, getdate()) ");
                                cn.Append(" order by fecha desc ");
                                break;
                            }
                    }
                    String sql = cn.ToString();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        SqlDataReader myReader = null;
                        myReader = command.ExecuteReader();

                        while (myReader.Read())
                        {
                            Empresa cajeroBean = new Empresa();

                            cajeroBean.cajero = myReader["cajero"].ToString();
                            cajeroBean.fecha = myReader["fecha"].ToString();
                            cajeroBean.empresa = myReader["empresa"].ToString();
                            //cajeroBean.conteo = myReader["conteo"].ToString();                            
                            cajeroBean.tipoFalla = myReader["tipofalla"].ToString();
                            cajeroBean.folio = myReader["folio"].ToString();

                            cajeros.Add(cajeroBean);
                        }

                    }

                }


            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            return cajeros;
        }


        public List<Grupo> obtenerHistoricoCajeroGrupo(string grupo, string periodo)
        {
            List<Grupo> cajeros = new List<Grupo>();
            try
            {


                string myConnStr = ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(myConnStr))
                {
                    connection.Open();
                    StringBuilder cn = new StringBuilder();
                    switch (periodo)
                    {
                        case "TOP 5":
                            cn.Append(" SELECT TOP 5a.id_cajero as cajero, s.tipo_falla as tipofalla, e.empresa, f.folio, f.fecha_inicio as fecha ");
                            cn.Append(" from falla_f_fallas_diaria f");
                            cn.Append(" left join falla_d_fallas s on s.id_falla =f.id_falla");
                            cn.Append(" left join atm_d_cajero a on a.id_cajero = f.id_producto");
                            cn.Append(" left join cat_d_empresa_grupo e on e.id_empresa=a.id_empresa");
                            cn.Append(" left join falla_d_fallas d on d.id_falla = f.id_falla");
                            cn.Append(" where f.id_tipo_producto = 2");
                            cn.Append(" and e.grupo like'%" + grupo + "%'");
                            cn.Append(" order by fecha_inicio desc ");
                            break;

                        case "MONTH":
                            cn.Append(" SELECT a.id_cajero as cajero, s.tipo_falla as tipofalla, e.empresa, f.folio, f.fecha as fecha ");
                            cn.Append(" from falla_f_fallas_diaria f");
                            cn.Append(" left join falla_d_fallas s on s.id_falla =f.id_falla");
                            cn.Append(" left join atm_d_cajero a on a.id_cajero = f.id_producto");
                            cn.Append(" left join cat_d_empresa_grupo e on e.id_empresa=a.id_empresa");
                            cn.Append(" left join falla_d_fallas d on d.id_falla = f.id_falla");
                            cn.Append(" where f.id_tipo_producto = 2");
                            cn.Append(" and e.grupo like'%" + grupo + "%'");
                            cn.Append(" and datepart(mm, f.fecha)= datepart(mm, getdate()) ");
                            cn.Append(" order by fecha desc ");
                            break;
                        default:
                            {
                                cn.Append(" SELECT a.id_cajero as cajero, s.tipo_falla as tipofalla, e.empresa, f.folio, f.fecha as fecha ");
                                cn.Append(" from falla_f_fallas_diaria f");
                                cn.Append(" left join falla_d_fallas s on s.id_falla =f.id_falla");
                                cn.Append(" left join atm_d_cajero a on a.id_cajero = f.id_producto");
                                cn.Append(" left join cat_d_empresa_grupo e on e.id_empresa=a.id_empresa");
                                cn.Append(" left join falla_d_fallas d on d.id_falla = f.id_falla");
                                cn.Append(" where f.id_tipo_producto = 2");
                                cn.Append(" and e.grupo like'%" + grupo + "%'");
                                cn.Append(" and datepart(mm, f.fecha)= datepart(mm, getdate()) ");
                                cn.Append(" order by fecha desc ");
                                break;
                            }
                    }
                    String sql = cn.ToString();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        SqlDataReader myReader = null;
                        myReader = command.ExecuteReader();

                        while (myReader.Read())
                        {
                            Grupo cajeroBean = new Grupo();

                            cajeroBean.cajero = myReader["cajero"].ToString();
                            cajeroBean.fecha = myReader["fecha"].ToString();
                            cajeroBean.empresa = myReader["empresa"].ToString();
                            //cajeroBean.conteo = myReader["conteo"].ToString();                            
                            cajeroBean.tipoFalla = myReader["tipofalla"].ToString();
                            cajeroBean.folio = myReader["folio"].ToString();

                            cajeros.Add(cajeroBean);
                        }

                    }

                }


            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            return cajeros;
        }

        public List<Cajero> obtenerHistoricoCajero(string cajero, string periodo)
        {
            List<Cajero> cajeros = new List<Cajero>();
            try
            {

                string myConnStr = ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(myConnStr))
                {
                    connection.Open();
                    StringBuilder cn = new StringBuilder();

                    switch (periodo)
                    {
                        case "TOP 5":
                            cn.Append(" SELECT TOP 5a.id_cajero as cajero, s.tipo_falla as tipofalla, e.empresa, f.folio, f.fecha_inicio as fecha ");
                            cn.Append(" from falla_f_fallas_diaria f");
                            cn.Append(" left join falla_d_fallas s on s.id_falla =f.id_falla");
                            cn.Append(" left join atm_d_cajero a on a.id_cajero = f.id_producto");
                            cn.Append(" left join cat_d_empresa_grupo e on e.id_empresa=a.id_empresa");
                            cn.Append(" left join falla_d_fallas d on d.id_falla = f.id_falla");
                            cn.Append(" where f.id_tipo_producto = 2");
                            cn.Append(" and f.id_producto ='"+cajero+"'");
                            cn.Append(" order by fecha_inicio desc ");
                            break;

                        case "MONTH":
                            cn.Append(" SELECT a.id_cajero as cajero, s.tipo_falla as tipofalla, e.empresa, f.folio, f.fecha as fecha ");
                            cn.Append(" from falla_f_fallas_diaria f");
                            cn.Append(" left join falla_d_fallas s on s.id_falla =f.id_falla");
                            cn.Append(" left join atm_d_cajero a on a.id_cajero = f.id_producto");
                            cn.Append(" left join cat_d_empresa_grupo e on e.id_empresa=a.id_empresa");
                            cn.Append(" left join falla_d_fallas d on d.id_falla = f.id_falla");
                            cn.Append(" where f.id_tipo_producto = 2");
                            cn.Append(" and f.id_producto ='" + cajero + "'");
                            cn.Append(" and datepart(mm, f.fecha)= datepart(mm, getdate()) ");
                            cn.Append(" order by fecha desc ");
                            break;
                        default: {
                                cn.Append(" SELECT a.id_cajero as cajero, s.tipo_falla as tipofalla, e.empresa, f.folio, f.fecha as fecha ");
                                cn.Append(" from falla_f_fallas_diaria f");
                                cn.Append(" left join falla_d_fallas s on s.id_falla =f.id_falla");
                                cn.Append(" left join atm_d_cajero a on a.id_cajero = f.id_producto");
                                cn.Append(" left join cat_d_empresa_grupo e on e.id_empresa=a.id_empresa");
                                cn.Append(" left join falla_d_fallas d on d.id_falla = f.id_falla");
                                cn.Append(" where f.id_tipo_producto = 2");
                                cn.Append(" and f.id_producto ='" + cajero + "'");
                                cn.Append(" and datepart(mm, f.fecha)= datepart(mm, getdate()) ");
                                cn.Append(" order by fecha desc ");
                                break;
                            }
                    }
                    String sql = cn.ToString();
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        SqlDataReader myReader = null;
                        myReader = command.ExecuteReader();

                        while (myReader.Read())
                        {
                            Cajero cajeroBean = new Cajero();

                            cajeroBean.cajero = myReader["cajero"].ToString();
                            cajeroBean.fecha = myReader["fecha"].ToString();
                            //cajeroBean.conteo = myReader["conteo"].ToString();                            
                            cajeroBean.tipoFalla = myReader["tipofalla"].ToString();
                            cajeroBean.folio = myReader["folio"].ToString();

                            cajeros.Add(cajeroBean);
                        }

                    }



                }

             }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            return cajeros;
        }

        public Boolean obtenerEstatusCajerosEmpresa(string empresa)
        {
            Boolean est = false;
            try
            {


                string myConnStr = ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(myConnStr))
                {
                    connection.Open();

                    var emp = String.Empty;

                    StringBuilder cn = new StringBuilder();
                    cn.Append(" select count(*) from falla_f_fallas_diaria f ");
                    cn.Append(" left join atm_d_cajero a on a.id_cajero = f.id_producto ");
                    cn.Append(" left join cat_d_empresa_grupo e on e.id_empresa = a.id_empresa ");
                    cn.Append(" where f.id_tipo_producto = 2");
                    cn.Append(" and e.empresa like  '%" +empresa+ "%'");
                    //cn.Append(" group by a.id_empresa ");
                    String res = cn.ToString();
                    SqlCommand comm = new SqlCommand(res, connection);
                    Int32 count = (Int32)comm.ExecuteScalar();
                    if (count > 0)
                    {
                        est = true;

                    }
                    else
                    {
                        est = false;
                    }
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return est;
        }


        public Int32 obtenerConteoCajerosEmpresa(string empresa)
        {
            Int32 count = 0;
            try
            {
                
                string myConnStr = ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(myConnStr))
                {
                    connection.Open();

                    var emp = String.Empty;
   
                    StringBuilder cn = new StringBuilder();
                    cn.Append(" select count(*) from falla_f_fallas_diaria f ");
                    cn.Append(" left join atm_d_cajero a on a.id_cajero = f.id_producto ");
                    cn.Append(" left join cat_d_empresa_grupo e on e.id_empresa = a.id_empresa ");
                    cn.Append(" where f.id_tipo_producto = 2");
                    cn.Append(" and e.empresa like  '%" + empresa + "%'");    
                    String res = cn.ToString();
                    SqlCommand comm = new SqlCommand(res, connection);
                    count = (Int32)comm.ExecuteScalar();
     
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return count;
        }


        public Int32 obtenerConteoCajerosGrupo(string grupo)
        {
            Int32 count = 0;
            try
            {

                string myConnStr = ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(myConnStr))
                {
                    connection.Open();

                    var emp = String.Empty;

                    StringBuilder cn = new StringBuilder();
                    cn.Append(" select count(*) from falla_f_fallas_diaria f ");
                    cn.Append(" left join atm_d_cajero a on a.id_cajero = f.id_producto ");
                    cn.Append(" left join cat_d_empresa_grupo e on e.id_empresa = a.id_empresa ");
                    cn.Append(" where f.id_tipo_producto = 2");
                    cn.Append(" and e.grupo like  '%" + grupo + "%'");
                    String res = cn.ToString();
                    SqlCommand comm = new SqlCommand(res, connection);
                    count = (Int32)comm.ExecuteScalar();

                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return count;
        }

        public Boolean obtenerEstatusCajerosGrupo(string grupo)
        {
            Boolean est = false;
            try
            {         
                string myConnStr = ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(myConnStr))
                {
                    connection.Open();

                    var emp = String.Empty;

                    StringBuilder cn = new StringBuilder();
                    cn.Append(" select count(*) from falla_f_fallas_diaria f ");
                    cn.Append(" left join atm_d_cajero a on a.id_cajero = f.id_producto ");
                    cn.Append(" left join cat_d_empresa_grupo e on e.id_empresa = a.id_empresa ");
                    cn.Append(" where f.id_tipo_producto = 2");
                    cn.Append(" and e.grupo like  '%" + grupo + "%'");              
                    String res = cn.ToString();
                    SqlCommand comm = new SqlCommand(res, connection);
                    Int32 count = (Int32)comm.ExecuteScalar();
                    if (count > 0)
                    {
                        est = true;

                    }
                    else
                    {
                        est = false;
                    }
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return est;
        }

        public List<Empresa> ObtenerFallasEmpresa(string empresa)
        {
            List<Empresa> cajeros = new List<Empresa>();
            try
            {


                string myConnStr = ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(myConnStr))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    connection.Open();
 

                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT a.id_cajero as cajero, e.empresa, s.tipo_falla as tipoFalla, a.id_empresa, f.folio, e.grupo, ");
                    sb.Append(" f.fecha_inicio as fecha, f.folio, f.responsable, f.fecha_estimada_solucion as fechasolucion ");
                    sb.Append(" FROM falla_f_fallas_diaria f");
                    sb.Append(" left join falla_d_fallas s on s.id_falla=f.id_falla ");
                    sb.Append(" left join atm_d_cajero a on a.id_cajero = f.id_producto");
                    sb.Append(" left join cat_d_empresa_grupo e on e.id_empresa = a.id_empresa");
                    sb.Append(" where e.empresa like '%' +replace('" + empresa + "',' ','_') +'%'");
                    sb.Append(" and f.id_tipo_producto = 2");
                    String sql = sb.ToString();


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        SqlDataReader myReader = null;
                        myReader = command.ExecuteReader();

                        while (myReader.Read())
                        {
                            Empresa cajeroBean = new Empresa();

                            cajeroBean.cajero = myReader["cajero"].ToString();
                            cajeroBean.empresa = myReader["empresa"].ToString();
                            cajeroBean.id_empresa = myReader["id_empresa"].ToString();
                          //  cajeroBean.fecha = myReader["fecha"].ToString();
                          // cajeroBean.conteo = myReader["conteo"].ToString();
                            cajeroBean.tipoFalla = myReader["tipoFalla"].ToString();
                            cajeroBean.folio = myReader["folio"].ToString();
                            cajeroBean.grupo= myReader["grupo"].ToString();
                            cajeroBean.fecha= myReader["fecha"].ToString();
                            cajeroBean.responsable= myReader["responsable"].ToString();
                            cajeroBean.fechasolucion= myReader["fechasolucion"].ToString();



                            cajeros.Add(cajeroBean);
                        }

                    }

                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return cajeros;
        }

        public List<Grupo> ObtenerFallasGrupo(string grupo)
        {
            List<Grupo> cajeros = new List<Grupo>();
            try
            {

                string myConnStr = ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(myConnStr))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    connection.Open();

                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT a.id_cajero as cajero, e.grupo,e.empresa, s.tipo_falla as tipoFalla, e.id_grupo, f.folio,  ");
                    sb.Append(" f.fecha_inicio as fecha, f.folio, f.responsable, f.fecha_estimada_solucion as fechasolucion ");
                    sb.Append("FROM falla_f_fallas_diaria f");
                    sb.Append(" left join falla_d_fallas s on s.id_falla=f.id_falla ");
                    sb.Append(" left join atm_d_cajero a on a.id_cajero = f.id_producto");
                    sb.Append(" left join cat_d_empresa_grupo e on e.id_empresa = a.id_empresa");
                    sb.Append(" where e.grupo like '%' +replace('" + grupo + "',' ','_') +'%'");
                    sb.Append(" and f.id_tipo_producto = 2");
                    String sql = sb.ToString();


                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        SqlDataReader myReader = null;
                        myReader = command.ExecuteReader();

                        while (myReader.Read())
                        {
                            Grupo cajeroBean = new Grupo();

                            cajeroBean.cajero = myReader["cajero"].ToString();
                            cajeroBean.grupo= myReader["grupo"].ToString();
                            cajeroBean.empresa = myReader["empresa"].ToString();
                            cajeroBean.id_grupo = myReader["id_grupo"].ToString();
                            cajeroBean.tipoFalla = myReader["tipoFalla"].ToString();
                            cajeroBean.folio = myReader["folio"].ToString();
                            cajeroBean.fecha = myReader["fecha"].ToString();
                            cajeroBean.responsable = myReader["responsable"].ToString();
                            cajeroBean.fechasolucion = myReader["fechasolucion"].ToString();

                            cajeros.Add(cajeroBean);
                        }

                    }

                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return cajeros;
        }


        public List<Empresa> ObtenerEmpresas(string empresa)
        {
            List<Empresa> empresas = new List<Empresa>();
            try
            {
 

                string myConnStr = ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;


                using (SqlConnection connection = new SqlConnection(myConnStr))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    connection.Open();

                    var emp = String.Empty;
 

                    StringBuilder sb = new StringBuilder();
                    sb.Append(" select distinct(e.empresa), e.empresa, e.id_empresa from atm_d_cajero a ");
                    sb.Append(" left join cat_d_empresa_grupo e on e.id_empresa = a.id_empresa  ");
                    sb.Append(" where a.tipo_producto = 2  ");
                    sb.Append(" and e.empresa like '%'+replace('" + empresa + "',' ','_') +'%'");
                   


                    String sql = sb.ToString();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        SqlDataReader myReader = null;
                        myReader = command.ExecuteReader();

                        while (myReader.Read())
                        {
                            Empresa cajeroBean = new Empresa();                          
                            cajeroBean.empresa = myReader["empresa"].ToString();
                            cajeroBean.id_empresa = myReader["id_empresa"].ToString();                             
                            empresas.Add(cajeroBean);
                        }

                    }

                }

            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }


            return empresas;
        }


        public List<Grupo> ObtenerGrupos(string grupo)
        {
            List<Grupo> grupos = new List<Grupo>();
            try
            {
 
                string myConnStr = ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(myConnStr))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    connection.Open();

                    var emp = String.Empty;


                    StringBuilder sb = new StringBuilder();
                    sb.Append(" select distinct(e.grupo), e.grupo, e.id_grupo from atm_d_cajero a ");
                    sb.Append(" left join cat_d_empresa_grupo e on e.id_empresa = a.id_empresa  ");
                    sb.Append(" where a.tipo_producto = 2  ");
                    sb.Append(" and e.grupo like '%'+replace('" + grupo + "',' ','_') +'%'");

                    String sql = sb.ToString();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        SqlDataReader myReader = null;
                        myReader = command.ExecuteReader();

                        while (myReader.Read())
                        {
                            Grupo cajeroBean = new Grupo();
                            cajeroBean.grupo = myReader["grupo"].ToString();
                            cajeroBean.id_grupo = myReader["id_grupo"].ToString();
                            grupos.Add(cajeroBean);
                        }

                    }

                }

            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }


            return grupos;
        }

        public List<Tiempo> obtenerPeriodoSolucion(string cajero)
        {
            List<Tiempo> tiempo = new List<Tiempo>();
            try
            {


                string myConnStr = ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(myConnStr))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    connection.Open();

                    StringBuilder sb = new StringBuilder();
                    sb.Append(" select  id_producto as cajero, fecha_estimada_solucion as fechaestimada, ");
                    sb.Append(" responsable from falla_f_fallas_diaria    ");                    
                    sb.Append(" where id_tipo_producto = 2  and id_producto='" + cajero +"'");                                        

                    String sql = sb.ToString();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        SqlDataReader myReader = null;
                        myReader = command.ExecuteReader();

                        while (myReader.Read())
                        {
                            Tiempo cajeroBean = new Tiempo();
                            cajeroBean.cajero = myReader["cajero"].ToString();
                            cajeroBean.fechaestimada = myReader["fechaestimada"].ToString();
                            cajeroBean.responsable = myReader["responsable"].ToString();
                            tiempo.Add(cajeroBean);
                        }

                    }

                }

                }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            return tiempo;
        }

        public List<Cajero> obtenerResponsable(string cajero) {
            List<Cajero> cajeros = new List<Cajero>();
            try
            {

                string myConnStr = ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;

                //using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                using (SqlConnection connection = new SqlConnection(myConnStr))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");
                    connection.Open();

                    StringBuilder sb = new StringBuilder();
                    sb.Append(" select  id_producto as cajero,  ");
                    sb.Append(" responsable from falla_f_fallas_diaria    ");
                    sb.Append(" where id_tipo_producto = 2  and id_producto='" + cajero + "'");

                    String sql = sb.ToString();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        SqlDataReader myReader = null;
                        myReader = command.ExecuteReader();

                        while (myReader.Read())
                        {
                            Cajero cajeroBean = new Cajero();
                            cajeroBean.cajero = myReader["cajero"].ToString();                           
                            cajeroBean.responsable = myReader["responsable"].ToString();
                            cajeros.Add(cajeroBean);
                        }

                    }

                }

            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            return cajeros;
        }

        public List<Empresa> obtenerResponsableEmpresa(string empresa)
        {
            List<Empresa> cajeros = new List<Empresa>();
            try
            {


                string myConnStr = ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(myConnStr))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");
                    connection.Open();

                    StringBuilder sb = new StringBuilder();
                    sb.Append(" select  id_producto as cajero,  ");
                    sb.Append(" responsable, e.empresa from falla_f_fallas_diaria   f ");
                    sb.Append(" left join atm_d_cajero a on a.id_cajero = f.id_producto ");
                    sb.Append(" left join cat_d_empresa_grupo e on e.id_empresa = a.id_empresa ");
                    sb.Append(" where id_tipo_producto = 2  and e.empresa like '%' +replace('" + empresa + "',' ','_')+' %'");

                    String sql = sb.ToString();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        SqlDataReader myReader = null;
                        myReader = command.ExecuteReader();

                        while (myReader.Read())
                        {
                            Empresa cajeroBean = new Empresa();
                            cajeroBean.cajero = myReader["cajero"].ToString();
                            cajeroBean.responsable = myReader["responsable"].ToString();
                            cajeros.Add(cajeroBean);
                        }

                    }

                }

            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            return cajeros;
        }

        public List<Grupo> obtenerResponsableGrupo(string grupo)
        {
            List<Grupo> cajeros = new List<Grupo>();
            try
            {

                string myConnStr = ConfigurationManager.ConnectionStrings["Dbconnection"].ConnectionString;

                //using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                using (SqlConnection connection = new SqlConnection(myConnStr))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");
                    connection.Open();

                    StringBuilder sb = new StringBuilder();
                    sb.Append(" select  id_producto as cajero,  ");
                    sb.Append(" responsable, e.grupo from falla_f_fallas_diaria   f ");
                    sb.Append(" left join atm_d_cajero a on a.id_cajero = f.id_producto ");
                    sb.Append(" left join cat_d_empresa_grupo e on e.id_empresa = a.id_empresa ");
                    sb.Append(" where id_tipo_producto = 2  and e.empresa like '%' +replace('" + grupo + "',' ','_')+' %'");

                    String sql = sb.ToString();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        SqlDataReader myReader = null;
                        myReader = command.ExecuteReader();

                        while (myReader.Read())
                        {
                            Grupo cajeroBean = new Grupo();
                            cajeroBean.cajero = myReader["cajero"].ToString();
                            cajeroBean.responsable = myReader["responsable"].ToString();
                            cajeros.Add(cajeroBean);
                        }

                    }

                }

            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            return cajeros;
        }
    }
}