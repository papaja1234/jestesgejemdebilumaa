using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using unit = ContraptionDataset.ContraptionDatasetUnit;

public class StructureBlock : BasePart
{
    public int L { get; private set; } = 3; //Length
    public int H { get; private set; } = 4;//Height--that's how you name like a mathematician!!
    [SerializeField] private LineRenderer lr;
    [SerializeField] private TextMeshPro tm;
    [SerializeField] private Button sb, lb, openMenu;
    public string structName = "_clipboard";

    public override void Awake()
    {
        base.Awake();
        structName = "_clipboard";
        SetLineRenderer();
        SetButtons();
    }
 

    private void SetLineRenderer()
    {
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
        lr.startWidth = 0.05f;
        lr.endWidth = lr.startWidth;
        lr.numCapVertices = 0;
        lr.numCornerVertices = 0;
        lr.loop = true;
        lr.positionCount = 4;
    }

    private void SetButtons()
    {
        sb.MethodToCall.SetMethod(this, nameof(SaveStructure));
        lb.MethodToCall.SetMethod(this,nameof(LoadStructure), structName);
        openMenu.MethodToCall.SetMethod(this, nameof(OpenMenu));
    }

    private void SaveStructure()
    {
        Debug.Log("Saving structure " + structName);
        REStructureManager.Instance.SaveStructureBlock(this);
    }

    private void OpenMenu()
    {
        if(INSettings.VersionType == 0) return;
        INAppInterface.Instance.OpenStructurePage(this);
    }
    public void SetSize(int length, int height)
    {
        L = length;
        H = height;
    }

    
    public void Update()
    {//
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
        Debug.Log("Loading structure " + structName);
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