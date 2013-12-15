using System;
using System.Collections.Generic;
using System.Text;

namespace Gp3Coursework
{
    static class GameConstants
    {
        //camera constants
        public const float CameraHeight = 25000.0f;
        public const float PlayfieldSizeX = 100f;
        public const float PlayfieldSizeZ = 300f;

        //Asteroid constants
        public const int NumAsteroids = 10;
        public const float AsteroidMinSpeed = 3.0f;
        public const float AsteroidMaxSpeed = 10.0f;
        public const float AsteroidSpeedAdjustment = 2.5f;

        //Scale Constants
        public const float ShipScaler = 0.005f;
        public const float AsteroidScalar = 0.001f;
        public const float LaserScalar = 3.0f;
        public const float StarScalar = 0.001f;
        public const float EShipScalar = 0.03f;

        //collision constants
        public const float AsteroidBoundingSphereScale =0.06f;  //50% size
        public const float ShipBoundingSphereScale = 0.5f;  //50% size
        public const float LaserBoundingSphereScale = 0.85f;  //50% size
        public const float EShipBoundingSphereScale = 0.03f;

        //bullet constants
        public const int NumLasers = 30;
        public const float LaserSpeedAdjustment = 5.0f;

        //star constants
        public const int NumStars = 15;
        public const float StarMinSpeed = 3.0f;
        public const float StarMaxSpeed = 10.0f;
        public const float StarSpeedAdjustment = 2.5f;


        //eShip constants
        public const int NumEShips = 6;
        public const float EShipsMinSpeed = 3.0f;
        public const float EShipsMaxSpeed = 10.0f;
        public const float EShipSpeedAdjustment = 2.5f;
    }
}
