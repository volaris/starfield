using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Starfield.Presence;

namespace Starfield
{
    class CriticalStarfieldModel : StarfieldModel
    {
        public CriticalStarfieldModel(float xStep, float yStep, float zStep, ulong numX, ulong numY, ulong numZ, PresenceClient presenceClient) : base(xStep,
                                                                                                                                                       yStep,
                                                                                                                                                       zStep,
                                                                                                                                                       numX,
                                                                                                                                                       numY,
                                                                                                                                                       numZ,
                                                                                                                                                       presenceClient)
        {
            NeedSafetyLight = true;
        }

        public override void SetColor(int x, int y, int z, Color color)
        {
            if(z > 4)
            {
                z = ((int)this.NumZ - 1) - (z - 5);
            }
            base.SetColor(x, y, z, color);
        }
    }
}
