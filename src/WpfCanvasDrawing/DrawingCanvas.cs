using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace WpfCanvasDrawing;
public class DrawingCanvas : Canvas {

    private VisualCollection graphicsList;
    public static readonly DependencyProperty ToolProperty;
    public static readonly DependencyProperty ActualScaleProperty;
    public static readonly DependencyProperty IsDirtyProperty;

    public static readonly RoutedEvent IsDirtyChangedEvent;

    private Tool[] tools;
    private ToolPointer toolPointer;

    public DrawingCanvas()
        : base() {
        graphicsList = new VisualCollection(this);

        tools = new Tool[(int)ToolType.Max];
        toolPointer = new ToolPointer();
        tools[(int)ToolType.Pointer] = toolPointer;

        this.Loaded += new RoutedEventHandler(DrawingCanvas_Loaded);
        this.MouseDown += new MouseButtonEventHandler(DrawingCanvas_MouseDown);
        this.MouseMove += new MouseEventHandler(DrawingCanvas_MouseMove);
        this.MouseUp += new MouseButtonEventHandler(DrawingCanvas_MouseUp);
        this.KeyDown += new KeyEventHandler(DrawingCanvas_KeyDown);
        this.LostMouseCapture += new MouseEventHandler(DrawingCanvas_LostMouseCapture);

    }


    static DrawingCanvas() {

        PropertyMetadata metaData;

        // Tool
        metaData = new PropertyMetadata(ToolType.Pointer);
        ToolProperty = DependencyProperty.Register(
            "Tool", typeof(ToolType), typeof(DrawingCanvas),
            metaData);

        // ActualScale
        metaData = new PropertyMetadata(
            1.0,                                                        // default value
            new PropertyChangedCallback(ActualScaleChanged));           // change callback

        ActualScaleProperty = DependencyProperty.Register(
            "ActualScale", typeof(double), typeof(DrawingCanvas),
            metaData);


        // IsDirty
        metaData = new PropertyMetadata(false);

        IsDirtyProperty = DependencyProperty.Register(
            "IsDirty", typeof(bool), typeof(DrawingCanvas),
            metaData);

        // IsDirtyChanged
        IsDirtyChangedEvent = EventManager.RegisterRoutedEvent("IsDirtyChangedChanged",
            RoutingStrategy.Bubble, typeof(DependencyPropertyChangedEventHandler), typeof(DrawingCanvas));

    }

    public ToolType Tool {
        get {
            return (ToolType)GetValue(ToolProperty);
        }
        set {
            if ((int)value >= 0 && (int)value < (int)ToolType.Max) {
                SetValue(ToolProperty, value);
                tools[(int)Tool].SetCursor(this);
            }
        }
    }

    public double ActualScale {
        get {
            return (double)GetValue(ActualScaleProperty);
        }
        set {
            SetValue(ActualScaleProperty, value);
        }
    }

    static void ActualScaleChanged(DependencyObject property, DependencyPropertyChangedEventArgs args) {
        var d = property as DrawingCanvas;

        double scale = d.ActualScale;

        foreach (GraphicsBase b in d.GraphicsList) {
            b.ActualScale = scale;
        }
    }


    public bool IsDirty {
        get {
            return (bool)GetValue(IsDirtyProperty);
        }
        internal set {
            SetValue(IsDirtyProperty, value);
            RoutedEventArgs newargs = new RoutedEventArgs(IsDirtyChangedEvent);
            RaiseEvent(newargs);
        }
    }


    public event RoutedEventHandler IsDirtyChanged {
        add { AddHandler(IsDirtyChangedEvent, value); }
        remove { RemoveHandler(IsDirtyChangedEvent, value); }
    }
    public void Draw(DrawingContext drawingContext) {
        foreach (GraphicsBase b in graphicsList) {
            b.Draw(drawingContext);
        }
    }

    public void Clear() {
        graphicsList.Clear();
    }

    public void RefreshClip() {
        foreach (GraphicsBase b in graphicsList) {
            b.Clip = new RectangleGeometry(new Rect(0, 0, this.ActualWidth, this.ActualHeight));

            // Good chance to refresh actual scale
            b.ActualScale = ActualScale;
        }
    }

    public void RemoveClip() {
        foreach (GraphicsBase b in graphicsList) {
            b.Clip = null;
        }
    }

    public GraphicsBase this[int index] {
        get {
            if (index >= 0 && index < Count) {
                return (GraphicsBase)graphicsList[index];
            }
            return null;
        }
    }

    public int Count {
        get {
            return graphicsList.Count;
        }
    }

    public int SelectionCount {
        get {
            int n = 0;

            foreach (GraphicsBase g in graphicsList) {
                if (g.IsSelected) {
                    n++;
                }
            }

            return n;
        }
    }
    public VisualCollection GraphicsList {
        get {
            return graphicsList;
        }
    }
    public IEnumerable<GraphicsBase> Selection {
        get {
            foreach (GraphicsBase o in graphicsList) {
                if (o.IsSelected) {
                    yield return o;
                }
            }
        }

    }

    public IEnumerable<T> GetGraphics<T>() where T : GraphicsBase {
        foreach (var o in graphicsList) {
            if (o is T) {
                yield return o as T;
            }
        }

    }

    public void ClearSelection() {
        foreach (GraphicsBase o in graphicsList) {
            if (o.IsSelected) {
                o.IsSelected = false;
            }
        }

    }

    protected override int VisualChildrenCount {
        get {
            int n = graphicsList.Count;
            return n;
        }
    }

    protected override Visual GetVisualChild(int index) {
        if (index < 0 || index >= graphicsList.Count) {
            throw new ArgumentOutOfRangeException("index");
        }

        return graphicsList[index];
    }



    private void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e) {
        if (tools[(int)Tool] == null) {
            return;
        }


        if (e.ChangedButton == MouseButton.Left) {
            Focus();
            if (e.ClickCount <= 1) {
                tools[(int)Tool].OnMouseDown(this, e);
            }
        }
    }

    private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e) {
        if (tools[(int)Tool] == null) {
            return;
        }

        if (e.MiddleButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released) {
            tools[(int)Tool].OnMouseMove(this, e);
        }
        else {
            Cursor = DrawingCanvasHelper.DefaultCursor;
        }
    }

    private void DrawingCanvas_MouseUp(object sender, MouseButtonEventArgs e) {
        if (tools[(int)Tool] == null) {
            return;
        }


        if (e.ChangedButton == MouseButton.Left) {
            tools[(int)Tool].OnMouseUp(this, e);
        }
    }


    private void DrawingCanvas_Loaded(object sender, RoutedEventArgs e) {
        Focusable = true;
    }


    private void DrawingCanvas_LostMouseCapture(object sender, MouseEventArgs e) {
        if (IsMouseCaptured) {
            CancelCurrentOperation();
        }
    }

    private void DrawingCanvas_KeyDown(object sender, KeyEventArgs e) {
        // Esc key stops currently active operation
        if (e.Key == Key.Escape) {
            if (IsMouseCaptured) {
                CancelCurrentOperation();
            }
        }
    }



    private void CancelCurrentOperation() {
        if (Tool == ToolType.Pointer) {
            if (graphicsList.Count > 0) {
                if (graphicsList[graphicsList.Count - 1] is GraphicsSelectionRectangle) {
                    graphicsList.RemoveAt(graphicsList.Count - 1);
                }
            }
        }
        else if (Tool > ToolType.Pointer && Tool < ToolType.Max) {
            if (graphicsList.Count > 0) {
                graphicsList.RemoveAt(graphicsList.Count - 1);
            }
        }

        Tool = ToolType.Pointer;

        ReleaseMouseCapture();
        Cursor = DrawingCanvasHelper.DefaultCursor;
    }


}