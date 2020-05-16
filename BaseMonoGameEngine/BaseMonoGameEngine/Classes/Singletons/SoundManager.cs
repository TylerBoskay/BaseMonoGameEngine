using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace BaseMonoGameEngine
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
        private Dictionary<string, SoundEffectInstance> MusicCache = null;

        /// <summary>
        /// The pool of sounds.
        /// <para>The key is the name of the SoundEffect, and the value is a list of <see cref="SoundInstanceHolder"/>s created from the SoundEffect.</para>
        /// </summary>
        private Dictionary<string, List<SoundInstanceHolder>> SoundPool = null;

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

        public SoundEffectInstance CurrentMusicTrack => CurMusicTrack;

        private SoundManager()
        {
            MusicCache = new Dictionary<string, SoundEffectInstance>(16);
            SoundPool = new Dictionary<string, List<SoundInstanceHolder>>(16);
        }

        public void CleanUp()
        {
            if (CurMusicTrack != null && CurMusicTrack.IsDisposed == false)
            {
                CurMusicTrack.Stop(true);
                CurMusicTrack.Dispose();
            }

            CurMusicTrack = null;

            if (MusicTrack != null && MusicTrack.IsDisposed == false)
            {
                MusicTrack.Dispose();
            }

            MusicTrack = null;
            ClearMusicCache();

            ClearAllSounds();

            instance = null;
        }

        private void UpdateLastSoundTimer()
        {
            LastPlayedSound = Time.TotalTime.TotalMilliseconds + ClearSoundTimer;
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

            if (SoundPool.Count == 0) return;

            //Change the volume of all current sound effects
            foreach (List<SoundInstanceHolder> soundHolders in SoundPool.Values)
            {
                for (int i = 0; i < soundHolders.Count; i++)
                {
                    soundHolders[i].SoundInstance.Volume = soundHolders[i].Volume * soundVolume;
                }
            }
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
                    CurMusicTrack.IsLooped = loop;
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
            if (MusicCache.TryGetValue(MusicTrack.Name, out CurMusicTrack) == false)
            {
                SoundEffectInstance trackInstance = MusicTrack.CreateInstance();

                MusicCache.Add(MusicTrack.Name, trackInstance);
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
        /// Immediately stops playing the current music track and clears its reference.
        /// </summary>
        public void StopAndClearMusicTrack()
        {
            CurMusicTrack?.Stop(true);
            CurMusicTrack = null;

            MusicTrack = null;
        }

        /// <summary>
        /// Pauses the current music track.
        /// </summary>
        public void PauseMusic()
        {
            CurMusicTrack?.Pause();
        }

        /// <summary>
        /// Resumes the current music track.
        /// </summary>
        public void ResumeMusic()
        {
            CurMusicTrack?.Resume();
        }

        /// <summary>
        /// Plays a sound.
        /// </summary>
        /// <param name="sound">The SoundEffect to play.</param>
        /// <param name="loop">Whether to loop the SoundEffect or not.</param>
        /// <param name="volume">The volume to play the sound at.</param>
        public void PlaySound(in SoundEffect sound, in bool loop, in float volume = 1f)
        {
            if (sound == null)
            {
                Debug.LogError($"Cannot play {nameof(sound)} because it is null!");
                return;
            }

            //Retrieve the next sound instance
            SoundInstanceHolder soundInstanceHolder = NextAvailableSound(sound);

            soundInstanceHolder.Volume = volume;

            //Scale the volume by the sound volume
            soundInstanceHolder.SoundInstance.Volume = volume * SoundVolume;
            soundInstanceHolder.SoundInstance.IsLooped = loop;
            soundInstanceHolder.SoundInstance.Play();

            UpdateLastSoundTimer();
        }

        /// <summary>
        /// Obtains the next available SoundInstanceHolder in the sound pool for a particular SoundEffect.
        /// If no SoundInstanceHolders are available, a new one is created.
        /// </summary>
        /// <param name="sound">The SoundEffect to retrieve the available SoundInstanceHolder for.</param>
        /// <returns>A SoundInstanceHolder that is not currently playing a sound.</returns>
        private SoundInstanceHolder NextAvailableSound(in SoundEffect sound)
        {
            //Check for the available SoundInstanceHolders associated with this SoundEffect
            List<SoundInstanceHolder> availableSounds = null;

            //If there are none, add them as an entry in the sound pool
            if (SoundPool.TryGetValue(sound.Name, out availableSounds) == false)
            {
                availableSounds = new List<SoundInstanceHolder>();
                SoundPool.Add(sound.Name, availableSounds);
            }

            //Check all existing sound instances and see if they're available for use
            for (int i = 0; i < availableSounds.Count; i++)
            {
                //Return ones that are not playing
                if (availableSounds[i].SoundInstance.State != SoundState.Playing) return availableSounds[i];
            }

            //No sounds are available, so create a new instance and add it to the pool
            SoundInstanceHolder soundInstanceHolder = new SoundInstanceHolder(sound.CreateInstance(), 1f);
            availableSounds.Add(soundInstanceHolder);

            return soundInstanceHolder;
        }

        public void Update()
        {
            //Clear all sounds after a set amount of time
            //if (Time.TotalTime.TotalMilliseconds >= LastPlayedSound)
            //{
            //    if (SoundPool.Count > 0)
            //    {
            //        ClearAllSounds();
            //    }
            //
            //    UpdateLastSoundTimer();
            //}
        }

        /// <summary>
        /// Immediately stops all sound instances for a particular sound.
        /// </summary>
        /// <param name="sound">The SoundEffect to stop all instances for.</param>
        public void StopAllSounds(in SoundEffect sound)
        {
            StopAllSounds(sound?.Name);
        }

        /// <summary>
        /// Immediately stops all sound instances for a sound with a particular name.
        /// </summary>
        /// <param name="soundName">The name of the sound to stop all instances for.</param>
        public void StopAllSounds(in string soundName)
        {
            if (string.IsNullOrEmpty(soundName) == true)
            {
                Debug.LogError($"Cannot stop all sound instances of {nameof(soundName)} because it is null or empty!");
                return;
            }

            List<SoundInstanceHolder> instanceList = null;

            if (SoundPool.TryGetValue(soundName, out instanceList) == true)
            {
                //Immediately stop the instances
                for (int i = 0; i < instanceList.Count; i++)
                {
                    instanceList[i].SoundInstance.Stop(true);
                }
            }
        }

        /// <summary>
        /// Immediately stops all playing sounds.
        /// </summary>
        public void StopAllSounds()
        {
            //Go through all sounds
            foreach (KeyValuePair<string, List<SoundInstanceHolder>> sounds in SoundPool)
            {
                List<SoundInstanceHolder> instanceList = sounds.Value;

                //Immediately stop all sound instances in the list
                for (int i = 0; i < instanceList.Count; i++)
                {
                    instanceList[i].SoundInstance.Stop(true);
                }
            }
        }

        /// <summary>
        /// Immediately stops all currently playing sounds and clears the sound pool.
        /// </summary>
        public void ClearAllSounds()
        {
            //Clear all sounds
            foreach (KeyValuePair<string, List<SoundInstanceHolder>> sounds in SoundPool)
            {
                List<SoundInstanceHolder> instanceList = sounds.Value;

                //Stop all sound instances in the list and dispose them
                for (int i = instanceList.Count - 1; i >= 0; i--)
                {
                    if (instanceList[i].SoundInstance.IsDisposed == false)
                    {
                        instanceList[i].SoundInstance.Stop(true);
                        instanceList[i].SoundInstance.Dispose();
                    }
                    instanceList.RemoveAt(i);
                }
            }

            SoundPool.Clear();
        }

        /// <summary>
        /// Immediately stops all music tracks in the cache and clears it. This includes the track currently playing.
        /// </summary>
        public void ClearMusicCache()
        {
            foreach (KeyValuePair<string, SoundEffectInstance> track in MusicCache)
            {
                if (track.Value.IsDisposed == false)
                {
                    track.Value.Stop(true);
                    track.Value.Dispose();
                }
            }

            MusicCache.Clear();

            CurMusicTrack = null;
            MusicTrack = null;
        }

        /// <summary>
        /// Holds a SoundEffectInstance and a value for its volume.
        /// This exists to help with the master sound volume.
        /// </summary>
        private class SoundInstanceHolder
        {
            public SoundEffectInstance SoundInstance = null;
            public float Volume = 0f;

            public SoundInstanceHolder(SoundEffectInstance soundInstance, in float volume)
            {
                SoundInstance = soundInstance;
                Volume = volume;
            }
        }
    }
}
