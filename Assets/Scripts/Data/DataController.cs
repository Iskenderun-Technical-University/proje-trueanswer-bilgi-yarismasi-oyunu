using UnityEngine;
using System.IO;
using System.Xml.Serialization;

[System.Serializable()]
public class Data
{
    public Soru[] Sorular = new Soru[0];

    public Data() { }

    public static void Write(Data data, string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Data));
        using (Stream stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, data);
        }
    }
    public static Data Fetch(string filePath)
    {
        return Fetch(out bool result, filePath);
    }
    public static Data Fetch(out bool result, string filePath)
    {
        if (!Resources.Load(filePath)) { result = false; return new Data(); }

        TextAsset _xml = Resources.Load<TextAsset>(filePath);
        XmlSerializer serializer = new XmlSerializer(typeof(Data));
        using (StringReader reader = new StringReader(_xml.ToString()))
        {
            var data = (Data)serializer.Deserialize(reader);// as Data;
            reader.Close(); // gerekli olmayabilir
            result = true;
            return data;
        }
    }


    public static void GorulenSoruKayitlariOlustur()
    {
        for (int i = 0; i < 100; i++)
        {
            var path = Path.Combine(GameRecords.FileDir, GameRecords.FileName + (i + 1));
            Data data1 = new Data();
            data1 = Data.Fetch(path);

            for (int j = 0; j < data1.Sorular.Length; j++)
            {
                PlayerPrefs.SetInt(i.ToString() + j.ToString(), 0);
            }
            //gorulenSorularList2d.Add(new List<int>());
            //PlayerPrefs.SetInt(GameUtility.SaveGorulenSorular, gorulenSorularList2d[i]);
        }
    }
}
