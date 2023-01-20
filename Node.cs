using DiabeteProject;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DiabeteProject;
public class Node
{
	public List<DiabetesData> Data { get; set; } = new List<DiabetesData>();

	/// <summary>
	/// Attributes left to split by
	/// </summary>
	public bool[] IsUsedAttribute { get; set; } = new bool[8];
	public Node? Right { get; set; }
	public Node? Left { get; set; }
	public Node? Parent { get; set; }

	public DiabeteAttribute? Attribute { get; set; }

	/// <summary>
	/// The average value of the attribute
	/// </summary>
	public double? SplitValue { get; set; }

	/// <summary>
	/// The result of the leaf node (0 or 1)
	/// </summary>
	public int? Outcome { get; set; }
}
