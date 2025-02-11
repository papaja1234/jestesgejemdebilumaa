using UnityEngine;

public class Frame : BasePart
{
	public enum FrameMaterial
	{
		Wood = 0,
		Metal = 1
	}

	public FrameMaterial m_material;

	public Texture2D[] m_brokenTextures;

	private bool m_colored;

	private MeshRenderer[] m_renderers;

	public override bool CanEncloseParts()
	{
		return true;
	}

	public override bool IsPartOfChassis()
	{
		return true;
	}

	public override void Initialize()
	{
		if (!(bool)m_enclosedPart || m_enclosedPart is Rope or HingePlate) return;
		FixedJoint fixedJoint = m_enclosedPart.gameObject.AddComponent<FixedJoint>();
		fixedJoint.connectedBody = base.rigidbody;
		float breakForce = base.contraption.GetJointConnectionStrength(GetJointConnectionStrength()) + base.contraption.GetJointConnectionStrength(m_enclosedPart.GetJointConnectionStrength());
		fixedJoint.breakForce = breakForce;
		fixedJoint.enablePreprocessing = false;
		base.contraption.AddJointToMap(this, m_enclosedPart, fixedJoint);
		IgnoreCollisionRecursive(base.collider, m_enclosedPart.gameObject);
	}

	private static void IgnoreCollisionRecursive(Collider collider, GameObject part)
	{
		if (part.activeInHierarchy && (bool)part.GetComponent<Collider>())
		{
			Physics.IgnoreCollision(collider, part.GetComponent<Collider>());
		}
		for (int i = 0; i < part.transform.childCount; i++)
		{
			IgnoreCollisionRecursive(collider, part.transform.GetChild(i).gameObject);
		}
	}

	public override void OnBreak()
	{
	}

	private new void Awake()
	{
		base.Awake();
		m_renderers = GetComponentsInChildren<MeshRenderer>();
	}

	private void FixedUpdate()
	{
		if (base.contraption == null || !INSettings.GetBool(INFeature.ColoredFrame) || ((!INSettings.GetBool(INFeature.CanColorSpecialFrames) || (!this.IsAlienMetalFrame() && !this.IsLightFrame())) && !INSettings.GetBool(INFeature.CanColorAllFrames)))
		{
			return;
		}
		int dx = 1;
		int dy = 0;
		Color clear = Color.clear;
		float alpha = 0f;
		for (int i = 0; i < 4; i++)
		{
			BasePart neighbour = base.contraption.FindPartAt(m_coordX + dx, m_coordY + dy, this);
			if (neighbour != null && neighbour.IsColoredrame())
			{
				ColoredFrame coloredFrame = neighbour as ColoredFrame;
				clear += coloredFrame.Color * coloredFrame.Color.a;
				alpha += coloredFrame.Color.a;
			}
			int temp = dx;
			dx = -dy;
			dy = temp;
		}
		if (alpha > 0f)
		{
			clear /= alpha;
			MeshRenderer[] renderers = m_renderers;
			foreach (MeshRenderer meshRenderer in renderers)
			{
				if (!m_colored)
				{
					meshRenderer.material.shader = INUnity.LoadShader("Unlit_ColorTransparent_GrayOverlay");//fix not coloring neighbour frames
				}
				meshRenderer.material.color = clear;
			}
			m_colored = true;
		}
		else if (m_colored)
		{
			MeshRenderer[] renderers = m_renderers;
			foreach (MeshRenderer obj in renderers)
			{
				Material material = obj.material;
				material.shader = INUnity.CustomTransparentShader;
				material.color = Color.white;
			}
			m_colored = false;
		}
	}
}
