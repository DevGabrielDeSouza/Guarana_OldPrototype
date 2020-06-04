// Simples interface do que um tocador de vídeo deve ter

public interface IMediaVideoPlayer{

	void SetResolution( int x, int y);
	void SetVideoURL( string url);
	void SetDefaultVideoClip();
	void Pause();
	void Stop();
	void Play();
}