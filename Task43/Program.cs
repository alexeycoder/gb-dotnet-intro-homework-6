// Задача 43: Напишите программу, которая найдёт точку пересечения двух прямых,
// заданных уравнениями y = k1 * x + b1, y = k2 * x + b2;
// значения b1, k1, b2 и k2 задаются пользователем.
// b1 = 2, k1 = 5, b2 = 4, k2 = 9 -> (-0,5; -0,5)

const string NumbersFormat = "0.###";

do
{
	Console.Clear();
	PrintTitle("Определение координат точки пересечения двух прямых на плоскости,"
				+ " заданных параметрическим уравнением вида y = kx + b", ConsoleColor.Cyan);

	double[,] kb = new double[2, 2]; // [ # of line , k or b ]

	Console.WriteLine("\nВведите параметры первой прямой:");
	kb[0, 0] = GetUserInputDbl("k\u2081 = ");
	kb[0, 1] = GetUserInputDbl("b\u2081 = ");

	Console.WriteLine("\nВведите параметры второй прямой:");
	kb[1, 0] = GetUserInputDbl("k\u2082 = ");
	kb[1, 1] = GetUserInputDbl("b\u2082 = ");

	var linesInterrelation = EstimateLinesInterrelation(kb);

	Console.WriteLine();
	switch (linesInterrelation)
	{
		case LinesInterrelation.Coincide:
			PrintColored("Прямые совпадают!", ConsoleColor.Yellow);
			break;
		case LinesInterrelation.Parallel:
			PrintColored("Прямые параллельны!", ConsoleColor.Yellow);
			break;
		case LinesInterrelation.Intersect:
		default:
			(double x, double y) = GetIntersectionCoords(kb);
			PrintColored($"Точка пересечения прямых {ToReadableEquation(kb[0, 0], kb[0, 1])} и {ToReadableEquation(kb[1, 0], kb[1, 1])}", ConsoleColor.DarkGray);
			PrintColored($" -> ( {x.ToString(NumbersFormat)} , {y.ToString(NumbersFormat)} )", ConsoleColor.Yellow);
			break;
	}
	Console.WriteLine();

} while (AskForRepeat());

// Methods

static LinesInterrelation EstimateLinesInterrelation(double[,] kb)
{
	if (kb[0, 0] == kb[1, 0])
	{
		if (kb[0, 1] == kb[1, 1])
			return LinesInterrelation.Coincide;

		return LinesInterrelation.Parallel;
	}

	return LinesInterrelation.Intersect;
}

static string ToReadableEquation(double k, double b)
{
	string result = "y = ";

	if (k == 1)
		result += "x";
	else if (k == -1)
		result += "-x";
	else if (k != 0)
		result += k.ToString() + "\u22c5x";


	if (b != 0)
	{
		if (k == 0)
			result += b.ToString();
		else
			result += (b < 0 ? " - " : " + ") + Math.Abs(b).ToString();
	}
	else if (k == 0)
		result += "0";

	return result;
}

static (double x, double y) GetIntersectionCoords(double[,] kb)
{
	double dk = kb[0, 0] - kb[1, 0];
	if (Math.Abs(dk) <= double.Epsilon)
		return (double.PositiveInfinity, double.PositiveInfinity); // не должны попасть сюда при корректном использовании

	double x = (kb[1, 1] - kb[0, 1]) / dk;
	double y = (kb[0, 0] * kb[1, 1] - kb[1, 0] * kb[0, 1]) / dk;

	return (x, y);
}

static double GetUserInputDbl(string inputMessage, double minAllowed = double.MinValue, double maxAllowed = double.MaxValue)
{
	const string errorMessageWrongType = "Некорректный ввод! Пожалуйста повторите\n";
	string errorMessageOutOfRange = string.Empty;
	if (minAllowed != double.MinValue && maxAllowed != double.MaxValue)
		errorMessageOutOfRange = $"Число должно быть в интервале от {minAllowed} до {maxAllowed}! Пожалуйста повторите\n";
	else if (minAllowed != double.MinValue)
		errorMessageOutOfRange = $"Число не должно быть меньше {minAllowed}! Пожалуйста повторите\n";
	else
		errorMessageOutOfRange = $"Число не должно быть больше {maxAllowed}! Пожалуйста повторите\n";

	double input = 0;
	bool notANumber = false;
	bool outOfRange = false;
	do
	{
		if (notANumber)
		{
			PrintError(errorMessageWrongType, ConsoleColor.Magenta);
		}
		if (outOfRange)
		{
			PrintError(errorMessageOutOfRange, ConsoleColor.Magenta);
		}
		Console.Write(inputMessage);

		string? inputStr = Console.ReadLine();
		if (string.IsNullOrWhiteSpace(inputStr))
		{
			notANumber = true;
			continue;
		}
		notANumber = !double.TryParse(MakeInvariantToSeparator(inputStr), out input);
		outOfRange = !notANumber && (input < minAllowed || input > maxAllowed);

	} while (notANumber || outOfRange);

	return input;
}

static string MakeInvariantToSeparator(string input)
{
	char decimalSeparator = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
	char wrongSeparator = decimalSeparator.Equals('.') ? ',' : '.';
	return input.Trim().Replace(wrongSeparator, decimalSeparator);
}

static void PrintTitle(string title, ConsoleColor foreColor)
{
	int feasibleWidth = Math.Min(title.Length, Console.BufferWidth);
	string titleDelimiter = new string('\u2550', feasibleWidth);
	PrintColored(titleDelimiter + Environment.NewLine + title + Environment.NewLine + titleDelimiter, foreColor);
	Console.WriteLine();
}

static void PrintError(string errorMessage, ConsoleColor foreColor)
{
	PrintColored("\u2757 Ошибка: " + errorMessage, foreColor);
}

static void PrintColored(string message, ConsoleColor foreColor)
{
	var bkpColor = Console.ForegroundColor;
	Console.ForegroundColor = foreColor;
	Console.Write(message);
	Console.ForegroundColor = bkpColor;
}

static bool AskForRepeat()
{
	Console.WriteLine();
	Console.WriteLine("Нажмите Enter, чтобы повторить или Esc, чтобы завершить...");
	ConsoleKeyInfo key = Console.ReadKey(true);
	return key.Key != ConsoleKey.Escape;
}

enum LinesInterrelation
{
	Coincide,
	Parallel,
	Intersect
}
