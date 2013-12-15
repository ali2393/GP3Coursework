using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gp3Coursework
{
    struct Laser
    {
        public Vector3 position;
        public Vector3 direction;
        public float speed;
        public bool isActive;

        //moves laser and diables it to be shot again when it moves outside the playfield
        public void Update(float delta)
        {
            position += direction * speed *
                        GameConstants.LaserSpeedAdjustment * delta;
            if (position.X > GameConstants.PlayfieldSizeX ||
                position.X < -GameConstants.PlayfieldSizeX ||
                position.Z > GameConstants.PlayfieldSizeZ ||
                position.Z < -GameConstants.PlayfieldSizeZ)
                isActive = false;
        }
    }
}
