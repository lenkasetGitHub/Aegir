﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AegirCore.Keyframe
{
    public interface IValueInterpolator
    {
        object InterpolateBetween(object fromValue, object toValue, double t);
    }
}
