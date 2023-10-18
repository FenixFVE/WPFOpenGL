using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.WPF;
using SharpGL.Enumerations;
using System.Collections.Generic;
using System.Linq;
using WPFOpenGL;

namespace WPFOpenGL
{
    public partial class MainWindow : Window
    {
        public const float pointMargin = 0.02f;
        public OpenGL gl;
        public Scene scene { get; set; }
        public Group? selectedGroup { get; set; } = null;

        public State state { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            
        }
        private void OpenGLControl_OnResized(object sender, EventArgs e) { }

        private void OpenGLControl_OnOpenGLInitialized(object sender, OpenGLRoutedEventArgs args)
        {
            gl = args.OpenGL;
            scene = new Scene();
        }

        private void OpenGLControl_OnOpenGLDraw(object sender, OpenGLRoutedEventArgs args)
        {
            //OpenGL gl = args.OpenGL;

            // Set the background color to white
            gl.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            Color black = new(0, 0, 0);
            gl.LineWidth(2.0f);

            var queue = new List<Group>(scene.Groups);
            queue.Sort((x, y) => x.Priority.CompareTo(y.Priority));

            foreach (var group in queue)
            {
                Color.SetColor(gl, group.Color);
                gl.Begin(OpenGL.GL_TRIANGLE_FAN);
                foreach (var point in group.Points)
                {
                    gl.Vertex(point.X, point.Y);
                }
                gl.End();
            }

            if (selectedGroup is not null)
            {
                var points = selectedGroup.Points;

                Color.SetColor(gl, black);
                gl.LineWidth(2.0f);
                gl.Begin(OpenGL.GL_LINES);
                for (var i = 1; i < points.Count; i++)
                {
                    gl.Vertex(points[i].X, points[i].Y);
                    gl.Vertex(points[i - 1].X, points[i - 1].Y);
                }
                if (points.Count > 2)
                {
                    gl.Vertex(points[0].X, points[0].Y);
                    gl.Vertex(points[^1].X, points[^1].Y);
                }
                gl.End();

                float margin = pointMargin;
                foreach (var point in points)
                {
                    gl.Begin(OpenGL.GL_TRIANGLE_FAN);
                    gl.Vertex(point.X + margin, point.Y + margin);
                    gl.Vertex(point.X + margin, point.Y - margin);
                    gl.Vertex(point.X - margin, point.Y - margin);
                    gl.Vertex(point.X - margin, point.Y + margin);
                    gl.End();
                }
            }

            gl.Flush();
        }


        public static (VPoint, Color) GetClickPositionAndColor(OpenGL gl, OpenGLControl openglControl, MouseButtonEventArgs e)
        {
            // Get the mouse click coordinates relative to the OpenGL control
            VPoint clickPoint = (VPoint)e.GetPosition(openglControl);

            // Convert the mouse click coordinates to OpenGL viewport coordinates
            int viewportX = (int)clickPoint.X;
            int viewportY = (int)(openglControl.ActualHeight - clickPoint.Y); // Invert Y-axis

            // Read the color information from the framebuffer at the clicked coordinates
            byte[] pixelColor = new byte[3]; // RGB color

            gl.ReadPixels(viewportX, viewportY, 1, 1, OpenGL.GL_RGB, OpenGL.GL_UNSIGNED_BYTE, pixelColor);

            // Calculate the mapping factor to go from click coordinates to your OpenGL coordinates
            float scaleX = 2.0f / (float)openglControl.ActualWidth;
            float scaleY = 2.0f / (float)openglControl.ActualHeight;

            // Map the click coordinates to your OpenGL coordinates
            float xClick = scaleX * (float)viewportX - 1.0f;
            float yClick = -(1.0f - scaleY * (float)viewportY);

            VPoint point = new VPoint(xClick, yClick);
            Color color = new Color(pixelColor[0], pixelColor[1], pixelColor[2]);

            return (point, color);
        }

        
        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                switch (state)
                {
                    case State.Add:
                        MouseButton_Add(sender, e);
                        break;
                    case State.Select:
                        MouseButton_Select(sender, e);
                        break;
                    case State.Move:
                        MouseButton_MoveClick1(sender, e);
                        break;
                    case State.MoveClick1:
                        MouseButton_MoveClick2(sender, e);
                        break;
                    case State.Delete:
                        MouseButton_Delete(sender, e);
                        break;
                }
            }
            catch
            {
                MessageBox.Show("error");
            }
        }

        public void MouseButton_Delete(object sender, MouseButtonEventArgs e)
        {
            var openglControl = (OpenGLControl)sender;

            var (clickPoint, clickColor) = GetClickPositionAndColor(gl, openglControl, e);

            var m = pointMargin;
            var c = clickPoint;

            bool isOnlyOne = false;
            for (var i = 0; i < selectedGroup.Points.Count; i++)
            {
                var p = selectedGroup.Points[i];
                if (p.X - m <= c.X && c.X <= p.X + m
                                   && p.Y - m <= c.Y && c.Y <= p.Y + m)
                {
                    selectedGroup.Points.Remove(selectedGroup.Points[i]);
                    isOnlyOne = true;
                    break;
                }
            }

            if (!isOnlyOne)
            {
                scene.Groups.Remove(selectedGroup);
                selectedGroup = null;
                state = State.Select;
            }
        }

        public void MouseButton_MoveClick1(object sender, MouseButtonEventArgs e)
        {
            var openglControl = (OpenGLControl)sender;

            var (clickPoint, clickColor) = GetClickPositionAndColor(gl, openglControl, e);

            MoveClick1 = clickPoint;

            state = State.MoveClick1;
        }
        private VPoint MoveClick1 { get; set; }
        public void MouseButton_MoveClick2(object sender, MouseButtonEventArgs e)
        {
            var openglControl = (OpenGLControl)sender;

            var (clickPoint, clickColor) = GetClickPositionAndColor(gl, openglControl, e);

            var change = clickPoint - MoveClick1;

            var m = pointMargin;
            var c = MoveClick1;

            bool isOnlyOne = false;
            for (var i = 0; i < selectedGroup.Points.Count; i++)
            {
                var p = selectedGroup.Points[i];
                if (p.X - m <= c.X && c.X <= p.X + m
                                   && p.Y - m <= c.Y && c.Y <= p.Y + m)
                {
                    selectedGroup.Points[i] += change;
                    isOnlyOne = true;
                    break;
                }
            }

            if (!isOnlyOne)
            {
                for (var i = 0; i < selectedGroup.Points.Count; i++)
                {
                    selectedGroup.Points[i] += change;
                }
            }

            state = State.Move;
        }

        public void MouseButton_Add(object sender, MouseButtonEventArgs e)
        {
            var openglControl = (OpenGLControl)sender;

            var (clickPoint, clickColor) = GetClickPositionAndColor(gl, openglControl, e);

            if (selectedGroup is not null)
            {
                selectedGroup.Points.Add(clickPoint);
            }
            else
            {
                selectedGroup = new Group();
                selectedGroup.Color = new Color(255, 0, 0);
                selectedGroup.Points.Add(clickPoint);
                scene.Groups.Add(selectedGroup);
            }
        }

        public void MouseButton_Select(object sender, MouseButtonEventArgs e)
        {

            selectedGroup = null;

            var openglControl = (OpenGLControl)sender;

            var (clickPoint, clickColor) = GetClickPositionAndColor(gl, openglControl, e);
            
            var groups = scene.Groups
                .Where(x => x.Color.Equals(clickColor))
                .OrderBy(x => x.Priority)
                .ToList();

            foreach (var group in groups)
            {
                if (group.IsPointInside(clickPoint))
                {
                    selectedGroup = group;
                    break;
                }
            }
        }

        private void Select_OnClick(object sender, RoutedEventArgs e)
        {
            state = State.Select;
        }

        private void Finish_OnClick(object sender, RoutedEventArgs e)
        {
            selectedGroup = null;
            state = State.Add;
        }

        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            if (selectedGroup is null) return;
            
            state = State.Delete;
        }

        private void Parameters_OnClick(object sender, RoutedEventArgs e)
        {
            if (selectedGroup is null) return;

            Window1 popup = new Window1();
            popup.Closed += (s, args) =>
            {
                if (popup.color is not null)
                {
                    selectedGroup.Color = popup.color;
                }
            };
            popup.ShowDialog();

        }

        private void Move_OnClick(object sender, RoutedEventArgs e)
        {
            if (selectedGroup is null) return;

            state = State.Move;
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
            state = State.Add;
        }
    }
}
