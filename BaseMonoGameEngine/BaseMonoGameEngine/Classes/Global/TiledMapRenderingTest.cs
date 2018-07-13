using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;

namespace TDMonoGameEngine
{
    /// <summary>
    /// Testing rendering for Tiled maps.
    /// </summary>
    public sealed class TiledMapRenderingTest
    {
        private TiledMap Map = null;

        private int NumTilesWidth => Map.Width;
        private int NumTilesHeight => Map.Height;

        public TiledMapRenderingTest(in TiledMap tiledMap)
        {
            Map = tiledMap;
        }

        public void DrawMap()
        {
            Camera2D camera = SceneManager.Instance.ActiveScene.Camera;

            RenderingManager.Instance.StartBatch(RenderingManager.Instance.spriteBatch, SpriteSortMode.Deferred, null, null, null, null, null, camera.TransformMatrix);

            //Layers are ordered by depth from top to bottom in the .tmx file
            for (int i = 0; i < Map.TileLayers.Count; i++)
            {
                TiledMapTileLayer layer = Map.TileLayers[i];

                //Draw all tiles in each layer
                for (int j = 0; j < layer.Tiles.Count; j++)
                {
                    //If the tile is blank, ignore it
                    if (layer.Tiles[j].IsBlank == true) continue;

                    TiledMapTile tile = layer.Tiles[j];

                    //Find which tileset this tile belongs to
                    TiledMapTileset tileSet = Map.GetTilesetByTileGlobalIdentifier(tile.GlobalIdentifier);

                    //Continue if the tileset wasn't found or its texture is null
                    if (tileSet == null || tileSet.Texture == null) continue;
                    
                    //For some reason, Tiled stores the GlobalIdentifier as 1 more than what it actually is
                    Rectangle getRegion = tileSet.GetTileRegion(tile.GlobalIdentifier - 1);

                    Vector2 pos = new Vector2(((j % NumTilesWidth) * Map.TileWidth), (j / NumTilesWidth) * Map.TileHeight);

                    RenderingManager.Instance.DrawSprite(tileSet.Texture, pos, getRegion, Color.White * layer.Opacity, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
                }
            }

            RenderingManager.Instance.EndCurrentBatch();
        }
    }
}
