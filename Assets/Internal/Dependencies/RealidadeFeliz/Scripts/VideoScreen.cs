using UnityEngine.Assertions;
using System.Collections;
using UnityEngine.Video;
using UnityEngine;
using System;

public class VideoScreen : MonoBehaviour, IMediaVideoPlayer{

	private static readonly Vector3 origin = new Vector3(0,0,0);

	[SerializeField]
	private MeshRenderer render = null;

	[SerializeField]
	private VideoPlayer player = null;

	private int height = 480;
	private int width = 720;

	// Start is called before the first frame update
	void Start(){

		Debug.Log("Criando tela...");

		Material material = new Material(Shader.Find("Sprites/Default"));
		RenderTexture texture = new RenderTexture(width,height,1);
		material.mainTexture = texture;

		this.player.targetTexture = texture;
		this.render.material = material;

		this.player.loopPointReached += delegate( UnityEngine.Video.VideoPlayer foo){ StartCoroutine(Fade()); Debug.Log("Vídeo terminado!"); };
	}

	public void SetResolution( int widthResolution, int heightResolution){

		/*#if UNITY_EDITOR
			Assert.IsNotNull(player);
		#endif*/
		

		if( player.targetTexture != null ){

			player.targetTexture.height = heightResolution;
			player.targetTexture.width = widthResolution;
		}

		height = widthResolution;
		width = heightResolution;
	}

	public void SetVideoURL( string url){

//		if( !url.StartsWith("http") && !url.StartsWith(@"smb://") )
//			throw new ArgumentException("the string must represent a valid URL");

		// #################################################################
		// TODO -> Make better autoconfiguration depending on file configs

		player.clip = null;
		player.source = UnityEngine.Video.VideoSource.Url;
		player.url = url;

		player.Prepare();
		player.prepareCompleted += delegate( UnityEngine.Video.VideoPlayer foo){ foo.Play(); Debug.Log("SetVideoURl feito! Iniciando video..."); };
	}

	public void SetDefaultVideoClip(){

		player.source = UnityEngine.Video.VideoSource.VideoClip;
		player.url = "";
		player.clip = null;
		player.Play();
	}

	public void LookAtOrigin(){

		this.gameObject.transform.LookAt(origin);
		// FIXIT: estão olhando no sentido certo, mas na direção oposta.
	}

	IEnumerator Fade(){

		//yield return new WaitUntil(() => player.time + 0.1f >= player.clip.length );
		Color c = render.material.color;

		for( float f = 1f; f > 0; f -= 0.1f){

			c.a = f;
			render.material.color = c;
			yield return new WaitForSeconds(0.01f);
		}

		c.a = 0;
		render.material.color = c;
		Destroy(gameObject);
	}

	public void Pause(){

		player.Pause();
	}

	public void Stop(){

		player.Stop();
	}

	public void Play(){

		player.Play();
	}

// ###########################
// Debugigng

	public string debuga(){

		return "Source: " + player.source + " ||| URL: " + player.url + "\n";
	}
}