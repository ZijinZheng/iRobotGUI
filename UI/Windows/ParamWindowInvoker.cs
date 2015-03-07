﻿using iRobotGUI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace iRobotGUI.Controls
{
	/// <summary>
	/// This is a helper class which takes the instruction and invoke the window for it.
	/// </summary>
	public class DialogInvoker
	{
		public static void ShowDialog(Instruction ins, Window owner)
		{
			BaseParamWindow dlg = null; 			

			switch (ins.opcode)
			{
				case Instruction.FORWARD:
					
					break;
				case Instruction.LEFT:
					break;
				case Instruction.LED:
					dlg = new LedWindow();
					break;
				case Instruction.SONG_DEF:
					dlg = new SongWindow();
					break;
				case Instruction.DEMO:
					dlg = new DemoWindow();					
					break;
			}          

			if (dlg != null)
			{
				dlg.Owner = owner;
				dlg.Ins = ins;
				dlg.ShowDialog();
			}
			else 
			{
				MessageBox.Show(ins.opcode + " no implemented.");
			}	
			
		}
	}
}
