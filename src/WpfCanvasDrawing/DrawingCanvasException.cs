using System;
using System.Runtime.Serialization;

namespace WpfCanvasDrawing;
[Serializable]
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
}
