using CajerosBTBot.Bean;
using CajerosBTBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace CajerosBTBot.implementaciones
{
    public class CajeroDaoImpl : IConsultorDB
    {
        public Boolean ObtenerEstatusCajero(string cajero)
        {
           Boolean est = false;
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "serviciobt.database.windows.net";
                builder.UserID = "adminservbt";
                builder.Password = "serv.bt0916";
                builder.InitialCatalog = "serviciobanorte-btdb";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
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

        public List<Cajero> ObtenerFallaCajero(string cajero)
        {
            List<Cajero> cajeros = new List<Cajero>();
            try
            {

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "serviciobt.database.windows.net";
                builder.UserID = "adminservbt";
                builder.Password = "serv.bt0916";
                builder.InitialCatalog = "serviciobanorte-btdb";


                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    connection.Open();
                      
                        StringBuilder sb = new StringBuilder();
                        sb.Append("SELECT d.id_producto as cajero, d.fecha, f.tipo_falla as falla, d.folio, sum(d.conteo) as conteo ");
                        sb.Append("FROM falla_f_fallas_diaria d");
                        sb.Append(" join falla_d_fallas f ");
                        sb.Append(" on f.id_falla = d.id_falla");
                        sb.Append(" where d.id_producto='" + cajero + "'");
                        sb.Append(" and d.id_tipo_producto = 2");
                        sb.Append(" group by d.id_producto, d.fecha, f.tipo_falla, d.folio");
                        // sb.Append("JOIN [SalesLT].[Product] p ");
                        //sb.Append("ON pc.productcategoryid = p.productcategoryid;");
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





        public List<Cajero> obtenerHistoricoCajeroEmpresa(string empresa)
        {
            List<Cajero> cajeros = new List<Cajero>();
            try
            {
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
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "serviciobt.database.windows.net";
                builder.UserID = "adminservbt";
                builder.Password = "serv.bt0916";
                builder.InitialCatalog = "serviciobanorte-btdb";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();

                    StringBuilder cn = new StringBuilder();
                    cn.Append(" select count(*) from falla_f_fallas_diaria f ");
                    cn.Append(" left join atm_d_cajero a on a.id_cajero = f.id_producto ");
                    cn.Append(" left join cat_d_empresa_grupo e on e.id_empresa = a.id_empresa ");
                    cn.Append(" where f.id_tipo_producto = 2");
                    cn.Append(" and e.empresa like = '%" + empresa + "%'");
                    cn.Append(" group by a.id_empresa ");
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

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "serviciobt.database.windows.net";
                builder.UserID = "adminservbt";
                builder.Password = "serv.bt0916";
                builder.InitialCatalog = "serviciobanorte-btdb";


                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    connection.Open();

                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT ai.id_cajero as cajero, e.empresa, s.tipo_falla as tipoFalla, a.id_empresa, f.folio ");
                    sb.Append("FROM falla_f_fallas_diaria f");
                    sb.Append(" left join falla_d_fallas s on s.id_falla=f.id_falla ");
                    sb.Append(" left join atm_d_cajero a on a.id_cajero = f.id_producto");
                    sb.Append(" left join cat_d_empresa_grupo e on e.id_falla = f.id_falla");
                    sb.Append(" where e.empresa like ='%" + empresa + "%'");
                    sb.Append(" and f.id_tipo_producto = 2");
                    sb.Append(" order by empresa asc, tipo_falla desc");
                    // sb.Append("JOIN [SalesLT].[Product] p ");
                    //sb.Append("ON pc.productcategoryid = p.productcategoryid;");
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



        /*   public List<Cajero> obtenerFallaCajerosEmpresa(string empresa)
           {
               List<Cajero> cajeros = new List<Cajero>();
               try
               {

                   SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                   builder.DataSource = "serviciobt.database.windows.net";
                   builder.UserID = "adminservbt";
                   builder.Password = "serv.bt0916";
                   builder.InitialCatalog = "serviciobanorte-btdb";


                   using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                   {
                       Console.WriteLine("\nQuery data example:");
                       Console.WriteLine("=========================================\n");

                       connection.Open();
                       StringBuilder sb = new StringBuilder();
                       sb.Append("SELECT d.id_producto as cajero, d.fecha, f.tipo_falla as falla, d.folio, sum(d.conteo) as conteo ");
                       sb.Append("FROM falla_f_fallas_diaria d");
                       sb.Append(" join falla_d_fallas f ");
                       sb.Append(" on f.id_falla = d.id_falla");
                       sb.Append(" where d.id_producto='" + empresa + "'");
                       sb.Append(" and d.id_tipo_producto = 2");
                       sb.Append(" group by d.id_producto, d.fecha, f.tipo_falla, d.folio");
                       // sb.Append("JOIN [SalesLT].[Product] p ");
                       //sb.Append("ON pc.productcategoryid = p.productcategoryid;");
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
           }*/

        /*public List<Cajero> obtenerHistoricoCajeroEmpresa(string fecha)
        {
            List<Cajero> cajeros = new List<Cajero>();
            try
            {

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "serviciobt.database.windows.net";
                builder.UserID = "adminservbt";
                builder.Password = "serv.bt0916";
                builder.InitialCatalog = "serviciobanorte-btdb";


                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT d.id_producto as cajero, d.fecha, f.tipo_falla as falla, d.folio, sum(d.conteo) as conteo ");
                    sb.Append("FROM falla_f_fallas_diaria d");
                    sb.Append(" join falla_d_fallas f ");
                    sb.Append(" on f.id_falla = d.id_falla");
                    sb.Append(" where d.id_producto='" + fecha + "'");
                    sb.Append(" and d.id_tipo_producto = 2");
                    sb.Append(" group by d.id_producto, d.fecha, f.tipo_falla, d.folio");
                    // sb.Append("JOIN [SalesLT].[Product] p ");
                    //sb.Append("ON pc.productcategoryid = p.productcategoryid;");
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
        }*/

    }
}