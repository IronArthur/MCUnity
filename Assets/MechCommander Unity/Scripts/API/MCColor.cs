
#region Using Statements
using System;
using System.Diagnostics;
using System.Text;
#endregion

namespace MechCommanderUnity.API
{
    /// <summary>
    /// Stores colour data.
    /// </summary>
    /// 
    [DebuggerDisplay("R:{R},G:{G},B:{B},A:{A}")]
    public class MCColor
    {
        #region Fields

        public byte R;

        public byte G;

        public byte B;

        public byte A;

        #endregion

        #region Constructors

        /// <summary>
        /// Value constructor RGB.
        /// </summary>
        public MCColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
            A = 0xff;
        }

        /// <summary>
        /// Value constructor RGBA.
        /// </summary>
        public MCColor(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public MCColor() { }

        public static MCColor FromRGBA(byte r, byte g, byte b, byte a = 255)
        {
            MCColor color = new MCColor()
            {
                R = r,
                G = g,
                B = b,
                A = a,
            };
            return color;
        }

        public override string ToString()
        {
            return string.Format("[{0},{1},{2},{3}]",R,G,B,A);
        }
        #endregion
    }
}