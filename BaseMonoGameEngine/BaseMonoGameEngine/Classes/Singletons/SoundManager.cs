using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace TDMonoGameEngine
{
    /// <summary>
    /// Handles all audio playback.
    /// <para>This is a Singleton.</para>
    /// </summary>
    public class SoundManager : ICleanup
    {
        #region Singleton Fields

        public static SoundManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SoundManager();
                }

                return instance;
            }
        }

        public bool HasInstance => (instance != null);

        private static SoundManager instance = null;

        #endregion

        #region Enumerations

        /// <summary>
        /// Enum for Sound IDs.
        /// </summary>
        public enum Sound
        {
            
        }

        #endregion

        private static readonly Dictionary<Sound, string> SoundMap = new Dictionary<Sound, string>()
        {
            
        };

        /// <summary>
        /// The global BGM volume.
        /// </summary>
        public float MusicVolume 
        {
            get
            {
                return musicVolume;
            }
            set
            {
                ChangeMusicVolume(value);
            }
        }

        /// <summary>
        /// The global SFX volume.
        /// </summary>
        public float SoundVolume
        {
            get
            {
                return soundVolume;
            }
            set
            {
                ChangeSoundVolume(value);
            }
        }

        private float musicVolume = .5f;
        private float soundVolume = .5f;

        /// <summary>
        /// The pool of sounds.
        /// <para>The key is the SoundEffect, and the value is a list of SoundEffectInstances created from it.</para>
        /// </summary>
        private Dictionary<SoundEffect, List<SoundEffectInstance>> SoundPool = null;

        private const double ClearSoundTimer = 30000d;
        private double LastPlayedSound = 0d;

        private SoundManager()
        {
            SoundPool = new Dictionary<SoundEffect, List<SoundEffectInstance>>();
        }

        public void CleanUp()
        {
            ClearAllSounds();

            instance = null;
        }

        private void UpdateLastSoundTimer()
        {
            LastPlayedSound = Time.TotalMilliseconds + ClearSoundTimer;
        }

        private void ChangeMusicVolume(in float mVolume)
        {
            musicVolume = UtilityGlobals.Clamp(mVolume, 0f, 1f);
        }

        private void ChangeSoundVolume(in float sVolume)
        {
            soundVolume = UtilityGlobals.Clamp(sVolume, 0f, 1f);
            SoundEffect.MasterVolume = soundVolume;
        }

        /// <summary>
        /// Plays a sound.
        /// </summary>
        /// <param name="sound">The SoundEffect to play.</param>
        /// <param name="volume">The volume to play the sound at.</param>
        public void PlaySound(in SoundEffect sound, in float volume = 1f)
        {
            if (sound == null)
            {
                Debug.LogError($"Cannot play {nameof(sound)} because it is null!");
                return;
            }

            //Retrieve the next sound instance
            SoundEffectInstance soundInstance = NextAvailableSound(sound);

            soundInstance.Volume = volume;
            soundInstance.Play();

            LastPlayedSound = Time.TotalMilliseconds + ClearSoundTimer;
        }

        /// <summary>
        /// Obtains the next available SoundEffectInstance in the sound pool for a particular SoundEffect.
        /// If no SoundEffectInstances are available, a new one is created.
        /// </summary>
        /// <param name="sound">The SoundEffect to retrieve the available SoundEffectInstance for.</param>
        /// <returns>A SoundEffectInstance that is not currently playing a sound.</returns>
        private SoundEffectInstance NextAvailableSound(in SoundEffect sound)
        {
            //Check for the available SoundEffectInstances associated with this SoundEffect
            List<SoundEffectInstance> availableSounds = null;

            //If there are none, add them as an entry in the sound pool
            if (SoundPool.TryGetValue(sound, out availableSounds) == false)
            {
                availableSounds = new List<SoundEffectInstance>();
                SoundPool.Add(sound, availableSounds);
            }

            //Check all existing sound instances and see if they're available for use
            for (int i = 0; i < availableSounds.Count; i++)
            {
                //Return ones that are not playing
                if (availableSounds[i].State != SoundState.Playing) return availableSounds[i];
            }

            //No sounds are available, so create a new instance and add it to the pool
            SoundEffectInstance newInstance = sound.CreateInstance();
            availableSounds.Add(newInstance);

            return newInstance;
        }

        public void Update()
        {
            //Clear all sounds after a set amount of time
            if (Time.TotalMilliseconds >= LastPlayedSound)
            {
                ClearAllSounds();

                LastPlayedSound = Time.TotalMilliseconds + ClearSoundTimer;
            }
        }

        /// <summary>
        /// Immediately stops all currently playing sounds and clears the sound pool.
        /// </summary>
        public void ClearAllSounds()
        {
            //Clear all sounds
            foreach (KeyValuePair<SoundEffect, List<SoundEffectInstance>> sounds in SoundPool)
            {
                List<SoundEffectInstance> instanceList = sounds.Value;

                //Stop all sound instances in the list and dispose them
                for (int i = instanceList.Count - 1; i >= 0; i--)
                {
                    instanceList[i].Stop(true);
                    instanceList[i].Dispose();
                    instanceList.RemoveAt(i);
                }
            }

            SoundPool.Clear();
        }
    }
}
