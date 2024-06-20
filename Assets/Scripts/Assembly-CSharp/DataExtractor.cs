using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DataExtractor : MonoBehaviour
{
	public static string prefabsFolder = "Assets/PrefabInstance";

	public static string filenameExpression = "Part_*.prefab";

	public static string indentString = "  ";

	public static int indentMultiplier = 3;

	public static bool isInitialized = false;

	public static string dataPath;

	public static string I(int value)
	{
		return value.ToString();
	}

	public static string F(float value)
	{
		return value.ToString("0.0###############") + "f";
	}

	public static string B(bool value)
	{
		return value ? "true" : "false";
	}

	public static string C(char value)
	{
		return "'" + value + "'";
	}

	public static string S(string value)
	{
		return "\"" + value + "\"";
	}

	public static string V2(Vector2 value)
	{
		return "V2 (" + F(value.x) + ", " + F(value.y) + ")";
	}

	public static string V3(Vector3 value)
	{
		return "V3 (" + F(value.x) + ", " + F(value.y) + ", " + F(value.z) + ")";
	}

	public static string V4(Vector4 value)
	{
		return "V4 (" + F(value.x) + ", " + F(value.y) + ", " + F(value.z) + ", " + F(value.w) + ")";
	}

	public static string Q(Quaternion value)
	{
		return "Q (" + F(value.x) + ", " + F(value.y) + ", " + F(value.z) + ", " + F(value.w) + ")";
	}

	public static string R(Rect value)
	{
		return "R (" + F(value.x) + ", " + F(value.y) + ", " + F(value.width) + ", " + F(value.height) + ")";
	}

	public static string VI2(int x, int y)
	{
		return "VI2 (" + I(x) + ", " + I(y) + ")";
	}

	public static string VF2(float x, float y)
	{
		return "VI2 (" + F(x) + ", " + F(y) + ")";
	}

	public static string VI3(int x, int y, int z)
	{
		return "VI2 (" + I(x) + ", " + I(y) + ", " + I(z) + ")";
	}

	public static string VF3(float x, float y, float z)
	{
		return "VI2 (" + F(x) + ", " + F(y) + ", " + F(z) + ")";
	}

	public static string VI4(int x, int y, int z, int w)
	{
		return "VI2 (" + I(x) + ", " + I(y) + ", " + I(z) + ", " + I(w) + ")";
	}

	public static string VF4(float x, float y, float z, float w)
	{
		return "VI2 (" + F(x) + ", " + F(y) + ", " + F(z) + ", " + F(w) + ")";
	}

	public static string RI(int x, int y, int width, int height)
	{
		return "RI (" + I(x) + ", " + I(y) + ", " + I(width) + ", " + I(height) + ")";
	}

	public static string RF(float x, float y, float width, float height)
	{
		return "RF (" + F(x) + ", " + F(y) + ", " + F(width) + ", " + F(height) + ")";
	}

	public static string Repeat(string str, int times = 1)
	{
		string result = "";
		for (int i = 0; i < times; i++)
		{
			result += str;
		}
		return result;
	}

	public static void Initialize()
	{
		dataPath = Path.Combine(Application.persistentDataPath, "DataExtractorData");
		if (!Directory.Exists(dataPath))
		{
			Directory.CreateDirectory(dataPath);
		}
		isInitialized = true;
	}

	[MenuItem("Data Extractor/Extract All Data")]
	public static void ExtractAllData()
	{
		if (!isInitialized)
		{
			Initialize();
		}

		string readmeFilename = Path.Combine(dataPath, "README_INFO.txt");
		using (FileStream fs = new FileStream(readmeFilename, FileMode.Create, FileAccess.Write))
		using (StreamWriter sw = new StreamWriter(fs))
		{
			sw.WriteLine("All the part prefabs from " + Path.Combine(prefabsFolder, filenameExpression) + ", material data from a predefined list and loaded sprite data are extracted to a text form from Unity.");
			sw.WriteLine("These data are meant for use in programming and are prepared for easy parse while maintaining human readability.");
			sw.WriteLine("Prefab data are from multiple components of the game objects, with all it's children extracted reecursively.");
			sw.WriteLine("Each recursion appends " + S(Repeat(indentString, indentMultiplier)) + " (or more depending on nestedness of sub-data) before the current line.");
			sw.WriteLine();
			sw.WriteLine("Common structured data types include:");
			sw.WriteLine("  V2: 2 component vector (float x, float y);");
			sw.WriteLine("  V3: 3 component vector (float x, float y, float z);");
			sw.WriteLine("  V4: 4 component vector (float x, float y, float z, float w);");
			sw.WriteLine("  Q: quaternion (float x, float y, float z, float w);");
			sw.WriteLine("  R: bounded rectangle (float x, float y, float width, float height);");
			sw.WriteLine();
			sw.WriteLine("Note: some data were in form of two integers or floating point numbers as X and Y without the internal use of Vector2Int or Vector2 data type.");
			sw.WriteLine("In that case, I have used VF2 to represent 2 floating point X and Y variables, and VI2 to represent integer counterpart.");
			sw.WriteLine("Version with more variables (up to 4) is also used.");
			sw.WriteLine("There is also rectangle (R) varients: RI and RF, which contains four integers or four floating point numbers: X, Y, Width and Height.");
			sw.WriteLine();
			sw.WriteLine("Below are the full definitions of the enumerators found during data extraction:");
			sw.WriteLine();
			sw.WriteLine("\tenum JointType\n\t{\n\t\tFixedJoint = 0,\n\t\tHingeJoint = 1\n\t}\n");
			sw.WriteLine("\tenum PartTier\n\t{\n\t\tRegular = 0,\n\t\tCommon = 1,\n\t\tRare = 2,\n\t\tEpic = 3,\n\t\tLegendary = 4\n\t}\n");
			sw.WriteLine("\tenum PartType\n\t{\n\t\tUnknown = 0,\n\t\tBalloon = 1,\n\t\tBalloons2 = 2,\n\t\tBalloons3 = 3,\n\t\tFan = 4,\n\t\tWoodenFrame = 5,\n\t\tBellows = 6,\n\t\tCartWheel = 7,\n\t\tBasket = 8,\n\t\tSandbag = 9,\n\t\tPig = 10,\n\t\tSandbag2 = 11,\n\t\tSandbag3 = 12,\n\t\tPropeller = 13,\n\t\tWings = 14,\n\t\tTailplane = 15,\n\t\tEngine = 16,\n\t\tRocket = 17,\n\t\tMetalFrame = 18,\n\t\tSmallWheel = 19,\n\t\tMetalWing = 20,\n\t\tMetalTail = 21,\n\t\tRotor = 22,\n\t\tMotorWheel = 23,\n\t\tTNT = 24,\n\t\tEngineSmall = 25,\n\t\tEngineBig = 26,\n\t\tNormalWheel = 27,\n\t\tSpring = 28,\n\t\tUmbrella = 29,\n\t\tRope = 30,\n\t\tCokeBottle = 31,\n\t\tKingPig = 32,\n\t\tRedRocket = 33,\n\t\tSodaBottle = 34,\n\t\tPoweredUmbrella = 35,\n\t\tEgg = 36,\n\t\tJetEngine = 37,\n\t\tObsoleteWheel = 38,\n\t\tSpringBoxingGlove = 39,\n\t\tStickyWheel = 40,\n\t\tGrapplingHook = 41,\n\t\tPumpkin = 42,\n\t\tKicker = 43,\n\t\tGearbox = 44,\n\t\tGoldenPig = 45,\n\t\tPointLight = 46,\n\t\tSpotLight = 47,\n\t\tTimeBomb = 48,\n\t\tElectricalPart = 49,\n\t\tMAX = 50\n\t}\n");
			sw.WriteLine("\tenum AutoAlignType\n\t{\n\t\tNone = 0,\n\t\tRotate = 1,\n\t\tFlipVertically = 2\n\t}\n");
			sw.WriteLine("\tenum Direction\n\t{\n\t\tRight = 0,\n\t\tUp = 1,\n\t\tLeft = 2,\n\t\tDown = 3,\n\t\tUpRight = 4,\n\t\tUpLeft = 5,\n\t\tDownLeft = 6,\n\t\tDownRight = 7\n\t}\n");
			sw.WriteLine("\tenum GridRotation\n\t{\n\t\tDeg_0 = 0,\n\t\tDeg_90 = 1,\n\t\tDeg_180 = 2,\n\t\tDeg_270 = 3,\n\t\tDeg_45 = 4,\n\t\tDeg_135 = 5,\n\t\tDeg_225 = 6,\n\t\tDeg_315 = 7,\n\t\tDeg_Max = 8\n\t}\n");
			sw.WriteLine("\tenum JointConnectionType\n\t{\n\t\tNone = 0,\n\t\tSource = 1,\n\t\tTarget = 2\n\t}\n");
			sw.WriteLine("\tenum JointConnectionDirection\n\t{\n\t\tAny = 0,\n\t\tRight = 1,\n\t\tUp = 2,\n\t\tLeft = 3,\n\t\tDown = 4,\n\t\tLeftAndRight = 5,\n\t\tUpAndDown = 6,\n\t\tNone = 7\n\t}\n");
			sw.WriteLine("\tenum JointConnectionStrength\n\t{\n\t\tWeak = 0,\n\t\tNormal = 1,\n\t\tHigh = 2,\n\t\tExtreme = 3,\n\t\tHighlyExtreme = 4\n\t}\n");
			sw.WriteLine("\tenum AudioManager.AudioMaterial\n\t{\n\t\tNone = 0,\n\t\tWood = 1,\n\t\tMetal = 2\n\t}\n");
			sw.Close();
		}

		ExtractPartData();
		ExtractMaterialData();
		ExtractSpriteData();
	}

	[MenuItem("Data Extractor/Extract Part Data")]
	public static void ExtractPartData()
	{
		if (!isInitialized)
		{
			Initialize();
		}

		string pdeFolder = Path.Combine(dataPath, "ExtractPartData");
		if (!Directory.Exists(pdeFolder))
		{
			Directory.CreateDirectory(pdeFolder);
		}

		foreach (string prefabFilename in Directory.GetFiles(prefabsFolder, filenameExpression))
		{
			string prefabFilenameNoExtension = Path.GetFileNameWithoutExtension(prefabFilename);
			Debug.Log("Current prefab: " + prefabFilenameNoExtension);

			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFilename);
			if (prefab == null)
			{
				Debug.Log("Error loading " + prefabFilenameNoExtension + " ("+ prefabFilename + ")");
				continue;
			}

			string prefabExtractFilename = Path.Combine(pdeFolder, prefabFilenameNoExtension + ".txt");
			using (FileStream fs = new FileStream(prefabExtractFilename, FileMode.Create, FileAccess.Write))
			using (StreamWriter sw = new StreamWriter(fs))
			{
				void Traverse(GameObject prefab, GameObject root, int level = 0)
				{
					string s = Repeat(indentString, level * indentMultiplier);
					string ss = Repeat(indentString, level * indentMultiplier + 1);
					string sss = Repeat(indentString, level * indentMultiplier + 2);
					sw.WriteLine(s + "name: " + S(prefab.name) + " (prefab to string: " + S(prefab.ToString()) + "):");
					sw.WriteLine(ss + "tag: " + S(prefab.tag) + ";");

					MonoBehaviour monoBehaviour = prefab.GetComponent<MonoBehaviour>();
					if (monoBehaviour != null)
					{
						sw.WriteLine(ss + "script: " + S(monoBehaviour.GetType().Name) + " (name: " + S(monoBehaviour.name) + ");");
					}

					sw.WriteLine("\n" + ss + "transform: {");
					sw.WriteLine(sss + "position: " + V3(prefab.transform.position));
					sw.WriteLine(sss + "local position: " + V3(prefab.transform.localPosition));
					sw.WriteLine(sss + "rotation: " + Q(prefab.transform.rotation));
					sw.WriteLine(sss + "local rotation: " + Q(prefab.transform.localRotation));
					sw.WriteLine(sss + "local scale: " + V3(prefab.transform.localScale));
					sw.WriteLine(sss + "lossy scale: " + V3(prefab.transform.lossyScale));
					sw.WriteLine(ss + "};\n");

					BoxCollider boxCollider = prefab.GetComponent<BoxCollider>();
					if (boxCollider != null)
					{
						sw.WriteLine(ss + "box collider (name: " + S(boxCollider.name) + "): {");
						sw.WriteLine(sss + "center: " + V3(boxCollider.center) + ";");
						sw.WriteLine(sss + "size: " + V3(boxCollider.size) + ";");
						sw.WriteLine(sss + "material: " + S(boxCollider.material.name) + ";");
						sw.WriteLine(ss + "};\n");
					}

					CapsuleCollider capsuleCollider = prefab.GetComponent<CapsuleCollider>();
					if (capsuleCollider != null)
					{
						sw.WriteLine(ss + "capsule collider (name: " + S(capsuleCollider.name) + "): {");
						sw.WriteLine(sss + "center: " + V3(capsuleCollider.center) + ";");
						sw.WriteLine(sss + "radius: " + F(capsuleCollider.radius) + ";");
						sw.WriteLine(sss + "height: " + F(capsuleCollider.height) + ";");
						sw.WriteLine(sss + "direction: " + F(capsuleCollider.direction) + ";");
						sw.WriteLine(sss + "material: " + S(capsuleCollider.material.name) + ";");
						sw.WriteLine(ss + "};\n");
					}

					SphereCollider sphereCollider = prefab.GetComponent<SphereCollider>();
					if (sphereCollider != null)
					{
						sw.WriteLine(ss + "sphere collider (name: " + S(sphereCollider.name) + "): {");
						sw.WriteLine(sss + "center: " + V3(sphereCollider.center) + ";");
						sw.WriteLine(sss + "radius: " + F(sphereCollider.radius) + ";");
						sw.WriteLine(sss + "material: " + S(sphereCollider.material.name) + ";");
						sw.WriteLine(ss + "};\n");
					}

					Sprite sprite = prefab.GetComponent<Sprite>();
					if (sprite != null)
					{
						sw.WriteLine(ss + "sprite (name: " + S(sprite.name) + "): {");
						sw.WriteLine(sss + "id: " + S(sprite.Id) + ";");
						sw.WriteLine(sss + "scale: " + VF2(sprite.m_scaleX, sprite.m_scaleY) + ";");
						sw.WriteLine(sss + "pivot: " + VF2(sprite.m_pivotX, sprite.m_pivotY) + ";");
						sw.WriteLine(sss + "update collider: " + B(sprite.m_updateCollider) + ";");
						sw.WriteLine(sss + "size: " + V2(sprite.Size) + ";");
						sw.WriteLine(sss + "pixel size: " + V2(sprite.PixelSize) + ";");
						sw.WriteLine(sss + "uv rectangle: " + R(sprite.UVRect) + ";");
						sw.WriteLine(ss + "};\n");
					}

					INSerializedSprite serializedSprite = prefab.GetComponent<INSerializedSprite>();
					if (serializedSprite != null)
					{
						sw.WriteLine(ss + "serialized sprite (name: " + S(serializedSprite.name) + "): sprite name: " + S(serializedSprite.SpriteName) + ";\n");
					}

					BasePart part = prefab.GetComponent<BasePart>();
					if (part != null)
					{
						sw.WriteLine(ss + "base part: {");
						sw.WriteLine(sss + "eight way: " + B(part.m_eightWay) + ";");
						sw.WriteLine(sss + "coord: " + VI2(part.m_coordX, part.m_coordY) + ";");
						sw.WriteLine(sss + "mass: " + F(part.m_mass) + ";");
						sw.WriteLine(sss + "interactive radius: " + F(part.m_interactiveRadius) + ";");
						sw.WriteLine(sss + "break velocity: " + F(part.m_breakVelocity) + ";");
						sw.WriteLine(sss + "power consumption: " + F(part.m_powerConsumption) + ";");
						sw.WriteLine(sss + "engine power: " + F(part.m_enginePower) + ";");
						sw.WriteLine(sss + "z offset: " + F(part.m_ZOffset) + ";");
						sw.WriteLine(sss + "craftable: " + B(part.craftable) + ";");
						sw.WriteLine(sss + "loot crate reward: " + B(part.lootCrateReward) + ";");
						sw.WriteLine(sss + "tags: " + string.Join(", ", part.tags) + ";");
						sw.WriteLine(sss + "joint type: " + part.m_jointType + " (" + I((int)part.m_jointType) + ");");
						sw.WriteLine(sss + "part tier: " + part.m_partTier + " (" + I((int)part.m_partTier) + ");");
						sw.WriteLine(sss + "part type: " + part.m_partType + " (" + I((int)part.m_partType) + ");");
						sw.WriteLine(sss + "auto align type: " + part.m_autoAlign + " (" + I((int)part.m_autoAlign) + ");");
						sw.WriteLine(sss + "flipped: " + B(part.m_flipped) + ";");
						sw.WriteLine(sss + "grid rotation: " + part.m_gridRotation + " (" + I((int)part.m_gridRotation) + ");");
						sw.WriteLine(sss + "grid min: (" + I(part.m_gridXmin) + ", " + I(part.m_gridYmin) + ");");
						sw.WriteLine(sss + "grid max: (" + I(part.m_gridXmax) + ", " + I(part.m_gridYmax) + ");");
						sw.WriteLine(sss + "static: " + B(part.m_static) + ";");
						sw.WriteLine(sss + "joint connection strength: " + part.m_jointConnectionStrength + " (" + I((int)part.m_jointConnectionStrength) + ");");
						sw.WriteLine(sss + "joint connection type: " + part.m_jointConnectionType + " (" + I((int)part.m_jointConnectionType) + ");");
						sw.WriteLine(sss + "joint connection direction: " + part.m_jointConnectionDirection + " (" + I((int)part.m_jointConnectionDirection) + ");");
						sw.WriteLine(sss + "custom joint connection direction: " + part.m_customJointConnectionDirection + " (" + I((int)part.m_customJointConnectionDirection) + ");");
						sw.WriteLine(sss + "visible on part list before unlocking: " + B(part.VisibleOnPartListBeforeUnlocking) + ";");
						sw.WriteLine(sss + "joint preprocessing: " + B(part.JointPreprocessing) + ";");
						sw.WriteLine(sss + "audio material: " + part.AudioMaterial + " (" + I((int)part.AudioMaterial) + ");");
						sw.WriteLine(sss + "connected component: " + I(part.ConnectedComponent) + ";");
						sw.WriteLine(sss + "wind velocity: " + V3(part.WindVelocity) + ";");
						sw.WriteLine(sss + "valid: " + B(part.valid) + ";");
						sw.WriteLine(sss + "strict connected component: " + I(part.StrictConnectedComponent) + ";");
						sw.WriteLine(sss + "general connected component: " + I(part.GeneralConnectedComponent) + ";");
						sw.WriteLine(sss + "generator ref count: " + I(part.GeneratorRefCount) + ";");
						sw.WriteLine(sss + "generation level: " + I(part.GenerationLevel) + ";");
						sw.WriteLine(sss + "generation index: " + I(part.GenerationIndex) + ";");
						sw.WriteLine(sss + "temperature: " + F(part.Temperature) + ";");
						sw.WriteLine(sss + "has generator reference: " + B(part.HasGeneratorRef) + ";");
						sw.WriteLine(ss + "};\n");
					}

					if (prefab.transform.childCount != 0)
					{
						sw.WriteLine(ss + "children:");
					}
					foreach (Transform child in prefab.transform)
					{
						Traverse(child.gameObject, root, level + 1);
					}

					sw.WriteLine();
				}

				Traverse(prefab, prefab);
				sw.Close();
			}
		}
	}

	[MenuItem("Data Extractor/Extract Material Data")]
	public static void ExtractMaterialData()
	{
		if (!isInitialized)
		{
			Initialize();
		}

		string materialsFilename = Path.Combine(dataPath, "ExtractMaterialData.txt");
		using (FileStream fs = new FileStream(materialsFilename, FileMode.Create, FileAccess.Write))
		using (StreamWriter sw = new StreamWriter(fs))
		{
			foreach (PhysicMaterial material in Singleton<DataExtractorData>.Instance.materials)
			{
				string materialName = material.name;
				Debug.Log("Current material: " + materialName);
				sw.WriteLine("material: " + S(materialName));
				sw.WriteLine("  bounciness: " + F(material.bounciness) + ";");
				sw.WriteLine("  static friction: " + F(material.staticFriction) + ";");
				sw.WriteLine("  dynamic friction: " + F(material.dynamicFriction) + ";");
				sw.WriteLine("  friction combine: " + material.frictionCombine + " (" + I((int)material.frictionCombine) + ");");
				sw.WriteLine("  bounce combine: " + material.bounceCombine + " (" + I((int)material.bounceCombine) + ");");
				sw.WriteLine();
			}
			sw.Close();
		}
	}

	[MenuItem("Data Extractor/Extract Sprite Data")]
	public static void ExtractSpriteData()
	{
		if (!isInitialized)
		{
			Initialize();
		}

		string spritesFilename = Path.Combine(dataPath, "ExtractSpriteData.txt");
		using (FileStream fs = new FileStream(spritesFilename, FileMode.Create, FileAccess.Write))
		using (StreamWriter sw = new StreamWriter(fs))
		{
			foreach (KeyValuePair<string, SpriteData> pair in Singleton<RuntimeSpriteDatabase>.Instance.Data)
			{
				string id = pair.Key;
				SpriteData data = pair.Value;
				Debug.Log("Current sprite: " + id);
				sw.WriteLine("sprite: " + id);
				sw.WriteLine("  id: " + S(data.id) + ";");
				sw.WriteLine("  name: " + S(data.name) + ";");
				sw.WriteLine("  material id: " + S(data.materialId) + ";");
				sw.WriteLine("  selection: " + RI(data.selectionX, data.selectionY, data.selectionWidth, data.selectionHeight) + ";");
				sw.WriteLine("  pivot: " + VI2(data.pivotX, data.pivotY) + ";");
				sw.WriteLine("  uv position: " + VI2(data.UVx, data.UVy) + ";");
				sw.WriteLine("  size: " + VI2(data.width, data.height) + ";");
				sw.WriteLine("  subdivisions: " + I(data.subdivisions) + ";");
				sw.WriteLine("  opaque border pixels: " + I(data.opaqueBorderPixels) + ";");
				sw.WriteLine("  atlas material path: " + S(data.atlasMaterialPath) + ";");
				sw.WriteLine("  uv: " + R(data.uv) + ";\n");
				sw.WriteLine("  precalculations: {");
				void WritePrecalculatedMesh(float m_scaleX, float m_scaleY, float m_pivotX, float m_pivotY)
				{
					int num = (int)(m_scaleX * (float)data.width);
					int num2 = (int)(m_scaleY * (float)data.height);
					int num3 = data.selectionX + data.selectionWidth / 2;
					int num4 = data.selectionY + data.selectionHeight / 2;
					int num5 = data.UVx + data.width / 2;
					int num6 = data.UVy + data.height / 2;
					int num7 = num3 - num5;
					int num8 = num4 - num6;
					int num9 = (int)(m_scaleX * (float)(num7 + data.pivotX + m_pivotX));
					int num10 = (int)(m_scaleY * (float)(num8 + data.pivotY + m_pivotY));
					float num11 = (float)num * 10f / 768f;
					float num12 = (float)num2 * 10f / 768f;
					float num13 = -2f * (float)num9 * 10f / 768f;
					float num14 = -2f * (float)num10 * 10f / 768f;

					Mesh mesh = new Mesh();
					if (!Application.isPlaying)
					{
						mesh.hideFlags = HideFlags.DontSave;
					}
					mesh.vertices = new Vector3[4]
					{
						new Vector3(num13 - num11, num14 - num12, 0f),
						new Vector3(num13 - num11, num14 + num12, 0f),
						new Vector3(num13 + num11, num14 + num12, 0f),
						new Vector3(num13 + num11, num14 - num12, 0f)
					};
					mesh.triangles = new int[6] { 0, 1, 2, 2, 3, 0 };

					float num21 = 0f;
					float num22 = 0f;
					if (data.opaqueBorderPixels > 0)
					{
						num21 = (float)data.opaqueBorderPixels * data.uv.width / (float)data.width;
						num22 = (float)data.opaqueBorderPixels * data.uv.height / (float)data.height;
					}
					Vector2[] array = new Vector2[4];
					array[0].x = data.uv.x + num21;
					array[0].y = data.uv.y + num22;
					array[1].x = data.uv.x + num21;
					array[1].y = data.uv.y + data.uv.height - 1f * num22;
					array[2].x = data.uv.x + data.uv.width - 1f * num21;
					array[2].y = data.uv.y + data.uv.height - 1f * num22;
					array[3].x = data.uv.x + data.uv.width - 1f * num21;
					array[3].y = data.uv.y + num22;
					Rect dest = new Rect(0f, 0f, array[2].x - array[0].x, array[2].y - array[0].y);
					Vector2 origin = new Vector2(-array[0].x, array[2].y);
					data.uv.x *= 2048;
					data.uv.y *= 2048;
					data.uv.width *= 2048;
					data.uv.height *= 2048;
					data.uv.y = data.height - data.uv.y - data.uv.height;
					sw.WriteLine("    mesh calculations (for scale: " + VF2(m_scaleX, m_scaleY) + " and pivot: " + VF2(m_pivotX, m_pivotY) + "): {");
					sw.WriteLine("      new scaled size (num, num2): " + VI2(num, num2) + ";");
					sw.WriteLine("      selection center (num3, num4): " + VI2(num3, num4) + ";");
					sw.WriteLine("      uv center (num5, num6): " + VI2(num5, num6) + ";");
					sw.WriteLine("      center difference (num7, num8): " + VI2(num7, num8) + ";");
					sw.WriteLine("      new transformed pivot (num9, num10): " + VI2(num9, num10) + ";");
					sw.WriteLine();
					sw.WriteLine("      mesh vertices (with num11 to num14: " + VF4(num11, num12, num13, num14) + "): {");
					sw.WriteLine("        " + V3(mesh.vertices[0]) + ";");
					sw.WriteLine("        " + V3(mesh.vertices[1]) + ";");
					sw.WriteLine("        " + V3(mesh.vertices[2]) + ";");
					sw.WriteLine("        " + V3(mesh.vertices[3]) + ";");
					sw.WriteLine("      };\n");
					mesh.RecalculateNormals();
					mesh.RecalculateBounds();
					sw.WriteLine("      unity recalculated mesh vertices: {");
					sw.WriteLine("        " + V3(mesh.vertices[0]) + ";");
					sw.WriteLine("        " + V3(mesh.vertices[1]) + ";");
					sw.WriteLine("        " + V3(mesh.vertices[2]) + ";");
					sw.WriteLine("        " + V3(mesh.vertices[3]) + ";");
					sw.WriteLine("      };\n");
					sw.WriteLine("      mesh uvs (with num21, num22: " + VF2(num21, num22) + "): {");
					sw.WriteLine("        " + V2(array[0]) + ";");
					sw.WriteLine("        " + V2(array[1]) + ";");
					sw.WriteLine("        " + V2(array[2]) + ";");
					sw.WriteLine("        " + V2(array[3]) + ";");
					sw.WriteLine("      };\n");
					sw.WriteLine("      for you, my dear love: {");
					sw.WriteLine("        src rectangle: " + R(data.uv) + ";");
					sw.WriteLine("        dest rectangle: " + R(dest));
					sw.WriteLine("        origin: " + V2(origin));
					sw.WriteLine("      };");
					sw.WriteLine("    };");
				}
				WritePrecalculatedMesh(1.0f, 1.0f, 0.0f, 0.0f);
				sw.WriteLine();
				WritePrecalculatedMesh(0.4f, 0.4f, 0.0f, 0.0f);
				sw.WriteLine("  };\n\n");
			}
			sw.Close();
		}
	}
}
