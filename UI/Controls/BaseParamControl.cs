﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace iRobotGUI.Controls
{
    public class BaseParamControl : UserControl
    {
        public virtual Instruction Ins
        {
            get;
            set;
        }

    }
}
