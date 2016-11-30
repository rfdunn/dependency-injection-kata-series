using System;
using Autofac;
using DependencyInjection.Console.CharacterWriters;
using DependencyInjection.Console.SquarePainters;
using NDesk.Options;

namespace DependencyInjection.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var useColors = false;
            var width = 25;
            var height = 15;
            var pattern = "circle";

            var optionSet = new OptionSet
            {
                {"c|colors", value => useColors = value != null},
                {"w|width=", value => width = int.Parse(value)},
                {"h|height=", value => height = int.Parse(value)},
                {"p|pattern=", value => pattern = value}
            };
            optionSet.Parse(args);

            var builder = new ContainerBuilder();
            builder.Register(c => new AsciiWriter());
            builder.Register(c => useColors ? (ICharacterWriter)new ColorWriter(c.Resolve<AsciiWriter>()) : (c.Resolve<AsciiWriter>()));
            builder.Register(c => new PatternWriter(c.Resolve<ICharacterWriter>()));

            var container = builder.Build();

            var patternWriter = container.Resolve<PatternWriter>();


            var squarePainter = GetSquarePainter(pattern);
            var patternGenerator = new PatternGenerator(squarePainter);

            var app = new PatternApp(patternWriter, patternGenerator);
            app.Run(width, height);
        }

        private static ISquarePainter GetSquarePainter(string pattern)
        {
            switch (pattern)
            {
                case "circle":
                    return new CircleSquarePainter();
                case "oddeven":
                    return new OddEvenSquarePainter();
                case "white":
                    return new WhiteSquarePainter();
                default:
                    throw new ArgumentException($"Pattern '{pattern}' not found!");
            }
        }
    }
}
