using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


static class MathExt
{
    public static double Clamp(double val, double max, double min)
    {
        return val > max ? max : val < min ? min : val;
    }
}

