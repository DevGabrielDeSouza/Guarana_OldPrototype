// Simples interface do que um tocador de vídeo deve ter

public interface VPlayer{

	void SetResolution( int x, int y);
	void SetVideoURL( string url);
	void SetDefaultVideoClip();
	void Pausar();
	void Parar();
	void Tocar();
}