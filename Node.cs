using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiabeteProject
{
	// make enum
	// Pregnancies,Glucose,BloodPressure,SkinThickness,Insulin,BMI,DiabetesPedigreeFunction,Age,Outcome
	public enum DiabeteAttribute
	{
		Pregnancies=0,
		Glucose,
		BloodPressure,
		SkinThickness,
		Insulin,
		BMI,
		DiabetesPedigreeFunction,
		Age,
		Outcome
	}
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
			var attributeValues = data.Select(x => x.GetType().GetProperty(attribute.ToString()).GetValue(x)).Distinct();
			var totalEntropy = CalculateEntropy(data);
			var total = data.Count;
			var sum = 0.0;
			foreach(var attributeValue in attributeValues)
			{
				var subset = data.Where(x => x.GetType().GetProperty(attribute.ToString()).GetValue(x).Equals(attributeValue)).ToList();
				var entropy = CalculateEntropy(subset);
				var probability = (double)subset.Count / total;
				sum += probability * entropy;
			}
			return totalEntropy - sum;
		}
		

		public DiabeteAttribute GetBestAttribute(List<DiabetesData> data)
		{
			var attributes = Enum.GetValues(typeof(DiabeteAttribute)).Cast<DiabeteAttribute>().ToList();
			attributes.Remove(DiabeteAttribute.Outcome);
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

		public void BuildTree(List<DiabetesData> data, Node node)
		{
			var bestAttribute = GetBestAttribute(data);
			node.Attribute = bestAttribute;
			var attributeValues = data.Select(x => x.GetType().GetProperty(bestAttribute.ToString()).GetValue(x)).Distinct();
			foreach(var attributeValue in attributeValues)
			{
				var child = new Node();
				child.Value = attributeValue;
				node.Children.Add(child);
				var subset = data.Where(x => x.GetType().GetProperty(bestAttribute.ToString()).GetValue(x).Equals(attributeValue)).ToList();
				if(subset.All(x => x.Outcome == 0))
				{
					child.Outcome = 0;
				}
				else if(subset.All(x => x.Outcome == 1))
				{
					child.Outcome = 1;
				}
				else
				{
					BuildTree(subset, child);
				}
			}
		}

		public void PrintTree(Node node, int level)
		{
			if(node.Outcome != null)
			{
				Console.WriteLine($"{new string(' ', level * 2)}{node.Outcome}");
			}
			else
			{
				Console.WriteLine($"{new string(' ', level * 2)}{node.Attribute} = {node.Value}");
				foreach(var child in node.Children)
				{
					PrintTree(child, level + 1);
				}
			}
		}
	}
	public class Node
	{
		public Node? Right { get; set; }
		public Node? Left { get; set; }
		public Node? Parent { get; set; }

		public DiabeteAttribute? Attribute { get; set; }

		public double? SplitValue { get; set; }
		public double? Value { get; set; }
	}
}
