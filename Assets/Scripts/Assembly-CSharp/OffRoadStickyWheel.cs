using System;
using System.Collections.Generic;
using UnityEngine;

public class OffRoadStickyWheel : BasePart
{
	public float m_force;

	public bool m_enabled;

	public float m_maximumSpeed;

	public float m_stickiness = 200f;

	private float m_maximumForce;

	private Vector3 m_lastPosition = Vector3.zero;

	private Vector3 m_lastContactDirection = Vector3.zero;

	private float m_radius;

	private float m_circumference;

	private Transform m_wheelPivot;

	private Transform m_fakeWheelPivot;

	private float m_spinSpeed;

	private Vector3 m_lastForceDirection;

	private float m_angle;

	private Collider m_supportCollider;

	private bool m_hasContact = true;

	private float m_thrust;

	private float m_thrustTimer;

	private Rigidbody colliderRigidbody;

	private AudioSource loopingWheelSound;

	private bool m_grounded;

	private Vector3 m_fallOffForce = Vector3.zero;

	private int m_nonCollideLayerMask;

	private int m_contraptionLayer;

	public float m_drag = 0.1f;

	private float m_spiderTimer;

	private bool m_spiderReported;

	public bool hasHit;

	private bool m_connected;

	public bool HasContact => m_hasContact;

	private List<BasePart> m_ignoredCollisionParts;

	public float m_springStiffness = 150f;

	public override bool CanBeEnabled()
	{
		return m_maximumSpeed > 0f;
	}

	public override bool HasOnOffToggle()
	{
		return true;
	}

	public override bool IsEnabled()
	{
		return m_enabled;
	}

	private void Start()
	{
		m_wheelPivot = base.transform.Find("WheelPivot");
		m_fakeWheelPivot = base.transform.Find("FakeWheelPivot");
		m_radius = GetComponent<SphereCollider>().radius;
		m_circumference = MathF.PI * 2f * m_radius;
		if ((bool)base.transform.Find("SupportCollider"))
		{
			m_supportCollider = base.transform.Find("SupportCollider").GetComponent<Collider>();
		}
		m_nonCollideLayerMask = 1 << LayerMask.NameToLayer("NonCollidingPart");
		m_nonCollideLayerMask |= 1 << LayerMask.NameToLayer("Light");
		m_nonCollideLayerMask |= 1 << LayerMask.NameToLayer("IcePart");
		BasePart.m_groundLayer = LayerMask.NameToLayer("Ground");
		m_contraptionLayer = LayerMask.NameToLayer("Contraption");
		CheckIsConnected();
	}

	public override void PostInitialize()
	{
		if (INSettings.GetBool(INFeature.NonCollisionOffRoadWheel))
		{
			foreach (BasePart part in base.contraption.Parts)
			{
				IgnoreCollisionRecursive(base.collider, part.gameObject);
			}
			return;
		}
		int num = -(int)MathF.Ceiling(m_radius + 0.1f);
		int num2 = (int)MathF.Ceiling(m_radius + 0.1f);
		int num3 = -(int)MathF.Ceiling(m_radius + 0.1f);
		int num4 = (int)MathF.Ceiling(m_radius + 0.1f) - 1;
		int num5 = num;
		int num6 = num2;
		int num7 = num3;
		int num8 = num4;
		switch (m_gridRotation)
		{
		case GridRotation.Deg_90:
			num5 = -num4;
			num6 = -num3;
			num7 = num;
			num8 = num2;
			break;
		case GridRotation.Deg_180:
			num5 = -num2;
			num6 = -num;
			num7 = -num4;
			num8 = -num3;
			break;
		case GridRotation.Deg_270:
			num5 = num3;
			num6 = num4;
			num7 = -num2;
			num8 = -num;
			break;
		}
		m_ignoredCollisionParts = new List<BasePart>();
		for (int i = num5; i <= num6; i++)
		{
			for (int j = num7; j <= num8; j++)
			{
				BasePart basePart = base.contraption.FindPartAt(m_coordX + i, m_coordY + j, this);
				if (basePart != null && basePart.ConnectedComponent == base.ConnectedComponent)
				{
					BasePart basePart2 = basePart.enclosedPart;
					IgnoreCollisionRecursive(base.collider, basePart.gameObject);
					m_ignoredCollisionParts.Add(basePart);
					if (basePart2 != null)
					{
						IgnoreCollisionRecursive(base.collider, basePart2.gameObject);
						m_ignoredCollisionParts.Add(basePart2);
					}
				}
			}
		}
	}

	private void IgnoreCollisionRecursive(Collider collider, GameObject part)
	{
		if ((bool)part.GetComponent<Collider>() && part.GetComponent<Collider>() != collider)
		{
			Physics.IgnoreCollision(collider, part.GetComponent<Collider>());
			for (int i = 0; i < part.transform.childCount; i++)
			{
				IgnoreCollisionRecursive(collider, part.transform.GetChild(i).gameObject);
			}
		}
	}

	private void CheckIsConnected()
	{
		List<BasePart> parts = base.contraption.Parts;
		m_connected = false;
		for (int i = 0; i < parts.Count; i++)
		{
			BasePart basePart = parts[i];
			if (!Equals(basePart) && base.contraption.IsConnectedTo(this, basePart))
			{
				m_connected = true;
				break;
			}
		}
	}

	private void SpawnSound()
	{
		loopingWheelSound = Singleton<AudioManager>.Instance.SpawnCombinedLoopingEffect(WPFMonoBehaviour.gameData.commonAudioCollection.stickyWheelLoop, base.transform).GetComponent<AudioSource>();
		loopingWheelSound.volume = 0f;
	}

	public override void InitializeEngine()
	{
		float enginePowerFactor = base.contraption.GetEnginePowerFactor(this);
		m_maximumForce = m_force * enginePowerFactor;
		m_maximumSpeed = 15f * enginePowerFactor;
	}

	private void ReinitializeEngine()
	{
		float enginePowerFactor = base.contraption.GetEnginePowerFactor(this);
		m_maximumForce = m_force * enginePowerFactor;
	}

	public override void OnCollisionStay(Collision collisionInfo)
	{
		base.OnCollisionStay(collisionInfo);
		for (int i = 0; i < collisionInfo.contacts.Length; i++)
		{
			if (collisionInfo.contacts[i].otherCollider != m_supportCollider)
			{
				m_lastContactDirection = (collisionInfo.contacts[i].point - m_lastPosition).normalized;
				break;
			}
		}
	}

	public override void OnLightEnter(EntityLightCollision collision)
	{
		Vector2 vector = m_lastPosition;
		m_lastContactDirection = (collision.ContactPoint - vector).normalized;
	}

	private void Update()
	{
		if (!base.contraption || !base.contraption.IsRunning)
		{
			return;
		}
		if (m_spinSpeed == 0f)
		{
			m_spinSpeed = SpeedInDirection(m_lastForceDirection);
		}
		float z = base.transform.rotation.eulerAngles.z;
		m_angle += -360f * m_spinSpeed / m_circumference * Time.deltaTime;
		if ((bool)m_wheelPivot)
		{
			m_wheelPivot.transform.localRotation = Quaternion.AngleAxis(0f - z + m_angle, Vector3.forward);
		}
		if ((bool)m_fakeWheelPivot)
		{
			float angle = 0f - z + Mathf.Sin(2f * m_angle * (MathF.PI / 180f)) * 8f;
			m_fakeWheelPivot.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
		if (!m_spiderReported && Singleton<SocialGameManager>.IsInstantiated() && base.transform.rotation.eulerAngles.z > 100f && base.transform.rotation.eulerAngles.z < 260f && m_hasContact)
		{
			m_spiderTimer += Time.deltaTime;
			if (m_spiderTimer > 3f)
			{
				Singleton<SocialGameManager>.Instance.ReportAchievementProgress("grp.SPIDER_PIG", 100.0);
				m_spiderReported = true;
			}
		}
		else
		{
			m_spiderTimer = 0f;
		}
		if (m_connected)
		{
			CheckIsConnected();
		}
		UpdateSoundEffect();
	}

	protected override void UpdateSoundEffect()
	{
		float num = 1f;
		if (Mathf.Abs(m_spinSpeed) > num && m_grounded)
		{
			if (!loopingWheelSound)
			{
				SpawnSound();
			}
			float num2 = 1f;
			float num3 = 0.1f;
			float num4 = SpeedInDirection(m_lastForceDirection);
			float value = (num2 - num3) / m_maximumSpeed * num4;
			value = Mathf.Clamp(value, num3, num2);
			loopingWheelSound.pitch = value;
			if (m_hasContact)
			{
				loopingWheelSound.volume = 0.15f * (Mathf.Abs(m_spinSpeed) / num - 1f);
			}
			else
			{
				loopingWheelSound.volume = 0f;
			}
		}
		else if ((bool)loopingWheelSound)
		{
			loopingWheelSound.volume = 0f;
		}
	}

	private bool CanCollide(int otherLayer)
	{
		return (m_nonCollideLayerMask & (1 << otherLayer)) == 0;
	}

	public override Joint CustomConnectToPart(BasePart part)
	{
		ConfigurableJoint configurableJoint = base.gameObject.AddComponent<ConfigurableJoint>();
		configurableJoint.connectedBody = part.rigidbody;
		configurableJoint.angularXMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularYMotion = ConfigurableJointMotion.Locked;
		configurableJoint.angularZMotion = ConfigurableJointMotion.Locked;
		configurableJoint.xMotion = ConfigurableJointMotion.Locked;
		configurableJoint.yMotion = ConfigurableJointMotion.Limited;
		configurableJoint.zMotion = ConfigurableJointMotion.Locked;
		SoftJointLimitSpring linearLimitSpring = configurableJoint.linearLimitSpring;
		linearLimitSpring.spring = m_springStiffness;
		linearLimitSpring.damper = 5f;
		configurableJoint.linearLimitSpring = linearLimitSpring;
		SoftJointLimit linearLimit = configurableJoint.linearLimit;
		linearLimit.limit = 0f;
		linearLimit.bounciness = 0f;
		configurableJoint.linearLimit = linearLimit;
		return configurableJoint;
	}

	private void FixedUpdate()
	{
		if (!base.contraption || !base.contraption.IsRunning)
		{
			return;
		}
		ReinitializeEngine();
		if (m_enabled)
		{
			m_thrustTimer += Time.deltaTime * 1f;
			m_thrustTimer = Mathf.Min(m_thrustTimer, 1f);
			m_thrust = Mathf.Pow(m_thrustTimer, 0.4f);
			if (base.contraption.ConnectedToGearbox(this) && base.contraption.GetGearbox(this).IsEnabled())
			{
				m_thrust = 0f - m_thrust;
			}
		}
		else
		{
			m_thrustTimer = 0f;
			m_thrust = 0f;
		}
		m_lastPosition = m_wheelPivot.transform.position;
		hasHit = Physics.Raycast(m_lastPosition, m_lastContactDirection, out var hitInfo, m_radius + 0.1f, ~m_nonCollideLayerMask);
		if (hasHit && hitInfo.collider != m_supportCollider && CanCollide(hitInfo.collider.gameObject.layer))
		{
			int layer = hitInfo.collider.gameObject.layer;
			Vector3 vector = Vector3.Cross(hitInfo.normal, Vector3.forward);
			float num = SpeedInDirection(vector);
			m_lastForceDirection = vector;
			m_spinSpeed = 0f;
			m_hasContact = true;
			colliderRigidbody = ((layer != BasePart.m_groundLayer) ? hitInfo.collider.gameObject.GetComponent<Rigidbody>() : null);
			if (m_enabled && m_maximumSpeed > 0f && num < m_maximumSpeed && num > 0f - m_maximumSpeed)
			{
				float f = 1f - Mathf.Abs(num) / m_maximumSpeed;
				f = Mathf.Pow(f, 0.5f);
				float num2 = m_thrust * m_maximumForce * f;
				base.rigidbody.AddForceAtPosition(vector * num2, hitInfo.point, ForceMode.Force);
				if (!colliderRigidbody && layer != BasePart.m_groundLayer && hitInfo.collider.transform.parent != null)
				{
					colliderRigidbody = hitInfo.collider.transform.parent.GetComponent<Rigidbody>();
				}
				if (colliderRigidbody != null && !colliderRigidbody.isKinematic)
				{
					colliderRigidbody.AddForceAtPosition(-vector * num2, hitInfo.point, ForceMode.Force);
				}
			}
			vector = Vector3.Cross(hitInfo.normal, Vector3.forward * -1f);
			base.rigidbody.AddForceAtPosition(vector * m_drag * num, (m_lastPosition + hitInfo.point) * 0.5f, ForceMode.Force);
			m_grounded = true;
		}
		else
		{
			m_hasContact = false;
			colliderRigidbody = null;
			if (m_enabled)
			{
				m_spinSpeed = m_maximumSpeed;
				if (base.contraption.ConnectedToGearbox(this) && base.contraption.GetGearbox(this).IsEnabled())
				{
					m_spinSpeed = 0f - m_maximumSpeed;
				}
			}
			else
			{
				m_spinSpeed *= 0.98f;
			}
		}
		if (hasHit && hitInfo.collider != m_supportCollider && hitInfo.collider.gameObject.layer != m_contraptionLayer && CanCollide(hitInfo.collider.gameObject.layer) && m_connected)
		{
			m_fallOffForce = m_lastContactDirection * m_stickiness;
			if (colliderRigidbody == null)
			{
				base.rigidbody.AddForceAtPosition(m_fallOffForce, (m_lastPosition + hitInfo.point) * 0.5f, ForceMode.Force);
			}
		}
		else
		{
			m_fallOffForce = Vector3.zero;
		}
	}

	private float SpeedInDirection(Vector3 direction)
	{
		Vector3 velocity = base.rigidbody.velocity;
		if ((bool)colliderRigidbody)
		{
			velocity -= colliderRigidbody.velocity;
		}
		return Vector3.Dot(velocity, direction);
	}

	protected override void OnTouch()
	{
		SetEnabled(!m_enabled);
	}

	public override void SetEnabled(bool enabled)
	{
		m_enabled = enabled;
		base.contraption.UpdateEngineStates(base.ConnectedComponent);
	}
}
