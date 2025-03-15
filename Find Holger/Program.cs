using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Microsoft.VisualBasic.FileIO;
List<char> CreateList(int width, int hight)
{
    Random tilf = new Random();
    int len = width * hight;
    List<char> list = new List<char>();
    for (int i = 0; i < len; i++)
    {
        list.Add((char)tilf.Next('A', 'z'));
    }
    //jeg kan ikke helt gennemskue hvorfor, men linjen nedenfor
    //krakker nogen gange med en 'index out of range' error
    //når jeg køre programmet for første gang efter jeg har haft VS lukket
    //måske bliver Random ikke initialiseret ordentligt?
    //starter jeg måske programmet for hurtigt før VS er klar?
    //ellers er der måske et problem med width og hight som gør at listen er 0 lang?
    //det plejer at virke igen når jeg lukker consol og starter programmet igen.
    int ran = tilf.Next(0, len);
    list[ran] = '@';
    return list;
}
void PassOver(List<char> liste)
{
    liste.Insert(0, liste.Last());
    liste.RemoveAt(liste.Count() - 1);
}
int[] GetCent(int width, List<char> list)
{

    int index = list.IndexOf('@');
    int[] nums = new int[2];
    nums[0] = index / width;
    nums[1] = index % width;
    if (nums[0] % 2 == 0)
    {
        nums[1] = width - 1 - nums[1];
    }
    return nums;

}
void PrintTop(int width)
{
    Console.ResetColor();
    Console.Write("    ");
    for (int t = 1; t <= width; t++)
    {
        Console.Write(t.ToString().PadLeft(3));
    }
    Console.WriteLine();
    Console.Write("    ");
    for (int t = 1; t <= width; t++)
    {
        Console.Write("___");
    }
    Console.WriteLine();
}
void PrintOptions(int width)
{
    string[] options = {"[ESC] Exit",
                        "[Q] Toggle Movement",
                        "[W] Toggle Color",
                        "[E] Toggle @",
                        "[D] Toggle Color Direction",
                        "[R] Refresh",
                        "[A] Randomize",
                        "[S] Resize",
                        "[+],[-] Toggle Speed",
                        "[SPACE] Toggle Pause"};
    int temp = width * 3;
    Console.ResetColor();
    if (width * 3 > options[5].Length + 1)
    {
        for (int i = 0; i < options.Length; i += 2)
        {
            Console.WriteLine($"{options[i]}{new String(' ', width * 3 + 4 - options[i].Length - options[i + 1].Length)}{options[i + 1]}");
        }
    }
    else
        foreach (string s in options)
        {
            Console.WriteLine(s.PadLeft((width * 3 + 4) / 2));
        }
}
void PrintLine(List<char> liste, int width, bool color, int ci, int[] nums, bool findA, ConsoleColor[] farver)
{
    int k = 1;
    for (int i = 0; i < liste.Count(); k++)
    {
        char[] linje = liste.Skip(i).Take(width).ToArray();
        Console.ResetColor();
        Console.Write(k.ToString().PadLeft(3));
        Console.Write("|");
        if ((i / width) % 2 == 0)
        {
            Array.Reverse(linje);
            foreach (char ca in linje)
            {
                PrintChars(ca, i, width, color, ci, farver, nums[1], nums[0], findA);
                i++;
            }
        }
        else
        {
            foreach (char ca in linje)
            {
                PrintChars(ca, i, width, color, ci, farver, nums[1], nums[0], findA);
                i++;
            }
        }
        Console.WriteLine();
    }
}
void PrintArray(List<char> liste, int width, int ci, int speed = 300, bool color = false, bool findA = false)
{
    int[] nums = GetCent(width, liste);
    PrintTop(width);
    ConsoleColor[] farver = {ConsoleColor.Blue,
                            ConsoleColor.Green,
                            ConsoleColor.Cyan,
                            ConsoleColor.Red,
                            ConsoleColor.Magenta,
                            ConsoleColor.Yellow,
                            ConsoleColor.White};
    PrintLine(liste, width, color, ci, nums, findA, farver);
    PrintOptions(width);
    Console.SetCursorPosition(0, 0);
    Thread.Sleep(speed);
}
void PrintChars(char ca, int i, int width, bool color, int ci, ConsoleColor[] farver, int centkol, int centrek, bool findA)
{
    int rek = i / width;
    int kol = i % width;
    if (color == true)
    {
        Console.ForegroundColor = farver[Math.Abs(i + ci) % farver.Length];
    }
    else if (findA == true)
    {
        int afstand = Math.Abs(rek - centrek) + Math.Abs(kol - centkol);
        Console.ForegroundColor = farver[Math.Abs(afstand + ci) % farver.Length];
    }
    Console.Write(ca.ToString().PadLeft(3));
}
void PrettyPrint(string text)
{
    foreach (char ca in text)
    {
        Console.Write(ca);
        Thread.Sleep(30);
    }
    Console.WriteLine();
}
int GetMes(string text)
{
    string temp = "";
    while (!(int.TryParse(temp, out _)))
    {
        PrettyPrint(text);
        temp = Console.ReadLine();
        Console.Clear();
    }
    if (int.Parse(temp) == 0)
    {
        GetMes(text);
    }

    return Convert.ToInt32(temp);
}















//PROGRAM - Det virker meget godt på en 20x20 matrix og virker også relativt fint på en 40x40.
//Det er primært matematikken der tager tid men jeg kender ik' til
//nogen form for vektorisering af lister eller lignende i c# indu så jeg ved ik' helt hvordan jeg skal løse det.
//Givet, der er sikkert en del der kan optimeres i koden uden at tale om vektorisering af noget.
//Men lordet virker, så jeg er --> (: 
Console.CursorVisible = false;
int hight = GetMes("Indtast højte");
int width = GetMes("Indtast bredde");
//En bredde på 600 og højde på 450 var nær grænsen for min skærm når helt zoomet ud i consol (dog pænt når man toggler @)
List<char> list = CreateList(width, hight);
int fart = 0;
int ca = 0;
bool farve = false;
bool finA = false;
bool pas = false;
//hvis den begynder at udskrive udenfor linjerne eller ovenpå andet tekst i consol
//så prøv lige at zoome ud og 'Refresh' med 'R' 
while (true)
{
    PrintArray(list, width, ca, speed: fart, color: farve, findA: finA);
    if (pas == true)
    {
        PassOver(list);
    }
    if (Console.KeyAvailable)
    {
        ConsoleKeyInfo key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.OemPlus || key.Key == ConsoleKey.Add)
        {
            if (fart > 0) fart -= 50;
        }
        else if (key.Key == ConsoleKey.OemMinus || key.Key == ConsoleKey.Subtract)
        {
            fart += 50;
        }
        else if (key.Key == ConsoleKey.W)
        {
            finA = false;
            farve = !(farve);
        }
        else if (key.Key == ConsoleKey.E)
        {
            farve = false;
            finA = !(finA);
        }
        else if (key.Key == ConsoleKey.D)
        {
            //virker også med toggle @ [E]
            ca = -ca;
        }
        else if (key.Key == ConsoleKey.Q)
        {
            pas = !(pas);
        }
        else if (key.Key == ConsoleKey.R)
        {
            Console.Clear();
        }
        else if (key.Key == ConsoleKey.S)
        {
            Console.Clear();
            hight = GetMes("Indtast højte");
            width = GetMes("Indtast bredde");
            list = CreateList(width, hight);
        }
        else if (key.Key == ConsoleKey.A)
        {
            list = CreateList(width, hight);
        }
        else if (key.Key == ConsoleKey.Spacebar)
        {
            Console.ReadKey();
            Console.Clear();
        }
        else if (key.Key == ConsoleKey.Escape)
        {
            Console.Clear();
            break;
        }

    }
    if (ca < 0)
    {
        ca--;
    }
    else
    {
        ca++;
    }
}