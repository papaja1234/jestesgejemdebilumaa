using System;
using System.Collections.Generic;

public class PowerTransmitterPart : ElectricalPart
{
	public enum PowerTransmitterType
	{
		Sender = 0,
		Receiver = 1
	}

	private ElectricalElement m_element;

	private PowerTransmitterPart m_connectedPart;

	public bool IsSender => TransmitterType == PowerTransmitterType.Sender;

	public bool IsReceiver => TransmitterType == PowerTransmitterType.Receiver;

	public PowerTransmitterType TransmitterType => (PowerTransmitterType)(customPartIndex - 40);

	public override IEnumerable<ElectricalElement> ElectricalElements => m_element.ToEnumerable();

	public override void Awake()
	{
		base.Awake();
	}

	public override void CreateElectricalElements()
	{
		m_element = TransmitterType switch
		{
			PowerTransmitterType.Sender => new Wire(),
			PowerTransmitterType.Receiver => new Resistor(0f),
			_ => m_element
		};
	}

	protected override BitDirection GetConnectionDirection()
	{
		return TransmitterType switch
		{
			PowerTransmitterType.Sender => BitDirection.Down.Rotate((int)m_gridRotation), 
			PowerTransmitterType.Receiver => BitDirection.Up.Rotate((int)m_gridRotation), 
			_ => BitDirection.None, 
		};
	}

	public void Connect(PowerTransmitterPart other, float distance)
	{
		if (m_connectedPart != other)
		{
			if (m_connectedPart != null)
			{
				CircuitFactory.Disconnect(m_element, m_connectedPart.m_element);
			}
			if (other != null && IsOnSameChannel(other))
			{
				CircuitFactory.Connect(m_element, other.m_element);
			}
			m_connectedPart = other;
		}
		Resistor resistor = (Resistor)m_element;
		if (other == null)
		{
			resistor.Resistance = 0f;
			return;
		}
		distance = Math.Max(distance - 1f, 0f);
		resistor.Resistance = 0.2f * distance * distance + 0.1f;
	}

	private bool IsOnSameChannel(PowerTransmitterPart other)
	{
		if (!GameRules.ChanneledRadio)return true;
		return this.enclosedInto switch
		{
			null when other.enclosedInto is null => true,
			not null when other.enclosedInto is null => false,
			null when other.enclosedInto is not null => false,
			not null when this.enclosedInto.m_partType is PartType.WoodenFrame || other.enclosedInto.m_partType is PartType.WoodenFrame => true,
			ColoredFrame coloredFrame when other.enclosedInto is ColoredFrame otherEnclosedInto => coloredFrame
				.customPartIndex == otherEnclosedInto.customPartIndex,
			_ => false
		};
	}
}
