using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public bool oda1Temizlendi;
    // İzlenen ara sahnelerin ID'lerini burada tutacağız
    public List<string> izlenenAraSahneler = new List<string>();
}