using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// The game camera.
    /// </summary>
    public class Camera2D : ICleanup
    {
        /// <summary>
        /// The constant for translating the camera to the center of the screen.
        /// </summary>
        private const float TranslationConstant = .5f;

        /// <summary>
        /// The scale for translating the camera relative to the screen, from 0 to 1.
        /// At a value of 0.5, (0,0) is the center of the screen.
        /// </summary>
        public float TranslationScale = TranslationConstant;

        /// <summary>
        /// The position of the camera.
        /// </summary>
        public Vector2 Position { get; protected set; } = Vector2.Zero;

        /// <summary>
        /// The scale of the Camera. Negative values will flip everything on-screen.
        /// </summary>
        public float Scale { get; protected set; } = 1f;

        /// <summary>
        /// The rotation of the camera.
        /// </summary>
        public float Rotation { get; protected set; } = 0f;

        /// <summary>
        /// The screen bounds of the camera.
        /// </summary>
        public Rectangle ScreenBounds { get; private set; } = Rectangle.Empty;

        /// <summary>
        /// The default position of the camera.
        /// </summary>
        public Vector2 DefaultPosition = Vector2.Zero;

        #region Cached Fields

        private Matrix CachedMatrix = Matrix.Identity;

        private Rectangle CachedVisibleArea = Rectangle.Empty;

        private bool MatrixDirty = true;

        private bool VisibleAreaDirty = true;

        #endregion

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
            if (ScreenBounds == bounds)
                return;

            ScreenBounds = bounds;
            MatrixDirty = true;
            VisibleAreaDirty = true;
        }

        public static Vector2 ClampCamera(in Vector2 cameraPos, RectangleF cameraBounds, RectangleF rectBounds)
        {
            Vector2 boundsTranslated = cameraBounds.Size * TranslationConstant;
            RectangleF curBounds = new RectangleF(cameraPos.X - boundsTranslated.X, cameraPos.Y - boundsTranslated.Y, cameraBounds.Width, cameraBounds.Height);

            if (curBounds.X < rectBounds.X) curBounds.X = rectBounds.X;
            if (curBounds.Right > rectBounds.Right) curBounds.X -= (curBounds.Right - rectBounds.Right);
            if (curBounds.Y < rectBounds.Y) curBounds.Y = rectBounds.Y;
            if (curBounds.Bottom > rectBounds.Bottom) curBounds.Y -= (curBounds.Bottom - rectBounds.Bottom);

            return curBounds.Center;
        }

        public void ClampCamera(in RectangleF rectBounds)
        {
            SetTranslation(ClampCamera(Position, ScreenBounds, rectBounds));
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
        public bool IsInCameraView(Rectangle visibleArea, in Rectangle bounds)
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
            SetTranslation(point);
        }

        /// <summary>
        /// Makes the camera look at a point in world space and clamps it to the designated bounds.
        /// </summary>
        /// <param name="point">The point to look at.</param>
        /// <param name="rectBounds">The bounds to clamp the camera in.</param>
        public void LookAtAndClamp(in Vector2 point, in RectangleF rectBounds)
        {
            LookAt(point);
            ClampCamera(rectBounds);
        }

        #region Transform Manipulations

        public void SetTranslation(in Vector2 translation)
        {
            if (Position == translation)
                return;

            Position = translation;
            MatrixDirty = true;
            VisibleAreaDirty = true;
        }

        public void Translate(in Vector2 amount)
        {
            if (amount == Vector2.Zero)
                return;

            Position += amount;
            MatrixDirty = true;
            VisibleAreaDirty = true;
        }

        public void SetRotation(in float rotation)
        {
            if (Rotation == rotation)
                return;

            Rotation = rotation;
            MatrixDirty = true;
            VisibleAreaDirty = true;
        }

        public void Rotate(in float amount)
        {
            if (amount == 0f)
                return;

            Rotation += amount;
            MatrixDirty = true;
            VisibleAreaDirty = true;
        }

        public void SetZoom(in float scale)
        {
            if (Scale == scale)
                return;

            Scale = scale;
            MatrixDirty = true;
            VisibleAreaDirty = true;
        }

        public void Zoom(in float amount)
        {
            if (amount == 0f)
                return;

            Scale += amount;
            MatrixDirty = true;
            VisibleAreaDirty = true;
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
                if (MatrixDirty == true)
                {

                    CachedMatrix = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0f)) *
                                Matrix.CreateRotationZ(Rotation) *
                                Matrix.CreateScale(Scale, Scale, 1) *
                                Matrix.CreateTranslation(RenderingGlobals.BaseResolutionWidth * TranslationScale, RenderingGlobals.BaseResolutionHeight * TranslationScale, 0f);

                    MatrixDirty = false;
                }

                return CachedMatrix;
            }
        }

        /// <summary>
        /// The visible area of the Camera.
        /// </summary>
        public Rectangle VisibleArea
        {
            get
            {
                if (VisibleAreaDirty == true)
                {
                    Vector2 screenSize = new Vector2(ScreenBounds.Width, ScreenBounds.Height);

                    //Invert the matrix to transform the camera in screen space to world space
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

                    CachedVisibleArea = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));

                    VisibleAreaDirty = false;
                }

                return CachedVisibleArea;
            }
        }
    }
}
