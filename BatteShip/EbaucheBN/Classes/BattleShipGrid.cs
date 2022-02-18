﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbaucheBN
{
    public class BattleShipGrid
    {
        public Cell[,] grid = new Cell[Constants.GridSizeX, Constants.GridSizeY];
        public void initGrid(bool Ally)
        {
            for (int i = 0; i < Constants.GridSizeX; i++)
            {
                for (int j = 0; j < Constants.GridSizeY; j++)
                {
                    grid[i, j] = new Cell(cellType.Water, Ally);
                    grid[i, j].initCell(i, j);
                }
            }
        }

        public Cell getCell(int x, int y)
        {
            return grid[x, y];
        }

        public void DisableGridButtons()
        {
            foreach(Cell cell in grid)
            {
                cell.cellButton.IsEnabled = false;
            }
        }
    }
}
