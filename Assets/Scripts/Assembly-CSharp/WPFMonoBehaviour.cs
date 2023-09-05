using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class WPFMonoBehaviour : MonoBehaviour
{
	private Animation cachedAnimation;

	private Collider cachedCollider;

	private Renderer cachedRenderer;

	private Rigidbody cachedRigidbody;

	protected static IngameCamera s_ingameCamera;

	protected static Camera s_hudCamera;

	protected static Camera s_mainCamera;

	protected static LevelManager s_levelManager;

	protected static GameData s_gameData;

	protected static EffectManager s_effectManager;

	protected static LevelSelector s_levelSelector;

	public Animation animation
	{
		get
		{
			if (cachedAnimation == null)
			{
				cachedAnimation = GetComponent<Animation>();
			}
			return cachedAnimation;
		}
		set
		{
			cachedAnimation = value;
		}
	}

	public Collider collider
	{
		get
		{
			if (cachedCollider == null)
			{
				cachedCollider = GetComponent<Collider>();
			}
			return cachedCollider;
		}
		set
		{
			cachedCollider = value;
		}
	}

	public Renderer renderer
	{
		get
		{
			if (cachedRenderer == null)
			{
				cachedRenderer = GetComponent<MeshRenderer>();
			}
			return cachedRenderer;
		}
		set
		{
			cachedRenderer = value;
		}
	}

	public Rigidbody rigidbody
	{
		get
		{
			if (cachedRigidbody == null)
			{
				cachedRigidbody = GetComponent<Rigidbody>();
			}
			return cachedRigidbody;
		}
		set
		{
			cachedRigidbody = value;
		}
	}

	public static IngameCamera ingameCamera
	{
		get
		{
			if ((bool)s_ingameCamera)
			{
				return s_ingameCamera;
			}
			IngameCamera[] array = Object.FindObjectsOfType<IngameCamera>();
			if (array.Length != 0)
			{
				s_ingameCamera = array[0];
			}
			return s_ingameCamera;
		}
	}

	public static LevelSelector levelSelector
	{
		get
		{
			if ((bool)s_levelSelector)
			{
				return s_levelSelector;
			}
			s_levelSelector = Object.FindObjectOfType<LevelSelector>();
			return s_levelSelector;
		}
	}

	public static Camera hudCamera
	{
		get
		{
			if ((bool)s_hudCamera)
			{
				return s_hudCamera;
			}
			GameObject gameObject = GameObject.FindGameObjectWithTag("HUDCamera");
			if ((bool)gameObject)
			{
				s_hudCamera = gameObject.GetComponent<Camera>();
			}
			return s_hudCamera;
		}
	}

	public static Camera mainCamera
	{
		get
		{
			if ((bool)s_mainCamera)
			{
				return s_mainCamera;
			}
			s_mainCamera = Camera.main;
			return s_mainCamera;
		}
	}

	public static LevelManager levelManager
	{
		get
		{
			if ((bool)s_levelManager)
			{
				return s_levelManager;
			}
			if (!Singleton<GameManager>.Instance.IsInGame())
			{
				return null;
			}
			LevelManager[] array = Object.FindObjectsOfType<LevelManager>();
			if (array.Length != 0)
			{
				s_levelManager = array[0];
			}
			return s_levelManager;
		}
	}

	public static EffectManager effectManager
	{
		get
		{
			if ((bool)s_effectManager)
			{
				return s_effectManager;
			}
			EffectManager[] array = Object.FindObjectsOfType<EffectManager>();
			if (array.Length != 0)
			{
				s_effectManager = array[0];
			}
			return s_effectManager;
		}
	}

	public static GameData gameData
	{
		get
		{
			if (INSettings.GetBool(INFeature.RuntimeGameData) && INRuntimeGameData.IsInitialized)
			{
				return Singleton<INRuntimeGameData>.Instance.GameData;
			}
			if ((bool)s_gameData)
			{
				return s_gameData;
			}
			s_gameData = Singleton<GameManager>.Instance.gameData;
			return s_gameData;
		}
	}

	public static Vector3 ScreenToZ0(Vector3 pos)
	{
		if ((bool)ingameCamera && ingameCamera.GetComponent<Camera>().orthographic)
		{
			Camera camera = mainCamera;
			pos.z = camera.farClipPlane;
			Vector3 result = camera.ScreenToWorldPoint(pos);
			result.z = 0f;
			return result;
		}
		Camera camera2 = mainCamera;
		pos.z = camera2.farClipPlane;
		Vector3 result2 = camera2.ScreenToWorldPoint(pos);
		result2.z = 0f;
		return result2;
	}

	public static T FindSceneObjectOfType<T>() where T : Object
	{
		T[] array = Object.FindObjectsOfType<T>();
		if (array.Length != 0)
		{
			return array[0];
		}
		return null;
	}

	public static int GetNumberOfHighestBit(int val)
	{
		for (int num = 30; num >= 0; num--)
		{
			if ((val & (1 << num)) != 0)
			{
				return num;
			}
		}
		return -1;
	}

	public static Vector3 ClipAgainstViewport(Vector3 pos1, Vector3 pos2)
	{
		Camera camera = mainCamera;
		Vector3 vector = camera.WorldToViewportPoint(pos1);
		Vector3 vector2 = camera.WorldToViewportPoint(pos2) - vector;
		float num = 1f;
		if (vector2.x < 0f)
		{
			float num2 = vector.x / (0f - vector2.x);
			if (num2 < num)
			{
				num = num2;
			}
		}
		if (vector2.y < 0f)
		{
			float num3 = vector.y / (0f - vector2.y);
			if (num3 < num)
			{
				num = num3;
			}
		}
		if (vector2.x > 0f)
		{
			float num4 = (1f - vector.x) / vector2.x;
			if (num4 < num)
			{
				num = num4;
			}
		}
		if (vector2.y > 0f)
		{
			float num5 = (1f - vector.y) / vector2.y;
			if (num5 < num)
			{
				num = num5;
			}
		}
		return camera.ViewportToWorldPoint(vector + vector2 * num);
	}

	public T[] GetComponentsOnlyInChildren<T>() where T : Component
	{
		List<T> list = new List<T>();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			list.AddRange(base.transform.GetChild(i).GetComponentsInChildren<T>());
		}
		return list.ToArray();
	}

	public List<T> GetActiveComponents<T>() where T : Component
	{
		List<T> list = new List<T>(GetComponentsInChildren<T>(includeInactive: true));
		for (int i = 0; i < list.Count; i++)
		{
			PropertyInfo property = list[i].GetType().GetProperty("enabled");
			bool flag = true;
			if (property != null && property.PropertyType == typeof(bool))
			{
				flag = (bool)property.GetValue(list[i], null);
			}
			if (!flag || !list[i].gameObject.activeSelf)
			{
				list.RemoveAt(i--);
			}
		}
		return list;
	}

	public static string GetPartStringData(BasePart part)
	{
		string str = "Part: " + part.ToString() + '\n';
		str += "	public bool m_eightWay = " + part.m_eightWay.ToString() + ";\n";
		str += "	public int m_coordX = " + part.m_coordX.ToString() + ";\n";
		str += "	public int m_coordY = " + part.m_coordY.ToString() + ";\n";
		str += "	public float m_mass = " + part.m_mass.ToString() + ";\n";
		str += "	public float m_interactiveRadius = " + part.m_interactiveRadius.ToString() + ";\n";
		str += "	public float m_breakVelocity = " + part.m_breakVelocity.ToString() + ";\n";
		str += "	public float m_powerConsumption = " + part.m_powerConsumption.ToString() + ";\n";
		str += "	public float m_enginePower = " + part.m_enginePower.ToString() + ";\n";
		str += "	public float m_ZOffset = " + part.m_ZOffset.ToString() + ";\n";
		str += "	public int customPartIndex = " + part.customPartIndex.ToString() + ";\n";
		str += "	public bool craftable = " + part.craftable.ToString() + ";\n";
		str += "	public bool lootCrateReward = " + part.lootCrateReward.ToString() + ";\n";
		// str += "	public List<string> tags = " + part.tags.ToString() + ";\n";
		str += "	public JointType m_jointType = " + part.m_jointType.ToString() + ";\n";
		str += "	public PartTier m_partTier = " + part.m_partTier.ToString() + ";\n";
		str += "	public PartType m_partType = " + part.m_partType.ToString() + ";\n";
		str += "	public AutoAlignType m_autoAlign = " + part.m_autoAlign.ToString() + ";\n";
		str += "	public bool m_flipped = " + part.m_flipped.ToString() + ";\n";
		str += "	public GridRotation m_gridRotation = " + part.m_gridRotation.ToString() + ";\n";
		str += "	public int m_gridXmin = " + part.m_gridXmin.ToString() + ";\n";
		str += "	public int m_gridXmax = " + part.m_gridXmax.ToString() + ";\n";
		str += "	public int m_gridYmin = " + part.m_gridYmin.ToString() + ";\n";
		str += "	public int m_gridYmax = " + part.m_gridYmax.ToString() + ";\n";
		str += "	public bool m_static = " + part.m_static.ToString() + ";\n";
		str += "	public JointConnectionStrength m_jointConnectionStrength = " + part.m_jointConnectionStrength.ToString() + ";\n";
		str += "	public JointConnectionType m_jointConnectionType = " + part.m_jointConnectionType.ToString() + ";\n";
		str += "	public JointConnectionDirection m_jointConnectionDirection = " + part.m_jointConnectionDirection.ToString() + ";\n";
		str += "	public JointConnectionDirection m_customJointConnectionDirection = " + part.m_customJointConnectionDirection.ToString() + ";\n";
		// str += "	public BasePart m_enclosedPart = " + part.m_enclosedPart.ToString() + ";\n";
		// str += "	public BasePart m_enclosedInto = " + part.m_enclosedInto.ToString() + ";\n";
		// str += "	public Sprite m_constructionIconSprite = " + part.m_constructionIconSprite.ToString() + ";\n";
		str += "	public bool VisibleOnPartListBeforeUnlocking = " + part.VisibleOnPartListBeforeUnlocking.ToString() + ";\n";
		str += "	public bool JointPreprocessing = " + part.JointPreprocessing.ToString() + ";\n";
		// str += "	public virtual Vector3 Position = " + part.Position.ToString() + ";\n";
		str += "	public AudioManager.AudioMaterial AudioMaterial = " + part.AudioMaterial.ToString() + ";\n";
		// str += "	public Contraption contraption = " + part.contraption.ToString() + ";\n";
		str += "	public int ConnectedComponent = " + part.ConnectedComponent.ToString() + ";\n";
		// str += "	public BasePart enclosedPart = " + part.enclosedPart.ToString() + ";\n";
		// str += "	public BasePart enclosedInto = " + part.enclosedInto.ToString() + ";\n";
		// str += "	public Vector3 WindVelocity = " + part.WindVelocity.ToString() + ";\n";
		str += "	public bool valid = " + part.valid.ToString() + ";\n";
		str += "	public PartType Type = " + part.Type.ToString() + ";\n";
		str += "	public PartTier Tier = " + part.Tier.ToString() + ";\n";
		str += "	public int Index = " + part.Index.ToString() + ";\n";
		str += "	public int CoordX = " + part.CoordX.ToString() + ";\n";
		str += "	public int CoordY = " + part.CoordY.ToString() + ";\n";
		str += "	public GridRotation Rotation = " + part.Rotation.ToString() + ";\n";
		str += "	public bool Flipped = " + part.Flipped.ToString() + ";\n";
		// str += "	public PartTypeInfo TypeInfo = " + part.TypeInfo.ToString() + ";\n";
		str += "	public int StrictConnectedComponent = " + part.StrictConnectedComponent.ToString() + ";\n";
		str += "	public int GeneralConnectedComponent = " + part.GeneralConnectedComponent.ToString() + ";\n";
		str += "	public int GeneratorRefCount = " + part.GeneratorRefCount.ToString() + ";\n";
		str += "	public int GenerationLevel = " + part.GenerationLevel.ToString() + ";\n";
		str += "	public int GenerationIndex = " + part.GenerationIndex.ToString() + ";\n";
		str += "	public float Temperature = " + part.Temperature.ToString() + ";\n";
		str += "	public bool HasGeneratorRef = " + part.HasGeneratorRef.ToString() + ";\n";
		return str;
	}

	public void ExportAllPartData(GameData currentGameData, string filename)
	{
		StreamWriter streamWriter = new StreamWriter(filename);
		int partTypeCount = System.Enum.GetNames(typeof(BasePart.PartType)).Length;
		try
		{
			for (int i = 0; i < partTypeCount; i++)
			{
				CustomPartInfo customPart = currentGameData.GetCustomPart((BasePart.PartType)i);
				if (customPart == null)
				{
					goto end;
				}
				if (customPart.PartList == null)
				{
					goto end;
				}
				foreach (BasePart part in customPart.PartList)
				{
					streamWriter.Write(GetPartStringData(part));
				}
			}
		}
		catch (System.Exception exception)
		{
			Debug.Log(exception.Message);
		}
	end:
		streamWriter.Close();
	}

	public static class PrefabExtractor
	{
		public static void ExportPartPrefabs(List<BasePart> parts)
		{
			foreach (BasePart part in parts)
			{
				StreamWriter streamWriter = new StreamWriter(@"C:\Users\Me\Documents\Parts\" + part.ToString() + ".txt");
				int nestingLevel = 1;
				GetResource(part.gameObject, streamWriter, 0);
				CollectResources(part.gameObject.transform, streamWriter, nestingLevel);
				streamWriter.Close();
			}
		}

		public static void CollectResources(Transform transform, StreamWriter streamWriter, int nestingLevel)
		{
			foreach (Transform child in transform)
			{
				GetResource(child.gameObject, streamWriter, nestingLevel);
				CollectResources(child, streamWriter, nestingLevel + 1);
			}
		}

		public static string GetTransformString(Transform transform)
		{
			return "Transform transform = { { " + transform.position.x + ", " + transform.position.y + ", " + transform.position.z + " }, { " + transform.rotation.eulerAngles.x + ", " + transform.rotation.eulerAngles.y + ", " + transform.rotation.eulerAngles.z + " }, { " + transform.localScale.x + ", " + transform.localScale.y + ", " + transform.localScale.z + " } };";
		}

		public static string GetSphereColliderString(SphereCollider sphereCollider)
		{
			return "SphereCollider sphereCollider = { " + sphereCollider.isTrigger + ", " + sphereCollider.material.ToString() + ", { " + sphereCollider.center.x + ", " + sphereCollider.center.y + ", " + sphereCollider.center.z + " }, " + sphereCollider.radius + " };";
		}

		public static string GetCapsuleColliderString(CapsuleCollider capsuleCollider)
		{
			return "CapsuleCollider capsuleCollider = { " + capsuleCollider.isTrigger + ", " + capsuleCollider.material.ToString() + ", { " + capsuleCollider.center.x + ", " + capsuleCollider.center.y + ", " + capsuleCollider.center.z + " }, " + capsuleCollider.radius + ", " + capsuleCollider.height + " };";
		}

		public static string GetBoxColliderString(BoxCollider boxCollider)
		{
			return "BoxCollider boxCollider = { " + boxCollider.isTrigger + ", { " + boxCollider.center.x + ", " + boxCollider.center.y + ", " + boxCollider.center.z + " }, { " + boxCollider.size.x + ", " + boxCollider.size.y + ", " + boxCollider.size.z + " } };";
		}

		public static string GetSpriteString(Sprite sprite)
		{
			return "Sprite sprite = { " + sprite.m_id + ", " + sprite.m_scaleX + ", " + sprite.m_scaleY + ", " + sprite.m_pivotX + ", " + sprite.m_pivotY + ", " + sprite.m_updateCollider + " };";
		}

		public static string GetINSerializedSpriteString(INSerializedSprite serializedSprite)
		{
			return "INSerializedSprite serializedSprite = { " + serializedSprite.SpriteName + " }";
		}

		public static void GetResource(GameObject gameObject, StreamWriter streamWriter, int nestingLevel)
		{
			for (int i = 0; i < nestingLevel; i++) streamWriter.Write("	");
			streamWriter.WriteLine(gameObject.ToString());
			for (int i = 0; i < nestingLevel + 1; i++) streamWriter.Write("	");
			streamWriter.WriteLine(GetTransformString(gameObject.transform));

			SphereCollider sphereCollider = gameObject.GetComponent<SphereCollider>();
			if (sphereCollider != null)
			{
				for (int i = 0; i < nestingLevel + 1; i++) streamWriter.Write("	");
				streamWriter.WriteLine(GetSphereColliderString(sphereCollider));
			}

			CapsuleCollider capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
			if (capsuleCollider != null)
			{
				for (int i = 0; i < nestingLevel + 1; i++) streamWriter.Write("	");
				streamWriter.WriteLine(GetCapsuleColliderString(capsuleCollider));
			}

			BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
			if (boxCollider != null)
			{
				for (int i = 0; i < nestingLevel + 1; i++) streamWriter.Write("	");
				streamWriter.WriteLine(GetBoxColliderString(boxCollider));
			}

			Sprite sprite = gameObject.GetComponent<Sprite>();
			if (sprite != null)
			{
				for (int i = 0; i < nestingLevel + 1; i++) streamWriter.Write("	");
				streamWriter.WriteLine(GetSpriteString(sprite));
			}

			INSerializedSprite serializedSprite = gameObject.GetComponent<INSerializedSprite>();
			if (serializedSprite != null)
			{
				for (int i = 0; i < nestingLevel + 1; i++) streamWriter.Write("	");
				streamWriter.WriteLine(GetINSerializedSpriteString(serializedSprite));
			}
		}
	}
}
