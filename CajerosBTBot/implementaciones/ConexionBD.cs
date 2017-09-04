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





        public List<Cajero> obtenerHistoricoCajeroEmpresa(string fecha)
        {
            throw new NotImplementedException();
        }

        public List<Cajero> obtenerEstatusCajerosEmpresa(string empresa)
        {
            throw new NotImplementedException();
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