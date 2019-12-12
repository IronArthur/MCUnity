

#region Using Statements
using System;
using System.Text;
#endregion
namespace MechCommanderUnity.API
{
    /// <summary>
    /// Stores size data.
    /// </summary>
    public class MCSize
    {
        #region Fields

        public int Width;

        public int Height;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MCSize()
        {
            Width = 0;
            Height = 0;
        }

        /// <summary>
        /// Value constructor.
        /// </summary>
        public MCSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        #endregion
    }
}