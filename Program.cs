// read diabets.csv 
using DiabeteProject;

var diabetesData = new List<DiabetesData>();
int id = 0;
using(var reader = new StreamReader("diabetes.csv"))
{
	// read data without header
	reader.ReadLine();
	while(!reader.EndOfStream)
	{
		var line = reader.ReadLine();
		var values = line.Split(',');
		diabetesData.Add(new DiabetesData()
		{
			Id = id++,
			Pregnancies = int.Parse(values[0]),
			Glucose = int.Parse(values[1]),
			BloodPressure = int.Parse(values[2]),
			SkinThickness = int.Parse(values[3]),
			Insulin = int.Parse(values[4]),
			BMI = double.Parse(values[5]),
			DiabetesPedigreeFunction = double.Parse(values[6]),
			Age = int.Parse(values[7]),
			Outcome = int.Parse(values[8])
		});
	}
}

const double TrainRate = 0.8;
var trainData = diabetesData.Take((int)(diabetesData.Count * TrainRate)).ToList();
var testData = diabetesData.Skip((int)(diabetesData.Count * TrainRate)).ToList();

var tree = new Tree();
var node = new Node() // can be root or child
{
	Data = trainData,
	IsUsedAttribute =new bool[8],
}; 
//var isUsedAttribute = new bool[8];

BuildTree(tree, node);

//tree.PrintTree(node, 10);
tree.PrintPretty(node, "", false);

var correct = 0;
foreach(var data in testData)
{
	var result = Predict(node, data);
	Console.WriteLine($"Result for {data.Id} is {result} and actual is {data.Outcome}");
	if(result == data.Outcome)
	{
		correct++;
	}
}
Console.WriteLine("===================");
Console.WriteLine($"Number of correct predicts : {correct} out of {testData.Count}");
Console.WriteLine("Accuracy: " + (double)correct / testData.Count);


int? Predict(Node node, DiabetesData data)
{
	while(node.Outcome == null)
	{
		var value = double.Parse(data.GetType().GetProperty(node.Attribute.ToString()).GetValue(data).ToString());
		if(value < node.SplitValue && node.Left != null)
		{
			node = node.Left;
		}
		else if(node.Right != null)
		{
			node = node.Right;
		}
	}
	
	return node.Outcome;
}

void BuildTree(Tree tree, Node node)
{
	// no attribute left
	if(node.IsUsedAttribute.All(x => x == true))
	{
		var posCount = node.Data.Count(x => x.Outcome == 1);
		if(posCount > node.Data.Count / 2)
		{
			node.Outcome = 1;
		}
		else
		{
			node.Outcome = 0;
		}
		return;
	}

	// entropy 0
	var entropy = tree.CalculateEntropy(node.Data);
	if(entropy == 0)
	{
		node.Outcome = diabetesData[0].Outcome;
		return;
	}

	var bestAttribute = tree.GetBestAttribute(node.Data, node.IsUsedAttribute);

	// TODO: get average of current or all???
	var average = node.Data.Average(x => double.Parse(x.GetType().GetProperty(bestAttribute.ToString()).GetValue(x).ToString()));
	
	var left = node.Data.Where(x => double.Parse(x.GetType().GetProperty(bestAttribute.ToString()).GetValue(x).ToString()) < average).ToList();
	var right = node.Data.Where(x => double.Parse(x.GetType().GetProperty(bestAttribute.ToString()).GetValue(x).ToString()) >= average).ToList();

	node.IsUsedAttribute[(int)bestAttribute] = true;
	node.Attribute = bestAttribute;
	node.SplitValue = average;
	//node.Average = average;

	var leftIsUsedAttribute = CreateDeepCopy(node.IsUsedAttribute);
	var rightIsUsedAttribute = CreateDeepCopy(node.IsUsedAttribute);

	if(left.Count > 0)
	{
		node.Left = new Node()
		{
			Data = left,
			IsUsedAttribute = leftIsUsedAttribute,
		};
	}
	if(right.Count > 0)
	{
		node.Right = new Node()
		{
			Data = right,
			IsUsedAttribute = rightIsUsedAttribute,
		};
	}

	if(node.Left != null)
	{
		BuildTree(tree, node.Left);
	}
	if(node.Right != null)
	{
		BuildTree(tree, node.Right);
	}
}

bool[] CreateDeepCopy(bool[] isUsedAttribute)
{
	var result = new bool[8];
	for(int i = 0; i < isUsedAttribute.Length; i++)
	{
		result[i] = isUsedAttribute[i];
	}
	return result;
}