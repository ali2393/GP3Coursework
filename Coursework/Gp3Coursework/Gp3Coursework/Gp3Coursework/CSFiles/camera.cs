using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Gp3Coursework
{
    class camera
    {

       public  Matrix camViewMatrix; //Cameras view
       public Matrix camRotationMatrix; //Rotation Matrix for camera to reflect movement around Y Axis
       public Matrix projectionMatrix;
       public Matrix worldMatrix;

       public Vector3 camPosition; //Position of Camera in world
       public Vector3 camLookat; //Where the camera is looking or pointing at
       public Vector3 camTransform; //Used for repositioning the camer after it has been rotated

       public Vector3 desiredPos;

       public float camRotationSpeed; //Defines the amount of rotation
       public float camYaw,camRoll,camPitch; //Cumulative rotation on Y

       public Vector3 ship;

       public float aspectRatio;

       public Vector3 pos;

        public camera(Vector3 pos)
        {
            camPosition = pos;
        }

       
        public void InitilizeTransform(GraphicsDeviceManager graphics)
        {
            //---------------------------------------------------------------------------------------------------------------------------------------
            //Create initial camera view
            //---------------------------------------------------------------------------------------------------------------------------------------
            //camPosition = pos;
            camLookat = Vector3.Zero;
            camViewMatrix = Matrix.CreateLookAt(camPosition, camLookat, Vector3.Up);

            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45),
                aspectRatio,
                1.0f, 600.0f);

            worldMatrix = Matrix.Identity;
            camRotationSpeed = MathHelper.ToDegrees(10);
        }

 
  

        //----------------------------------------------------------------------------
        // Updates camera view
        //----------------------------------------------------------------------------
        public void camUpdate(Vector3 ship)
        {
            //camRotationMatrix = Matrix.CreateRotationY(camYaw);
            //camTransform = Vector3.Transform(Vector3.Forward, camRotationMatrix);
            //camLookat = ship;

            camRotationMatrix.Forward.Normalize();

            camRotationMatrix = Matrix.CreateRotationX(camPitch) * Matrix.CreateRotationY(camYaw) * Matrix.CreateFromAxisAngle(camRotationMatrix.Forward, camRoll);

            desiredPos = Vector3.Transform(new Vector3(0,0,1), camRotationMatrix);
            desiredPos += ship;

            camPosition = camPosition = Vector3.SmoothStep(camPosition, desiredPos, .15f);
            ship.Normalize();
            camLookat = ship;

            camYaw = MathHelper.SmoothStep(camYaw, 0f, .2f);

            camViewMatrix = Matrix.CreateLookAt(camPosition, camLookat, Vector3.Up);    
        }

        //--------------------------------------------------
        // Update the position and direction of the camera.
        //--------------------------------------------------
        public void RotateCamera(Vector3 target)
        {

            camPosition = Vector3.Transform(camPosition - target, Matrix.CreateFromAxisAngle(Vector3.UnitY,MathHelper.ToDegrees(180))) + target;
            camViewMatrix = Matrix.CreateLookAt(camPosition, target, Vector3.Up);
        }
    }
}
