using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TDMonoGameEngine
{
    /// <summary>
    /// A test game HUD. Tests drawing health in a unique way (Illusion of Gaia/Time's method)
    /// </summary>
    public class TestGameHUD : SceneObject
    {
        private HUDRenderer hudRenderer = null;

        private double UpdateRate = 100d;
        private double ElapsedTime = 0d;

        public int DisplayHealth = 0;
        public int MaxHealth = 0;

        public Sprite EmptyHealth { get; private set; } = null;
        public Sprite HalfHealth { get; private set; } = null;
        public Sprite FullHealth { get; private set; } = null;

        private Player PlayerRef = null;

        public Vector2 BaseHealthPos = new Vector2(200, 30);
        public int NumPerRow = 10;

        public TestGameHUD(Player player, int renderOrder)
        {
            PlayerRef = player;

            DisplayHealth = 0;
            MaxHealth = PlayerRef.MaxHealth;

            hudRenderer = new HUDRenderer(this);

            renderer = hudRenderer;
            renderer.Order = renderOrder;

            Texture2D tex = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}HUD.png");

            EmptyHealth = new Sprite(tex, new Rectangle(220, 47, 4, 4));
            HalfHealth = new Sprite(tex, new Rectangle(228, 46, 5, 5));
            FullHealth = new Sprite(tex, new Rectangle(237, 45, 8, 8));
        }

        public override void CleanUp()
        {
            base.CleanUp();

            PlayerRef = null;
        }

        public override void Update()
        {
            ElapsedTime += Time.ElapsedMilliseconds;

            if (ElapsedTime >= UpdateRate)
            {
                //Slowly increment the display health
                if (DisplayHealth > PlayerRef.Health)
                    DisplayHealth--;
                else if (DisplayHealth < PlayerRef.Health)
                    DisplayHealth++;

                ElapsedTime = 0d;
            }
            MaxHealth = PlayerRef.MaxHealth;
        }
    }
}
