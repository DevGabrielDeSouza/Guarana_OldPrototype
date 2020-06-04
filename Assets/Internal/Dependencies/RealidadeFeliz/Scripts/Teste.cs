using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Assertions;
using UnityEngine;
using TMPro;

using System.Collections.Generic;
using System.Collections;
using System.Xml;
using System.IO;
using System;

public class Teste : MonoBehaviour{
	[SerializeField]
	private TextAsset descriptorFile;

	//public TextMeshPro debugTextBox { get; private set; }	// Text debugTextBox canvas

	[SerializeField]
	private GameObject telaPrefab = null;		// Screen's Prefab

	// Temporary directory path
	private string cachedVideosPath;

	// Start is called before the first frame update
	void Start(){

		// Get the debugTextBoxging object
//this.debugTextBox = GameObject.Find("/Text (TMP)").GetComponent<TextMeshPro>();

		// Executa a função principal
		this.StartCoroutine(this.AssyncMainCoroutine());
	}

	// "Main" function
	private IEnumerator AssyncMainCoroutine(){

		// ################################################################
		// Create an empty download directory to serve as cache

		//this.cachedVideosPath = string.Format("{0}/{1}",Application.persistentDataPath,"tmp_media");
		this.cachedVideosPath = Path.Combine(Application.temporaryCachePath,"tmp_media");

		try{

			if( Directory.Exists(this.cachedVideosPath) )
				Directory.Delete(this.cachedVideosPath,true);

			Directory.CreateDirectory(this.cachedVideosPath);

		}catch( Exception e){

//this.debugTextBox.text += "\nErro para saber se o directory existe?\n" + e;
			yield break;
		}

		//this.debugTextBox.text += "\n\t00";

		// ################################################################
		// Makes initialization assertions

		/*#if UNITY_EDITOR
			//Assert.IsTrue(Directory.Exists(this.cachedVideosPath));
			//Assert.IsNotNull(this.telaPrefab);
			//Assert.IsNotNull(this.debugTextBox);
			//Assert.IsNotNull(descriptorFile);
		#endif*/

		//this.debugTextBox.text += "\n\t01";

		// ################################################################
		// Lê o doc NCL



		////this.debugTextBox.text += "\n\t Resources: " + (TextAsset)Resources.Load("amostra");

		var parsedNCL = new DescriptorLoader(

			//(TextAsset)Resources.Load<TextAsset>("amostra.xml"),
			descriptorFile,

		//string.Format("file:///{0}/{1}",Application.dataPath,"Internal/Dependencies/RealidadeFeliz/Scripts/amostra.ncl"),
			string.Format("file:///{0}",this.cachedVideosPath)//,
			//this.debugTextBox
		);

//this.debugTextBox.text += "\n\t02";

		// ################################################################
		// Faz download das médias

		foreach( KeyValuePair<string,Media> kvp in parsedNCL.media){

			Region region = kvp.Value.defaultRegion;

//this.debugTextBox.text += "\n\tEm foreach 01: 00";

			// Temporariamente não queremos testar vídeos 360
			if( region != null && !region.IsTotal() ){

//this.debugTextBox.text += "\n\tEm foreach 01: 01";

				string fileName = Path.GetFileName(kvp.Value.src);
				string URL = string.Format("{0}/{1}",@"http://happyserver.lan/shared",fileName);

				yield return DownloadURL(URL,string.Format("{0}/{1}",this.cachedVideosPath,fileName));
			}
		}

//this.debugTextBox.text += "\n\t03";

		// ################################################################
		// Cria as telas para exibir as mídias

		foreach( KeyValuePair<string,Media> kvp in parsedNCL.media){

			Region region = kvp.Value.defaultRegion;

//this.debugTextBox.text += "\n\tEm foreach 02: 00";

			// Temporariamente não queremos testar vídeos 360
			if( region != null && !region.IsTotal() ){

//this.debugTextBox.text += "\n\tEm foreach 02: 01";

				// ################################
				// Cria uma screen para exibí-la

				var temp = (GameObject) Instantiate(telaPrefab);
				temp.name = "VideoScreen teste";

				//temp.transform.localRotation = Quaternion.Euler(0.0f,0.0f,0.0f);
				temp.transform.localPosition = region.Transform.position;
				temp.transform.localScale = region.Transform.scale;

				var screen = temp.GetComponent<VideoScreen>();

				/*#if UNITY_EDITOR
						Assert.IsNotNull(screen);
				#endif*/

				try{
					if(File.Exists(kvp.Value.src.Substring(8))){
						screen.SetVideoURL(kvp.Value.src);
						//screen.SetVideoURL("http://happyserver.lan/shared/eggman.mp4");
//this.debugTextBox.text += "\n\t Achei o arquivo" + kvp.Value.src + " !! =D";
					}
					else{
//this.debugTextBox.text += "\n\t Não achei o arquivo" + kvp.Value.src + " !! ='(";
					}

				}catch( Exception e){

//this.debugTextBox.text += "\nErro para saber se o arquivo existe? " + e;
					yield break;
				}

				screen.SetResolution(720,480);
				screen.LookAtOrigin();
			}
		}

//this.debugTextBox.text += "\n\t04";
	}

	private IEnumerator DownloadURL( string url, string fileName){


		/*#if UNITY_EDITOR
			Assert.IsTrue(url.Contains(@"/"));
		#endif*/

		using( UnityWebRequest www = UnityWebRequest.Get(url) ){

			yield return www.SendWebRequest();

			if( www.isNetworkError || www.isHttpError ){

//this.debugTextBox.text += "\n" + www.error;
				yield break;
			}


//this.debugTextBox.text += "\n\t Fiz Download do " + fileName;


			/*#if UNITY_EDITOR
				Assert.IsFalse(string.IsNullOrEmpty(fileName));
			#endif*/

			BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create));
			writer.Write(www.downloadHandler.data);
		}
	}

	private void OnApplicationQuit(){

		if( Directory.Exists(cachedVideosPath) )
			Directory.Delete(cachedVideosPath,true);
	}
}