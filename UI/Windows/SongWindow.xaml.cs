﻿using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Midi.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace iRobotGUI
{
	/// <summary>
	/// A class representing the musical note.
	/// </summary>
	public class Note : INotifyPropertyChanged
	{
		private int duration;
		private int number;
		public Note(int number, int duration)
		{
			this.Number = number;
			this.Duration = duration;
		}

		public Note(int nubmer)
			: this(nubmer, 32)
		{

		}

		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Note duration.
		/// </summary>
		public int Duration
		{
			get
			{
				return duration;
			}
			set
			{
				duration = value;
				NotifyPropertyChanged();
			}
		}

		/// <summary>
		/// Note number.
		/// </summary>
		public int Number
		{
			get
			{
				return number;
			}
			set
			{
				number = value;
				NotifyPropertyChanged();
			}
		}

		public override string ToString()
		{
			return Number.ToString() + "," + Duration.ToString();
		}

		// This method is called by the Set accessor of each property. 
		// The CallerMemberName attribute that is applied to the optional propertyName 
		// parameter causes the property name of the caller to be substituted as an argument. 
		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}

	/// <summary>
	/// Interaction logic for Song.xaml
	/// API of Song is in iRobot Open API p.11.
	/// </summary>
	public partial class SongWindow : BaseParamWindow
	{
		private ObservableCollection<Note> noteList;
		private OutputDevice outDevice;

		public SongWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Set and get the instruction.
		/// </summary>
		public override iRobotGUI.Instruction Ins
		{
			get
			{
				// 1. Create a default ins
				Instruction result = Instruction.CreatFromOpcode(Instruction.SONG_DEF);
				
				// 2. Clear the parameters.
				result.paramList.Clear();

				// 3. Song No
				result.paramList.Add(comboBoxSongNo.SelectedIndex);
				
				// 4. Notes
				foreach(Note note in noteList)
				{
					result.paramList.Add(note.Number);
					result.paramList.Add(note.Duration);
				}
				return result;
			}
			set
			{
				Instruction ins = value;
				base.Ins = ins;

				// 1. Set song No.
				comboBoxSongNo.SelectedIndex = ins.paramList[0];

				// 2. Display the song
				noteList = new ObservableCollection<Note>();
				int i = 1;
				while (i < ins.paramList.Count)
				{
					noteList.Add(new Note(ins.paramList[i], ins.paramList[i + 1]));
					i += 2;
				}
				listViewNotes.ItemsSource = noteList;
				listViewNotes.SelectedIndex = 0;
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			if (outDevice != null)
			{
				outDevice.Dispose();
			}
			base.OnClosed(e);
		}

		private void buttonClear_Click(object sender, RoutedEventArgs e)
		{
			noteList.Clear();
		}

		private void buttonDelete_Click(object sender, RoutedEventArgs e)
		{
			int select = listViewNotes.SelectedIndex;
			noteList.RemoveAt(listViewNotes.SelectedIndex);

			if (select == noteList.Count)
			{
				// Select the preview one if the tail is deleted.
				listViewNotes.SelectedIndex = select - 1;
			}
			else
			{
				// Select the next one if a body is deleted.
				listViewNotes.SelectedIndex = select;
			}
		}

		private void buttonMoveDown_Click(object sender, RoutedEventArgs e)
		{
			noteList.Move(listViewNotes.SelectedIndex, listViewNotes.SelectedIndex + 1);
		}

		private void buttonMoveUp_Click(object sender, RoutedEventArgs e)
		{
			noteList.Move(listViewNotes.SelectedIndex, listViewNotes.SelectedIndex - 1);
		}
		private void buttonNew_Click(object sender, RoutedEventArgs e)
		{
			noteList.Clear();
		}

		private void comboBoxSongNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Ins.paramList[0] = comboBoxSongNo.SelectedIndex;
		}

		/// <summary>
		/// Get the string of SONG_DEF
		/// </summary>
		private string GetInsString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(Instruction.SONG_DEF + " " + comboBoxSongNo.SelectedItem.ToString());
			foreach (Note note in listViewNotes.Items)
			{
				sb.Append("," + note.ToString());
			}
			return sb.ToString();
		}

		private void listViewNotes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Note n = listViewNotes.SelectedItem as Note;
			if (n != null)
			{
				sliderDuration.Value = n.Duration;				
			}
		}

		private void pianoKeyboard_PianoKeyDown(object sender, PianoKeyEventArgs e)
		{
			Note note = new Note(e.NoteID);

			// Insert a note to current position
			if (listViewNotes.SelectedIndex >= 0)
				noteList.Insert(listViewNotes.SelectedIndex, note);
			else
			{
				noteList.Add(note);				
			}
			listViewNotes.UpdateLayout();
			listViewNotes.ScrollIntoView(listViewNotes.SelectedItem);

			outDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, e.NoteID, 127));

		}

		private void pianoKeyboard_PianoKeyUp(object sender, PianoKeyEventArgs e)
		{
			outDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, 0, e.NoteID, 0));
		}

		private void sliderDuration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (listViewNotes.SelectedItem != null)
			{
				(listViewNotes.SelectedItem as Note).Duration = (int)sliderDuration.Value;
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i <= 15; i++)
			{
				ComboBoxItem item = new ComboBoxItem();
				item.Content = i.ToString();
				comboBoxSongNo.Items.Add(item);
			}	

			outDevice = new OutputDevice(0);
		}
	}
}
