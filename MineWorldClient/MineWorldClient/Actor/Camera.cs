using System;
using Microsoft.Xna.Framework;

namespace MineWorld.Actor
{
    public class Camera
    {
        // Properties
        Vector3 _pos, _ang;
        Matrix _view, _proj;

        const float PiOver2 = MathHelper.PiOver2;
        const float TwoPi = MathHelper.TwoPi;

        /// <summary>
        /// Creates a 3D camera object
        /// </summary>
        /// <param name="game">Game reference</param>
        /// <param name="pos">Camera position</param>
        /// <param name="ang">Camera angle</param>
        public Camera(PropertyBag game, Vector3 pos, Vector3 ang)
        {
            _pos = pos;
            _ang = ang;
            SetPerspective(MathHelper.ToRadians(90),game.GameManager.Device.Viewport.AspectRatio, 0.01f, 100000.0f);
        }

        public void Update()
        {
            // Use modulus on angles to keep values between (0 - 2PI) radians
            _ang.X = MathHelper.Clamp(_ang.X, -PiOver2 + 0.01f, PiOver2 - 0.01f);// Clamp pitch
            _ang.Y %= TwoPi;
            _ang.Z %= TwoPi;

            // Create view matrix
            _view = Matrix.Identity *
                   Matrix.CreateTranslation(-_pos) *
                   Matrix.CreateRotationZ(_ang.Z) *
                   Matrix.CreateRotationY(_ang.Y) *
                   Matrix.CreateRotationX(_ang.X);
        }

        /// <summary>
        /// Gets or sets the position of the camera in 3D space
        /// </summary>
        public Vector3 Position
        {
            get { return _pos; }
            set { _pos = value; }
        }

        /// <summary>
        /// Gets or sets the local angle of the camera
        /// </summary>
        public Vector3 Angle
        {
            get { return _ang; }
            set { _ang = value; }
        }

        /// <summary>
        /// Gets or sets the camera's pitch
        /// </summary>
        public float Pitch
        {
            get { return _ang.X; }
            set
            {
                _ang.X = value;
            }
        }

        /// <summary>
        /// Gets or sets the camera's yaw
        /// </summary>
        public float Yaw
        {
            get { return _ang.Y; }
            set { _ang.Y = value; }
        }

        /// <summary>
        /// Gets or sets the camera's roll
        /// </summary>
        public float Roll
        {
            get { return _ang.Z; }
            set { _ang.Z = value; }
        }

        /// <summary>
        /// Gets the forward direction of the camera
        /// </summary>
        public Vector3 Forward
        {
            get
            {
                return Vector3.Normalize(
                    new Vector3(
                        -(float)(Math.Sin(_ang.Z) * Math.Sin(_ang.X) + Math.Cos(_ang.Z) * Math.Sin(_ang.Y) * Math.Cos(_ang.X)),
                        -(float)(-Math.Cos(_ang.Z) * Math.Sin(_ang.X) + Math.Sin(_ang.Z) * Math.Sin(_ang.Y) * Math.Cos(_ang.X)),
                        (float)(Math.Cos(_ang.Y) * Math.Cos(_ang.X))
                    )
                );
            }
        }

        /// <summary>
        /// Gets the right direction of the camera
        /// </summary>
        public Vector3 Right
        {
            get
            {
                return Vector3.Normalize(
                    Vector3.Cross(Vector3.Up, Forward)
                );
            }
        }

        /// <summary>
        /// Gets the view matrix
        /// </summary>
        public Matrix View
        {
            get { return _view; }
        }

        /// <summary>
        /// Gets the projection matrix
        /// </summary>
        public Matrix Projection
        {
            get { return _proj; }
        }

        /// <summary>
        /// Sets the perspective for the camera
        /// </summary>
        /// <param name="fov">Field of view</param>
        /// <param name="aspratio">Aspect ratio</param>
        /// <param name="znear">Near clipping plane</param>
        /// <param name="zfar">Far clipping plane</param>
        public void SetPerspective(float fov, float aspratio, float znear, float zfar)
        {
            // Create projection matrix
            _proj = Matrix.CreatePerspectiveFieldOfView(fov, aspratio, znear, zfar);
        }

        /// <summary>
        /// Adds offset to the camera angle each time this is called
        /// </summary>
        /// <param name="pitch">Pitch</param>
        /// <param name="yaw">Yaw</param>
        /// <param name="roll">Roll</param>
        public void Rotate(float pitch, float yaw, float roll)
        {
            _ang.X += pitch;
            _ang.Y += yaw;
            _ang.Z += roll;
        }

        /// <summary>
        /// Retrieves a string representation of the current object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Position: [{0}, {1}, {2}]\nAngle: [{3}, {4}, {5}]",
                Math.Floor(_pos.X), Math.Floor(_pos.Y), Math.Floor(_pos.Z),
                Math.Round(_ang.X, 2), Math.Round(_ang.Y, 2), Math.Round(_ang.Z, 2)
            );
        }

    }
}


