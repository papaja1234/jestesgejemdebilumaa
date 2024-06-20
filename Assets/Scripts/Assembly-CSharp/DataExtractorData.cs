using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataExtractorData : Singleton<DataExtractorData>
{
	public List<PhysicMaterial> materials;

	private void Awake()
	{
		DontDestroyOnLoad(this);
		instance = this;
	}
}
