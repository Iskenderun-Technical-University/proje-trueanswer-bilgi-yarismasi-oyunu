using System;

public enum CevapTipi { Metin, Resim, Ses, Video }

[Serializable()]
public class Cevap
{
    public string Cevap_Text = string.Empty;
    public bool Dogru = false;

    public Cevap() { }
}
[Serializable()]
public class Soru
{
    public String Soru_Text = null;
    public Cevap[] Cevaplar = null;
    public Boolean SureKullan = true;
    public Int32 Sure = 10;
    public CevapTipi Tip = CevapTipi.Metin;
    public Int32 SkorEkle = 10;

    public Soru() { }

    /// <summary>
    /// Function that is called to collect and return correct answer indexes.
    /// </summary>
    public int GetDogruCevapIndex()
    {
        int DogruCevapIn = -1;
        for (int i = 0; i < Cevaplar.Length; i++)
        {
            if (Cevaplar[i].Dogru)
                DogruCevapIn = i;
        }
        return DogruCevapIn;
    }
}