public static class PartTypeExtensions
{
	public static bool IsValid(this SortedPartType sortedPartType)
	{
		return sortedPartType switch
		{
			SortedPartType.JetEngine => INSettings.GetBool(INFeature.FuelSystem), 
			SortedPartType.ElectricalPart => INSettings.GetBool(INFeature.ElectricalSystem), 
			SortedPartType.NeuralPart => INSettings.GetBool(INFeature.NeuralPart), 
			_ => true, 
		};
	}
}
