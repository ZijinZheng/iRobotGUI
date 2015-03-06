﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRobotGUI
{
	public class Instruction
	{
		public string opcode;
		public List<int> parameters;
		public string _string;

		#region OpCode
		public const string FORWARD = "FORWARD";
		public const string BACKWARD = "BACKWARD";
		public const string LEFT = "LEFT";
		public const string RIGHT = "RIGHT";
		public const string DRIVE = "DRIVE";
		public const string LED = "LED";
		public const string DEMO = "DEMO";
		public const string SONG_DEF = "SONG_DEF";
		public const string SONG_PLAY = "SONG_PLAY";        
		public const string IF = "IF";
		public const string ELSE = "ELSE";
		public const string END_IF = "END_IF";
		public const string LOOP = "LOOP";
		public const string END_LOOP = "END_LOOP";
		public const string DELAY = "DELAY";
		#endregion

		#region Operator
		public const byte NOT_EQUAL = 0;
		public const byte EQUAL = 1;
		public const byte GREATER_THAN = 2;
		public const byte GRAETER_THAN_OR_EQUAL = 3;
		public const byte LESS_THAN = 4;
		public const byte LESS_THAN_OR_EQUAL = 5;
		#endregion


		// For MVVM
		public string InsString
		{
			get
			{
				return ToString();
			}
		}



		public readonly string[] OpCodeSet = new string[] 
		{ 
			FORWARD,
			BACKWARD, 
			LEFT, 
			RIGHT, 
			DRIVE,
			LED, 
			DEMO,
			SONG_DEF,             
			SONG_PLAY, 
			IF, 
			ELSE,
			END_IF,
			LOOP,
			END_LOOP, 
			DELAY,
		};

		


		public Instruction(string opcode, string[] parameters)
		{
			setFields(opcode, parameters);
		}

		public Instruction(string instructionString)
		{
			string[] insSplitted = instructionString.Split(new char[] { ' ' });
			if (insSplitted.Count() == 1)
			{
				setFields(insSplitted[0]);
			}
			else
			{
				setFields(insSplitted[0], insSplitted[1].Split(new char[] { ',' }));
			}
			
		}

		/// <summary>
		/// A factory to create new Instruction object by opcode
		/// </summary>
		/// <returns></returns>
		public static Instruction CreatFromOpcode(string opcode)
		{
			Instruction newIns = null;
			switch (opcode)
			{
				case FORWARD:
					newIns = new Instruction(Instruction.FORWARD + " 500,3");
					break;
				case Instruction.LEFT:
					newIns = new Instruction(Instruction.LEFT + " 90");
					break;
				case Instruction.LED:
					newIns = new Instruction(Instruction.LED + " 10,128,128");
					break;
				case Instruction.SONG_DEF:
					newIns = new Instruction(Instruction.SONG_DEF + " 0");
					break;
				case Instruction.DEMO:
					newIns = new Instruction(Instruction.DEMO + " 0");
					break;
				case IF:
					newIns = new Instruction(IF);
					break;
				case LOOP:
					newIns = new Instruction(LOOP);
					break;
			}
			return newIns;
		}

		private void setFields(string opcode)
		{
			if (OpCodeSet.Contains(opcode))
			{
				// opcode
				this.opcode = opcode;
			}
			else
			{
				throw new InvalidOpcodeException();
			}
		}

		private void setFields(string opcode, string[] parameters)
		{
			if (OpCodeSet.Contains(opcode))
			{
				// opcode
				this.opcode = opcode;
			}
			else
			{
				throw new InvalidOpcodeException();
			}
			// parameters
			this.parameters = new List<int>();
			foreach (string para in parameters)
			{
				this.parameters.Add(Convert.ToInt32(para));
			}


		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(opcode);
			if (parameters != null)
				sb.Append(" ").Append(string.Join(",", parameters));
			return sb.ToString();
		}

		public static string GetOperatorSymbol(byte opeartorName)
		{
			switch (opeartorName)
			{
				case NOT_EQUAL:
					return "!=";
				case EQUAL:
					return "==";
				case GREATER_THAN:
					return ">";
				case GRAETER_THAN_OR_EQUAL:
					return ">=";
				case LESS_THAN:
					return "<";
				case LESS_THAN_OR_EQUAL:
					return "<=";
				default:
					return "";
			}
		}

		

	}
}
