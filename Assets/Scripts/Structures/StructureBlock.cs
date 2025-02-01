using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using unit = ContraptionDataset.ContraptionDatasetUnit;

public class StructureBlock : BasePart
{
    public int L { get; private set; } //Length
    public int H { get; private set; } //Height--that's how you name like a mathematician!!
    [SerializeField] private LineRenderer lr;
    [SerializeField] private TextMesh tm;
    [SerializeField] private Button sb, lb;
    public string structName;

    public override void Awake()
    {
        base.Awake();
        if (!lr)
        {
            lr = gameObject.AddComponent<LineRenderer>();
        }
        else
        {
            lr = GetComponent<LineRenderer>();
        }
        lr.startColor = Color.white;
        lr.endColor = Color.white;
        lr.useWorldSpace = true;
        lr.startWidth = 0.1f;
        lr.endWidth = lr.startWidth;
        lr.material = new Material(INUnity.ColorShader);
        lr.numCapVertices = 8;
        lr.numCornerVertices = 8;
        lr.loop = true;
        lr.positionCount = 4;
    }

    public void SetSize(int length, int height)
    {
        L = length;
        H = height;
    }

    
    public void FixedUpdate()
    {
        Vector3 o = new Vector3(0.5f, 0.5f, -0.1f) + transform.position;
        Vector3 l = new Vector3(L, 0, 0);
        Vector3 h = new Vector3(0, H, 0);
        lr.SetPosition(0,o);
        lr.SetPosition(1,o+l);
        lr.SetPosition(2,o+l+h);
        lr.SetPosition(3,o+h);
        tm.text = structName;
    }

    public void LoadStructure(string s)
    {
        structName = s;
        bool su = REStructureManager.Instance.TryGet(s, out Structure st);
        if (su)
        {
            SetSize(st.Length,st.Height);
            for (int i = 1; i <= L; i++)//remove parts
            {
                for (int j = 1; j <= H; j++)
                { 
                    Contraption.Instance.RemovePartsAt(this.CoordX + i, this.CoordY + j);
                }
            }

            foreach (var u in st.Units)
            {
                Contraption.Instance.DataSet.AddPart(u,this.CoordX,this.CoordY);
            }
        }
        else
        {
            //insert fail message
            Debug.Log("Not found");
            return;
        }
    }

    public Structure GetStructure()
    {
        List<unit> units = new List<unit>();
        for (int i = 1; i <= L; i++)//scan parts
        {
            for (int j = 1; j <= H; j++)
            {
                for (int k = 0; k < 2; k++)//2 layers, i suppose
                {
                    bool ok = Contraption.Instance.TryGet(this.CoordX + i, this.CoordY + j, k, out BasePart p);
                    if (ok)
                    {
                        units.Add(new unit(p.CoordX, p.CoordY, (int)p.m_partType, p.customPartIndex, (int)p.Rotation,
                            p.Flipped, p.offsetX, p.offsetY));
                    }
                }
            }
        }

        Structure structure = new Structure() { LastModified = DateTime.Now, Name = structName, Units = units };
        structure.RecalibratePosition();
        return structure;
    }
}