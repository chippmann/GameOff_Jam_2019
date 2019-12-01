using Godot;
using Planty.EngineUtils;
using Planty.Entities.DemonEntity.States;
using Planty.Entities.PlayerEntity.States;

namespace Planty.SoundEngine {
    public class SoundEngineNode : Node {
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";
        private AudioStreamPlayer musicPlayer;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready() {
            musicPlayer = new AudioStreamPlayer();
            //musicPlayer.SetBus("Music");

            AddChild(musicPlayer);
            musicPlayer.SetVolumeDb(-12);
        }

        public void PlayMusic(string name, bool repeat = true, bool fade = true, string bus = "Music") { 
            AudioStreamOGGVorbis stream = GetMusicStream(name, repeat);
            if (stream != null) {
                musicPlayer.SetStream(stream);
                musicPlayer.SetBus(bus);
                Logger.Debug("Play Music :"+stream.ResourcePath);
                musicPlayer.Play();
            }
            else {
                Logger.Warning("Music not found: " + name);
            }
        }

        public void StopMusic() {
            musicPlayer.Stop();
        }

        public AudioStreamPlayer PlaySfxLoop(AudioStreamOGGVorbis stream, Node owner,
            string bus = "SfxA") {
            AudioStreamPlayer player = new AudioStreamPlayer();
            player.SetStream(stream);
            player.SetBus(bus);
            owner.AddChild(player);

            player.Play();
            Logger.Debug("Play Sfx Loop: "+stream.ResourcePath);
            return player;
        }

        public AudioStreamPlayer2D PlaySfxLoop2D(AudioStreamOGGVorbis stream, Node2D target, string bus = "SfxA") {
            PositionalAudioStreamPlayer2D player = new PositionalAudioStreamPlayer2D();
            player.Init(target);
            player.SetStream(stream);
            player.SetBus(bus);
            target.AddChild(player);

            player.Play();
            Logger.Debug("Play Sfx Loop 2D: "+stream.ResourcePath);
            return player;
        }

        public void PlaySfx(AudioStreamSample stream, Node owner, string bus = "SfxA") {
            AudioStreamPlayer player = new AudioStreamPlayer();
            player.SetStream(stream);
            player.SetBus(bus);
            owner.AddChild(player);

            player.Play();
            Logger.Debug("Play Sfx: "+stream.ResourcePath);
        }
        
        public AudioStreamPlayer PlaySfx(AudioStreamOGGVorbis stream, Node owner, string bus = "SfxA") {
            AudioStreamPlayer player = new AudioStreamPlayer();
            player.SetStream(stream);
            player.SetBus(bus);
            owner.AddChild(player);

            player.Play();
            Logger.Debug("Play Sfx: "+stream.ResourcePath);
            return player;
        }

        public bool StopSfx(AudioStreamPlayer2D player) {
            player.Stop();
            Logger.Debug("Stop Sfx 2D: "+player.Stream.ResourcePath);
            RemoveChild(player);
            return true;
        }

        public bool StopSfx(AudioStreamPlayer player) {
            player.Stop();
            Logger.Debug("Stop Sfx: "+player.Stream.ResourcePath);
            RemoveChild(player);
            return true;
        }

        private AudioStreamOGGVorbis GetMusicStream(string name, bool repeat = true) {
            AudioStreamOGGVorbis stream = ResourceLoader.Load<AudioStreamOGGVorbis>("res://SoundEngine/Assets/Music/" + name + ".ogg");
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