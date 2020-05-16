using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// An animation manager.
    /// </summary>
    public class AnimManager
    {
        /// <summary>
        /// The Sprite to use for the animations.
        /// </summary>
        public Sprite AnimSprite { get; private set; } = null;

        /// <summary>
        /// The animations, referred to by string.
        /// </summary>
        protected readonly Dictionary<string, SimpleAnimation> Animations = new Dictionary<string, SimpleAnimation>();
        
        /// <summary>
        /// The current animation being played.
        /// </summary>
        public SimpleAnimation CurrentAnim { get; private set; } = null;

        public AnimManager(Sprite animSprite)
        {
            AnimSprite = animSprite;
        }

        /// <summary>
        /// Adds an animation.
        /// If an animation already exists, it will be replaced.
        /// </summary>
        /// <param name="animName">The name of the animation.</param>
        /// <param name="anim">The animation reference.</param>
        public void AddAnimation(string animName, SimpleAnimation anim)
        {
            //Return if trying to add null animation
            if (anim == null)
            {
                Debug.LogError($"Trying to add null animation called \"{animName}\", so it won't be added");
                return;
            }

            if (Animations.ContainsKey(animName) == true)
            {
                Debug.LogWarning($"There already is an animation called \"{animName}\" and will be replaced");

                //Clear the current animation reference if it is the animation being removed
                SimpleAnimation prevAnim = Animations[animName];
                if (CurrentAnim == prevAnim)
                {
                    CurrentAnim = null;
                }

                Animations.Remove(animName);
            }

            //Set the Animation's Sprite to this one if it's not defined
            if (anim.SpriteToChange == null)
                anim.SpriteToChange = AnimSprite;

            //Set the animation's key
            anim.SetKey(animName);

            Animations.Add(animName, anim);

            //Play the first animation that gets added by default
            //This allows us to safely have a valid animation reference at all times, provided at least one is added
            if (CurrentAnim == null)
            {
                PlayAnimation(animName);
            }
        }

        /// <summary>
        /// Gets an animation by name.
        /// </summary>
        /// <param name="animName">The name of the animation.</param>
        /// <returns>An animation if found, otherwise null.</returns>
        public SimpleAnimation GetAnimation(string animName)
        {
            //If animation cannot be found
            if (Animations.ContainsKey(animName) == false)
            {
                Debug.LogError($"Cannot find animation called \"{animName}\" to play");
                return null;
            }

            return Animations[animName];
        }

        /// <summary>
        /// Gets a set of animations by their names.
        /// </summary>
        /// <param name="animNames">The names of the animations.</param>
        /// <returns>An array of animations. If none were found, an empty array.</returns>
        public SimpleAnimation[] GetAnimations(params string[] animNames)
        {
            List<SimpleAnimation> animations = new List<SimpleAnimation>();

            for (int i = 0; i < animNames.Length; i++)
            {
                SimpleAnimation anim = GetAnimation(animNames[i]);
                if (anim != null) animations.Add(anim);
            }

            return animations.ToArray();
        }

        /// <summary>
        /// Gets all animations.
        /// </summary>
        /// <returns>An array of all the animations.</returns>
        public SimpleAnimation[] GetAllAnimations()
        {
            return GetAnimations(Animations.Keys.ToArray());
        }

        /// <summary>
        /// Plays an animation, specified by name. If an animation cannot be found with the name, nothing happens.
        /// </summary>
        /// <param name="animName">The name of the animation to play.</param>
        public void PlayAnimation(string animName)
        {
            SimpleAnimation animToPlay = GetAnimation(animName);

            //Return if null;
            if (animToPlay == null)
                return;

            //Play animation
            CurrentAnim = animToPlay;

            CurrentAnim.Play();
        }

        /// <summary>
        /// Plays an animation, specified by name, if and only if it's different than the current animation.
        /// If an Animation cannot be found with the name, nothing happens.
        /// </summary>
        /// <param name="animName"></param>
        public void PlayAnimationIfDiff(string animName)
        {
            if (animName != CurrentAnim?.Key)
            {
                PlayAnimation(animName);
            }
        }
    }
}
