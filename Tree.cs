namespace DiabeteProject;

public class Tree
{
	public Node Root { get; set; } = new Node();

	public double CalculateEntropy(List<DiabetesData> data)
	{
		var positive = data.Count(x => x.Outcome == 1);
		var negative = data.Count(x => x.Outcome == 0);
		var total = positive + negative;
		var positiveProbability = (double)positive / total;
		var negativeProbability = (double)negative / total;
		var entropy = -positiveProbability * Math.Log(positiveProbability, 2) - negativeProbability * Math.Log(negativeProbability, 2);
		return entropy;
	}

	public double CalculateInformationGain(List<DiabetesData> data, DiabeteAttribute attribute)
	{
		// TODO: get average of current or all???
		var average = data.Average(x => double.Parse(x.GetType().GetProperty(attribute.ToString()).GetValue(x).ToString()));

		var left = data.Where(x => double.Parse(x.GetType().GetProperty(attribute.ToString()).GetValue(x).ToString()) < average).ToList();
		var right = data.Where(x => double.Parse(x.GetType().GetProperty(attribute.ToString()).GetValue(x).ToString()) >= average).ToList();

		var entropy = CalculateEntropy(data);
		var leftEntropy = CalculateEntropy(left);
		var rightEntropy = CalculateEntropy(right);
		var informationGain = entropy - (left.Count / (double)data.Count) * leftEntropy - (right.Count / (double)data.Count) * rightEntropy;
		return informationGain;
	}

	public DiabeteAttribute GetBestAttribute(List<DiabetesData> data, bool[] isUsedAttribute)
	{
		var attributes = new List<DiabeteAttribute>();
		for(int i = 0; i < 8; i++)
		{
			if(isUsedAttribute[i] == false)
			{
				attributes.Add((DiabeteAttribute)i);
			}
		}

		var bestAttribute = attributes.First();
		var bestInformationGain = CalculateInformationGain(data, bestAttribute);
		foreach(var attribute in attributes)
		{
			var informationGain = CalculateInformationGain(data, attribute);
			if(informationGain > bestInformationGain)
			{
				bestInformationGain = informationGain;
				bestAttribute = attribute;
			}
		}
		return bestAttribute;
	}

	#region Temp
	//public void BuildTree(List<DiabetesData> data, Node node, bool[] isUsedAttribute)
	//{
	//	var positive = data.Count(x => x.Outcome == 1);
	//	var negative = data.Count(x => x.Outcome == 0);
	//	if(positive == 0 || negative == 0)
	//	{
	//		node.Outcome = positive > negative ? 1 : 0;
	//		return;
	//	}

	//	var bestAttribute = GetBestAttribute(data, isUsedAttribute);
	//	isUsedAttribute[(int)bestAttribute] = true;
	//	node.Attribute = bestAttribute;

	//	var average = data.Average(x => (double)x.GetType().GetProperty(bestAttribute.ToString()).GetValue(x));
	//	//node.Average = average;

	//	var left = data.Where(x => (double)x.GetType().GetProperty(bestAttribute.ToString()).GetValue(x) < average).ToList();
	//	var right = data.Where(x => (double)x.GetType().GetProperty(bestAttribute.ToString()).GetValue(x) >= average).ToList();

	//	node.Left = new Node();
	//	node.Right = new Node();
	//	BuildTree(left, node.Left, isUsedAttribute);
	//	BuildTree(right, node.Right, isUsedAttribute);
	//}
	#endregion

	public void PrintPretty(Node node,string indent, bool last)
	{
		Console.Write(indent);
		if(last)
		{
			Console.Write("└─");
			indent += "  ";
		}
		else
		{
			Console.Write("├─");
			indent += "| ";
		}
		var nodeData = $"Number of datas: {node.Data.Count} , Attribute : {node.Attribute}";
		if(node.Outcome != null)
		{
			nodeData += $" , Outcome : {node.Outcome}";
		}
		Console.WriteLine(nodeData);

		var children = new List<Node>();
		if(node.Left != null)
			children.Add(node.Left);
		if(node.Right != null)
			children.Add(node.Right);

		for(int i = 0; i < children.Count; i++)
			PrintPretty(children[i],indent, i == children.Count - 1);

	}

	public void PrintTree(Node node, int level)
	{
		Console.WriteLine($"Number of datas: {node.Data.Count} , Attribute : {node.Attribute}");
		if(node.Outcome != null)
		{
			Console.WriteLine($"Outcome: {node.Outcome}");
			return;
		}
		if(node.Left != null)
		{
			Console.WriteLine($"Left: ");
			PrintTree(node.Left, level + 1);
		}
		if(node.Right != null)
		{
			Console.WriteLine($"Right: ");
			PrintTree(node.Right, level + 1);
		}
	}
}
