﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using iRobotGUI.WinAvr;
using System.Diagnostics;

namespace iRobotGUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static RoutedCommand ComPortCmd = new RoutedUICommand("Load Configuration", "comn", typeof(Window));
		public static RoutedCommand OpenSourceCmd = new RoutedUICommand("Open Source File", "srcfile", typeof(Window));

		private string fileName;

		private HLProgram program;

		public MainWindow()
		{
			// Init Cmd
			ComPortCmd.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control | ModifierKeys.Shift, "Ctrl+Shift+C"));

			InitializeComponent();

			// Set the current folder to cprogram
			Directory.SetCurrentDirectory(@".");

			program = new HLProgram();
			programList1.Program = program;

			textBlockStatus.Text = "new file";
		}

		#region Commands

		// WPF use command binding to handle shortcuts, 
		// See: http://stackoverflow.com/questions/4682915/defining-menuitem-shortcuts

		// Create(New), Save and Load. Traceability: WC_3305: As an ESS, I can create, save and load program files.

		/// <summary>
		/// Show Configuration Window
		/// </summary>
		private void ConfigCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			// Instantiate the dialog box
			ConfigurationWindow dlg = new ConfigurationWindow();


			// Configure the dialog box
			dlg.Owner = this;
			dlg.Config = WinAvrConnector.config;

			// Open the dialog box modally 
			dlg.ShowDialog();
			if (dlg.DialogResult == true)
			{
				// MessageBox.Show(WinAvrConnector.config.ToString());

			}
		}

		/// <summary>
		/// Create a new HLProgram and pass it to ProgramList
		/// </summary>
		/// <param name="target"></param>
		/// <param name="e"></param>
		void NewCmdExecuted(object target, ExecutedRoutedEventArgs e)
		{
			fileName = null;
			program = new HLProgram();
			programList1.Program = program;
		}

		/// <summary>
		/// Show a open dialog to let user choose the igp file to be loaded
		/// </summary>
		/// <param name="target"></param>
		/// <param name="e"></param>
		void OpenCmdExecuted(object target, ExecutedRoutedEventArgs e)
		{
			// Configure open file dialog box
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.InitialDirectory = Directory.GetCurrentDirectory();
			dlg.DefaultExt = ".igp"; // Default file extension
			dlg.Filter = "iRobot Graphical Program|*.igp"; // Filter files by extension 

			// Show open file dialog box
			Nullable<bool> result = dlg.ShowDialog();

			// Process open file dialog box results 
			if (result == true)
			{
				// Open document 
				fileName = dlg.FileName;
				OpenProgram(fileName);
				textBlockStatus.Text = fileName;
			}
		}


		/// <summary>
		/// Save the program as an igp file.
		/// Traceability: WC_3305, As an ESS, I can create, save and load program files.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveAsCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			// Configure save file dialog box
			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
			dlg.InitialDirectory = Directory.GetCurrentDirectory();
			dlg.FileName = fileName; // Default file name
			dlg.DefaultExt = ".igp"; // Default file extension
			dlg.Filter = "Text documents|*.igp"; // Filter files by extension 

			// Show save file dialog box
			Nullable<bool> result = dlg.ShowDialog();

			// Process save file dialog box results 
			if (result == true)
			{
				// Save document 
				string filename = dlg.FileName;
				SaveProgram(filename);
			}
		}

		/// <summary>
		/// Save the program to an igp file.
		/// Traceability: WC_3305, As an ESS, I can create, save and load program files.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (String.IsNullOrEmpty(fileName))
			{
				// If no file is currently opened, call SaveAs.
				SaveAsCmdExecuted(sender, e);
			}
			else
			{
				SaveProgram(fileName);
			}
		}
		#endregion



		#region Private Methods
		/// <summary>
		/// Translate igp file to C file and compile it to executable program using WinAVR and load it to iRobot.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BuildAndLoad(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(System.IO.Directory.GetCurrentDirectory());
			string template = File.ReadAllText(@"template.c");
			File.WriteAllText(@"output.c", template);
			MessageBox.Show("Compiling and loading succeeded");
		}

		/// <summary>
		/// Load program from file.
		/// </summary>
		/// <param name="filePath">Path of file.</param>
		private void OpenProgram(string filePath)
		{
			try
			{
				string proStr = File.ReadAllText(filePath);
				program = new HLProgram(proStr);
				programList1.Program = program;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		/// <summary>
		/// Save program from file.
		/// </summary>
		/// <param name="filePath"></param>
		private void SaveProgram(string filePath)
		{
			try
			{
				string proStr = programList1.Program.ToString();
				program = programList1.Program;
				File.WriteAllText(filePath, proStr);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void OpenSrcCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if (!String.IsNullOrEmpty(fileName))
			{
				e.CanExecute = true;
			}
			else
			{
				e.CanExecute = false;
			}
		}

		private void OpenSrcCmdExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			Process.Start(fileName);
		}
		#endregion



		#region Control Callbacks

		private void buttonRefreshSource_Click(object sender, RoutedEventArgs e)
		{
			HLProgram program = programList1.Program;
			textBoxSrc.Text = program.ToString();
		}

		/// <summary>
		/// Load specified igp file [Debug use only] 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonLoadExampleCode_Click(object sender, RoutedEventArgs e)
		{
			OpenProgram("song.igp");	
		}
		#endregion


		#region Menu callbacks
		private void MenuItem_ShowCCode(object sender, RoutedEventArgs e)
		{
			System.Diagnostics.Process.Start(@"output.c");
		}

		private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Mission Science iRobots\nUSC CSCI-577 Team 07");
		}

		private void MenuItemBuild_Click(object sender, RoutedEventArgs e)
		{
			//Window errorWin = new OutputWindow();
			//errorWin.Show();
			//  WinAvrConnector.Clean();


			WinAvrConnector.Make();
		}

		private void MenuItemClean_Click(object sender, RoutedEventArgs e)
		{
			File.Delete("C_result.c");
			WinAvrConnector.Clean();
		}

		private void MenuItemLoad_Click(object sender, RoutedEventArgs e)
		{
			WinAvrConnector.Load();
		}

		private void MenuItemShowSrcFolder_Click(object sender, RoutedEventArgs e)
		{
			// Open current folder in explorer.exe
			// http://stackoverflow.com/questions/1132422/open-a-folder-using-process-start
			Process.Start("explorer.exe", ".");
		}

		private void MenuItemTranslate_Click(object sender, RoutedEventArgs e)
		{
			string cCode = Translator.TranslateProgram(program);

			MessageBox.Show(cCode);
			Translator.GenerateCSource(Translator.SourceType.Microcontroller, cCode);
			Translator.GenerateCSource(Translator.SourceType.Emulator, cCode);
		}

		#endregion
		// textbox input form validation function
		private void number_validation(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}
	}
}

