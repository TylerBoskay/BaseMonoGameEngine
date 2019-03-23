using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TDMonoGameEngine
{
    /// <summary>
    /// Represents a row-major tile engine.
    /// </summary>
    /// <typeparam name="T">The type for the tile engine.</typeparam>
    public class TileEngine<T> : IPosition where T : struct
    {
        /// <summary>
        /// The row-major set of tiles.
        /// </summary>
        protected T[] Tiles = null;

        /// <summary>
        /// The position of the tile engine.
        /// </summary>
        public Vector2 Position { get; set; } = Vector2.Zero;

        /// <summary>
        /// The size of each tile in the tile engine.
        /// </summary>
        public Vector2 TileSize { get; private set; } = Vector2.Zero;

        /// <summary>
        /// Half of the size of each tile in the tile engine.
        /// </summary>
        public Vector2 TileSizeHalf => TileSize / 2;

        /// <summary>
        /// The number of columns in the tile engine.
        /// </summary>
        public int Columns { get; private set; } = 0;

        /// <summary>
        /// The number of rows in the tile engine.
        /// </summary>
        public int Rows { get; private set; } = 0;

        /// <summary>
        /// The total width of the tile engine.
        /// </summary>
        public float Width => TileSize.X * Columns;

        /// <summary>
        /// The total height of the tile engine.
        /// </summary>
        public float Height => TileSize.Y * Rows;

        public TileEngine(in Vector2 position, in Vector2 tileSize, in int columns, in int rows)
        {
            Position = position;
            TileSize = tileSize;

            Columns = columns;
            Rows = rows;

            Tiles = new T[columns * rows];
        }

        /// <summary>
        /// Gets the tile at the specified column and row.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="row">The row.</param>
        /// <returns>The tile at the value.</returns>
        public T GetTile(in int column, in int row)
        {
            int index = GetIndex(column, row);
            return Tiles[index];
        }

        /// <summary>
        /// Returns an index of a tile in the tile engine, given a particular column and row.
        /// </summary>
        /// <param name="column">The column of the tile.</param>
        /// <param name="row">The row of the tile.</param>
        /// <returns>An integer representing the index of the tile at the given column and row.</returns>
        public int GetIndex(in int column, in int row)
        {
            return (row * Columns) + column;
        }

        /// <summary>
        /// Gets the tile at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The tile at the value.</returns>
        public T GetTile(in int index)
        {
            return Tiles[index];
        }

        /// <summary>
        /// Returns a column and row in the tile engine from a particular tile index.
        /// </summary>
        /// <param name="index">The index of the tile.</param>
        /// <param name="column">The returned column of the tile.</param>
        /// <param name="row">The returned row of the tile.</param>
        public void GetColRowFromIndex(in int index, out int column, out int row)
        {
            //Perform Modulo to obtain the column number and division to obtain the row number
            column = index % Columns;
            row = index / Columns;
        }

        /// <summary>
        /// Returns a column and row in the tile engine from an object's position.
        /// </summary>
        /// <param name="objPosition">The position of the object.</param>
        /// <param name="column">The returned column of the tile.</param>
        /// <param name="row">The returned row of the tile.</param>
        public void GetColRowFromPos(in Vector2 objPosition, out int column, out int row)
        {
            //Subtract the tile engine's position from the object's position to get where the object is relative in the tile engine
            Vector2 coordinates = (objPosition - Position) / TileSize;
            column = (int)coordinates.X;
            row = (int)coordinates.Y;
        }

        /// <summary>
        /// Gets the tile associated with the position.
        /// </summary>
        /// <param name="objPosition">The position of the object.</param>
        /// <returns>The tile corresponding to the position of the object.</returns>
        public T GetTile(in Vector2 objPosition)
        {
            GetColRowFromPos(objPosition, out int tileX, out int tileY);

            return GetTile(tileX, tileY);
        }

        /// <summary>
        /// Sets the tile at a particular column and row to the designated value.
        /// </summary>
        /// <param name="column">The column of the tile.</param>
        /// <param name="row">The row of the tile.</param>
        /// <param name="value">The value to change the tile to.</param>
        public void ChangeTileAtRowCol(in int column, in int row, in T value)
        {
            int index = GetIndex(column, row);

            ChangeTileAtIndex(index, value);
        }

        /// <summary>
        /// Sets the tile at a particular index to the designated value.
        /// </summary>
        /// <param name="index">The index of the tile.</param>
        /// <param name="value">The value to change the index to.</param>
        public void ChangeTileAtIndex(in int index, in T value)
        {
            Tiles[index] = value;
        }
    }
}