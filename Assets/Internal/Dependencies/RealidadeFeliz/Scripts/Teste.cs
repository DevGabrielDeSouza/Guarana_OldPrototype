using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Collections;
using UnityEngine;

public class Teste : MonoBehaviour{

	[SerializeField]
	private GameObject _Tela_ = null;	// Prefab da Tela

	// Start is called before the first frame update
	void Start(){

		var tmp = new DescriptorLoader();
		tmp.Load();

		foreach( KeyValuePair<string,Region> kvp in tmp.region){

			Region.Transform aux = kvp.Value.GetTransform();

			// Não é a região total
			if( aux != null ){

//				var obj = new GameObject();

//				obj.transform.position = aux.position;
//				obj.transform.localScale = aux.scale;

				// ################################
				// Cria uma tela

				var temp = (GameObject) Instantiate(_Tela_);
				temp.name = "Tela teste";

				//temp.transform.localRotation = Quaternion.Euler(0.0f,0.0f,0.0f);
				temp.transform.localPosition = aux.position;
				temp.transform.localScale = aux.scale;

				var tela = temp.GetComponent<Tela>();
				Assert.IsNotNull(tela);

				//tela.SetVideoURL(@"\\localhost\B$\temp\junk_Matheus\Assets\Storage\Videos\evangelion.mp4");
				//tela.SetVideoURL(@"\\happyserver.lan\shared\evangelion.mp4");
				//tela.SetVideoURL(TMP_PATH + "/evangelion.mp4");
				tela.SetResolution(720,480);
				tela.LookAtOrigin();
			}
		}
	}
}