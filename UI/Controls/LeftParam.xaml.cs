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
using System.Windows.Shapes;

namespace iRobotGUI.Controls
{
    /// <summary>
    /// Interaction logic for LeftParam.xaml
    /// </summary>
    public partial class LeftParam : BaseParamControl
    {
         private double Angle;
        private enum Quadrants : int { nw = 2, ne = 1, sw = 4, se = 3 }
        private double GetAngle(Point touchPoint, Size circleSize)
        {
            var _X = touchPoint.X - (circleSize.Width / 2d);
            var _Y = circleSize.Height - touchPoint.Y - (circleSize.Height / 2d);
            var _Hypot = Math.Sqrt(_X * _X + _Y * _Y);
            var _Value = Math.Asin(_Y / _Hypot) * 180 / Math.PI;
            var _Quadrant = (_X >= 0) ?
                (_Y >= 0) ? Quadrants.ne : Quadrants.se :
                (_Y >= 0) ? Quadrants.nw : Quadrants.sw;
            switch (_Quadrant)
            {
                case Quadrants.ne: _Value = 090 - _Value; break;
                case Quadrants.nw: _Value = 270 + _Value; break;
                case Quadrants.se: _Value = 090 /*- _Value*/; break;
                case Quadrants.sw: _Value = 270 /*+  _Value*/; break;
            }
            return _Value;
        }
        public override Instruction Ins
        {
            get
            {
                return base.Ins;
            }
            set
            {
                base.Ins = value;

                this.Angle = Ins.paramList[0];
                ///rotate the control image specified number of degrees:
                RotateTransform rotateTransform1 = new RotateTransform(this.Angle);
                rotateTransform1.CenterX = 75;
                rotateTransform1.CenterY = 75;
                RotateGrid.RenderTransform = rotateTransform1;

            }
        }
        public LeftParam()
        {
            InitializeComponent();
            ///this.DataContext = this;
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
           
               Ins.paramList[0] =  (int)this.Angle;
               RotateTransform rotateTransform1 = new RotateTransform((int)this.Angle);
               rotateTransform1.CenterX = 75;
               rotateTransform1.CenterY = 75;
               RotateGrid.RenderTransform = rotateTransform1;
            }
        }      
        
    }
}
