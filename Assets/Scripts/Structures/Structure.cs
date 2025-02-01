using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using unit = ContraptionDataset.ContraptionDatasetUnit;
public struct Structure
{
    [XmlElement] public int Length, Height;
    
    [XmlElement] public string Name;

    [XmlElement] public DateTime LastModified;
    
    [XmlArray("ContraptionDatasetList")]
    [XmlArrayItem("ContraptionDatasetUnit")]
    public List<unit> Units { get; set; }
/*
    private Dictionary<(int, int), unit> _dictionary;
    public void GenerateLut()
    {
        
    }*/

    public void RecalibratePosition()
    {
        if (Units.Count==0)
        {
            Length = 0;
            Height = 0;
            return;
        }
        R<unit> r;
        int x = Units[0].x;
        int y = Units[0].y;
        int xm = Units[0].x;
        int ym = Units[0].y;
        foreach (var u in Units)
        {
            x = math.min(x, u.x);
            y = math.min(y, u.y);
            xm = math.max(x, u.x);
            ym = math.max(y, u.y);
        }
        r = (ref unit u) => 
        {
            u.x -= x;
            u.y -= y;
        };
        for (int i = 0; i < Units.Count; i++)
        {
            unit c = Units[i];
            r(ref c);
            Units[i] = c;
        }

        Length = xm - x + 1;
        Height = ym - y + 1;
    }

    public void AddPart(int x, int y, int partType, int customPartIndex, BasePart.GridRotation rotation, bool flipped, float offsetx , float offsety )
    {
        unit contraptionDatasetUnit = new unit
        {
            x = x,
            y = y,
            partType = partType,
            customPartIndex = customPartIndex,
            rot = (int)rotation,
            flipped = flipped,
            offsetX = offsetx,
            offsetY = offsety
        };
        Units.Add(contraptionDatasetUnit);
    }
    public void AddPart(int x, int y, int partType, int customPartIndex, int rotation, bool flipped, out unit unit, float offsetx, float offsety)
    {
        unit contraptionDatasetUnit = new unit
        {
            x = x,
            y = y,
            partType = partType,
            customPartIndex = customPartIndex,
            rot = rotation,
            flipped = flipped,
            offsetX = offsetx,
            offsetY = offsety
        };
        Units.Add(contraptionDatasetUnit);
        unit = contraptionDatasetUnit;
    }
}
