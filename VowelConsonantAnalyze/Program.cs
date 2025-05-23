using System;
using System.Linq; // Не используется в ядре анализатора, но может быть полезен

// Перечисление для состояний нашего автомата-контролёра
public enum AutomatonState
{
    Start,           // Начальное состояние
    ExpectConsonant, // Предыдущая буква была гласной, ожидаем согласную
    ExpectVowel,     // Предыдущая буква была согласной, ожидаем гласную
    Error            // Обнаружено нарушение правила
}

public class WordAnalyzer
{
    // Определяем, является ли символ гласной нашего алфавита
    private bool IsVowel(char ch)
    {
        return ch == 'a';
    }

    // Определяем, является ли символ согласной нашего алфавита
    private bool IsConsonant(char ch)
    {
        return ch == 'b' || ch == 'c' || ch == 'd';
    }

    // Проверяем, принадлежит ли символ нашему алфавиту {a, b, c, d}
    private bool IsInAlphabet(char ch)
    {
        return IsVowel(ch) || IsConsonant(ch);
    }

    // Главный метод, который анализирует слово
    public bool BelongsToLanguage(string word)
    {
        AutomatonState currentState = AutomatonState.Start;

        foreach (char c in word)
        {
            if (!IsInAlphabet(c))
            {
                return false;
            }

            switch (currentState)
            {
                case AutomatonState.Start:
                    if (IsVowel(c)) currentState = AutomatonState.ExpectConsonant;
                    else currentState = AutomatonState.ExpectVowel;
                    break;

                case AutomatonState.ExpectConsonant:
                    if (IsConsonant(c)) currentState = AutomatonState.ExpectVowel;
                    else currentState = AutomatonState.Error;
                    break;

                case AutomatonState.ExpectVowel:
                    if (IsVowel(c)) currentState = AutomatonState.ExpectConsonant;
                    else currentState = AutomatonState.Error;
                    break;

                case AutomatonState.Error:
                    break;
            }

            if (currentState == AutomatonState.Error)
            {
                break;
            }
        }
        return currentState != AutomatonState.Error;
    }
}

// Класс для демонстрации работы анализатора
public class Program
{
    public static void Main(string[] args)
    {
        WordAnalyzer analyzer = new WordAnalyzer();

        Console.WriteLine("Анализатор слов для языка №7: 'в которых гласные и согласные буквы перемежаются'");
        Console.WriteLine("Алфавит: {a, b, c, d}. Гласная: 'a'. Согласные: 'b', 'c', 'd'.");
        Console.WriteLine("Введите слово для анализа. Для выхода введите 'exit' или 'quit'.");

        string userInput;
        while (true) // Бесконечный цикл для ввода слов
        {
            Console.Write("\nВведите слово: ");
            userInput = Console.ReadLine();

            // Проверка на случай, если пользователь нажал Ctrl+Z (EOF) или ввод пустой
            if (userInput == null)
            {
                Console.WriteLine("Ввод завершен. Выход...");
                break;
            }

            // Приводим к нижнему регистру и убираем пробелы по краям для удобства
            // Наш анализатор ожидает символы 'a', 'b', 'c', 'd' в нижнем регистре
            string processedInput = userInput.Trim().ToLower();

            // Условие выхода из цикла
            if (processedInput == "exit" || processedInput == "quit")
            {
                Console.WriteLine("Выход из программы.");
                break;
            }

            // Анализируем введенное слово
            bool belongs = analyzer.BelongsToLanguage(processedInput);

            // Форматируем вывод для пустой строки
            string displayWord = string.IsNullOrEmpty(processedInput) ? "<пустое слово>" : $"\"{processedInput}\"";

            Console.WriteLine($"Слово {displayWord} : {(belongs ? "принадлежит" : "НЕ принадлежит")} языку.");
        }
    }
}