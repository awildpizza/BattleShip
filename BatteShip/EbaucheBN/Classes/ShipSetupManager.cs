﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace EbaucheBN.Classes
{
    public class ShipSetupManager
    {
        public HashSet<ShipSelection> buttons = new HashSet<ShipSelection>();
        public TextBlock ShipSelectionText = new TextBlock();
        private Grid UI, SelectionUI;
        public StartGameButton startGameButton;

        public ShipSelection CurrentShip;

        public List<Cell> ShipCells = new List<Cell>();
        private List<Cell> CurrentShipCells = new List<Cell>();

        public void Initialize()
        {
            UI = MainPage.Instance.GetShipSetupUI();
            SelectionUI = MainPage.Instance.GetShipSelectionSetupUI();
            InitializeInstructionText();
            InitializeShipSelection();
        }
        private void InitializeInstructionText()
        {
            Button ShipSelectionButton = new Button();

            ShipSelectionText.Text = "Select a ship to set its coordinates";
            ShipSelectionText.Foreground = new SolidColorBrush(Colors.White);
            ShipSelectionText.FontSize = UI.Width / ShipSelectionText.ToString().Length;

            ShipSelectionButton.Width = UI.Width;
            ShipSelectionButton.Height = UI.Height;
            ShipSelectionButton.Content = ShipSelectionText;
            ShipSelectionButton.IsEnabled = false;

            UI.Children.Add(ShipSelectionButton);
        }
        private void InitializeShipSelection()
        {
            for (int ColumnAmount = 0; ColumnAmount < GameDesign.MaxSize + 2; ColumnAmount++)
            {
                SelectionUI.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int ShipCount = 0; ShipCount < GameDesign.ShipCount; ShipCount++)
            {
                SelectionUI.RowDefinitions.Add(new RowDefinition());

                ShipSelection newShipSelectionButton = new ShipSelection(ShipCount, SelectionUI, GameDesign.ShipArray[ShipCount]);
                buttons.Add(newShipSelectionButton);
            }

            startGameButton = new StartGameButton(SelectionUI);
        }
        public void GetAllyShipCoordinates()
        {
            foreach (ShipSelection shipSelection in buttons)
            {
                shipSelection.ship.Position.AddRange(shipSelection.ShipCells);
            }
        }
        public void ChangeAllyShipsColor()
        {
            foreach (Ship allyShip in MainPage.Instance.allyShips)
            {
                foreach (Cell cell in allyShip.Position)
                    cell.cellButton.Background = new SolidColorBrush(GameDesign.InGameShipColor);
            }
        }
        public void Click(ShipSelection ButtonClicked)
        {
            ShipSelectionText.Text = "Now Click on the grid to select the ship's coordinates";
            ResetCurrentBoatSelection();
            UpdateButtonColor(ButtonClicked);
            StartCoordinateSelection(ButtonClicked);
        }
        public void UpdateButtonColor(ShipSelection ButtonClicked)
        {
            foreach (ShipSelection Button in buttons)
            {
                if (Button != ButtonClicked)
                    Button.ShipButton.Background = new SolidColorBrush(Button.CurrentColor);
                else
                    Button.ShipButton.Background = new SolidColorBrush(GameDesign.ShipSelectColor);
            }
        }
        public void StartCoordinateSelection(ShipSelection ButtonClicked)
        {
            MainPage.Instance.Selection = true;
            CurrentShip = ButtonClicked;
        }
        public void ResetCurrentBoatSelection()
        {
            if (CurrentShipCells.Count > 0)
            {
                foreach (Cell cell in CurrentShipCells)
                {
                    ShipCells.Remove(cell);
                    cell.cellButton.Background = new SolidColorBrush(GameDesign.DefaultColor);
                }

                foreach (Coordinate coord in CurrentShip.Coordinates)
                {
                    coord.TextBlock.Text = string.Empty;
                }

                CurrentShipCells.Clear();
            }
        }
        public void ProceedShipSetup(Cell CellClicked)
        {
            if (!ShipCells.Contains(CellClicked))
            {
                CellClicked.cellButton.Background = new SolidColorBrush(GameDesign.CellSelectColor);

                ShipCells.Add(CellClicked);

                CurrentShipCells.Add(CellClicked);
                CurrentShipCells.Sort(CompareCellsByCoordinate);

                for (int count = 0; count < CurrentShipCells.Count; count++)
                {
                    CurrentShip.Coordinates[count].TextBlock.Text = CurrentShipCells[count].cellCoordinate;
                }

                if (CurrentShipCells.Count == CurrentShip.ShipSize)
                {
                    MainPage.Instance.Selection = false;
                    CurrentShip.ShipButton.Background = new SolidColorBrush(GameDesign.DefaultColor);

                    if (VerifyCoordinates())
                    {
                        for (int count = 0; count < CurrentShip.ShipSize; count++)
                        {
                            CurrentShip.Coordinates[count].Button.Background = new SolidColorBrush(GameDesign.ShipCoordColor);
                            CurrentShipCells[count].cellButton.Background = new SolidColorBrush(GameDesign.ShipColor);
                        }

                        CurrentShip.ShipCells.AddRange(CurrentShipCells);
                        CurrentShip.Complete = true;

                        CurrentShip.ShipName.Foreground = new SolidColorBrush(Colors.White);
                        CurrentShip.ShipButton.Background = new SolidColorBrush(GameDesign.ShipColor);
                        CurrentShip.CurrentColor = GameDesign.ShipColor;

                        CurrentShip.reset.button.IsEnabled = true;
                        CurrentShip.reset.button.Background = new SolidColorBrush(GameDesign.ResetButtonColor);
                        CurrentShip.reset.textBlock.Foreground = new SolidColorBrush(Colors.White);
                    }
                    else
                    {
                        for (int count = 0; count < CurrentShipCells.Count; count++)
                        {
                            ShipCells.Remove(CurrentShipCells[count]);
                            CurrentShip.Coordinates[count].TextBlock.Text = string.Empty;
                            CurrentShipCells[count].cellButton.Background = new SolidColorBrush(GameDesign.DefaultColor);
                        }
                    }

                    ShipSelectionText.Text = "Select a ship to set its coordinates";
                    CurrentShipCells.Clear();
                    TestSelectionComplete();
                }
            }
        }
        private void TestSelectionComplete()
        {
            if (ShipCells.Count == GameDesign.TotalSize)
            {
                ShipSelectionText.Text = "Press Play Game to play !";
                startGameButton.button.IsEnabled = true;
                startGameButton.textBlock.Foreground = new SolidColorBrush(Colors.White);
            }
        }
        private bool VerifyCoordinates()
        {
            int difference = CurrentShipCells[1].cellIndex - CurrentShipCells[0].cellIndex;

            if (Math.Abs(difference) != 1 && Math.Abs(difference) != GameDesign.GridSizeX)
                return false;

            for (int coordIndex = 0; coordIndex < CurrentShip.ShipSize - 1; coordIndex++)
            {
                if (CurrentShipCells[coordIndex].cellIndex % GameDesign.GridSizeX == 0 && Math.Abs(difference) == 1)
                    return false;
                if (CurrentShipCells[coordIndex + 1].cellIndex - CurrentShipCells[coordIndex].cellIndex != difference)
                    return false;
            }

            return true;
        }
        private static int CompareCellsByCoordinate(Cell cell1, Cell cell2)
        {
            if (cell1 == null)
            {
                if (cell2 == null)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (cell2 == null)
                    return 1;
                else
                {
                    return cell1.cellIndex.CompareTo(cell2.cellIndex);
                }
            }
        }
    }
}