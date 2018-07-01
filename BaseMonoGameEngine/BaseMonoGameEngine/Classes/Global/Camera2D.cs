using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TDMonoGameEngine
{
    /// <summary>
    /// The game camera.
    /// </summary>
    public class Camera2D : ICleanup
    {
        /// <summary>
        /// The constant for translating the camera to the center of the screen
        /// </summary>
        private const float TranslationConstant = .5f;
        
        /// <summary>
        /// The position of the camera. (0,0) is the center of the screen
        /// </summary>
        public Vector2 Position { get; protected set; } = Vector2.Zero;

        /// <summary>
        /// The scale of the Camera. Negative values will flip everything on-screen
        /// </summary>
        public float Scale { get; protected set; } = 1f;

        /// <summary>
        /// The rotation of the camera
        /// </summary>
        public float Rotation { get; protected set; } = 0f;

        /// <summary>
        /// The screen bounds of the camera.
        /// </summary>
        public Rectangle ScreenBounds { get; private set; } = Rectangle.Empty;

        public Camera2D()
        {
            SetTransform(new Vector2(0f, 0f), 0f, 1f);

            RenderingManager.Instance.ScreenResizedEvent -= WindowSizeChanged;
            RenderingManager.Instance.ScreenResizedEvent += WindowSizeChanged;
        }

        public void CleanUp()
        {
            RenderingManager.Instance.ScreenResizedEvent -= WindowSizeChanged;
        }

        private void WindowSizeChanged(in Vector2 newSize)
        {
            SetBounds(new Rectangle(0, 0, (int)RenderingManager.Instance.BackBufferDimensions.X, (int)RenderingManager.Instance.BackBufferDimensions.Y));
        }

        public void SetTransform(in Vector2 position, in float rotation, in float scale)
        {
            SetTranslation(position);
            SetRotation(rotation);
            SetZoom(scale);
        }

        public void SetBounds(in Rectangle bounds)
        {
            ScreenBounds = bounds;
        }

        /// <summary>
        /// Tells if bounds are in the Camera's view.
        /// </summary>
        /// <param name="bounds">A Rectangle representing the bounding region to check is in the Camera's view.</param>
        /// <returns>true if the bounds is in the Camera's view, otherwise false.</returns>
        public bool IsInCameraView(in Rectangle bounds)
        {
            return VisibleArea.Intersects(bounds);
        }

        /// <summary>
        /// Tells if bounds are in a visible area's view.
        /// </summary>
        /// <param name="visibleArea">The visible region of the screen.</param>
        /// <param name="bounds">A Rectangle representing the bounding region to check is in the view.</param>
        /// <returns>true if the bounds is in the view, otherwise false.</returns>
        public bool IsInCameraView(in Rectangle visibleArea, in Rectangle bounds)
        {
            return visibleArea.Intersects(bounds);
        }

        /// <summary>
        /// Transforms a point from screen space to world space.
        /// </summary>
        /// <param name="point">The point in screen space.</param>
        /// <returns>A Vector2 of the point in world space.</returns>
        public Vector2 ScreenToWorldSpace(in Vector2 point)
        {
            Matrix invertedMatrix = Matrix.Invert(TransformMatrix);
            return Vector2.Transform(point, invertedMatrix);
        }

        /// <summary>
        /// Transforms a point from world space to screen space.
        /// </summary>
        /// <param name="point">The point in world space.</param>
        /// <returns>A Vector2 of the point in screen space.</returns>
        public Vector2 WorldToScreenSpace(in Vector2 point)
        {
            return Vector2.Transform(point, TransformMatrix);
        }

        /// <summary>
        /// Makes the camera look at a point in world space.
        /// </summary>
        /// <param name="point">The point to look at.</param>
        public void LookAt(in Vector2 point)
        {
            Position = point;
        }

        #region Transform Manipulations

        public void SetTranslation(in Vector2 translation)
        {
            Position = translation;
        }

        public void Translate(in Vector2 amount)
        {
            Position += amount;
        }

        public void SetRotation(in float rotation)
        {
            Rotation = rotation;
        }

        public void Rotate(in float amount)
        {
            Rotation += amount;
        }

        public void SetZoom(in float scale)
        {
            Scale = scale;
        }

        public void Zoom(in float amount)
        {
            Scale += amount;
        }

        #endregion

        /// <summary>
        /// Calculates the Camera's transform matrix using Matrix multiplication.
        /// </summary>
        /// <returns>The Matrix representing the Camera's transform.</returns>
        public Matrix TransformMatrix
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0f)) *
                            Matrix.CreateRotationZ(Rotation) *
                            Matrix.CreateScale(Scale, Scale, 1) *
                            Matrix.CreateTranslation(RenderingGlobals.BaseResolutionWidth * TranslationConstant, RenderingGlobals.BaseResolutionHeight * TranslationConstant, 0f);
            }
        }

        /// <summary>
        /// The visible area of the Camera.
        /// </summary>
        public Rectangle VisibleArea
        {
            get
            {
                Vector2 screenSize = new Vector2(ScreenBounds.Width, ScreenBounds.Height);

                Matrix invertedMatrix = Matrix.Invert(TransformMatrix);

                //Transform the camera corners and get their location in world space
                Vector2 topLeft = Vector2.Transform(Vector2.Zero, invertedMatrix);
                Vector2 topRight = Vector2.Transform(new Vector2(screenSize.X, 0), invertedMatrix);
                Vector2 bottomLeft = Vector2.Transform(new Vector2(0, screenSize.Y), invertedMatrix);
                Vector2 bottomRight = Vector2.Transform(screenSize, invertedMatrix);

                //Min and max the corners to get the rectangle around the viewable area
                Vector2 min = new Vector2(
                    Math.Min(topLeft.X, Math.Min(topRight.X, Math.Min(bottomLeft.X, bottomRight.X))),
                    Math.Min(topLeft.Y, Math.Min(topRight.Y, Math.Min(bottomLeft.Y, bottomRight.Y))));
                Vector2 max = new Vector2(
                    Math.Max(topLeft.X, Math.Max(topRight.X, Math.Max(bottomLeft.X, bottomRight.X))),
                    Math.Max(topLeft.Y, Math.Max(topRight.Y, Math.Max(bottomLeft.Y, bottomRight.Y))));

                return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
            }
        }
    }
}
