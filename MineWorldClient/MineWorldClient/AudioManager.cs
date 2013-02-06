using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace MineWorld
{
    public class AudioManager
    {
        //Todo extend audiomanager to fit my needs
        SoundEffectInstance _soundinstance;
        public float Volume;

        public void SetVolume(int vol)
        {
            Volume = vol;
        }

        public int GetVolume()
        {
            return (int)(Volume);
        }

        public void PlaySong(Song song,bool repeat)
        {
            if(MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
            }
            MediaPlayer.Volume = Volume;
            MediaPlayer.IsRepeating = repeat;
            MediaPlayer.Play(song);
        }

        public void StopPlaying()
        {
            MediaPlayer.Stop();
        }

        public void PlaySound(SoundEffect sound)
        {
            _soundinstance = sound.CreateInstance();
            _soundinstance.Volume = Volume;
            _soundinstance.Play();
        }
    }
}
