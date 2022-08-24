// Задача 41: Пользователь вводит с клавиатуры M чисел. Посчитайте, сколько чисел больше 0 ввёл пользователь.
// 0, 7, 8, -2, -2 -> 2
// 1, -7, 567, 89, 223-> 3

const string NumbersFormat = "G";

do
{
	Console.Clear();
	PrintTitle("Определение количества положительных чисел среди введённых", ConsoleColor.Cyan);

	double[] nums = GetUserInputArray("Введите несколько чисел в одной строке, разделённых пробелом:\n");
	int numOfPos = CountPositiveItems(nums);
	string answer = numOfPos > 0 ?
					$" -> {ToReadableNotion(numOfPos)} из {nums.Length} больше нуля"
					:
					" -> нет положительных чисел";

	Console.WriteLine();
	PrintArrayDbl(nums, ConsoleColor.DarkGray);
	PrintColored(answer, ConsoleColor.Yellow);
	Console.WriteLine();

} while (AskForRepeat());

// Methods

static int CountPositiveItems(double[] numbers)
{
	int count = 0;
	for (int i = 0; i < numbers.Length; ++i)
	{
		if (numbers[i] > 0)
			++count;
	}
	return count;
}

static string ToReadableNotion(int number)
{
	string notion = " чисел";
	if (number <= 4 || number >= 21)
	{
		int lowDigit = number % 10;
		if (lowDigit == 1)
			notion = " число";
		else if (lowDigit >= 2 && lowDigit <= 4)
			notion = " числа";
	}

	return number.ToString() + notion;
}

static double[] GetUserInputArray(string prompt)
{
	char[] itemDelimiters = { ' ', '\t', ';' };
	// There can be 2 types of error.
	var NoNaNError = new { NeedHandle = false, Inputs = new string[] { }, Outputs = new double[] { } };
	var errNotANumber = NoNaNError; // complex
	bool errEmpty = false; // simple

	do
	{
		if (errEmpty)
		{
			PrintError("Вы ничего не ввели! Пожалуйста, введите запрашиваемые данные...\n", ConsoleColor.Red);
			errEmpty = false;
		}
		if (errNotANumber.NeedHandle)
		{
			PrintError("Некорректный ввод ", ConsoleColor.Red);
			PrintArrayDbl(errNotANumber.Outputs, ConsoleColor.DarkGray, errNotANumber.Inputs, ConsoleColor.DarkRed);
			PrintColored("\nПожалуйста, попробуйте ещё раз...\n", ConsoleColor.Red);
			errNotANumber = NoNaNError;
		}

		Console.Write(prompt);
		string? inputStr = Console.ReadLine();

		if (string.IsNullOrWhiteSpace(inputStr))
		{
			errEmpty = true;
			continue;
		}

		inputStr = MakeInvariantToSeparator(inputStr.Trim());
		var inputItems = inputStr.Split(itemDelimiters, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (inputItems.Length <= 0)
		{
			errEmpty = true;
			continue;
		}

		if (!TryParseArray(inputItems, out double[] numbers))
		{
			errNotANumber = new { NeedHandle = true, Inputs = inputItems, Outputs = numbers };
			continue;
		}

		return numbers;

	} while (errEmpty || errNotANumber.NeedHandle);

	return new double[] { }; // must never get in here!
}

static bool TryParseArray(string[] strNumbers, out double[] numbers)
{
	numbers = new double[strNumbers.Length];
	bool anyWrong = false;

	for (int i = 0; i < strNumbers.Length; ++i)
	{
		string strItem = strNumbers[i];
		if (double.TryParse(strItem, out double dblItem))
		{
			numbers[i] = dblItem;
		}
		else
		{
			numbers[i] = double.NaN;
			anyWrong = true;
		}
	}

	return !anyWrong;
}

static string MakeInvariantToSeparator(string input)
{
	char decimalSeparator = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
	char wrongSeparator = decimalSeparator.Equals('.') ? ',' : '.';
	return input.Trim().Replace(wrongSeparator, decimalSeparator);
}

static void PrintArrayDbl(double[] array, ConsoleColor? foreColor = null, string[]? inputs = null, ConsoleColor? highlightError = null)
{
	if (array.Length <= 0)
	{
		Console.WriteLine("Массив пуст.");
		return;
	}

	var bkpForeColor = Console.ForegroundColor;
	var bkpBackColor = Console.BackgroundColor;
	if (foreColor.HasValue)
		Console.ForegroundColor = foreColor.Value;
	else
		foreColor = bkpForeColor;

	Console.Write("[");
	int lastIndex = array.Length - 1;
	for (int i = 0; i <= lastIndex; i++)
	{
		double itemValue = array[i];

		if (!double.IsNaN(itemValue))
		{
			Console.Write(itemValue.ToString(NumbersFormat));
		}
		else if (inputs != null)
		{
			string itemFromUserInput = inputs[i];
			// highlight erroneous item
			if (highlightError.HasValue)
			{
				var backColor = highlightError.Value;
				var oppositeForeColor = backColor <= ConsoleColor.DarkGray ? ConsoleColor.White : ConsoleColor.Black;
				Console.BackgroundColor = backColor;
				Console.ForegroundColor = oppositeForeColor;
				Console.Write(itemFromUserInput);
				Console.BackgroundColor = bkpBackColor;
				Console.ForegroundColor = foreColor.Value;
			}
			else
			{
				Console.Write(itemFromUserInput);
			}
		}

		Console.Write(i == lastIndex ? "]" : "  ");
	}

	Console.ForegroundColor = bkpForeColor;
	Console.BackgroundColor = bkpBackColor;
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
