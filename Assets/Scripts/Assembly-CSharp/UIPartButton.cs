using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPartButton : MonoBehaviour
{
	protected RawImage m_partTexture;

	protected Text m_indexText;

	protected UIPartButtonList m_buttonList;

	protected UIPartButtonInfo m_info;

	protected SortedPartType m_sortedPartType;

	protected List<BasePart> m_parts;

	protected Vector2 m_averagePosition;

	protected Color m_disabledColor;

	protected Color m_enabledColor;

	public UIPartButtonInfo Info => m_info;

	public List<BasePart> Parts => m_parts;

	public SortedPartType SortedPartType => m_sortedPartType;

	public Vector2 AveragePosition => m_averagePosition;

	public virtual int SubButtonCount => 0;

	public virtual IEnumerable<UIPartButton> SubButtons { get; }

	public void setMaxComponents(int maxComponents)
	{
		m_info.maxComponents = maxComponents;
	}

	protected virtual void Awake()
	{
		m_buttonList = UIPartButtonList.Instance;
		m_partTexture = base.transform.Find("PartTexture").GetComponent<RawImage>();
		m_indexText = base.transform.Find("Index").GetComponent<Text>();
		m_parts = new List<BasePart>();
	}

	public void SetInfo(UIPartButtonInfo info)
	{
		m_info = info;
		m_sortedPartType = info.PartType.ToSortedPartType();
	}

	public void SetSprite(bool enabled, Texture texture, Rect uvRect, Vector2 scale, Quaternion rotation)
	{
		RectTransform obj = (RectTransform)m_partTexture.transform;
		obj.sizeDelta = scale;
		obj.rotation = rotation;
		m_partTexture.enabled = enabled;
		m_partTexture.texture = texture;
		m_partTexture.uvRect = uvRect;
	}

	public void DisplayIndexText(string text)
	{
		m_indexText.text = text;
	}

	public virtual void Initialize()
	{
		float num = (float)m_info.ComponentRank / (m_info.maxComponents == 0 ? 1 : m_info.maxComponents);
		// float num3 = 2f - Math.Abs(num2 - 240f) / 60f;
		// float num4 = 0.8f * (1.5f / num3);
		m_disabledColor = Color.HSVToRGB(num, 1f, 0.7f);
		m_disabledColor.a = 1f;
		m_enabledColor = Color.HSVToRGB(num, 1f, 1f);
		m_enabledColor.a = 1f;
		CalculateAveragePosition();
	}

	private void CalculateAveragePosition()
	{
		int count = m_parts.Count;
		if (count == 0)
		{
			m_averagePosition = Vector2.zero;
			return;
		}
		int num = 0;
		int num2 = 0;
		foreach (BasePart part in m_parts)
		{
			num += part.CoordX;
			num2 += part.CoordY;
		}
		m_averagePosition = new Vector2((float)num / (float)count, (float)num2 / (float)count);
	}

	public virtual void Reset()
	{
		m_info = default(UIPartButtonInfo);
		m_sortedPartType = SortedPartType.Unknown;
		m_averagePosition = Vector2.zero;
		m_parts.Clear();
		m_partTexture.texture = null;
		m_partTexture.uvRect = Rect.zero;
		m_partTexture.enabled = false;
	}
}
