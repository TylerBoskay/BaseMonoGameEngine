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
    /// <para>This is a Singleton for easy access.</para>
    /// </summary>
    public class Camera2D
    {
        #region Singleton Fields

        public static Camera2D Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Camera2D();
                }

                return instance;
            }
        }

        public static bool HasInstance => (instance != null);

        private static Camera2D instance = null;

        #endregion

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

        private Camera2D()
        {
            SetTransform(new Vector2(0f, 0f), 0f, 1f);
        }

        public void SetTransform(Vector2 position, float rotation, float scale)
        {
            SetTranslation(position);
            SetRotation(rotation);
            SetZoom(scale);
        }

        public void SetBounds(Rectangle bounds)
        {
            ScreenBounds = bounds;
        }

        /// <summary>
        /// Tells if bounds are in the Camera's view.
        /// </summary>
        /// <param name="bounds">A Rectangle representing the bounding region to check is in the Camera's view.</param>
        /// <returns>true if the bounds is in the Camera's view, otherwise false.</returns>
        public bool IsInCameraView(Rectangle bounds)
        {
            return VisibleArea.Intersects(bounds);
        }

        /// <summary>
        /// Transforms a point from screen space to world space.
        /// </summary>
        /// <param name="point">The point in screen space.</param>
        /// <returns>A Vector2 of the point in world space.</returns>
        public Vector2 ScreenToWorldSpace(Vector2 point)
        {
            Matrix invertedMatrix = Matrix.Invert(TransformMatrix);
            return Vector2.Transform(point, invertedMatrix);
        }

        /// <summary>
        /// Transforms a point from world space to screen space.
        /// </summary>
        /// <param name="point">The point in world space.</param>
        /// <returns>A Vector2 of the point in screen space.</returns>
        public Vector2 WorldToScreenSpace(Vector2 point)
        {
            return Vector2.Transform(point, TransformMatrix);
        }

        /// <summary>
        /// Makes the camera look at a point in world space.
        /// </summary>
        /// <param name="point">The point to look at.</param>
        public void LookAt(Vector2 point)
        {
            Position = point - new Vector2(ScreenBounds.Width * TranslationConstant, ScreenBounds.Height * TranslationConstant);
        }

        #region Transform Manipulations

        public void SetTranslation(Vector2 translation)
        {
            Position = translation;
        }

        public void Translate(Vector2 amount)
        {
            Position += amount;
        }

        public void SetRotation(float rotation)
        {
            Rotation = rotation;
        }

        public void Rotate(float amount)
        {
            Rotation += amount;
        }

        public void SetZoom(float scale)
        {
            Scale = scale;
        }

        public void Zoom(float amount)
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
                            Matrix.CreateScale(Scale) *
                            Matrix.CreateTranslation(ScreenBounds.Width * TranslationConstant, ScreenBounds.Height * TranslationConstant, 0f);
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

                //Transform the camera corners and get their location in world space
                Vector2 topLeft = ScreenToWorldSpace(Vector2.Zero);
                Vector2 topRight = ScreenToWorldSpace(new Vector2(screenSize.X, 0));
                Vector2 bottomLeft = ScreenToWorldSpace(new Vector2(0, screenSize.Y));
                Vector2 bottomRight = ScreenToWorldSpace(screenSize);

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
