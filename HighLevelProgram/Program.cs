﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotPrototypeWpf
{
    /// <summary>
    /// High-level program
    /// </summary>
    public class HLProgram
    {
        private List<Instruction> _program;

        public const string driveExample =
@"FORWARD 10,3";

        public const string driveExampleSerial =
@"137 0 10 127 255
137 0 0 127 255";

        public HLProgram()
        {
            _program = new List<Instruction>();
        }

        public HLProgram(String programString)
        {
            _program = new List<Instruction>();

            string[] insList = programString.Split(new char[] { '\n','\r' },StringSplitOptions.RemoveEmptyEntries);
            foreach (string insStr in insList)
            {
                _program.Add(new Instruction(insStr));
            }
        }

        public List<Instruction> GetInstructionList()
        {
            return _program;
        }

        public void Add(Instruction ins)
        {
            _program.Add(ins);
        }

        public void Add(String ins)
        {
            _program.Add(new Instruction(ins));
        }

        public override string ToString()
        {            
            return string.Join("\n", _program);
        }
    }
}
