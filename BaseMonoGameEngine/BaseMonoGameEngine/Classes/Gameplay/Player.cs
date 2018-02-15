﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// Player object
    /// </summary>
    public class Player : SceneObject
    {
        public Vector2 Speed = Vector2.One;

        private Sprite playerSprite = null;
        private SpriteRenderer spriteRenderer = null;

        public Player()
        {
            playerSprite = new Sprite(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}Will.png"),
                new Rectangle(210, 374, 25, 38));

            spriteRenderer = new SpriteRenderer(transform, playerSprite);
            spriteRenderer.Depth = .1f;

            renderer = spriteRenderer;
        }

        public override void CleanUp()
        {

        }

        public override void Update()
        {
            HandleMove();
            ChangeColor();
        }

        private void ChangeColor()
        {
            if (Input.GetButtonDown(0, InputActions.A) == true)
            {
                spriteRenderer.TintColor = Color.White;
            }
            else if (Input.GetButtonDown(0, InputActions.B) == true)
            {
                spriteRenderer.TintColor = Color.Red;
            }
            else if (Input.GetButtonDown(0, InputActions.X) == true)
            {
                spriteRenderer.TintColor = Color.GreenYellow;
            }
            else if (Input.GetButtonDown(0, InputActions.Y) == true)
            {
                spriteRenderer.TintColor = Color.Blue;
            }
        }

        private void HandleMove()
        {
            Vector2 moveAmt = Vector2.Zero;

            moveAmt.X = Input.GetAxis(0, InputActions.Horizontal) * Speed.X;
            moveAmt.Y = Input.GetAxis(0, InputActions.Vertical) * Speed.Y;

            if (moveAmt != Vector2.Zero)
            {
                transform.Position += moveAmt;
            }
        }
    }
}
