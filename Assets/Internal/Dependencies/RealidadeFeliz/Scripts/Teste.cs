using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Assertions;
using UnityEngine;

using System.Collections.Generic;
using System.Collections;
using System.Xml;
using System.IO;
using System;

public class Teste : MonoBehaviour{

	[SerializeField]
	private GameObject _Tela_ = null;	// Prefab da Tela

	// Temporary directory path
	private string TMP_PATH;

	// Start is called before the first frame update
	void Start(){

		// Executa a função principal
		this.StartCoroutine(this.AssyncMain());
	}

	// "Main" function
	private IEnumerator AssyncMain(){

		// ################################################################
		// Create an empty download directory to serve as cache

		//this.TMP_PATH = string.Format("{0}/{1}",Application.persistentDataPath,"tmp_media");
		this.TMP_PATH = Path.Combine(Application.temporaryCachePath,"tmp_media");

		if( Directory.Exists(this.TMP_PATH) )
			Directory.Delete(this.TMP_PATH,true);

		Directory.CreateDirectory(this.TMP_PATH);

		// ################################################################
		// Makes initialization assertions

		Assert.IsTrue(Directory.Exists(this.TMP_PATH));
		Assert.IsNotNull(this._Tela_);

		// ################################################################
		// Lê o doc NCL

		var parsedNCL = new DescriptorLoader(

			string.Format("file:///{0}/{1}",Application.dataPath,"/Internal/Dependencies/RealidadeFeliz/Scripts/amostra.ncl"),
			string.Format("file:///{0}",this.TMP_PATH)
		);

		// ################################################################
		// Faz download das médias

		foreach( KeyValuePair<string,Media> kvp in parsedNCL.media){

			Region reg = kvp.Value.defaultReg;

			// Temporariamente não queremos testar vídeos 360
			if( reg != null && !reg.IsTotal() ){

				string fileName = Path.GetFileName(kvp.Value.src);
				string URL = string.Format("{0}/{1}",@"http://happyserver.lan/shared",fileName);

				yield return this.StartCoroutine(DownloadURL(URL));
			}
		}

		// ################################################################
		// Cria as telas para exibir as mídias

		foreach( KeyValuePair<string,Media> kvp in parsedNCL.media){

			Region reg = kvp.Value.defaultReg;

			// Temporariamente não queremos testar vídeos 360
			if( reg != null && !reg.IsTotal() ){

				// ################################
				// Cria uma tela para exibí-la

				var temp = (GameObject) Instantiate(_Tela_);
				temp.name = "Tela teste";

				//temp.transform.localRotation = Quaternion.Euler(0.0f,0.0f,0.0f);
				temp.transform.localPosition = reg.trans.position;
				temp.transform.localScale = reg.trans.scale;

				var tela = temp.GetComponent<Tela>();
				Assert.IsNotNull(tela);

				//tela.SetVideoURL(@"\\localhost\B$\temp\junk_Matheus\Assets\Storage\Videos\evangelion.mp4");
				//tela.SetVideoURL(@"\\happyserver.lan\shared\evangelion.mp4");

				tela.SetVideoURL(kvp.Value.src);
				tela.SetResolution(720,480);
				tela.LookAtOrigin();
			}
		}
	}

	private IEnumerator DownloadURL( string url){

		Assert.IsTrue(url.Contains(@"/"));

		using ( UnityWebRequest www = UnityWebRequest.Get(url) ){

			yield return www.SendWebRequest();

			if( www.isNetworkError || www.isHttpError )
				Debug.Log(www.error);

			else{

				string file_name = url.Substring(url.LastIndexOf(@"/"));
				file_name = file_name.Contains(@"?") ? file_name.Substring(0,file_name.LastIndexOf(@"?")+1) : file_name;

				Assert.IsFalse(string.IsNullOrEmpty(file_name));
				Assert.IsTrue(Directory.Exists(TMP_PATH));

				string savingPath = string.Format("{0}/{1}",TMP_PATH,file_name);

				BinaryWriter writer = new BinaryWriter(File.Open(savingPath, FileMode.Create));
				writer.Write(www.downloadHandler.data);
            }
		}
	}

	private void OnApplicationQuit(){

		if( Directory.Exists(TMP_PATH) )
			Directory.Delete(TMP_PATH,true);
	}
}