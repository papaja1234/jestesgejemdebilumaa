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

	public static int indentMultiplier = 2;

	public static bool isInitialized = false;

	public static string dataPath;

	public static bool splitFiles = false;

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

	[MenuItem("Data Extractor/Reinitialize")]
	public static void Initialize()
	{
		dataPath = Path.Combine(Application.persistentDataPath, "DataExtractorData");
		if (!Directory.Exists(dataPath))
		{
			Directory.CreateDirectory(dataPath);
		}
		isInitialized = true;
	}

	[MenuItem("Data Extractor/Set Split Files")]
	public static void SetSplitFiles()
	{
		splitFiles = true;
	}

	[MenuItem("Data Extractor/Set Combine Files")]
	public static void SetCombineFiles()
	{
		splitFiles = false;
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

	public static void WritePartData(StreamWriter sw, GameObject part)
	{
		void Traverse(GameObject part, GameObject root, int level = 0)
		{
			string s = Repeat(indentString, level * indentMultiplier);
			string ss = Repeat(indentString, level * indentMultiplier + 1);
			string sss = Repeat(indentString, level * indentMultiplier + 2);
			sw.WriteLine(s + "part (name: " + S(part.name) + "): {");
			sw.WriteLine(ss + "tag: " + S(part.tag) + ";");

			MonoBehaviour monoBehaviour = part.GetComponent<MonoBehaviour>();
			if (monoBehaviour != null)
			{
				sw.WriteLine(ss + "script: " + S(monoBehaviour.GetType().Name) + " (name: " + S(monoBehaviour.name) + ");");
			}

			sw.WriteLine("\n" + ss + "transform: {");
			sw.WriteLine(sss + "position: " + V3(part.transform.position) + ";");
			sw.WriteLine(sss + "local position: " + V3(part.transform.localPosition) + ";");
			sw.WriteLine(sss + "rotation: " + Q(part.transform.rotation) + ";");
			sw.WriteLine(sss + "local rotation: " + Q(part.transform.localRotation) + ";");
			sw.WriteLine(sss + "lossy scale: " + V3(part.transform.lossyScale) + ";");
			sw.WriteLine(sss + "local scale: " + V3(part.transform.localScale) + ";");
			sw.WriteLine(ss + "}; # transform");

			BoxCollider boxCollider = part.GetComponent<BoxCollider>();
			if (boxCollider != null)
			{
				sw.WriteLine("\n" + ss + "box collider (name: " + S(boxCollider.name) + "): {");
				sw.WriteLine(sss + "center: " + V3(boxCollider.center) + ";");
				sw.WriteLine(sss + "size: " + V3(boxCollider.size) + ";");
				sw.WriteLine(sss + "trigger: " + B(boxCollider.isTrigger) + ";");
				sw.WriteLine(sss + "contact offset: " + F(boxCollider.contactOffset) + ";");
				if (boxCollider.sharedMaterial != null)
				{
					sw.WriteLine(sss + "material: " + S(boxCollider.sharedMaterial.name) + ";");
					sw.WriteLine();
					WriteMaterialData(sw, boxCollider.sharedMaterial.name, level, 2, boxCollider.sharedMaterial);
				}
				sw.WriteLine(ss + "}; # box collider");
			}

			CapsuleCollider capsuleCollider = part.GetComponent<CapsuleCollider>();
			if (capsuleCollider != null)
			{
				sw.WriteLine("\n" + ss + "capsule collider (name: " + S(capsuleCollider.name) + "): {");
				sw.WriteLine(sss + "center: " + V3(capsuleCollider.center) + ";");
				sw.WriteLine(sss + "radius: " + F(capsuleCollider.radius) + ";");
				sw.WriteLine(sss + "height: " + F(capsuleCollider.height) + ";");
				sw.WriteLine(sss + "direction: " + F(capsuleCollider.direction) + ";");
				sw.WriteLine(sss + "trigger: " + B(capsuleCollider.isTrigger) + ";");
				sw.WriteLine(sss + "contact offset: " + F(capsuleCollider.contactOffset) + ";");
				if (capsuleCollider.sharedMaterial != null)
				{
					sw.WriteLine(sss + "material: " + S(capsuleCollider.sharedMaterial.name) + ";");
					sw.WriteLine();
					WriteMaterialData(sw, capsuleCollider.sharedMaterial.name, level, 2, capsuleCollider.sharedMaterial);
				}
				sw.WriteLine(ss + "}; # capsule collider");
			}

			SphereCollider sphereCollider = part.GetComponent<SphereCollider>();
			if (sphereCollider != null)
			{
				sw.WriteLine("\n" + ss + "sphere collider (name: " + S(sphereCollider.name) + "): {");
				sw.WriteLine(sss + "center: " + V3(sphereCollider.center) + ";");
				sw.WriteLine(sss + "radius: " + F(sphereCollider.radius) + ";");
				sw.WriteLine(sss + "trigger: " + B(sphereCollider.isTrigger) + ";");
				sw.WriteLine(sss + "contact offset: " + F(sphereCollider.contactOffset) + ";");
				if (sphereCollider.sharedMaterial != null)
				{
					sw.WriteLine(sss + "material: " + S(sphereCollider.sharedMaterial.name) + ";");
					sw.WriteLine();
					WriteMaterialData(sw, sphereCollider.sharedMaterial.name, level, 2, sphereCollider.sharedMaterial);
				}
				sw.WriteLine(ss + "}; # sphere collider");
			}

			Sprite sprite = part.GetComponent<Sprite>();
			if (sprite != null)
			{
				sw.WriteLine("\n" + ss + "sprite (name: " + S(sprite.name) + "): {");
				sw.WriteLine(sss + "id: " + S(sprite.Id) + ";");
				sw.WriteLine(sss + "scale: " + VF2(sprite.m_scaleX, sprite.m_scaleY) + ";");
				sw.WriteLine(sss + "pivot: " + VF2(sprite.m_pivotX, sprite.m_pivotY) + ";");
				sw.WriteLine(sss + "update collider: " + B(sprite.m_updateCollider) + ";");
				sw.WriteLine(sss + "size: " + V2(sprite.Size) + ";");
				sw.WriteLine(sss + "pixel size: " + V2(sprite.PixelSize) + ";");
				sw.WriteLine(sss + "uv rectangle: " + R(sprite.UVRect) + ";");
				sw.WriteLine();
				WriteSpriteData(sw, Singleton<RuntimeSpriteDatabase>.Instance.Find(sprite.Id), sprite.Id, level, 2, sprite.m_scaleX, sprite.m_scaleY, sprite.m_pivotX, sprite.m_pivotY);
				sw.WriteLine(ss + "}; # sprite");
			}

			INSerializedSprite serializedSprite = part.GetComponent<INSerializedSprite>();
			if (serializedSprite != null)
			{
				sw.WriteLine("\n" + ss + "serialized sprite (name: " + S(serializedSprite.name) + "): sprite name: " + S(serializedSprite.SpriteName) + ";");
			}

			BasePart basePart = part.GetComponent<BasePart>();
			if (basePart != null)
			{
				sw.WriteLine("\n" + ss + "base part: {");
				sw.WriteLine(sss + "eight way: " + B(basePart.m_eightWay) + ";");
				//sw.WriteLine(sss + "coord: " + VI2(basePart.m_coordX, basePart.m_coordY) + ";");
				sw.WriteLine(sss + "mass: " + F(basePart.m_mass) + ";");
				sw.WriteLine(sss + "interactive radius: " + F(basePart.m_interactiveRadius) + ";");
				sw.WriteLine(sss + "break velocity: " + F(basePart.m_breakVelocity) + ";");
				sw.WriteLine(sss + "power consumption: " + F(basePart.m_powerConsumption) + ";");
				sw.WriteLine(sss + "engine power: " + F(basePart.m_enginePower) + ";");
				sw.WriteLine(sss + "z offset: " + F(basePart.m_ZOffset) + ";");
				//sw.WriteLine(sss + "craftable: " + B(basePart.craftable) + ";");
				//sw.WriteLine(sss + "loot crate reward: " + B(basePart.lootCrateReward) + ";");
				//sw.WriteLine(sss + "tags: " + string.Join(", ", basePart.tags) + ";");
				sw.WriteLine(sss + "joint type: " + basePart.m_jointType + " (" + I((int)basePart.m_jointType) + ");");
				sw.WriteLine(sss + "part tier: " + basePart.m_partTier + " (" + I((int)basePart.m_partTier) + ");");
				sw.WriteLine(sss + "part type: " + basePart.m_partType + " (" + I((int)basePart.m_partType) + ");");
				sw.WriteLine(sss + "auto align type: " + basePart.m_autoAlign + " (" + I((int)basePart.m_autoAlign) + ");");
				sw.WriteLine(sss + "flipped: " + B(basePart.m_flipped) + ";");
				//sw.WriteLine(sss + "grid rotation: " + basePart.m_gridRotation + " (" + I((int)basePart.m_gridRotation) + ");");
				sw.WriteLine(sss + "grid min: (" + I(basePart.m_gridXmin) + ", " + I(basePart.m_gridYmin) + ");");
				sw.WriteLine(sss + "grid max: (" + I(basePart.m_gridXmax) + ", " + I(basePart.m_gridYmax) + ");");
				//sw.WriteLine(sss + "static: " + B(basePart.m_static) + ";");
				sw.WriteLine(sss + "joint connection strength: " + basePart.m_jointConnectionStrength + " (" + I((int)basePart.m_jointConnectionStrength) + ");");
				sw.WriteLine(sss + "joint connection type: " + basePart.m_jointConnectionType + " (" + I((int)basePart.m_jointConnectionType) + ");");
				sw.WriteLine(sss + "joint connection direction: " + basePart.m_jointConnectionDirection + " (" + I((int)basePart.m_jointConnectionDirection) + ");");
				sw.WriteLine(sss + "custom joint connection direction: " + basePart.m_customJointConnectionDirection + " (" + I((int)basePart.m_customJointConnectionDirection) + ");");
				//sw.WriteLine(sss + "visible on part list before unlocking: " + B(basePart.VisibleOnPartListBeforeUnlocking) + ";");
				sw.WriteLine(sss + "joint preprocessing: " + B(basePart.JointPreprocessing) + ";");
				//sw.WriteLine(sss + "audio material: " + basePart.AudioMaterial + " (" + I((int)basePart.AudioMaterial) + ");");
				sw.WriteLine(sss + "connected component: " + I(basePart.ConnectedComponent) + ";");
				sw.WriteLine(sss + "wind velocity: " + V3(basePart.WindVelocity) + ";");
				//sw.WriteLine(sss + "valid: " + B(basePart.valid) + ";");
				sw.WriteLine(sss + "strict connected component: " + I(basePart.StrictConnectedComponent) + ";");
				sw.WriteLine(sss + "general connected component: " + I(basePart.GeneralConnectedComponent) + ";");
				sw.WriteLine(sss + "generator ref count: " + I(basePart.GeneratorRefCount) + ";");
				sw.WriteLine(sss + "generation level: " + I(basePart.GenerationLevel) + ";");
				sw.WriteLine(sss + "generation index: " + I(basePart.GenerationIndex) + ";");
				sw.WriteLine(sss + "temperature: " + F(basePart.Temperature) + ";");
				sw.WriteLine(sss + "has generator reference: " + B(basePart.HasGeneratorRef) + ";");
				sw.WriteLine(ss + "}; # base part");
			}

			if (part.transform.childCount != 0)
			{
				sw.WriteLine("\n" + ss + "children: {");
			}
			bool once = true;
			foreach (Transform child in part.transform)
			{
				if (once)
				{
					once = false;
				}
				else
				{
					sw.WriteLine();
				}
				Traverse(child.gameObject, root, level + 1);
			}

			if (part.transform.childCount != 0)
			{
				sw.WriteLine(ss + "}; # children");
			}
			sw.WriteLine(s + "}; # part (name: " + S(part.name) + ")");
		}

		Traverse(part, part);
	}

	[MenuItem("Data Extractor/Extract Part Data")]
	public static void ExtractPartData()
	{
		if (!isInitialized)
		{
			Initialize();
		}

		if (splitFiles)
		{
			string partsFolder = Path.Combine(dataPath, "ExtractPartData");
			if (!Directory.Exists(partsFolder))
			{
				Directory.CreateDirectory(partsFolder);
			}

			foreach (string prefabFilename in Directory.GetFiles(prefabsFolder, filenameExpression))
			{
				string prefabFilenameNoExtension = Path.GetFileNameWithoutExtension(prefabFilename);
				Debug.Log("Current prefab: " + prefabFilenameNoExtension);

				GameObject part = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFilename);
				if (part == null)
				{
					Debug.Log("Error loading " + prefabFilenameNoExtension + " (" + prefabFilename + ")");
					continue;
				}

				string partFilename = Path.Combine(partsFolder, prefabFilenameNoExtension + ".txt");
				using (FileStream fs = new FileStream(partFilename, FileMode.Create, FileAccess.Write))
				using (StreamWriter sw = new StreamWriter(fs))
				{
					WritePartData(sw, part);
					sw.Close();
				}
			}
		}
		else
		{
			string partsFilename = Path.Combine(dataPath, "ExtractPartData.txt");
			using (FileStream fs = new FileStream(partsFilename, FileMode.Create, FileAccess.Write))
			using (StreamWriter sw = new StreamWriter(fs))
			{
				foreach (string prefabFilename in Directory.GetFiles(prefabsFolder, filenameExpression))
				{
					string prefabFilenameNoExtension = Path.GetFileNameWithoutExtension(prefabFilename);
					Debug.Log("Current prefab: " + prefabFilenameNoExtension);

					GameObject part = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFilename);
					if (part == null)
					{
						Debug.Log("Error loading " + prefabFilenameNoExtension + " (" + prefabFilename + ")");
						continue;
					}

					WritePartData(sw, part);
					sw.WriteLine();
				}
				sw.Close();
			}
		}
	}

	public static void WriteMaterialData(StreamWriter sw, string materialName, int nestLevel, int nestStart, PhysicMaterial material)
	{
		string ss = Repeat(indentString, nestLevel * indentMultiplier + nestStart); nestStart++;
		string sss = Repeat(indentString, nestLevel * indentMultiplier + nestStart);
		sw.WriteLine(ss + "material data values (name: " + S(materialName) + "): {");
		sw.WriteLine(sss + "bounciness: " + F(material.bounciness) + ";");
		sw.WriteLine(sss + "static friction: " + F(material.staticFriction) + ";");
		sw.WriteLine(sss + "dynamic friction: " + F(material.dynamicFriction) + ";");
		sw.WriteLine(sss + "friction combine: " + material.frictionCombine + " (" + I((int)material.frictionCombine) + ");");
		sw.WriteLine(sss + "bounce combine: " + material.bounceCombine + " (" + I((int)material.bounceCombine) + ");");
		sw.WriteLine(ss + "}; # material data values (name: " + S(materialName) + ")");
	}

	[MenuItem("Data Extractor/Extract Material Data")]
	public static void ExtractMaterialData()
	{
		if (!isInitialized)
		{
			Initialize();
		}

		if (splitFiles)
		{
			string materialsFolder = Path.Combine(dataPath, "ExtractMaterialData");
			if (!Directory.Exists(materialsFolder))
			{
				Directory.CreateDirectory(materialsFolder);
			}

			foreach (PhysicMaterial material in Singleton<DataExtractorData>.Instance.materials)
			{
				string materialName = material.name;
				Debug.Log("Current material: " + materialName);

				string materialFilename = Path.Combine(materialsFolder, materialName + ".txt");
				using (FileStream fs = new FileStream(materialFilename, FileMode.Create, FileAccess.Write))
				using (StreamWriter sw = new StreamWriter(fs))
				{
					WriteMaterialData(sw, materialName, 0, 0, material);
					sw.Close();
				}
			}
		}
		else
		{
			string materialsFilename = Path.Combine(dataPath, "ExtractMaterialData.txt");
			using (FileStream fs = new FileStream(materialsFilename, FileMode.Create, FileAccess.Write))
			using (StreamWriter sw = new StreamWriter(fs))
			{
				foreach (PhysicMaterial material in Singleton<DataExtractorData>.Instance.materials)
				{
					string materialName = material.name;
					Debug.Log("Current material: " + materialName);

					WriteMaterialData(sw, materialName, 0, 0, material);
					sw.WriteLine();
				}
				sw.Close();
			}
		}
	}

	public static void WriteSpriteData(StreamWriter sw, SpriteData data, string id, int nestLevel, int nestStart, float m_scaleX, float m_scaleY, float m_pivotX, float m_pivotY)
	{
		string ss = Repeat(indentString, nestLevel * indentMultiplier + nestStart); nestStart++;
		string sss = Repeat(indentString, nestLevel * indentMultiplier + nestStart); nestStart++;
		string ssss = Repeat(indentString, nestLevel * indentMultiplier + nestStart);

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
		Rect dest = new Rect(0f, 0f, mesh.vertices[2].x - mesh.vertices[0].x, mesh.vertices[2].y - mesh.vertices[0].y);
		Vector2 origin = new Vector2(-mesh.vertices[0].x, mesh.vertices[2].y);
		data.uv.x *= 2048;
		data.uv.y *= 2048;
		data.uv.width *= 2048;
		data.uv.height *= 2048;
		data.uv.y = data.height - data.uv.y - data.uv.height;

		sw.WriteLine(ss + "sprite data values (id: " + id + "): {");
		sw.WriteLine(sss + "id: " + S(data.id) + ";");
		sw.WriteLine(sss + "name: " + S(data.name) + ";");
		sw.WriteLine(sss + "material id: " + S(data.materialId) + ";");
		//sw.WriteLine(sss + "selection: " + RI(data.selectionX, data.selectionY, data.selectionWidth, data.selectionHeight) + ";");
		//sw.WriteLine(sss + "pivot: " + VI2(data.pivotX, data.pivotY) + ";");
		//sw.WriteLine(sss + "uv position: " + VI2(data.UVx, data.UVy) + ";");
		//sw.WriteLine(sss + "size: " + VI2(data.width, data.height) + ";");
		//sw.WriteLine(sss + "subdivisions: " + I(data.subdivisions) + ";");
		//sw.WriteLine(sss + "opaque border pixels: " + I(data.opaqueBorderPixels) + ";");
		//sw.WriteLine(sss + "atlas material path: " + S(data.atlasMaterialPath) + ";");
		//sw.WriteLine(sss + "uv: " + R(data.uv) + ";");
		sw.WriteLine();
		//sw.WriteLine(sss + "mesh calculations (for scale: " + VF2(m_scaleX, m_scaleY) + " and pivot: " + VF2(m_pivotX, m_pivotY) + "): {");
		//sw.WriteLine(ssss + "new scaled size (num, num2): " + VI2(num, num2) + ";");
		//sw.WriteLine(ssss + "selection center (num3, num4): " + VI2(num3, num4) + ";");
		//sw.WriteLine(ssss + "uv center (num5, num6): " + VI2(num5, num6) + ";");
		//sw.WriteLine(ssss + "center difference (num7, num8): " + VI2(num7, num8) + ";");
		//sw.WriteLine(ssss + "new transformed pivot (num9, num10): " + VI2(num9, num10) + ";");
		//sw.WriteLine();
		//sw.WriteLine(ssss + "mesh vertices (with num11 to num14: " + VF4(num11, num12, num13, num14) + "): {");
		//sw.WriteLine(sssss + V3(mesh.vertices[0]) + ";");
		//sw.WriteLine(sssss + V3(mesh.vertices[1]) + ";");
		//sw.WriteLine(sssss + V3(mesh.vertices[2]) + ";");
		//sw.WriteLine(sssss + V3(mesh.vertices[3]) + ";");
		//sw.WriteLine(ssss + "}; # mesh vertices\n");
		//mesh.RecalculateNormals();
		//mesh.RecalculateBounds();
		//sw.WriteLine(ssss + "unity recalculated mesh vertices: {");
		//sw.WriteLine(sssss + V3(mesh.vertices[0]) + ";");
		//sw.WriteLine(sssss + V3(mesh.vertices[1]) + ";");
		//sw.WriteLine(sssss + V3(mesh.vertices[2]) + ";");
		//sw.WriteLine(sssss + V3(mesh.vertices[3]) + ";");
		//sw.WriteLine(ssss + "}; # unity recalculated mesh vertices\n");
		//sw.WriteLine(ssss + "mesh uvs (with num21, num22: " + VF2(num21, num22) + "): {");
		//sw.WriteLine(sssss + V2(array[0]) + ";");
		//sw.WriteLine(sssss + V2(array[1]) + ";");
		//sw.WriteLine(sssss + V2(array[2]) + ";");
		//sw.WriteLine(sssss + V2(array[3]) + ";");
		//sw.WriteLine(ssss + "}; # mesh uvs\n");
		sw.WriteLine(sss + "for you, my dear love: {");
		sw.WriteLine(ssss + "src rectangle: " + R(data.uv) + ";");
		sw.WriteLine(ssss + "dest rectangle: " + R(dest) + ";");
		sw.WriteLine(ssss + "origin: " + V2(origin) + ";");
		sw.WriteLine(sss + "}; # for you, my dear love");
		//sw.WriteLine(sss + "}; # mesh calculations");
		sw.WriteLine(ss + "}; # sprite data values (id: " + id + ")");
	}

	[MenuItem("Data Extractor/Extract Sprite Data")]
	public static void ExtractSpriteData()
	{
		if (!isInitialized)
		{
			Initialize();
		}

		if (splitFiles)
		{
			string spritesFolder = Path.Combine(dataPath, "ExtractSpriteData");
			if (!Directory.Exists(spritesFolder))
			{
				Directory.CreateDirectory(spritesFolder);
			}

			foreach (KeyValuePair<string, SpriteData> pair in Singleton<RuntimeSpriteDatabase>.Instance.Data)
			{
				string id = pair.Key;
				SpriteData data = pair.Value;
				Debug.Log("Current sprite: " + id);

				string spriteFilename = Path.Combine(spritesFolder, id + ".txt");
				using (FileStream fs = new FileStream(spriteFilename, FileMode.Create, FileAccess.Write))
				using (StreamWriter sw = new StreamWriter(fs))
				{
					WriteSpriteData(sw, data, id, 0, 0, 0.4f, 0.4f, 0.0f, 0.0f);
					sw.Close();
				}
			}
		}
		else
		{
			string spritesFilename = Path.Combine(dataPath, "ExtractSpriteData.txt");
			using (FileStream fs = new FileStream(spritesFilename, FileMode.Create, FileAccess.Write))
			using (StreamWriter sw = new StreamWriter(fs))
			{
				foreach (KeyValuePair<string, SpriteData> pair in Singleton<RuntimeSpriteDatabase>.Instance.Data)
				{
					string id = pair.Key;
					SpriteData data = pair.Value;
					Debug.Log("Current sprite: " + id);

					WriteSpriteData(sw, data, id, 0, 0, 0.4f, 0.4f, 0.0f, 0.0f);
					sw.WriteLine();
				}
				sw.Close();
			}
		}
	}
}
