using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;

namespace DotN64.Diagnostics
{
    using CPU;

    public partial class Debugger
    {
        #region Fields
        private readonly Nintendo64 nintendo64;
        private readonly IReadOnlyCollection<Command> commands;
        private readonly IDictionary<ulong, string> labels = new Dictionary<ulong, string>();
        private readonly IList<ulong> breakpoints = new List<ulong>();
        private readonly IDictionary<uint, Func<VR4300.Instruction, VR4300, string>> operationFormats = new Dictionary<uint, Func<VR4300.Instruction, VR4300, string>>
        {
            [VR4300.Instruction.FromOpCode(VR4300.OpCode.ADDI)] = InstructionFormat.I,
            [VR4300.Instruction.FromOpCode(VR4300.OpCode.ADDIU)] = InstructionFormat.I,
            [VR4300.Instruction.FromOpCode(VR4300.OpCode.ANDI)] = InstructionFormat.I,
            [VR4300.Instruction.FromOpCode(VR4300.OpCode.BEQ)] = InstructionFormat.I,
            [VR4300.Instruction.FromOpCode(VR4300.OpCode.BEQL)] = InstructionFormat.I,
            [VR4300.Instruction.FromOpCode(VR4300.OpCode.BLEZL)] = InstructionFormat.I,
            [VR4300.Instruction.FromOpCode(VR4300.OpCode.BNE)] = InstructionFormat.I,
            [VR4300.Instruction.FromOpCode(VR4300.OpCode.BNEL)] = InstructionFormat.I,
            [VR4300.Instruction.FromOpCode(VR4300.OpCode.CACHE)] = null,
            [VR4300.Instruction.FromOpCode(VR4300.OpCode.JAL)] = InstructionFormat.J,
            [VR4300.Instruction.FromOpCode(VR4300.OpCode.LBU)] = InstructionFormat.I,
            [VR4300.Instruction.FromOpCode(VR4300.OpCode.LUI)] = InstructionFormat.I,
            [VR4300.Instruction.FromOpCode(VR4300.OpCode.LW)] = InstructionFormat.I,
            [VR4300.Instruction.FromOpCode(VR4300.OpCode.MTC0)] = InstructionFormat.R,
            [VR4300.Instruction.FromOpCode(VR4300.OpCode.ORI)] = InstructionFormat.I,
            [VR4300.Instruction.FromOpCode(VR4300.OpCode.SB)] = InstructionFormat.I,
            [VR4300.Instruction.FromOpCode(VR4300.OpCode.SLTI)] = InstructionFormat.I,
            [VR4300.Instruction.FromOpCode(VR4300.OpCode.SW)] = InstructionFormat.I,
            [VR4300.Instruction.FromOpCode(VR4300.OpCode.XORI)] = InstructionFormat.I,
            [VR4300.Instruction.FromOpCode(VR4300.SpecialOpCode.ADD)] = InstructionFormat.R,
            [VR4300.Instruction.FromOpCode(VR4300.SpecialOpCode.ADDU)] = InstructionFormat.R,
            [VR4300.Instruction.FromOpCode(VR4300.SpecialOpCode.AND)] = InstructionFormat.R,
            [VR4300.Instruction.FromOpCode(VR4300.SpecialOpCode.JR)] = InstructionFormat.R,
            [VR4300.Instruction.FromOpCode(VR4300.SpecialOpCode.MFHI)] = InstructionFormat.R,
            [VR4300.Instruction.FromOpCode(VR4300.SpecialOpCode.MFLO)] = InstructionFormat.R,
            [VR4300.Instruction.FromOpCode(VR4300.SpecialOpCode.MULTU)] = InstructionFormat.R,
            [VR4300.Instruction.FromOpCode(VR4300.SpecialOpCode.OR)] = InstructionFormat.R,
            [VR4300.Instruction.FromOpCode(VR4300.SpecialOpCode.SLL)] = InstructionFormat.R,
            [VR4300.Instruction.FromOpCode(VR4300.SpecialOpCode.SLLV)] = InstructionFormat.R,
            [VR4300.Instruction.FromOpCode(VR4300.SpecialOpCode.SLT)] = InstructionFormat.R,
            [VR4300.Instruction.FromOpCode(VR4300.SpecialOpCode.SLTU)] = InstructionFormat.R,
            [VR4300.Instruction.FromOpCode(VR4300.SpecialOpCode.SRL)] = InstructionFormat.R,
            [VR4300.Instruction.FromOpCode(VR4300.SpecialOpCode.SRLV)] = InstructionFormat.R,
            [VR4300.Instruction.FromOpCode(VR4300.SpecialOpCode.SUBU)] = InstructionFormat.R,
            [VR4300.Instruction.FromOpCode(VR4300.SpecialOpCode.XOR)] = InstructionFormat.R,
            [VR4300.Instruction.FromOpCode(VR4300.RegImmOpCode.BGEZAL)] = InstructionFormat.I,
            [VR4300.Instruction.FromOpCode(VR4300.RegImmOpCode.BGEZL)] = InstructionFormat.I
        };
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
                    var count = args.Length > 0 ? BigInteger.Parse(args.First()) : 1;

                    for (var i = BigInteger.Zero; i < count; i++)
                    {
                        Disassemble();
                        nintendo64.CPU.Cycle();
                    }
                }),
                new Command(new[] { "goto", "g" }, "Sets the CPU's PC to the specified address.", args => nintendo64.CPU.PC = ulong.Parse(args.First(), NumberStyles.HexNumber)),
                new Command(new[] { "disassemble", "d" }, "Disassembles instructions from the current PC.", args =>
                {
                    var count = args.Length > 0 ? BigInteger.Parse(args.First()) : 1;

                    for (var i = BigInteger.Zero; i < count; i++)
                    {
                        Disassemble(nintendo64.CPU.PC + (ulong)(i * VR4300.Instruction.Size), false);
                    }
                }),
                new Command(new[] { "label", "labels", "l" }, "Shows, adds or removes a label attached to an address.", args =>
                {
                    var index = 0;

                    switch (args.Length > 0 ? args[index++] : "show")
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

                    switch (args.Length > 0 ? args[index++] : "show")
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
                            Console.WriteLine("- " + command);
                        }
                    }
                    else
                    {
                        var commandName = args.First();
                        var command = commands.First(c => c.Names.Contains(commandName));

                        Console.WriteLine(command);
                    }
                }),
                new Command(new[] { "clear" }, "Clears the terminal.", args => Console.Clear())
            };
        }
        #endregion

        #region Methods
        private string Disassemble(VR4300.Instruction instruction, bool withRegisterContents) => operationFormats.TryGetValue(instruction.ToOpCode(), out var format) && format != null ? format(instruction, withRegisterContents ? nintendo64.CPU : null) : instruction.ToString();

        private void Disassemble(ulong? address = null, bool withRegisterContents = true)
        {
            if (!address.HasValue)
                address = nintendo64.CPU.DelaySlot ?? nintendo64.CPU.PC;

            var instruction = nintendo64.CPU.ReadSysAD(nintendo64.CPU.CP0.Translate(address.Value));

            if (labels.TryGetValue(address.Value, out var label))
            {
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine($".{label}:");
                Console.ResetColor();
            }

            var hasBreakpoint = breakpoints.Contains(address.Value);

            if (hasBreakpoint)
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine($"{(hasBreakpoint ? '●' : ' ')} {address.Value:X16}: {instruction:X8} {Disassemble(instruction, withRegisterContents)}");

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
                            nintendo64.CPU.Cycle();
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
