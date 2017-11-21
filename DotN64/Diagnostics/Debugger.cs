using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DotN64.Diagnostics
{
    using CPU;
    using Extensions;

    public partial class Debugger
    {
        #region Fields
        private readonly Nintendo64 nintendo64;
        private readonly IReadOnlyCollection<Command> commands;
        private readonly IDictionary<ulong, string> labels = new Dictionary<ulong, string>();
        private readonly IList<ulong> breakpoints = new List<ulong>();
        #endregion

        #region Properties
        public Status DebuggerStatus { get; private set; }
        #endregion

        #region Constructors
        public Debugger(Nintendo64 nintendo64)
        {
            this.nintendo64 = nintendo64;
            commands = new[]
            {
                new Command(new[] { "continue", "c" }, "Continues execution of the CPU.", args => DebuggerStatus = Status.Running),
                new Command(new[] { "step", "s" }, "Steps the CPU a specified amount of times.", args =>
                {
                    var count = args.Length > 0 ? int.Parse(args.First()) : 1;

                    for (var i = 0; i < count; i++)
                    {
                        Disassemble();
                        nintendo64.CPU.Step();
                    }
                }),
                new Command(new[] { "goto", "g" }, "Sets the CPU's PC to the specified address.", args => nintendo64.CPU.PC = ulong.Parse(args.First(), NumberStyles.HexNumber)),
                new Command(new[] { "disassemble", "d" }, "Disassembles instructions from the current PC.", args =>
                {
                    var count = args.Length > 0 ? ulong.Parse(args.First()) : 1;

                    for (var i = 0ul; i < count; i++)
                    {
                        Disassemble(nintendo64.CPU.PC + i * VR4300.Instruction.Size);
                    }
                }),
                new Command(new[] { "label", "labels", "l" }, "Shows, adds or removes a label attached to an address.", args =>
                {
                    var index = 0;

                    switch (args.Length == 0 ? "show" : args[index++])
                    {
                        case "add":
                        case "a":
                            labels[args.Length <= index + 1 ? nintendo64.CPU.PC : ulong.Parse(args[index++], NumberStyles.HexNumber)] = args[index++];
                            break;
                        case "remove":
                        case "r":
                            labels.Remove(args.Length <= index ? nintendo64.CPU.PC : ulong.Parse(args[index++], NumberStyles.HexNumber));
                            break;
                        case "clear":
                        case "c":
                            labels.Clear();
                            break;
                        case "show":
                        case "s":
                            {
                                if (args.Length <= index)
                                {
                                    foreach (var pair in labels)
                                    {
                                        Console.WriteLine($".{pair.Value}: {pair.Key:X16}");
                                    }

                                    break;
                                }

                                var address = ulong.Parse(args[index++], NumberStyles.HexNumber);

                                Console.WriteLine($".{labels[address]}: {address:X16}");
                            }
                            break;
                    }
                }),
                new Command(new[] { "breakpoint", "breakpoints", "b" }, "Shows, adds or removes breakpoints.", args =>
                {
                    var index = 0;

                    switch (args.Length == 0 ? "show" : args[index++])
                    {
                        case "add":
                        case "a":
                            breakpoints.Add(args.Length <= index ? nintendo64.CPU.PC : ulong.Parse(args[index++], NumberStyles.HexNumber));
                            break;
                        case "remove":
                        case "r":
                            breakpoints.Remove(args.Length <= index ? nintendo64.CPU.PC : ulong.Parse(args[index++], NumberStyles.HexNumber));
                            break;
                        case "clear":
                        case "c":
                            breakpoints.Clear();
                            break;
                        case "show":
                        case "s":
                            foreach (var breakpoint in breakpoints)
                            {
                                Console.WriteLine($"● {breakpoint:X16}");
                            }
                            break;
                    }
                }),
                new Command(new[] { "exit", "quit", "q" }, "Exits the debugger.", args => DebuggerStatus = Status.Stopped),
                new Command(new[] { "help", "h" }, "Shows help for commands.", args =>
                {
                    if (args.Length == 0)
                    {
                        foreach (var command in commands)
                        {
                            Console.WriteLine($"- {string.Join(", ", command.Names)}: {command.Description}");
                        }
                    }
                    else
                    {
                        var commandName = args.First();
                        var command = commands.First(c => c.Names.Contains(commandName));

                        Console.WriteLine($"{string.Join(", ", command.Names)}: {command.Description}");
                    }
                }),
                new Command(new[] { "clear" }, "Clears the terminal.", args => Console.Clear())
            };
        }
        #endregion

        #region Methods
        private string Disassemble(VR4300.Instruction instruction)
        {
            switch (instruction.OP)
            {
                case VR4300.OpCode.SPECIAL:
                    return ((VR4300.SpecialOpCode)instruction.Funct).ToString();
                case VR4300.OpCode.REGIMM:
                    return ((VR4300.RegImmOpCode)instruction.RT).ToString();
                default:
                    return instruction.OP.ToString();
            }
        }

        private void Disassemble(ulong? address = null)
        {
            if (!address.HasValue)
                address = nintendo64.CPU.DelaySlot ?? nintendo64.CPU.PC;

            var physicalAddress = nintendo64.CPU.CP0.Map(address.Value);
            var instruction = nintendo64.MemoryMaps.GetEntry(physicalAddress).ReadWord(physicalAddress);

            if (labels.ContainsKey(address.Value))
            {
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine($".{labels[address.Value]}:");
                Console.ResetColor();
            }

            var hasBreakpoint = breakpoints.Contains(address.Value);

            if (hasBreakpoint)
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine($"{(hasBreakpoint ? '●' : ' ')} {address.Value:X16}: {instruction:X8} {Disassemble(instruction)}");

            if (hasBreakpoint)
                Console.ResetColor();
        }

        public void Debug()
        {
            DebuggerStatus = Status.Debugging;

            while (DebuggerStatus == Status.Debugging)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;

                Console.Write($"{nameof(DotN64)}-dbg▶️");
                Console.ResetColor();

                var input = Console.ReadLine().Trim();

                if (input.Length == 0)
                    continue;

                var arguments = input.Split();
                var commandName = arguments.First();
                Command command;

                try
                {
                    command = commands.First(c => c.Names.Contains(commandName));
                }
                catch (InvalidOperationException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("Command not found.");
                    Console.ResetColor();
                    continue;
                }

                try
                {
                    command.Action(arguments.Skip(1).ToArray());
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine($"Exception: {e.Message}");
                    Console.ResetColor();
                }
            }
        }

        public void Run(bool debug = false)
        {
            DebuggerStatus = debug ? Status.Debugging : Status.Running;

            while (DebuggerStatus != Status.Stopped)
            {
                switch (DebuggerStatus)
                {
                    case Status.Running:
                        if (breakpoints.Contains(nintendo64.CPU.PC))
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;

                            Console.WriteLine("Hit a breakpoint; entering debug prompt.");
                            Console.ResetColor();
                            Debug();
                        }

                        try
                        {
                            nintendo64.CPU.Step();
                        }
                        catch (Exception e)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;

                            Console.WriteLine($"Exception during CPU execution: {e.Message}");
                            Console.ResetColor();
                            Debug();
                        }
                        break;
                    case Status.Debugging:
                        Debug();
                        break;
                }
            }
        }
        #endregion
    }
}
