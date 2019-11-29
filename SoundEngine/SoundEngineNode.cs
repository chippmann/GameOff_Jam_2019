using Godot;
using System;
using System.IO;
using GameOff_2019.EngineUtils;

namespace GameOff_2019.SoundEngine {
    public class SoundEngineNode : Node
    {
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";
        private AudioStreamPlayer musicPlayer;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            musicPlayer = new AudioStreamPlayer();
            musicPlayer.SetBus("Music");

            AddChild(musicPlayer);

            PlayMusic("rainforest-01", true);
            musicPlayer.SetVolumeDb(-12);
        }

        public void PlayMusic(string name, bool repeat = true, bool fade = true) {
            Logger.Debug("Play Music: "+name);
            AudioStreamOGGVorbis stream = GetMusicStream(name, repeat);
            if (stream != null)
            {
                musicPlayer.SetStream(stream);
                musicPlayer.Play();
            }
            else
            {
                Logger.Warning("Music " + name + " not found");
            }
        }

        public void StopMusic() {
            musicPlayer.Stop();
        }

        public AudioStreamPlayer PlaySfxLoop(AudioStreamOGGVorbis stream)
        {
            AudioStreamPlayer player = new AudioStreamPlayer();
            player.SetStream(stream);
            AddChild(player);
            
            player.Play();
            return player;
        }
        
        public AudioStreamPlayer2D PlaySfxLoop2D(AudioStreamOGGVorbis stream, Node2D target)
        {
            PositionalAudioStreamPlayer2D player = new PositionalAudioStreamPlayer2D();
            player.Init(target);
            player.SetStream(stream);
            AddChild(player);
            
            player.Play();
            return player;
        }
        
        public void PlaySfx(AudioStreamSample stream)
        {
            AudioStreamPlayer player = new AudioStreamPlayer();
            player.SetStream(stream);
            AddChild(player);
            
            player.Play();
        }

        public bool StopSfx(AudioStreamPlayer2D player)
        {
            player.Stop();
            RemoveChild(player);
            return true;
        }
        
        public bool StopSfx(AudioStreamPlayer player)
        {
            player.Stop();
            RemoveChild(player);
            return true;
        }
        
        private AudioStreamOGGVorbis GetMusicStream(string name, bool repeat = true) {
            AudioStreamOGGVorbis stream = ResourceLoader.Load<AudioStreamOGGVorbis>("res://SoundEngine/Assets/Music/"+name+".ogg");
            stream.SetLoop(repeat);
            return stream;
        }
        

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
    }
}