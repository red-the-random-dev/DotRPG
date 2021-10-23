using DotRPG.Objects;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Text;

namespace DotRPG.Behavior.Management
{
    public class SoundManager : IDisposable
    {
        protected ResourceHeap Resources;
        Dictionary<String, SoundEffectInstance> PlayedSounds = new Dictionary<string, SoundEffectInstance>();

        public SoundManager(ResourceHeap resources)
        {
            Resources = resources;
        }

        public void PlayLocal(String ID)
        {
            Resources.Sounds[ID].Play();
        }

        public void BufferLocal(String ID, String playID)
        {
            PlayedSounds.Add(playID, Resources.Sounds[ID].CreateInstance());
        }

        public void BufferGlobal(String ID, String playID)
        {
            PlayedSounds.Add(playID, Resources.Global.Sounds[ID].CreateInstance());
        }

        public void PlayLocal(String ID, Single volume, Single pitch, Single pan)
        {
            Resources.Sounds[ID].Play(volume, pitch, pan);
        }

        public void PlayGlobal(String ID)
        {
            Resources.Global.Sounds[ID].Play();
        }

        public void PlayGlobal(String ID, Single volume, Single pitch, Single pan)
        {
            Resources.Global.Sounds[ID].Play(volume, pitch, pan);
        }

        public void Play(String ID)
        {
            PlayedSounds[ID].Play();
        }

        public void SetLooped(String ID, Boolean looped)
        {
            PlayedSounds[ID].IsLooped = looped;
        }

        public void SetParameters(String ID, Single volume, Single pitch, Single pan)
        {
            PlayedSounds[ID].Volume = volume;
            PlayedSounds[ID].Pitch = pitch;
            PlayedSounds[ID].Pan = pan;
        }

        public void Stop(String ID)
        {
            PlayedSounds[ID].Stop();
        }

        public void Unbuffer(String ID)
        {
            PlayedSounds[ID].Stop();
            PlayedSounds.Remove(ID);
        }

        public void StopAll()
        {
            foreach (String x in PlayedSounds.Keys)
            {
                Stop(x);
            }
        }

        public void UnbufferAll()
        {
            foreach (String x in PlayedSounds.Keys)
            {
                Unbuffer(x);
            }
        }

        public void Dispose()
        {
            UnbufferAll();
        }
    }
}
