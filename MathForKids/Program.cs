using System;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CommandLineApplication app = new CommandLineApplication();
            app.HelpOption("--help|-h|-?");
            app.VersionOption("--version|-v", "1.0.0"); //为了简化起见，直接返回静态的 1.0.0

            CommandArgument argOperator = app.Argument("operator", "算式类型，有效值：加、减、乘、除，可以设置多个类型", multipleValues: true);

            CommandOption optMin = app.Option("--minValue -min <value>", "最小值，默认为0", CommandOptionType.SingleValue);
            CommandOption optMax = app.Option("--maxValue -max <value>", "最大值，默认为100", CommandOptionType.SingleValue);
            CommandOption optCount = app.Option("--count -c <value>", "生成的算式数量，默认为10", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                return OnAppExecute(argOperator, optMin, optMax, optCount);
            });

            app.Execute(args);
        }

        private static int OnAppExecute(CommandArgument argOperator, CommandOption optMin, CommandOption optMax, CommandOption optCount)
        {
            if (argOperator.Values.Count == 0)
            {
                Console.Error.WriteLine("未指定算数类型。");
                return -1;
            }

            // 简单起见，我们假设 argOperator 始终有正确的值，因此忽略参数检测
            string[] operators = argOperator.Values.Select(o =>
            {
                switch (o)
                {
                    case "减":
                        return "-";
                    case "乘":
                        return "×";
                    case "除":
                        return "÷";
                    case "加":
                    default:
                        return "+";
                }
            }).ToArray();

            int minValue, maxValue, count;

            if (optMin.HasValue())
            {
                if (!int.TryParse(optMin.Value(), out minValue))
                {
                    Console.Error.WriteLine($"invalid minValue: {optMin.Value()}");
                    return -2;
                }
            }
            else
            {
                minValue = 0;
            }

            if (optMax.HasValue())
            {
                if (!int.TryParse(optMax.Value(), out maxValue))
                {
                    Console.Error.WriteLine($"invalid maxValue: {optMax.Value()}");
                    return -3;
                }
            }
            else
            {
                maxValue = 100;
            }

            if (optCount.HasValue())
            {
                if (!int.TryParse(optCount.Value(), out count))
                {
                    Console.Error.WriteLine($"invalid count: {optCount.Value()}");
                    return -4;
                }
            }
            else
            {
                count = 10;
            }

            Random rndForOp = new Random();
            Random rnd = new Random();
            int rndMaxValue = maxValue + 1; // 因为 Random 类的 Next 方法返回的值是大于等于最小值，小于最大值，因此我们需要 maxValue + 1，确保 maxValue 是可能的返回结果之一
            int valueWidth = maxValue.ToString().Length;
            for (int i = 1; i <= count; i++)
            {
                Console.Write("{0} {1} {2} = "
                    , rnd.Next(minValue, rndMaxValue).ToString().PadLeft(valueWidth)
                    , operators[rndForOp.Next(0, operators.Length)]
                    , rnd.Next(minValue, rndMaxValue).ToString().PadLeft(valueWidth));

                if (i % 3 == 0)
                {
                    Console.WriteLine();
                }
                else
                {
                    Console.Write("\t");
                }
            }

            Console.WriteLine();
            return 0;
        }
    }
}
