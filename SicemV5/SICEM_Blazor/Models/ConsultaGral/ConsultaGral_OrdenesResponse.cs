using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;

namespace SICEM_Blazor.Models{
    public class ConsultaGral_Ordenesresponse{
        public string  Id_Orden {get;set;}
        public DateTime? Fecha_Genero {get;set;}
        public bool Ejecutada {get;set;}
        public int Tipo_Orden {get;set;} //Rezago o Micromedicion
        public string Tipo_OrdenDesc {get;set;} //Rezago o Micromedicion


        /* Datos de la Orden */
        public string Estatus {get;set;}
        public string Trabajo {get;set;}
        public string Genero {get;set;}
        public string Imprimio {get;set;}
        public DateTime? Fecha_Imprimio {get;set;}
        public string Observaciones_Orden {get;set;}

        
        /* Resultados de la Orden */
        public DateTime? Fecha_Ini {get;set;}
        public DateTime? Fecha_Fin {get;set;}
        public string Realizo {get;set;} = "";
        public string Capturo {get;set;} = "";
        public DateTime? Fecha_Capturo {get;set;}
        public int Duracion {get;set;}
        // public string Ord_Ejecutada {get;set;}
        public string Respuesta_Orden {get;set;}
        

        /* Datos Medidor */
        public string Medidor_Reg{get;set;}
        public string Marca_Reg{get;set;}
        public string Modelo_Reg{get;set;}
        public string Diametro_Reg{get;set;}
        
        public string Medidor_Enc{get;set;}
        public string Marca_Enc{get;set;}
        public string Modelo_Enc{get;set;}
        public string Diametro_Enc{get;set;}

        public string Medidor_Ret{get;set;}
        public string Marca_Ret{get;set;}
        public string Modelo_Ret{get;set;}
        public string Diametro_Ret{get;set;}

        public string Medidor_Inst{get;set;}
        public string Marca_Inst{get;set;}
        public string Modelo_Inst{get;set;}
        public string Diametro_Inst{get;set;}

        public string Anomalia_Reg{get;set;}
        public int Lectura_Reg{get;set;}

        public string Anomalia_Act{get;set;}
        public int Lectura_Act{get;set;}
        public bool ErrorLectura{get;set;}


        /* Datos Inspeccion */
        public double Lecutar_Ini{get;set;} = 0;
        public double Lecutar_Fin{get;set;} = 0;
        public int Consumo_Estimado{get;set;} = 0;
        public double PruebaMed_A{get;set;} = 0;
        public double PruebaMed_B{get;set;} = 0;
        public int Sanit_FV{get;set;} = 0;
        public int Sanit_FS{get;set;} = 0;
        public int Sanit_HNA{get;set;} = 0;
        public int Sanit_Nor{get;set;} = 0;
        public int Llav_Coc{get;set;} = 0;
        public int Llav_Arc{get;set;} = 0;
        public int Llav_Ban{get;set;} = 0;
        public int Llav_Tom{get;set;} = 0;
        public int Llav_Tinacos{get;set;} = 0;
        public int Llav_Jard{get;set;} = 0;
        public int Llav_Lavado{get;set;} = 0;
        public int Llav_Pilas{get;set;} = 0;
        public int Llav_Cisterna{get;set;} = 0;
        public int Llav_Otros{get;set;} = 0;

        public int Person_Adultos{get;set;} = 0;
        public int Person_Menore{get;set;} = 0;
        public int Promedio{get;set;} = 0;
        public string Tarifa {get;set;} = "";


        public static ConsultaGral_Ordenesresponse FromSqlDataReader( SqlDataReader sqlReader){
            var tmpItem = new ConsultaGral_Ordenesresponse();

            tmpItem.Id_Orden = sqlReader.GetValue("id_orden").ToString();
            try {
                tmpItem.Fecha_Genero = DateTime.Parse(sqlReader.GetValue("fecha_genero").ToString());
            }
            catch(Exception) {
                tmpItem.Fecha_Genero = null;
            }
            tmpItem.Ejecutada = (int.Parse(sqlReader.GetValue("id_estatus").ToString()) == 9) ? true : false;
            tmpItem.Trabajo = sqlReader.GetValue("_trabajo").ToString();
            tmpItem.Tipo_Orden = int.Parse(sqlReader.GetValue("id_tipoorden").ToString());
            tmpItem.Tipo_OrdenDesc = sqlReader.GetValue("_tipoorden").ToString();


            //Datos de la Orden
            tmpItem.Estatus = sqlReader.GetValue("_estatus").ToString();
            tmpItem.Trabajo = sqlReader.GetValue("_trabajo").ToString();
            tmpItem.Genero = sqlReader.GetValue("_genero").ToString();
            tmpItem.Imprimio = sqlReader.GetValue("_imprimio").ToString();
            try {
                tmpItem.Fecha_Imprimio = DateTime.Parse(sqlReader.GetValue("_fecha_imprimio").ToString());
            }
            catch(Exception) {
                tmpItem.Fecha_Imprimio = null;
            }
            tmpItem.Observaciones_Orden = sqlReader.GetValue("observa_a").ToString();


            //Resultados de la Orden
            try {
                tmpItem.Fecha_Ini = DateTime.Parse(sqlReader.GetValue("fecha_ini").ToString());
            }
            catch(Exception) {
                tmpItem.Fecha_Ini = null;
            }
            try {
                tmpItem.Fecha_Fin = DateTime.Parse(sqlReader.GetValue("fecha_fin").ToString());
            }
            catch(Exception) {
                tmpItem.Fecha_Fin = null;
            }
            tmpItem.Realizo = sqlReader.GetValue("_realizo").ToString();
            tmpItem.Capturo = sqlReader.GetValue("_capturo").ToString();
            try {
                tmpItem.Fecha_Capturo = DateTime.Parse(sqlReader.GetValue("_fecha_capturo").ToString());
            }
            catch(Exception) {
                tmpItem.Fecha_Capturo = null;
            }

            tmpItem.Duracion = int.Parse(sqlReader.GetValue("duracion").ToString());
            tmpItem.Respuesta_Orden = sqlReader.GetValue("observa_b").ToString();


            // Datos Medidor 
            tmpItem.Medidor_Reg = sqlReader.GetValue("medidor_reg").ToString();
            tmpItem.Marca_Reg = sqlReader.GetValue("_marca_reg").ToString();
            tmpItem.Modelo_Reg = sqlReader.GetValue("_modelo_reg").ToString();
            tmpItem.Diametro_Reg = sqlReader.GetValue("_diametro_reg").ToString();

            tmpItem.Medidor_Enc = sqlReader.GetValue("medidor_enc").ToString();
            tmpItem.Marca_Enc = sqlReader.GetValue("_marca_enc").ToString();
            tmpItem.Modelo_Enc = sqlReader.GetValue("_modelo_enc").ToString();
            tmpItem.Diametro_Enc = sqlReader.GetValue("_diametro_enc").ToString();

            tmpItem.Medidor_Ret = sqlReader.GetValue("medidor_ret").ToString();
            tmpItem.Marca_Ret = sqlReader.GetValue("_marca_ret").ToString();
            tmpItem.Modelo_Ret = sqlReader.GetValue("_modelo_ret").ToString();
            tmpItem.Diametro_Ret = sqlReader.GetValue("_diametro_ret").ToString();

            tmpItem.Medidor_Inst = sqlReader.GetValue("medidor_ins").ToString();
            tmpItem.Marca_Inst = sqlReader.GetValue("_marca_ins").ToString();
            tmpItem.Modelo_Inst = sqlReader.GetValue("_modelo_ins").ToString();
            tmpItem.Diametro_Inst = sqlReader.GetValue("_diametro_ins").ToString();

            tmpItem.Anomalia_Reg = sqlReader.GetValue("_anomalia_reg").ToString();
            tmpItem.Lectura_Reg = int.Parse(sqlReader.GetValue("lectura_reg").ToString());

            tmpItem.Anomalia_Act = sqlReader.GetValue("_anomalia_enc").ToString();
            tmpItem.Lectura_Act = int.Parse(sqlReader.GetValue("lectura_enc").ToString());
            tmpItem.ErrorLectura = false;


            // Datos Inspeccion
            tmpItem.Lecutar_Ini = double.Parse(sqlReader.GetValue("lectura_ini").ToString());
            tmpItem.Lecutar_Fin = double.Parse(sqlReader.GetValue("lectura_fin").ToString());
            tmpItem.Consumo_Estimado = int.Parse(sqlReader.GetValue("consumo_estimado").ToString());
            tmpItem.PruebaMed_A = double.Parse(sqlReader.GetValue("prueba_med1").ToString());
            tmpItem.PruebaMed_B = double.Parse(sqlReader.GetValue("prueba_med2").ToString());
            tmpItem.Sanit_FV = int.Parse(sqlReader.GetValue("fuga_vertedero").ToString());
            tmpItem.Sanit_FS = int.Parse(sqlReader.GetValue("fuga_sapito").ToString());
            tmpItem.Sanit_HNA = int.Parse(sqlReader.GetValue("huella_nivelalto").ToString());
            tmpItem.Sanit_Nor = int.Parse(sqlReader.GetValue("normales").ToString());
            tmpItem.Llav_Coc = int.Parse(sqlReader.GetValue("cocina").ToString());
            tmpItem.Llav_Arc = int.Parse(sqlReader.GetValue("arco").ToString());
            tmpItem.Llav_Ban = int.Parse(sqlReader.GetValue("banos").ToString());
            tmpItem.Llav_Tom = int.Parse(sqlReader.GetValue("tomas").ToString());
            tmpItem.Llav_Tinacos = int.Parse(sqlReader.GetValue("tinacos").ToString());
            tmpItem.Llav_Jard = int.Parse(sqlReader.GetValue("jardin").ToString());
            tmpItem.Llav_Lavado = int.Parse(sqlReader.GetValue("lavado").ToString());
            tmpItem.Llav_Pilas = int.Parse(sqlReader.GetValue("pilas").ToString());
            tmpItem.Llav_Cisterna = int.Parse(sqlReader.GetValue("cisternas").ToString());
            tmpItem.Llav_Otros = int.Parse(sqlReader.GetValue("otros").ToString());

            tmpItem.Person_Adultos = int.Parse(sqlReader.GetValue("adultos").ToString());
            tmpItem.Person_Menore = int.Parse(sqlReader.GetValue("menores").ToString());
            tmpItem.Promedio = int.Parse(sqlReader.GetValue("promedio_sugerido").ToString());
            tmpItem.Tarifa = sqlReader.GetValue("_tarifa").ToString();

            return tmpItem;
        }


    }

}