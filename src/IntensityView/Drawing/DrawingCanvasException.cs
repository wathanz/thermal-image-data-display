using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;


namespace DrawToolsLib {
    /// <summary>
    /// Exception thrown by DrawingCanvas Load and Save methods
    /// </summary>
    [Serializable]      // make FxCop happy
    public class DrawingCanvasException : Exception {
        public DrawingCanvasException(string message)
            : base(message) {
        }

        public DrawingCanvasException(string message, Exception innerException)
            : base(message, innerException) {
        }

        public DrawingCanvasException()
            : base("Unknown") {

        }

        protected DrawingCanvasException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

    }
}