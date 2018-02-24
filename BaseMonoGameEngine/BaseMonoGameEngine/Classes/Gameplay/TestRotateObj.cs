using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaseMonoGameEngine
{
    public class TestRotateObj : SceneObject
    {
        private SceneObject RotateAround;
        private double Angle = 0d;
        private double Distance = 50d;

        public TestRotateObj(SceneObject rotateAround)
        {
            RotateAround = rotateAround;

            Texture2D tex = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Enemies.png");
            renderer = new SpriteRenderer(transform, new Sprite(tex, new Rectangle(2, 397, 16, 16)));
        }

        public override void Update()
        {
            transform.Position = UtilityGlobals.GetPointAroundCircle(new Circle(RotateAround.transform.Position, Distance), Angle, true);
            Angle = (Angle + 1) % 360d;
        }
    }
}
