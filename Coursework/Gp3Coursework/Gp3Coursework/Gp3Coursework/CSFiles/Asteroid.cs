using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gp3Coursework
{
    struct Asteroids
    {
        //see eShip.cs for code explination
        public Vector3 position;
        public Vector3 direction;
        public float speed;
        public bool isActive;
            float xStart;
            float yStart;
            float zStart;



            public void reset()
            {
                Random random = new Random();
                xStart = random.Next(-7, 8);
                yStart = random.Next(-7, 8);
                zStart = random.Next(-7, 8);

                position = new Vector3(xStart, yStart, zStart);
            }

        public void Update(float delta)
        {
            Random random=new Random();
            xStart = random.Next(-7, 8);
            yStart = random.Next(-7, 8);
            zStart = random.Next(-7, 8);

            position += direction * speed *
                        GameConstants.AsteroidSpeedAdjustment * delta ;


            if (position.Z > GameConstants.PlayfieldSizeZ)
            {
                position = new Vector3(xStart, yStart, zStart);
                if (direction.Z < 20)
                {
                    direction.Z = direction.Z + 1.5f;
                }
            }
        }
    }
}
