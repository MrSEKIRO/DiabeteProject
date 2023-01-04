// read diabets.csv 
using DiabeteProject;
using System.Runtime.CompilerServices;

var diabetesData = new List<DiabetesData>();
using(var reader = new StreamReader("diabetes.csv"))
{
	while(!reader.EndOfStream)
	{
		var line = reader.ReadLine();
		var values = line.Split(',');
		diabetesData.Add(new  DiabetesData()
		{
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

var tree = new Tree();
var isUsedAttribute = new bool[8];
for(int i = 0; i < 8; i++)
{
	if(isUsedAttribute[i] == false)
	{
		var entropiesByAttribute = tree.CalculateInformationGain(diabetesData, (DiabeteAttribute)i);
	}
}


