using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;



namespace DrawingLib {

    [XmlRoot("Graphics")]
    public class SerializationHelper {
        DataGraphicsBase[] graphics;
        public SerializationHelper() {

        }
        public SerializationHelper(VisualCollection collection) {
            if (collection == null) {
                throw new ArgumentNullException("collection");
            }

            graphics = new DataGraphicsBase[collection.Count];

            int i = 0;

            foreach (GraphicsBase g in collection) {
                graphics[i++] = g.CreateSerializedObject();
            }
        }

        [XmlArrayItem(typeof(DataGraphicsRectangle))]
        public DataGraphicsBase[] Graphics {
            get { return graphics; }
            set { graphics = value; }
        }
    }
}