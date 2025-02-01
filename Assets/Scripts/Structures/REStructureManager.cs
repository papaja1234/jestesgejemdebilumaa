using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class REStructureManager
{
    
    public string StructureDataDirectory { get; private set; }
    
    public List<Structure> Structures { get; private set; }

    private readonly XmlSerializer _serializer = new XmlSerializer(typeof(Structure));
    
    public static REStructureManager Instance { get; private set; }

    public static ContraptionDataSettings Settings => INUserSettings.Instance.ContraptionDataSettings;
    public static void Create()
    {
        REStructureManager re = new ();
        re.Initialize();
        Instance = re;
    }

    private void Initialize()
    {
        
        StructureDataDirectory = INContraptionDataManager.Instance.DataDirectory + "/structures";
        Structures = new List<Structure>();
        if (!Directory.Exists(StructureDataDirectory))
        {
            Directory.CreateDirectory(StructureDataDirectory);//building base for structure block   
        }
        ReloadStructures();
    }

    public bool TryGet(string name, out Structure structure)
    {
        Structure find = Structures.Find((s) => s.Name == name);
        if (find.Equals(null) || find.Equals(default))
        {
            structure = default;
            return false;
        }
        else
        {
            structure = find;
            return true;
        }
    }

    public void ReloadStructures()
    {
        ClearLoadedStructures();
        var files = Directory.EnumerateDirectories(StructureDataDirectory);
        foreach (var s in files)
        {
            try
            {
                Structure st = (Structure)_serializer.Deserialize(File.OpenRead(s));
                Structures.Add(st);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }

    public void SaveStructureBlock(StructureBlock sb)
    {
        using var sw = new StreamWriter(File.Create(StructureDataDirectory + "/" + sb.structName + ".xml"));
        _serializer.Serialize(sw,sb.GetStructure());
        ReloadStructures();
    }

    public void ClearLoadedStructures()
    {
        Structures.Clear();
    }

}
