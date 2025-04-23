using System;
using System.Data.SqlClient;

namespace SICEM_Blazor.Ordenes.Models;

public class Ordenes_PagoRealizadoItem {
    public string Orden {get;set;}
    public long Cuenta {get;set;}
    public decimal Adeudo_Orden {get;set;}
    public decimal Importe_Pagado {get;set;}
    public DateTime? Fecha_Pago {get; set;}
    public int Dias {get; set;}

    public static Ordenes_PagoRealizadoItem FromDataReader(SqlDataReader reader){
        var item = new Ordenes_PagoRealizadoItem();
        item.Orden = reader["id_orden"].ToString();
        item.Cuenta = long.Parse(reader["cuenta"].ToString());
        item.Adeudo_Orden = Decimal.Parse(reader["adeudo_al_generar_orden"].ToString());
        item.Importe_Pagado = Decimal.Parse(reader["importe_pagado"].ToString());
        item.Fecha_Pago = DateTime.TryParse(reader["fecha_pago"].ToString(), out DateTime n)?n:null;
        item.Dias = int.Parse(reader["dias"].ToString());
        return item;
    }
}