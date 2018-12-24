﻿using System;
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
        private readonly IDictionary<VR4300.Instruction, Func<VR4300.Instruction, VR4300, string>> operationFormats = new Dictionary<VR4300.Instruction, Func<VR4300.Instruction, VR4300, string>>
        {
            [VR4300.Instruction.From(VR4300.OpCode.ADDI)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.ADDIU)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.ANDI)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.BEQ)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.BEQL)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.BGTZ)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.BLEZL)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.BLEZ)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.BNE)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.BNEL)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.CACHE)] = null,
            [VR4300.Instruction.From(VR4300.OpCode.COP0)] = InstructionFormat.CP0,
            [VR4300.Instruction.From(VR4300.OpCode.JAL)] = InstructionFormat.J,
            [VR4300.Instruction.From(VR4300.OpCode.J)] = InstructionFormat.J,
            [VR4300.Instruction.From(VR4300.OpCode.LB)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.LBU)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.LUI)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.LD)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.LH)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.LHU)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.LW)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.ORI)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.SB)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.SD)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.SH)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.SLTI)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.SLTIU)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.SW)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.OpCode.XORI)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.ADD)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.ADDU)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.AND)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.DDIVU)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.DMULTU)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.DSLL32)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.DSRA32)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.JR)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.JALR)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.MFHI)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.MFLO)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.MTHI)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.MTLO)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.MULTU)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.OR)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.SLL)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.SLLV)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.SLT)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.SLTU)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.SRA)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.SRL)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.SRLV)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.SUBU)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.SpecialOpCode.XOR)] = InstructionFormat.R,
            [VR4300.Instruction.From(VR4300.RegImmOpCode.BGEZAL)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.RegImmOpCode.BGEZL)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.RegImmOpCode.BLTZ)] = InstructionFormat.I,
            [VR4300.Instruction.From(VR4300.RegImmOpCode.BGEZ)] = InstructionFormat.I
        };
        #endregion

        #region Properties
        private ulong Cursor => nintendo64.CPU.DelaySlot ?? nintendo64.CPU.PC;

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
                    var i = 0;
                    var mode = StepMode.In;

                    if (i < args.Length && Enum.GetNames(typeof(StepMode)).Any(n => n.ToLower() == args[i].ToLower()))
                        mode = (StepMode)Enum.Parse(typeof(StepMode), args[i++], true);

                    var count = i < args.Length ? BigInteger.Parse(args[i++]) : 1;

                    switch (mode)
                    {
                        case StepMode.In:
                            for (BigInteger j = 0; j < count; j++)
                            {
                                Disassemble();
                                nintendo64.CPU.Cycle();
                            }
                            break;
                        case StepMode.Out:
                            for (BigInteger j = 0; j < count; j++)
                            {
                                while (true)
                                {
                                    VR4300.Instruction instruction = nintendo64.CPU.ReadSysAD(nintendo64.CPU.CP0.Translate(Cursor));

                                    if (instruction.Special == VR4300.SpecialOpCode.JR && instruction.RS == (byte)VR4300.GPRIndex.RA)
                                        break;

                                    nintendo64.CPU.Cycle();
                                }

                                Disassemble();
                                nintendo64.CPU.Cycle();
                            }
                            break;
                        case StepMode.Over:
                            for (BigInteger j = 0; j < count; j++)
                            {
                                VR4300.Instruction instruction = nintendo64.CPU.ReadSysAD(nintendo64.CPU.CP0.Translate(Cursor));
                                var target = Cursor + VR4300.Instruction.Size * 2;

                                Disassemble();
                                nintendo64.CPU.Cycle();

                                if (instruction.OP == VR4300.OpCode.JAL || instruction.Special == VR4300.SpecialOpCode.JALR)
                                {
                                    while (Cursor != target)
                                    {
                                        nintendo64.CPU.Cycle();
                                    }
                                }
                            }
                            break;
                    }
                }),
                new Command(new[] { "goto", "g" }, "Sets the CPU's PC to the specified address or register value.", args =>
                {
                    var target = args.First();
                    const char RegisterPrefix = '$';

                    if (target[0] == RegisterPrefix)
                    {
                        target = target.Substring(1);

                        if (Enum.TryParse<VR4300.GPRIndex>(target, true, out var gprIndex))
                            nintendo64.CPU.PC = nintendo64.CPU.GPR[(int)gprIndex];
                        else if (Enum.TryParse<VR4300.SystemControlUnit.RegisterIndex>(target, true, out var cp0Index))
                            nintendo64.CPU.PC = nintendo64.CPU.CP0.Registers[(int)cp0Index];
                    }
                    else
                        nintendo64.CPU.PC = ulong.Parse(target, NumberStyles.HexNumber);
                }),
                new Command(new[] { "disassemble", "disasm", "d" }, "Disassembles instructions from the current PC.", args =>
                {
                    var count = args.Length > 0 ? BigInteger.Parse(args.First()) : 1;

                    for (var i = BigInteger.Zero; i < count; i++)
                    {
                        Disassemble(Cursor + (ulong)(i * VR4300.Instruction.Size), false);
                    }
                }),
                new Command(new[] { "label", "labels", "l" }, "Shows, adds or removes a label attached to an address.", args =>
                {
                    var index = 0;

                    switch (args.Length > 0 ? args[index++] : "show")
                    {
                        case "add":
                        case "a":
                            var label = args[index++];
                            labels[args.Length <= index ? Cursor : ulong.Parse(args[index++], NumberStyles.HexNumber)] = label;
                            break;
                        case "remove":
                        case "r":
                            labels.Remove(args.Length <= index ? Cursor : ulong.Parse(args[index++], NumberStyles.HexNumber));
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
                                        Console.WriteLine($"{pair.Value}: {pair.Key:X16}");
                                    }
                                    break;
                                }

                                var address = ulong.Parse(args[index++], NumberStyles.HexNumber);

                                Console.WriteLine($"{labels[address]}: {address:X16}");
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
                            breakpoints.Add(args.Length <= index ? Cursor : ulong.Parse(args[index++], NumberStyles.HexNumber));
                            break;
                        case "remove":
                        case "r":
                            breakpoints.Remove(args.Length <= index ? Cursor : ulong.Parse(args[index++], NumberStyles.HexNumber));
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
        private string Disassemble(VR4300.Instruction instruction, bool withRegisterContents) => operationFormats.TryGetValue(instruction.ToOpCode(), out var format) && format != null ? format(instruction, withRegisterContents ? nintendo64.CPU : null) : InstructionFormat.Unknown(instruction);

        private void Disassemble(ulong? address = null, bool withRegisterContents = true)
        {
            if (!address.HasValue)
                address = Cursor;

            var instruction = nintendo64.CPU.ReadSysAD(nintendo64.CPU.CP0.Translate(address.Value));

            if (labels.TryGetValue(address.Value, out var label))
            {
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine($"{label}:");
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

            while (DebuggerStatus != Status.Stopped && nintendo64.Power == Switch.On)
            {
                switch (DebuggerStatus)
                {
                    case Status.Running:
                        if (breakpoints.Contains(Cursor))
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;

                            Console.WriteLine("Hit a breakpoint; entering debug prompt.");
                            Console.ResetColor();
                            Debug();

                            if (DebuggerStatus != Status.Running) // Prevents running the instruction if we chose to quit (e.g. covers the case where the next one causes an exception, dropping us right back into the debugger).
                                break;
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

        #region Enumerations
        private enum StepMode : byte
        {
            In,
            Out,
            Over
        }
        #endregion
    }
}
