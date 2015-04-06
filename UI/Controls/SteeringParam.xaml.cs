﻿using System;
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
using System.ComponentModel;

namespace iRobotGUI.Controls
{
	/// <summary>
	/// Interaction logic for SteeringControl.xaml
	/// </summary>
	public partial class SteeringParam : UserControl, INotifyPropertyChanged
	{
		
		public double Angle
		{
			get { return _Angle; }
			set { setProperty(ref _Angle, value); }
		}
		public int Radius
		{
			get { return _Radius; }
			set { setProperty(ref _Radius, value); }
		}

		private const int STRAIGHT = 32768;
		private const int MAX_RADIUS = 2000;
		private int _Radius = default(int);
		private double _Angle = default(double);
		private enum Quadrants : int { nw = 2, ne = 1, sw = 4, se = 3 }
		private int roundToInt(double val)
		{
			return (int)Math.Round(val, 0, MidpointRounding.AwayFromZero);
		}
		private int angleToRadius(double angle)
		{
			double rad_angle = (angle * Math.PI) / 180; ///convert angle from centigrade to radians
			if (angle <= 0)
			{										
				return roundToInt((Math.Cos(rad_angle) * MAX_RADIUS)); ///(int)Math.Round((Math.Cos(rad_angle) * MAX_RADIUS), 0, MidpointRounding.AwayFromZero);
			};
			return -roundToInt((Math.Cos(rad_angle) * MAX_RADIUS));
		}
		private double radiusToAngle(int radius)
		{
			if (radius == STRAIGHT)
			{
				return 0.0;
			}
			double d = (double)radius / (double)MAX_RADIUS;
			double rad_angle = Math.Acos(d);
			if(radius >= 0) {
				return -(rad_angle * 180.0) / Math.PI; //((rad_angle - Math.PI) * 180.0) / Math.PI;	
			}
			 return 180-((rad_angle * 180.0) / Math.PI); /// return centigrade angle
		}
		private double GetAngle(Point touchPoint, Size circleSize)
		{
			var _X = touchPoint.X - (circleSize.Width / 2d);
			var _Y = circleSize.Height - touchPoint.Y - (circleSize.Height / 2d);
			var _Hypot = Math.Sqrt(_X * _X + _Y * _Y);
			var _Value = Math.Asin(_Y / _Hypot) * 180 / Math.PI;
			var _Quadrant = (_X >= 0) ?
				(_Y >= 0) ? Quadrants.ne : Quadrants.se :
				(_Y >= 0) ? Quadrants.nw : Quadrants.sw;
			if (_Value >= 89.9)
			{
				_Value = 89.9;
			}
			switch (_Quadrant)
			{
				case Quadrants.ne: _Value = 089.9 - _Value; break;
				case Quadrants.nw: _Value = _Value - 089.9; break;
				case Quadrants.se: _Value = 089.9 /*- _Value*/; break;
				case Quadrants.sw: _Value = -089.9 /*+  _Value*/; break;
			}
			return _Value;
		}
		public SteeringParam()
		{
			InitializeComponent();
			this.DataContext = this;
			this.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
			this.MouseUp += new MouseButtonEventHandler(OnMouseUp);
			this.MouseMove += new MouseEventHandler(OnMouseMove);
		}

		private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Mouse.Capture(this);
		}

		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			Mouse.Capture(null);
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (Mouse.Captured == this)
			{
				/// Get the current mouse position relative to the control
				Point currentLocation = Mouse.GetPosition(this);
				/// Calculate an angle
				this.Angle = GetAngle(currentLocation, this.RenderSize);
				this.Radius = angleToRadius(this.Angle);
				RotateTransform rotateTransform1 = new RotateTransform((int)this.Angle);
				rotateTransform1.CenterX = (this.ActualWidth) / 2;
				rotateTransform1.CenterY = (this.ActualHeight) / 2;
				RotateGrid.RenderTransform = rotateTransform1;
			}
		}
		
	
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		void setProperty<T>(ref T storage, T value, [System.Runtime.CompilerServices.CallerMemberName] String propertyName = null)	{
			if (!object.Equals(storage, value))
			{
				storage = value;
				if(PropertyChanged!=null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}

		}

	}
}