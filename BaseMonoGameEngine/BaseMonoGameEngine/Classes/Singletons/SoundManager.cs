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

        /// <summary>
        /// The global music volume.
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
        /// The global sound volume.
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
        /// A cache of SoundEffectInstances for each music track.
        /// </summary>
        private Dictionary<SoundEffect, SoundEffectInstance> MusicCache = null;

        /// <summary>
        /// The pool of sounds.
        /// <para>The key is the SoundEffect, and the value is a list of SoundEffectInstances created from it.</para>
        /// </summary>
        private Dictionary<SoundEffect, List<SoundEffectInstance>> SoundPool = null;

        private const double ClearSoundTimer = 30000d;
        private double LastPlayedSound = 0d;

        /// <summary>
        /// The current music track being played.
        /// </summary>
        private SoundEffect MusicTrack = null;

        /// <summary>
        /// The current instance for the music track being played.
        /// </summary>
        private SoundEffectInstance CurMusicTrack = null;

        private SoundManager()
        {
            MusicCache = new Dictionary<SoundEffect, SoundEffectInstance>();
            SoundPool = new Dictionary<SoundEffect, List<SoundEffectInstance>>();
        }

        public void CleanUp()
        {
            CurMusicTrack?.Dispose();
            CurMusicTrack = null;

            MusicTrack = null;
            ClearMusicCache();

            ClearAllSounds();

            instance = null;
        }

        private void UpdateLastSoundTimer()
        {
            LastPlayedSound = Time.TotalMilliseconds + ClearSoundTimer;
        }

        /// <summary>
        /// Changes the global music volume.
        /// </summary>
        /// <param name="mVolume">A float from 0 to 1 representing the music volume.</param>
        private void ChangeMusicVolume(in float mVolume)
        {
            musicVolume = UtilityGlobals.Clamp(mVolume, 0f, 1f);

            //Change the volume of the currently playing music track
            if (CurMusicTrack != null)
                CurMusicTrack.Volume = MusicVolume;
        }

        /// <summary>
        /// Changes the global sound volume.
        /// </summary>
        /// <param name="sVolume">A float from 0 to 1 representing the sound volume.</param>
        private void ChangeSoundVolume(in float sVolume)
        {
            soundVolume = UtilityGlobals.Clamp(sVolume, 0f, 1f);

            //Change the volume of all sound effects
            SoundEffect.MasterVolume = SoundVolume;
        }

        /// <summary>
        /// Plays a music track.
        /// </summary>
        /// <param name="music">The music track to play.</param>
        /// <param name="loop">Whether to loop the music or not.</param>
        /// <param name="dontPlayIfSame">Indicates to not play the music track if the same one is passed in. Defaults to true.</param>
        public void PlayMusic(in SoundEffect music, in bool loop, in bool dontPlayIfSame = true)
        {
            if (music == null)
            {
                Debug.LogError($"Cannot play {nameof(music)} because it is null!");
                return;
            }

            //Check if we're playing the same track
            if (MusicTrack == music)
            {
                //Play the track if we should
                if (dontPlayIfSame == false)
                {
                    StopMusic(true);

                    CurMusicTrack.Volume = MusicVolume;
                    CurMusicTrack.Play();
                }

                return;
            }

            //Stop the current track
            if (CurMusicTrack != null)
            {
                CurMusicTrack.Stop(true);
                CurMusicTrack = null;
            }

            //Set the new track
            MusicTrack = music;

            //Check if we have an instance for this track in the music cache
            //If not, create one
            if (MusicCache.TryGetValue(MusicTrack, out CurMusicTrack) == false)
            {
                SoundEffectInstance trackInstance = MusicTrack.CreateInstance();

                MusicCache.Add(MusicTrack, trackInstance);
                CurMusicTrack = trackInstance;
            }

            //Play the instance
            CurMusicTrack.Volume = MusicVolume;
            CurMusicTrack.IsLooped = loop;
            CurMusicTrack.Play();
        }

        /// <summary>
        /// Stops playing the current music track.
        /// </summary>
        /// <param name="immediate">Whether to stop playing the music track immediately or not.</param>
        public void StopMusic(in bool immediate)
        {
            CurMusicTrack?.Stop(immediate);
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

            UpdateLastSoundTimer();
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

                UpdateLastSoundTimer();
            }
        }

        /// <summary>
        /// Immediately stops all sound instances for a particular sound.
        /// </summary>
        /// <param name="sound">The SoundEffect to stop all instances for.</param>
        public void StopAllSounds(in SoundEffect sound)
        {
            if (sound == null)
            {
                Debug.LogError($"Cannot stop all sound instances of {nameof(sound)} because it is null!");
                return;
            }

            List<SoundEffectInstance> instanceList = null;

            if (SoundPool.TryGetValue(sound, out instanceList) == true)
            {
                //Immediately stop the instances
                for (int i = 0; i < instanceList.Count; i++)
                {
                    instanceList[i].Stop(true);
                }
            }
        }

        /// <summary>
        /// Immediately stops all playing sounds.
        /// </summary>
        public void StopAllSounds()
        {
            //Go through all sounds
            foreach (KeyValuePair<SoundEffect, List<SoundEffectInstance>> sounds in SoundPool)
            {
                List<SoundEffectInstance> instanceList = sounds.Value;

                //Immediately stop all sound instances in the list
                for (int i = 0; i < instanceList.Count; i++)
                {
                    instanceList[i].Stop(true);
                }
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

        /// <summary>
        /// Immediately stops all music tracks in the cache and clears it.
        /// </summary>
        public void ClearMusicCache()
        {
            foreach (KeyValuePair<SoundEffect, SoundEffectInstance> track in MusicCache)
            {
                track.Value.Stop(true);
                track.Value.Dispose();
            }

            MusicCache.Clear();
            CurMusicTrack = null;
        }
    }
}
