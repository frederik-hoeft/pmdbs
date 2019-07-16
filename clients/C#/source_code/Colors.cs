using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pmdbs
{
    /// <summary>
    /// Provides a few default Colors used throughout pmdbs.
    /// </summary>
    public struct Colors
    {
        /// <summary>
        /// Gets the pmdbs orange color that has an ARGB value of #FFFF6031.
        /// </summary>
        public static Color Orange => Color.FromArgb(255, 96, 49);

        /// <summary>
        /// Gets a user-defined color that has an ARGB value of #FF404040.
        /// </summary>
        public static Color Gray => Color.FromArgb(64, 64, 64);

        /// <summary>
        /// Gets a user-defined color that has an ARGB value of #FFE2574C.
        /// </summary>
        public static Color Red => Color.FromArgb(226, 87, 76);

        /// <summary>
        /// Gets a user-defined color that has an ARGB value of #FF90C564.
        /// </summary>
        public static Color Green => Color.FromArgb(144, 197, 100);

        /// <summary>
        /// Gets a user-defined color that has an ARGB value of #FF212121.
        /// </summary>
        public static Color DarkGray => Color.FromArgb(33, 33, 33);

        /// <summary>
        /// Gets a user-defined color that has an ARGB value of #FF2680EB.
        /// </summary>
        public static Color Blue => Color.FromArgb(38, 128, 235);

        /// <summary>
        /// Gets a user-defined color that has an ARGB value of #FFDCDCDC.
        /// </summary>
        public static Color LightGray => Color.FromArgb(220, 220, 220);

        /// <summary>
        /// Gets a user-defined color that has an ARGB value of #FFFFD8CC.
        /// </summary>
        public static Color LightOrange => Color.FromArgb(255, 216, 204);
    }
}
