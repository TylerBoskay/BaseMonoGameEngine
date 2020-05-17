using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// Helps manage content.
    /// <para>This is a Singleton.</para>
    /// </summary>
    public class AssetManager : ICleanup
    {
        #region Singleton Fields

        public static AssetManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AssetManager();
                }

                return instance;
            }
        }

        public static bool HasInstance => (instance != null);

        private static AssetManager instance = null;

        #endregion

        public BContentManager MusicContent { get; private set; } = null;

        public BContentManager SoundContent { get; private set; } = null;

        public BContentManager FontContent { get; private set; } = null;

        public BContentManager TextureContent { get; private set; } = null;

        public BContentManager ShaderContent { get; private set; } = null;

        private ContentManager Content { get; set; } = null;
        
        /// <summary>
        /// Holds loaded raw Texture2Ds. These are disposed on cleanup.
        /// </summary>
        private Dictionary<string, Texture2D> RawTextures = null;

        /// <summary>
        /// Holds loaded raw SoundEffects. These are disposed on cleanup.
        /// </summary>
        private Dictionary<string, SoundEffect> RawSounds = null;

        private AssetManager()
        {
            
        }

        public void Initialize(in ContentManager content)
        {
            MusicContent = new BContentManager(content.ServiceProvider, ContentGlobals.ContentRoot);
            SoundContent = new BContentManager(content.ServiceProvider, ContentGlobals.ContentRoot);
            FontContent = new BContentManager(content.ServiceProvider, ContentGlobals.ContentRoot);
            TextureContent = new BContentManager(content.ServiceProvider, ContentGlobals.ContentRoot);
            ShaderContent = new BContentManager(content.ServiceProvider, ContentGlobals.ContentRoot);

            RawTextures = new Dictionary<string, Texture2D>();
            RawSounds = new Dictionary<string, SoundEffect>();
        }

        public void CleanUp()
        {
            //Unload all content
            UnloadLoadedContent();

            //Dispose each raw texture
            foreach (KeyValuePair<string, Texture2D> texPair in RawTextures)
            {
                texPair.Value.Dispose();
            }

            //Dispose each raw sound
            foreach (KeyValuePair<string, SoundEffect> soundPair in RawSounds)
            {
                soundPair.Value.Dispose();
            }

            //Clear the dictionaries
            RawTextures.Clear();
            RawSounds.Clear();

            instance = null;
        }

        /// <summary>
        /// Loads a raw Texture2D. They're cached for quick fetching.
        /// </summary>
        /// <param name="texturePath">The path to load the Texture2D from.
        /// The content root directory is appended to the start of this.</param>
        /// <returns>A Texture2D found at <paramref name="texturePath"/>, otherwise null.</returns>
        public Texture2D LoadRawTexture2D(in string texturePath)
        {
            Texture2D tex = null;

            //Insert content at the start
            string realTexPath = texturePath.Insert(0, Content.RootDirectory);

            //Return the cached texture if we have it
            if (RawTextures.ContainsKey(realTexPath) == true)
            {
                tex = RawTextures[realTexPath];
            }
            else
            {
                //Load the raw texture
                try
                {
                    using (FileStream fileStream = new FileStream(realTexPath, FileMode.Open))
                    {
                        tex = Texture2D.FromStream(RenderingManager.Instance.graphicsDeviceManager.GraphicsDevice, fileStream);

                        //Cache the texture for faster loading next time
                        if (tex != null)
                        {
                            RawTextures.Add(realTexPath, tex);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading raw {nameof(Texture2D)} {realTexPath}: {e.Message}");
                }
            }

            return tex;
        }

        /// <summary>
        /// Loads a raw sound effect. They're cached for quick fetching.
        /// <para>Sounds must have at most 24 bit depth!</para>
        /// </summary>
        /// <param name="soundPath">The path to the sound file.</param>
        /// <returns>A SoundEffect loaded from <paramref name="soundPath"/>. null if the sound cannot be found or loaded.</returns>
        public SoundEffect LoadRawSound(in string soundPath)
        {
            SoundEffect sound = null;
            
            //Insert content at the start
            string realSoundPath = soundPath.Insert(0, Content.RootDirectory);

            //Return the cached sound if we have it
            if (RawSounds.ContainsKey(realSoundPath) == true)
            {
                sound = RawSounds[realSoundPath];
            }
            else
            {
                //Load the raw sound
                try
                {
                    using (FileStream fileStream = new FileStream(realSoundPath, FileMode.Open))
                    {
                        sound = SoundEffect.FromStream(fileStream);

                        //Cache the sound for faster loading next time
                        if (sound != null)
                        {
                            RawSounds.Add(realSoundPath, sound);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading raw {nameof(SoundEffect)} {realSoundPath}: {e.Message}");
                }
            }

            return sound;
        }

        /// <summary>
        /// Loads a font with a specified name. <see cref="ContentGlobals.FontRoot"/> is used as the path.
        /// </summary>
        /// <param name="fontName">The name of the font.</param>
        /// <returns>A SpriteFont instance if the font was successfully loaded, otherwise null.</returns>
        public SpriteFont LoadFont(in string fontName)
        {
            string fullPath = ContentGlobals.FontRoot + fontName;

            return LoadAsset<SpriteFont>(fullPath, FontContent);
        }

        /// <summary>
        /// Loads a shader with a specified name. <see cref="ContentGlobals.ShaderRoot"/> is used as the path.
        /// </summary>
        /// <param name="shaderName">The name of the shader.</param>
        /// <returns>An Effect instance if the shader was successfully loaded, otherwise null.</returns>
        public Effect LoadShader(in string shaderName)
        {
            string fullPath = ContentGlobals.ShaderRoot + shaderName;

            return LoadAsset<Effect>(fullPath, ShaderContent);
        }

        /// <summary>
        /// Loads a texture with a specified name. <see cref="ContentGlobals.SpriteRoot"/> is used as the path.
        /// </summary>
        /// <param name="textureName">The name of the texture.</param>
        /// <returns>A Texture2D instance if the texture was successfully loaded, otherwise null.</returns>
        public Texture2D LoadTexture(in string textureName)
        {
            string fullPath = ContentGlobals.SpriteRoot + textureName;

            return LoadAsset<Texture2D>(fullPath, TextureContent);
        }

        /// <summary>
        /// Loads a SoundEffect containing music with a specified name. <see cref="ContentGlobals.MusicRoot"/> is used as the path.
        /// </summary>
        /// <param name="musicName">The name of the music to load.</param>
        /// <returns>A SoundEffect instance if the sound was successfully loaded, otherwise null.</returns>
        public SoundEffect LoadMusic(in string musicName)
        {
            string fullPath = ContentGlobals.MusicRoot + musicName;

            //Set the name of the music to exclude the path
            SoundEffect sound = LoadAsset<SoundEffect>(fullPath, MusicContent);
            if (sound != null) sound.Name = musicName;

            return sound;
        }

        /// <summary>
        /// Loads a SoundEffect containing a sound with a specified name. <see cref="ContentGlobals.SFXRoot"/> is used as the path.
        /// </summary>
        /// <param name="soundName">The name of the sound to load.</param>
        /// <returns>A SoundEffect instance if the sound was successfully loaded, otherwise null.</returns>
        public SoundEffect LoadSound(in string soundName)
        {
            string fullPath = ContentGlobals.SFXRoot + soundName;

            //Set the name of the sound to exclude the path
            SoundEffect sound = LoadAsset<SoundEffect>(fullPath, SoundContent);
            if (sound != null) sound.Name = soundName;

            return sound;
        }

        /// <summary>
        /// Load an asset of a particular type from a content manager.
        /// </summary>
        /// <typeparam name="T">The type of content to load.</typeparam>
        /// <param name="assetPath">The path to load the asset from.</param>
        /// <param name="contentManager">The ContentManager to load the asset from.</param>
        /// <returns>The asset if it was successfully found. Returns the same instance if the same asset was loaded previously.</returns>
        public T LoadAsset<T>(in string assetPath, BContentManager contentManager)
        {
            //I opt for this rather than not handling the exception to make the content workflow less of a hassle for debug builds
            //I find that missing assets are very easy to spot, so just look at the logs if you notice an asset missing
            //Also, in the event there's no audio hardware, it'll throw an exception and let the player play without audio instead of crashing
            try
            {
                return contentManager.Load<T>(assetPath);
            }
            catch (NoAudioHardwareException noAudioHardwareException)
            {
                if (Engine.IgnoreAudioErrors == false)
                {
                    Debug.LogError($"Error loading sound {assetPath} due to audio hardware being unavailable. Full message: {noAudioHardwareException.Message}");
                }
                return default(T);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Error loading asset {assetPath}: {exception.Message}\nTrace: {exception.StackTrace}");
                return default(T);
            }
        }

        /// <summary>
        /// Unloads all currently loaded content.
        /// </summary>
        public void UnloadLoadedContent()
        {
            MusicContent.Unload();
            SoundContent.Unload();
            FontContent.Unload();
            TextureContent.Unload();
            ShaderContent.Unload();
        }

        /// <summary>
        /// Gets the total number of loaded assets.
        /// </summary>
        /// <returns>An integer representing the total number of loaded assets.</returns>
        public int GetTotalLoadedAssetCounts()
        {
            return (MusicContent.LoadedAssetCount + SoundContent.LoadedAssetCount
                + FontContent.LoadedAssetCount + TextureContent.LoadedAssetCount + ShaderContent.LoadedAssetCount);
        }
    }
}
