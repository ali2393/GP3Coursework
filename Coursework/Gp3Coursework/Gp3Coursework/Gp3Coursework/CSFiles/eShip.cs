using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gp3Coursework
{
    struct EShips
    {
        //sets up variables to be used
        public Vector3 position;
        public Vector3 direction;
        public float speed;
        public bool isActive;
        float xStart;
        float yStart;
        float zStart;

        //updates the position of the ship
        public void Update(float delta)
        {
            //randomises the postion of the ship
            Random random = new Random();
            xStart = random.Next(-7, 8);
            yStart = random.Next(-7, 8);
            zStart = random.Next(-7, 8);

            //moves the ship
            position += direction * speed *
                        GameConstants.EShipSpeedAdjustment * delta;

            //when the ship goes behind the screen reset its position 
            if (position.Z > GameConstants.PlayfieldSizeZ)
            {
                position = new Vector3(xStart, yStart, zStart);
                //increase the ships speed to a set amount everytime it is reset
                if (direction.Z < 4)
                {
                    direction.Z = direction.Z + 1.5f;
                }
            }
        }

        //resets the position of the ship(called when it is shot)
        public void reset()
        {
            Random random = new Random();
            xStart = random.Next(-7, 8);
            yStart = random.Next(-7, 8);
            zStart = random.Next(-7, 8);

            position = new Vector3(xStart, yStart, zStart);
        }
    }
}
