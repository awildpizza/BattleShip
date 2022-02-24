﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace EbaucheBN.Classes
{
    internal class GameDesign
    {
        public const int GridSizeX = 13;
        public const int GridSizeY = 13;

        public const int GridHeight = 700;
        public const int GridWidth = 700;

        public static Ship Scout = new Ship("Scout", 3);
        public static Ship Scout2 = new Ship("Scout", 3);
        public static Ship Submarine = new Ship("Submarine", 3);
        public static Ship Destroyer = new Ship("Destroyer", 4);
        public static Ship PlaneCarrier = new Ship("Plane\nCarrier", 5);

        public static readonly HashSet<Ship> ShipList = new HashSet<Ship>()
        {
            Scout,
            Scout2,
            Submarine,
            Destroyer,
            PlaneCarrier
        };
        public static int ShipCount = ShipList.Count;
        public static readonly Ship[] ShipArray = ShipList.ToArray();

        public static Windows.UI.Color DefaultColor = Colors.LightGray;
        public static Windows.UI.Color ShipColor = Colors.DarkOrange;
        public static Windows.UI.Color WaterColor = Colors.SkyBlue;
        public static Windows.UI.Color ShipSelectColor = Colors.MediumPurple;
        public static Windows.UI.Color CellSelectColor = Colors.MediumPurple;
        public static Windows.UI.Color ShipCoordColor = Colors.Orange;
        public static Windows.UI.Color ResetButtonColor = Colors.Red;

        public static int MaxSize = maxSize();

        public static int maxSize()
        {
            int max = 0;

            for (int count = 0; count < ShipList.Count; count++)
            {
                if(max < ShipArray[count].Size)
                    max = ShipArray[count].Size;
            }

            return max;
        }
    }
}
