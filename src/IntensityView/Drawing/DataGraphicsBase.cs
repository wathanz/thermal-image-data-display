
using System.Xml.Serialization;


namespace DrawingLib {
    public abstract class DataGraphicsBase {
        [XmlIgnore]
        internal int ID;

        [XmlIgnore]
        internal bool selected;

        [XmlIgnore]
        internal double actualScale;

        public abstract GraphicsBase CreateGraphics();
    }
}