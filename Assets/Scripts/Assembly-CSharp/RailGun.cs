using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class RailGun : BasePart
{
    private RailGunProjectile currentProjectile;
    private GameObject EnergyCore;
    	[SerializeField]
	private GameObject m_projectilePrefab;

	[SerializeField]
	private ParticleSystem m_particleEffect;

	private bool m_enabled;

	private GameObject m_leftAttachment;

	private GameObject m_rightAttachment;

	private GameObject m_topAttachment;

	private GameObject m_bottomAttachment;

	private GameObject m_bottomLeftAttachment;

	private GameObject m_bottomRightAttachment;

	private GameObject m_topLeftAttachment;

	private GameObject m_topRightAttachment;

	private float m_shootTime;
	
	public float m_rapidCooldownTime = 0.5f;

	protected float CoolingTime { get; set; }

	protected float ForcedCoolingTime { get; set; }

	public override bool CanBeEnabled()
	{
		return true;
	}

	public override bool IsEnabled()
	{
		return m_enabled;
	}

	public override Direction EffectDirection()
	{
		return BasePart.RotateWithEightDirections(Direction.Right, m_gridRotation);
	}

	public override void Awake()
	{
		base.Awake();
		m_eightWay = true;
	}

	public override void ChangeVisualConnections()
	{

	}
	protected override void OnTouch()
	{
		Shoot();
	}

	protected void Shoot()
	{
		EnsureRigidbody();
		float rapidCooldownTime = 0.0f;
		rapidCooldownTime = m_rapidCooldownTime;
		if (!m_enabled && !(Time.time - m_shootTime < rapidCooldownTime))
		{
			m_shootTime = Time.time;
			Singleton<AudioManager>.Instance.SpawnOneShotEffect(WPFMonoBehaviour.gameData.commonAudioCollection.alienLaserFire, base.transform);
			currentProjectile = UnityEngine.Object.Instantiate(m_projectilePrefab).GetComponent<RailGunProjectile>();
			//m_particleEffect.Play();
			currentProjectile.transform.parent = base.transform;
			currentProjectile.transform.localPosition = Vector3.forward * 0.1f;
			currentProjectile.transform.rotation = base.transform.rotation;
			currentProjectile.m_Acceleration = base.transform.right*2;
			currentProjectile.m_AccelerationMod = base.transform.right * 0.01f;
			Vector3 right = base.transform.right;
			currentProjectile.transform.position = base.rigidbody.position + right;
			currentProjectile.GetComponent<Rigidbody>().position = base.rigidbody.position + right;
			currentProjectile.GetComponent<Rigidbody>().velocity = base.rigidbody.velocity + base.rigidbody.velocity.normalized*2f;
			this.m_enabled = false;
		}
	}

	public override void EnsureRigidbody()
	{
		base.EnsureRigidbody();
		base.rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
	}
}
