import clr
clr.AddReferenceToFileAndPath("Starfield.dll")
from Starfield import *
clr.AddReferenceToFileAndPath("Utils.dll")
from StarfieldUtils import *
clr.AddReference("System.Drawing")
import System.Drawing
import clrtype

class Clear(IStarfieldDriver):
    """Sample Python Driver"""

    __metaclass__ = clrtype.ClrClass
    _clrnamespace = "PythonDrivers"

    DriverTypeAttrib = clrtype.attribute(DriverType)
    _clrclassattribs = [
        DriverTypeAttrib(DriverTypes.Experimental)
        ]

    @clrtype.accepts(StarfieldModel)
    @clrtype.returns()
    def Render(self, Starfield):
        for x in range(Starfield.NumX):
            for y in range(Starfield.NumY):
                for z in range(Starfield.NumZ):
                    Starfield.SetColor(x,y,z, System.Drawing.Color.Black)

    @clrtype.accepts(StarfieldModel)
    @clrtype.returns()
    def Start(self, Starfield):
        pass
    
    @clrtype.accepts()
    @clrtype.returns()
    def Stop(self):
        pass
    
    @clrtype.accepts()
    @clrtype.returns()
    def ToString(self):
        return "Clear"
